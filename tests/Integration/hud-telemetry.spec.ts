import { test, expect } from '@playwright/test';

const waitForSnapshot = async (page: any) => {
  await page.waitForFunction(() => typeof window !== 'undefined' && typeof (window as any).chip8DebugGetSnapshot === 'function');
  await page.waitForFunction(() => {
    const snapshot = (window as any).chip8DebugGetSnapshot?.();
    return snapshot && Array.isArray(snapshot.Registers) && snapshot.Registers.length === 16;
  });
};

test.describe('HUD telemetry', () => {
  test('registers update and keyboard highlights respond', async ({ page }) => {
    await page.goto('/');

    await waitForSnapshot(page);

    const initialRegister = await page.evaluate(() => (window as any).chip8DebugGetSnapshot()?.Registers?.[0] ?? 0);

    await page.waitForFunction((initial) => {
      const snapshot = (window as any).chip8DebugGetSnapshot?.();
      return snapshot && snapshot.Registers && snapshot.Registers[0] !== initial;
    }, initialRegister);

    await page.click('.app-shell');
    await page.keyboard.down('1');

    await page.waitForFunction(() => {
      const state = (window as any).chip8DebugGetKeyState?.();
      return state && state['1'] === true;
    });

    await page.keyboard.up('1');
    await page.waitForFunction(() => {
      const state = (window as any).chip8DebugGetKeyState?.();
      return state && state['1'] === false;
    });

    const opcodeText = await page.locator('.hud-panel-status dd').nth(1).innerText();
    expect(opcodeText).toContain('0x');
  });
});
