import { test, expect } from '@playwright/test';
import { PlaywrightConfig } from '../src/config/playwrightConfig';

test.describe('PlaywrightConfig', () => {
  test('throws for unsupported browser option', () => {
    expect(() => PlaywrightConfig.fromEnv({ BROWSER_OPTION: 'firefox' } as NodeJS.ProcessEnv)).toThrow(/Unsupported BROWSER_OPTION/);
  });

  test('parses headless flag from env', () => {
    const config = PlaywrightConfig.fromEnv({ BROWSER_OPTION: 'chrome', HEADLESS: 'false', PAGE_URL: 'https://example.com' } as NodeJS.ProcessEnv);
    expect(config.headless).toBe(false);
  });

  test('throws for invalid headless value', () => {
    expect(() => PlaywrightConfig.fromEnv({ BROWSER_OPTION: 'chrome', PAGE_URL: 'https://example.com', HEADLESS: 'maybe' } as NodeJS.ProcessEnv)).toThrow(
      /HEADLESS must be 'true' or 'false'/
    );
  });

  test('falls back for non-integer numeric values', () => {
    const config = PlaywrightConfig.fromEnv({
      BROWSER_OPTION: 'chrome',
      PAGE_URL: 'https://example.com',
      DEFAULT_TIMEOUT_MS: '1000.5',
      POLLING_INTERVAL_MS: 'abc'
    } as NodeJS.ProcessEnv);

    expect(config.defaultTimeoutMs).toBe(60_000);
    expect(config.pollingIntervalMs).toBe(1_000);
  });
});
