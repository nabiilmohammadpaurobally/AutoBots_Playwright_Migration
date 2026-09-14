import { expect, Locator, Page } from '@playwright/test';

export async function waitUntilVisible(page: Page, selector: string, timeoutMs = 60_000): Promise<Locator> {
  const locator = page.locator(selector);
  await locator.waitFor({ state: 'visible', timeout: timeoutMs });
  return locator;
}

export async function waitUntilClickable(page: Page, selector: string, timeoutMs = 60_000): Promise<Locator> {
  const locator = await waitUntilVisible(page, selector, timeoutMs);
  await expect(locator).toBeEnabled({ timeout: timeoutMs });
  return locator;
}

export async function waitUntilTextVisible(page: Page, selector: string, expectedText: string, timeoutMs = 60_000): Promise<Locator> {
  const locator = await waitUntilVisible(page, selector, timeoutMs);
  await expect(locator).toContainText(expectedText, { timeout: timeoutMs, ignoreCase: true });
  return locator;
}

export async function waitUntilElementNotFound(page: Page, selector: string, timeoutMs = 60_000): Promise<void> {
  await page.locator(selector).waitFor({ state: 'hidden', timeout: timeoutMs });
}

export async function scrollToElementWait(page: Page, selector: string, waitMs = 500): Promise<void> {
  const locator = page.locator(selector);
  await locator.scrollIntoViewIfNeeded();
  await page.waitForTimeout(waitMs);
}

export async function javaScriptClick(page: Page, selector: string): Promise<void> {
  await page.locator(selector).first().evaluate((element) => {
    (element as HTMLElement).click();
  });
}

export async function scrollToElementWaitAndJavaScriptClick(page: Page, selector: string): Promise<void> {
  await scrollToElementWait(page, selector);
  await javaScriptClick(page, selector);
  await smartWaitPageLoader(page);
}

export async function waitForAlertAndAccept(page: Page, trigger: () => Promise<void>, timeoutMs = 10_000): Promise<string> {
  const dialogPromise = page.waitForEvent('dialog', { timeout: timeoutMs });
  await trigger();
  const dialog = await dialogPromise;
  const message = dialog.message();
  await dialog.accept();
  return message;
}

export async function smartWaitPageLoader(page: Page, timeoutMs = 60_000): Promise<void> {
  await page.waitForLoadState('domcontentloaded', { timeout: timeoutMs });
  await page.waitForLoadState('networkidle', { timeout: timeoutMs });
}

export async function staticWait(seconds: number): Promise<void> {
  if (seconds < 0) {
    throw new RangeError('Wait time must be non-negative.');
  }

  await new Promise((resolve) => setTimeout(resolve, seconds * 1000));
}

export async function isElementPresent(page: Page, selector: string): Promise<boolean> {
  return (await page.locator(selector).count()) > 0;
}

export async function isAlertPresent(page: Page, timeoutMs = 2_000): Promise<boolean> {
  try {
    const dialog = await page.waitForEvent('dialog', { timeout: timeoutMs });
    await dialog.dismiss();
    return true;
  } catch {
    return false;
  }
}

export async function waitForAlertValidateMessageAndAccept(
  page: Page,
  expectedText: string,
  trigger: () => Promise<void>,
  timeoutMs = 10_000
): Promise<void> {
  const dialogPromise = page.waitForEvent('dialog', { timeout: timeoutMs });
  await trigger();
  const dialog = await dialogPromise;
  expect(dialog.message().toLowerCase()).toContain(expectedText.toLowerCase());
  await dialog.accept();
}

export async function scrollToElementWaitAndClick(page: Page, selector: string): Promise<void> {
  await scrollToElementWait(page, selector);
  await page.locator(selector).click();
}

export async function scrollToElementAndJavaScriptClickAlert(
  page: Page,
  selector: string,
  expectedMessages: readonly string[]
): Promise<void> {
  await scrollToElementWait(page, selector);
  const text = await waitForAlertAndAccept(page, async () => javaScriptClick(page, selector));
  const matched = expectedMessages.some((message) => text.toLowerCase().includes(message.toLowerCase()));
  expect(matched).toBeTruthy();
  await smartWaitPageLoader(page);
}

export async function scrollToElementWaitAndJavaScriptClickWithoutSmartWait(page: Page, selector: string): Promise<void> {
  await scrollToElementWait(page, selector);
  await javaScriptClick(page, selector);
}
