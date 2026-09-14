import { defineConfig } from '@playwright/test';

const workers = parsePositiveInt(process.env.PLAYWRIGHT_WORKERS, 4);
const timeout = parsePositiveInt(process.env.DEFAULT_TIMEOUT_MS, 60_000);

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

function parsePositiveInt(value: string | undefined, fallback: number): number {
  const parsed = Number(value);
  return Number.isInteger(parsed) && parsed > 0 ? parsed : fallback;
}
