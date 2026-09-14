import { fetchData } from '../../src/helpers/jsonReader';
import { EmployeeBookingData } from '../../src/models/neonatal/employeeBookingData';
import { PersonalDetailsFormPlaywright } from '../../src/pageObjects/neonatal/personalDetailsFormPlaywright';
import { test } from '../../src/fixtures/basePlaywrightTest';

test.describe('T386672 Employee Books Neonatal (Playwright TS)', () => {
  test.skip('sample migrated test - requires AUT selectors and URL', async ({ page, config }) => {
    await page.goto(config.pageUrl, { waitUntil: 'domcontentloaded' });

    const data = fetchData<EmployeeBookingData>('Neonatal/T386672_Employee_Books_Neonatal');

    const personalDetailsForm = new PersonalDetailsFormPlaywright(page);
    await personalDetailsForm.fill(data.employee);
    await personalDetailsForm.submitAndValidateAlert(data.expectedAlerts);
  });
});
