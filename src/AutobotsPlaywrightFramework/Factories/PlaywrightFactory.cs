using System.Text;
using System.Text.Json;
using AutobotsPlaywrightFramework.Configurations;
using Microsoft.Playwright;
using NUnit.Framework;

namespace AutobotsPlaywrightFramework.Factories;

/// <summary>
/// Factory used to initialize local and BrowserStack Playwright sessions.
/// </summary>
public static class PlaywrightFactory
{
    /// <summary>
    /// Creates a fully initialized Playwright session using runsettings-backed configuration.
    /// </summary>
    /// <param name="browserName">The browser option from run settings.</param>
    public static Task<PlaywrightSession> GetBrowserAsync(string browserName)
    {
        var config = PlaywrightConfig.FromTestContext();
        config.ValidateOrThrow();
        return GetBrowserAsync(browserName, config, TestContext.CurrentContext.Test.Name);
    }

    /// <summary>
    /// Creates a fully initialized Playwright session based on run settings.
    /// </summary>
    /// <param name="browserName">The browser option from run settings.</param>
    /// <param name="config">Runtime Playwright configuration.</param>
    /// <param name="testName">Optional test name for BrowserStack session naming.</param>
    public static async Task<PlaywrightSession> GetBrowserAsync(string browserName, PlaywrightConfig config, string? testName = null)
    {
        ArgumentNullException.ThrowIfNull(config);

        var playwright = await Microsoft.Playwright.Playwright.CreateAsync().ConfigureAwait(false);
        IBrowser browser = await CreateBrowserAsync(playwright, browserName, config, testName).ConfigureAwait(false);

        var context = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            IgnoreHTTPSErrors = true,
            ViewportSize = new ViewportSize { Width = 1920, Height = 1080 }
        }).ConfigureAwait(false);

        var page = await context.NewPageAsync().ConfigureAwait(false);
        page.SetDefaultTimeout(config.DefaultTimeoutMs);

        return new PlaywrightSession(playwright, browser, context, page);
    }

    private static Task<IBrowser> CreateBrowserAsync(IPlaywright playwright, string browserName, PlaywrightConfig config, string? testName)
    {
        return browserName.ToLowerInvariant() switch
        {
            "chrome" => playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Channel = "chrome",
                Headless = false
            }),
            "edge" => playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Channel = "msedge",
                Headless = false
            }),
            "bs_chrome" => ConnectBrowserStackAsync(playwright, "chrome", config, testName),
            "bs_edge" => ConnectBrowserStackAsync(playwright, "edge", config, testName),
            _ => throw new ArgumentOutOfRangeException(nameof(browserName), browserName, "Unsupported browser option.")
        };
    }

    private static Task<IBrowser> ConnectBrowserStackAsync(IPlaywright playwright, string browserName, PlaywrightConfig config, string? testName)
    {
        var capabilities = new Dictionary<string, object?>
        {
            ["browser"] = browserName,
            ["browser_version"] = "latest",
            ["os"] = "Windows",
            ["os_version"] = "11",
            ["name"] = testName ?? TestContext.CurrentContext.Test.Name,
            ["build"] = $"AutoBots Playwright Migration - {DateTime.UtcNow:yyyyMMdd}",
            ["browserstack.username"] = config.BrowserStackUserName,
            ["browserstack.accessKey"] = config.BrowserStackAccessKey
        };

        var capabilitiesJson = JsonSerializer.Serialize(capabilities);
        var encodedCaps = Convert.ToBase64String(Encoding.UTF8.GetBytes(capabilitiesJson));
        var endpoint = $"{config.ConnectionString}?caps={encodedCaps}";

        return playwright.Chromium.ConnectAsync(endpoint);
    }
}

/// <summary>
/// Encapsulates Playwright runtime objects for a test session.
/// </summary>
public sealed record PlaywrightSession(
    IPlaywright Playwright,
    IBrowser Browser,
    IBrowserContext Context,
    IPage Page);
