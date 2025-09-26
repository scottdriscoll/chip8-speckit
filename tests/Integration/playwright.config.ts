import { defineConfig } from '@playwright/test';

const port = Number(process.env.CHIP8_WEB_PORT ?? 5179);
const baseURL = process.env.CHIP8_BASE_URL ?? `http://localhost:${port}`;

export default defineConfig({
  timeout: 60000,
  retries: 0,
  testDir: './',
  use: {
    headless: true,
    screenshot: 'only-on-failure',
    trace: 'retain-on-failure',
    baseURL,
  },
  webServer: {
    command: `dotnet run --project src/Chip8.Emulator.Web/Chip8.Emulator.Web.csproj --urls http://localhost:${port}`,
    port,
    reuseExistingServer: !process.env.CI,
    timeout: 120000,
  },
});
