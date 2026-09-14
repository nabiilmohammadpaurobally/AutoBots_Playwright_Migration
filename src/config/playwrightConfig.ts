export type BrowserOption = 'chrome' | 'edge' | 'bs_chrome' | 'bs_edge';
const supportedBrowserOptions: readonly BrowserOption[] = ['chrome', 'edge', 'bs_chrome', 'bs_edge'];

/**
 * Runtime Playwright configuration loaded from environment variables.
 */
export class PlaywrightConfig {
  public readonly browserType: BrowserOption;
  public readonly pageUrl: string;
  public readonly connectionString: string;
  public readonly browserStackUserName: string;
  public readonly browserStackAccessKey: string;
  public readonly defaultTimeoutMs: number;
  public readonly pollingIntervalMs: number;
  public readonly testDataPath: string;
  public readonly headless: boolean;

  private constructor(values: {
    browserType: BrowserOption;
    pageUrl: string;
    connectionString: string;
    browserStackUserName: string;
    browserStackAccessKey: string;
    defaultTimeoutMs: number;
    pollingIntervalMs: number;
    testDataPath: string;
    headless: boolean;
  }) {
    this.browserType = values.browserType;
    this.pageUrl = values.pageUrl;
    this.connectionString = values.connectionString;
    this.browserStackUserName = values.browserStackUserName;
    this.browserStackAccessKey = values.browserStackAccessKey;
    this.defaultTimeoutMs = values.defaultTimeoutMs;
    this.pollingIntervalMs = values.pollingIntervalMs;
    this.testDataPath = values.testDataPath;
    this.headless = values.headless;
  }

  /** Loads config from process environment. */
  public static fromEnv(env: NodeJS.ProcessEnv = process.env): PlaywrightConfig {
    const rawBrowserType = env.BROWSER_OPTION ?? 'chrome';
    if (!supportedBrowserOptions.includes(rawBrowserType as BrowserOption)) {
      throw new Error(`Unsupported BROWSER_OPTION '${rawBrowserType}'. Supported values: ${supportedBrowserOptions.join(', ')}.`);
    }
    const browserType = rawBrowserType as BrowserOption;

    return new PlaywrightConfig({
      browserType,
      pageUrl: env.PAGE_URL ?? '',
      connectionString: env.BROWSERSTACK_CDP_URL ?? 'wss://cdp.browserstack.com/playwright',
      browserStackUserName: env.BROWSERSTACK_USERNAME ?? '',
      browserStackAccessKey: env.BROWSERSTACK_ACCESS_KEY ?? '',
      defaultTimeoutMs: PlaywrightConfig.parsePositiveInt(env.DEFAULT_TIMEOUT_MS, 60_000),
      pollingIntervalMs: PlaywrightConfig.parsePositiveInt(env.POLLING_INTERVAL_MS, 1_000),
      testDataPath: env.TEST_DATA_PATH ?? 'TestData',
      headless: PlaywrightConfig.parseBoolean(env.HEADLESS, true, 'HEADLESS')
    });
  }

  /** Returns BrowserStack credential pair in user:key format. */
  public get browserStackCredentials(): string {
    return `${this.browserStackUserName}:${this.browserStackAccessKey}`;
  }

  /** True when BrowserStack execution mode is selected. */
  public isBrowserStackExecution(): boolean {
    return this.browserType === 'bs_chrome' || this.browserType === 'bs_edge';
  }

  /** Validates required configuration and throws descriptive errors if invalid. */
  public validateOrThrow(): void {
    if (!this.pageUrl.trim()) {
      throw new Error('PAGE_URL must be provided.');
    }

    if (this.isBrowserStackExecution()) {
      if (!this.browserStackUserName.trim() || !this.browserStackAccessKey.trim()) {
        throw new Error('BROWSERSTACK_USERNAME and BROWSERSTACK_ACCESS_KEY are required for BrowserStack runs.');
      }

      if (!this.connectionString.trim()) {
        throw new Error('BROWSERSTACK_CDP_URL must be provided for BrowserStack runs.');
      }
    }
  }

  private static parsePositiveInt(value: string | undefined, fallback: number): number {
    const parsed = Number(value);
    return Number.isInteger(parsed) && parsed > 0 ? parsed : fallback;
  }

  private static parseBoolean(value: string | undefined, fallback: boolean, keyName: string): boolean {
    if (value === undefined) {
      return fallback;
    }

    const normalized = value.trim().toLowerCase();
    if (normalized === 'true') {
      return true;
    }

    if (normalized === 'false') {
      return false;
    }

    throw new Error(`${keyName} must be 'true' or 'false'.`);
  }
}
