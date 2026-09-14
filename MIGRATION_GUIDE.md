# Selenium C# NUnit to Playwright TypeScript Migration Guide

## Executive Summary
This starter package migrates AutoBots from Selenium C# NUnit to Playwright TypeScript while preserving JSON-driven test data, BrowserStack support, and parallel execution.

**Effort estimate:** **4-6 weeks**.

## 6-Phase Plan
1. **Foundation setup**: Node/TypeScript/Playwright project bootstrap.
2. **Core helper conversion**: Selenium extensions to Playwright async helpers.
3. **Page object conversion**: `IWebDriver/By` patterns to `Page/Locator`.
4. **Test conversion**: NUnit tests to Playwright Test fixtures.
5. **Cloud + parallel hardening**: BrowserStack + worker tuning.
6. **Regression and stabilization**: execute suites and fix flakes.

## Architecture Comparison
| Area | Selenium C# NUnit | Playwright TypeScript |
|---|---|---|
| Runner | NUnit | Playwright Test |
| Driver lifecycle | IWebDriver | BrowserContext + Page fixtures |
| Waits | Explicit waits + custom helpers | Auto-wait + locator assertions |
| iFrame | SwitchTo().Frame | frameLocator |
| Alert handling | IAlert | page.waitForEvent('dialog') |
| Parallelization | Parallelizable + ThreadLocal | fullyParallel + workers |

## Step-by-Step Conversion
1. Replace `.csproj`/NuGet setup with `package.json` + TypeScript config.
2. Move runtime configuration from `app.runsettings` to environment variables.
3. Replace browser factory with `src/factories/playwrightFactory.ts`.
4. Replace base class setup/teardown with `src/fixtures/basePlaywrightTest.ts`.
5. Convert Selenium helper methods to `src/extensions/playwrightExtensions.ts`.
6. Keep JSON flow with `src/helpers/jsonReader.ts` and migrate tests incrementally.

## TypeScript Foundation Files Included
- `src/config/playwrightConfig.ts`
- `src/factories/playwrightFactory.ts`
- `src/fixtures/basePlaywrightTest.ts`
- `src/extensions/playwrightExtensions.ts`
- `playwright.config.ts`

## BrowserStack Integration
Environment variables:
- `BROWSER_OPTION=bs_chrome|bs_edge`
- `BROWSERSTACK_USERNAME`
- `BROWSERSTACK_ACCESS_KEY`
- `BROWSERSTACK_CDP_URL` (default: `wss://cdp.browserstack.com/playwright`)

Session naming uses the current test title.

## Parallel Execution
- `playwright.config.ts`: `fullyParallel: true`
- Worker count controlled by `PLAYWRIGHT_WORKERS`.

## Before/After Samples

### PersonalDetailsForm Conversion
**Before (Selenium C#)**
```csharp
_driver.FindElement(By.Id("firstName")).SendKeys(firstName);
_driver.SwitchTo().Frame(_driver.FindElement(By.Id("genderFrame")));
_driver.FindElement(By.Id("genderMale")).Click();
```

**After (Playwright TS)**
```ts
await page.locator('#firstName').fill(firstName);
await page.frameLocator('iframe#genderFrame').locator('#genderMale').click();
```

### T386672 Test Conversion
**Before (Selenium C# NUnit)**
```csharp
var data = JsonReader.FetchData<BookingData>("Neonatal/T386672_Employee_Books_Neonatal");
var page = new PersonalDetailsForm(driver);
page.Fill(data.Employee.FirstName);
```

**After (Playwright TypeScript)**
```ts
const data = fetchData<EmployeeBookingData>('Neonatal/T386672_Employee_Books_Neonatal');
const form = new PersonalDetailsFormPlaywright(page);
await form.fill(data.employee);
```

### Setup Pattern Conversion
**Before**: NUnit `[SetUp]/[TearDown]` + `ThreadLocal<IWebDriver>`.

**After**: Playwright Test fixture extension in `basePlaywrightTest.ts` returning `config`, `session`, and `page` per test.

## Locator Conversion Reference
| Selenium | Playwright TS |
|---|---|
| `By.Id("id")` | `page.locator('#id')` |
| `By.XPath("//div")` | `page.locator('xpath=//div')` |
| `By.ClassName("btn")` | `page.locator('.btn')` |
| `SelectElement` | `page.locator('select').selectOption()` |
| `FindElement` | `page.locator()` |
| `FindElements` | `page.locator().all()` |

## Environment Template
Create `.env`:
```bash
BROWSER_OPTION=chrome
PAGE_URL=https://your-app-url
DEFAULT_TIMEOUT_MS=60000
POLLING_INTERVAL_MS=1000
PLAYWRIGHT_WORKERS=4
TEST_DATA_PATH=TestData
BROWSERSTACK_USERNAME=
BROWSERSTACK_ACCESS_KEY=
BROWSERSTACK_CDP_URL=wss://cdp.browserstack.com/playwright
```

## Required Packages
`package.json` includes:
- `@playwright/test`
- `typescript`
- `@types/node`

## Common Pitfalls
- Mixing old Selenium sync patterns with Playwright async APIs.
- Keeping brittle explicit waits instead of locator assertions.
- Not setting BrowserStack credentials for `bs_*` runs.
- Forgetting `npm run install:browsers` in new environments.
