import { Page } from '@playwright/test';
import { waitForAlertAndAccept, waitUntilVisible } from '../../extensions/playwrightExtensions';
import { EmployeeDetails } from '../../models/neonatal/employeeBookingData';

/** Playwright page object for Personal Details form. */
export class PersonalDetailsFormPlaywright {
  private readonly firstName = '#firstName';
  private readonly surname = '#surname';
  private readonly email = '#email';
  private readonly genderFrame = 'iframe#genderFrame';
  private readonly maleOption = '#genderMale';
  private readonly continueButton = '#continue';

  public constructor(private readonly page: Page) {}

  public async fill(employee: EmployeeDetails): Promise<void> {
    await (await waitUntilVisible(this.page, this.firstName)).fill(employee.firstName);
    await (await waitUntilVisible(this.page, this.surname)).fill(employee.surname);
    await (await waitUntilVisible(this.page, this.email)).fill(employee.email);
    await this.page.frameLocator(this.genderFrame).locator(this.maleOption).click();
  }

  public async submitAndValidateAlert(expectedMessages: readonly string[]): Promise<void> {
    const message = await waitForAlertAndAccept(this.page, () => this.page.locator(this.continueButton).click());
    const matched = matchesExpectedAlert(message, expectedMessages);
    if (!matched) {
      throw new Error(`Alert message '${message}' does not match expected values: ${expectedMessages.join(', ')}`);
    }
  }
}

/** Returns true when dialog message contains one of the expected values (case-insensitive). */
export function matchesExpectedAlert(actualMessage: string, expectedMessages: readonly string[]): boolean {
  return expectedMessages.some((expected) => actualMessage.toLowerCase().includes(expected.toLowerCase()));
}
