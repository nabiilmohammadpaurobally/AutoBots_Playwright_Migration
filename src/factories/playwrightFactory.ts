import { chromium, Browser, BrowserContext, Page } from '@playwright/test';
import { PlaywrightConfig } from '../config/playwrightConfig';

export interface PlaywrightSession {
  browser: Browser;
  context: BrowserContext;
  page: Page;
}

/**
 * Creates local and BrowserStack Playwright sessions.
 */
export class PlaywrightFactory {
  public static async getBrowser(config: PlaywrightConfig, testName?: string): Promise<PlaywrightSession> {
    config.validateOrThrow();

    const browser = config.isBrowserStackExecution()
      ? await this.connectBrowserStack(config, testName)
      : await this.launchLocal(config);

    const context = await browser.newContext({
      ignoreHTTPSErrors: true,
      viewport: { width: 1920, height: 1080 }
    });

    const page = await context.newPage();
    page.setDefaultTimeout(config.defaultTimeoutMs);

    return {
      browser,
      context,
      page
    };
  }

  private static async launchLocal(config: PlaywrightConfig): Promise<Browser> {
    const channel = config.browserType === 'edge' ? 'msedge' : 'chrome';

    return chromium.launch({
      headless: config.headless,
      channel
    });
  }

  private static async connectBrowserStack(config: PlaywrightConfig, testName?: string): Promise<Browser> {
    const browser = config.browserType === 'bs_edge' ? 'edge' : 'chrome';
    const caps = {
      browser,
      browser_version: 'latest',
      os: 'Windows',
      os_version: '11',
      name: testName ?? 'Playwright TypeScript Session',
      build: `AutoBots Playwright TS Migration - ${new Date().toISOString().slice(0, 10)}`,
      'browserstack.username': config.browserStackUserName,
      'browserstack.accessKey': config.browserStackAccessKey
    };

    const endpoint = `${config.connectionString}?caps=${encodeURIComponent(JSON.stringify(caps))}`;
    return chromium.connect(endpoint);
  }
}
