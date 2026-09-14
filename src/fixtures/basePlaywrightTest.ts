import { expect, test as base } from '@playwright/test';
import { PlaywrightConfig } from '../config/playwrightConfig';
import { PlaywrightFactory, PlaywrightSession } from '../factories/playwrightFactory';

export type BaseFixtures = {
  config: PlaywrightConfig;
  session: PlaywrightSession;
};

/**
 * Base test fixture replacing C# SetUp/TearDown and ThreadLocal patterns.
 */
export const test = base.extend<BaseFixtures>({
  config: async ({}, use) => {
    const config = PlaywrightConfig.fromEnv();
    config.validateOrThrow();
    await use(config);
  },

  session: async ({ config }, use, testInfo) => {
    const session = await PlaywrightFactory.getBrowser(config, testInfo.title);
    await session.page.goto(config.pageUrl, { waitUntil: 'domcontentloaded' });

    await use(session);

    await session.page.close();
    await session.context.close();
    await session.browser.close();
  },

  page: async ({ session }, use) => {
    await use(session.page);
  },

  context: async ({ session }, use) => {
    await use(session.context);
  }
});

export { expect };
