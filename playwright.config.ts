import { defineConfig } from '@playwright/test';

const workers = Number(process.env.PLAYWRIGHT_WORKERS ?? 4);
const timeout = Number(process.env.DEFAULT_TIMEOUT_MS ?? 60_000);

export default defineConfig({
  testDir: './tests',
  timeout,
  fullyParallel: true,
  workers,
  retries: 0,
  use: {
    baseURL: process.env.PAGE_URL ?? 'https://example.com',
    trace: 'retain-on-failure',
    screenshot: 'only-on-failure',
    video: 'retain-on-failure',
    actionTimeout: timeout,
    navigationTimeout: timeout
  },
  reporter: [['html', { outputFolder: 'playwright-report', open: 'never' }], ['list']]
});
