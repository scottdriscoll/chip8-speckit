import { test, expect } from '@playwright/test';

test.describe('HUD controls', () => {
  test('pause/resume toggles execution state snapshot', async ({ page }) => {
    await page.goto('/');

    await page.waitForFunction(() => typeof (window as any).chip8DebugGetSnapshot === 'function');

    const readState = async () => {
      return page.evaluate(() => (window as any).chip8DebugGetSnapshot()?.ExecutionState);
    };

    const initialState = await readState();
    await page.click('button:has-text("Pause")');
    await page.waitForFunction((state) => {
      const snapshot = (window as any).chip8DebugGetSnapshot?.();
      return snapshot && snapshot.ExecutionState !== state;
    }, initialState);

    const pausedState = await readState();
    expect(pausedState).not.toBe(initialState);

    await page.click('button:has-text("Resume")');
    await page.waitForFunction((state) => {
      const snapshot = (window as any).chip8DebugGetSnapshot?.();
      return snapshot && snapshot.ExecutionState === state;
    }, initialState);

    const resumedState = await readState();
    expect(resumedState).toBe(initialState);
  });
});
