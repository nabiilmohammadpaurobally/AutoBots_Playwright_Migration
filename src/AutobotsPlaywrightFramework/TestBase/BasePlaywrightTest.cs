using System.Globalization;
using AutobotsPlaywrightFramework.Configurations;
using AutobotsPlaywrightFramework.Factories;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using Microsoft.Playwright;
using NUnit.Framework;

namespace AutobotsPlaywrightFramework.TestBase;

/// <summary>
/// Base test class for Playwright NUnit tests with parallel-safe session handling.
/// </summary>
[Parallelizable(ParallelScope.Fixtures)]
public abstract class BasePlaywrightTest
{
    private static readonly AsyncLocal<PlaywrightSession?> Session = new();
    private static readonly AsyncLocal<ExtentTest?> ExtentNode = new();

    private static ExtentReports? _extentReports;
    private static readonly object ExtentLock = new();

    /// <summary>
    /// Gets the current page instance for the executing test thread.
    /// </summary>
    protected IPage Page => Session.Value?.Page
        ?? throw new InvalidOperationException("Playwright session is not initialized.");

    /// <summary>
    /// Gets the current browser context for the executing test thread.
    /// </summary>
    protected IBrowserContext Context => Session.Value?.Context
        ?? throw new InvalidOperationException("Playwright session is not initialized.");

    /// <summary>
    /// Gets the loaded Playwright configuration.
    /// </summary>
    protected PlaywrightConfig Config { get; private set; } = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-GB");

        lock (ExtentLock)
        {
            if (_extentReports is not null)
            {
                return;
            }

            var reportPath = Path.Combine(TestContext.CurrentContext.WorkDirectory, "ExtentReport.html");
            var reporter = new ExtentSparkReporter(reportPath);
            _extentReports = new ExtentReports();
            _extentReports.AttachReporter(reporter);
        }
    }

    /// <summary>
    /// Initializes Playwright browser, context and page before each test.
    /// </summary>
    [SetUp]
    public async Task SetUpAsync()
    {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-GB");

        Config = PlaywrightConfig.FromTestContext();
        Config.ValidateOrThrow();

        Session.Value = await PlaywrightFactory.GetBrowserAsync(
            Config.BrowserType,
            Config,
            TestContext.CurrentContext.Test.Name).ConfigureAwait(false);

        lock (ExtentLock)
        {
            ExtentNode.Value = _extentReports?.CreateTest(TestContext.CurrentContext.Test.Name);
        }

        await Page.GotoAsync(Config.PageUrl, new PageGotoOptions
        {
            WaitUntil = WaitUntilState.DOMContentLoaded
        }).ConfigureAwait(false);
    }

    /// <summary>
    /// Closes all Playwright resources and tracks pass/fail in ExtentReports.
    /// </summary>
    [TearDown]
    public async Task TearDownAsync()
    {
        lock (ExtentLock)
        {
            if (ExtentNode.Value is not null)
            {
                if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Passed)
                {
                    ExtentNode.Value.Pass("Test passed");
                }
                else
                {
                    var error = TestContext.CurrentContext.Result.Message;
                    ExtentNode.Value.Fail($"Test failed: {error}");
                }
            }
        }

        if (Session.Value is not null)
        {
            await Session.Value.Page.CloseAsync().ConfigureAwait(false);
            await Session.Value.Context.CloseAsync().ConfigureAwait(false);
            await Session.Value.Browser.CloseAsync().ConfigureAwait(false);
            Session.Value.Playwright.Dispose();
            Session.Value = null;
        }
        lock (ExtentLock)
        {
            _extentReports?.Flush();
        }
    }
}
