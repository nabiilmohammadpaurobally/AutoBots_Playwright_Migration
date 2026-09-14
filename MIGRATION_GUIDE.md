# Selenium to Playwright C# Migration Guide (AutoBots)

## Executive Summary
This starter package migrates the AutoBots Selenium NUnit framework to Playwright NUnit while preserving:
- JSON-driven test data flow
- ExtentReports reporting
- Parallel execution
- BrowserStack + local browser execution

**Estimated effort:** **4-6 weeks** for phased, low-risk migration.

## 6-Phase Migration Plan
1. **Foundation Setup (Week 1)**
   - Add Playwright packages/configuration
   - Add `PlaywrightFactory`, `PlaywrightConfig`, `BasePlaywrightTest`, `PlaywrightExtensions`
2. **Core Utility Conversion (Week 1-2)**
   - Convert Selenium extension helpers to Playwright async helpers
3. **Page Object Conversion (Week 2-3)**
   - Replace `IWebDriver`/`By` usage with `IPage`/locator strings
4. **Test Class Conversion (Week 3-4)**
   - Move to async setup/teardown fixture pattern
5. **BrowserStack + Parallel Hardening (Week 4-5)**
   - Validate local/cloud browser matrix and thread-safe execution
6. **Regression + Stabilization (Week 5-6)**
   - Run target suites, tune timeouts, fix flaky locators

## Architecture Comparison
| Area | Selenium | Playwright |
|---|---|---|
| Driver lifecycle | `IWebDriver` | `IPlaywright + IBrowser + IBrowserContext + IPage` |
| Wait strategy | Explicit waits + custom smart waits | Auto-waiting + load state waits |
| Locators | `By.Id`, `By.XPath` | CSS/XPath/text locators via `page.Locator(...)` |
| iFrames | `SwitchTo().Frame(...)` | `page.FrameLocator(...)` |
| Alerts | `SwitchTo().Alert()` | `page.Dialog` event |
| Parallel model | `ThreadLocal<IWebDriver>` | Thread-local session with context/page |

## Step-by-Step Conversion Process
1. Add Playwright + NUnit packages to `.csproj`.
2. Add `PlaywrightConfig` to load `app.runsettings` values from `TestContext.Parameters`.
3. Replace browser factory with `PlaywrightFactory` (local Chrome/Edge + BrowserStack).
4. Replace base test class with async `BasePlaywrightTest`.
5. Convert extension methods from `IWebDriver` to `IPage` async extensions.
6. Convert page objects and tests incrementally by module.
7. Run targeted tests after each module conversion.

## BrowserStack Integration (Playwright)
- Use BrowserStack Playwright CDP endpoint (`ConnectionString`) with encoded capabilities.
- Pass session name as current NUnit test name.
- Required settings:
  - `BrowserStackUserName`
  - `BrowserStackAccessKey`
  - `ConnectionString`

## Parallel Execution Setup
- Use `[assembly: Parallelizable(ParallelScope.Fixtures)]`
- Use `[assembly: LevelOfParallelism(4)]` (tune per CI capacity)
- Keep runtime objects in thread-local storage in base fixture.

## Common Pitfalls and Solutions
- **Pitfall:** Mixing sync + async test code.  
  **Fix:** Use async `[SetUp]`, `[TearDown]`, and `Task` test methods.
- **Pitfall:** brittle waits copied from Selenium.  
  **Fix:** Prefer Playwright auto-wait + `WaitForLoadStateAsync`.
- **Pitfall:** BrowserStack credentials missing in runsettings.  
  **Fix:** validate config early with `ValidateOrThrow()`.
- **Pitfall:** stale element assumptions.  
  **Fix:** re-query using locators instead of storing element snapshots.

---

## Before/After Code Samples

### 1) PersonalDetailsForm Conversion

#### Before (Selenium)
```csharp
public class PersonalDetailsForm
{
    private readonly IWebDriver _driver;
    private readonly By FirstName = By.Id("firstName");
    private readonly By GenderFrame = By.XPath("//iframe[@id='genderFrame']");

    public PersonalDetailsForm(IWebDriver driver) => _driver = driver;

    public void Fill(string firstName)
    {
        _driver.WaitUntilVisible(FirstName).SendKeys(firstName);
        _driver.SwitchTo().Frame(_driver.FindElement(GenderFrame));
        _driver.FindElement(By.Id("genderMale")).Click();
        _driver.SwitchTo().DefaultContent();
    }
}
```

