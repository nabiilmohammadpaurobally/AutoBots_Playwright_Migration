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
});