#### After (Playwright)
```csharp
public sealed class PersonalDetailsForm
{
    private readonly IPage _page;
    private const string FirstName = "#firstName";
    private const string GenderFrame = "iframe#genderFrame";

    public PersonalDetailsForm(IPage page) => _page = page;

    public async Task FillAsync(string firstName)
    {
        await (await _page.WaitUntilVisible(FirstName)).FillAsync(firstName);
        await _page.FrameLocator(GenderFrame).Locator("#genderMale").ClickAsync();
    }
}
```

### 2) T386672_Employee_Books_Neonatal Test Conversion

#### Before (Selenium)
```csharp
[TestFixture]
public class T386672_Employee_Books_Neonatal : BaseClass
{
    [Test]
    public void Validate_Booking()
    {
        var data = JsonReader.FetchData<BookingData>("Neonatal/T386672_Employee_Books_Neonatal");
        var page = new PersonalDetailsForm(Driver);
        page.Fill(data.Employee.FirstName);
    }
}
```

#### After (Playwright)
```csharp
[TestFixture]
public class T386672_Employee_Books_Neonatal : BasePlaywrightTest
{
    [Test]
    public async Task Validate_Booking()
    {
        var data = JsonReader.FetchData<BookingData>("Neonatal/T386672_Employee_Books_Neonatal"); // unchanged
        var pageObject = new PersonalDetailsForm(Page);
        await pageObject.FillAsync(data.Employee.FirstName);
    }
}
```

### 3) Setup Pattern Conversion

#### Before (Selenium ThreadLocal)
```csharp
[SetUp]
public void SetUp()
{
    Driver = BrowserFactory.GetBrowser(TestContext.Parameters["BrowserOption"]);
}

[TearDown]
public void TearDown()
{
    Driver?.Quit();
}
```

#### After (Playwright async + context)
```csharp
[SetUp]
public async Task SetUpAsync()
{
    Config = PlaywrightConfig.FromTestContext();
    Session = await PlaywrightFactory.GetBrowserAsync(Config.BrowserType, Config);
}

[TearDown]
public async Task TearDownAsync()
{
    await Session.Page.CloseAsync();
    await Session.Context.CloseAsync();
    await Session.Browser.CloseAsync();
    Session.Playwright.Dispose();
}
```

---

## Locator Conversion Reference
| Selenium | Playwright |
|---|---|
| `By.Id("id")` | `page.Locator("#id")` |
| `By.XPath("//div")` | `page.Locator("xpath=//div")` |
| `By.ClassName("btn")` | `page.Locator(".btn")` |
| `new SelectElement(element).SelectByText("A")` | `page.SelectOptionAsync("#select", new() { Label = "A" })` |
| `driver.FindElement(by)` | `page.Locator("...")` |
| `driver.FindElements(by)` | `page.Locator("...").AllAsync()` |

---

## Playwright app.runsettings Template
```xml
<?xml version="1.0" encoding="utf-8"?>
<RunSettings>
  <RunConfiguration>
    <MaxCpuCount>4</MaxCpuCount>
    <TargetFrameworkVersion>net8.0</TargetFrameworkVersion>
  </RunConfiguration>
  <TestRunParameters>
    <Parameter name="BrowserOption" value="chrome" />
    <Parameter name="PageUrl" value="https://your-app-url" />
    <Parameter name="ConnectionString" value="wss://cdp.browserstack.com/playwright" />
    <Parameter name="BrowserStackUserName" value="${BROWSERSTACK_USERNAME}" />
    <Parameter name="BrowserStackAccessKey" value="${BROWSERSTACK_ACCESS_KEY}" />
    <Parameter name="DefaultTimeoutMs" value="60000" />
    <Parameter name="PollingIntervalMs" value="1000" />
    <Parameter name="ParallelScope" value="Fixtures" />
    <Parameter name="LevelOfParallelism" value="4" />
  </TestRunParameters>
</RunSettings>
```

## .csproj NuGet Update (Required)
> Note: `AventStack.ExtentReports` namespace is delivered by the `ExtentReports` NuGet package.

```xml
<ItemGroup>
  <PackageReference Include="Microsoft.Playwright" Version="1.54.0" />
  <PackageReference Include="NUnit" Version="3.14.0" />
  <PackageReference Include="NUnit3TestAdapter" Version="4.6.0" />
  <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.1" />
  <PackageReference Include="ExtentReports" Version="5.0.4" />
  <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
</ItemGroup>
```
