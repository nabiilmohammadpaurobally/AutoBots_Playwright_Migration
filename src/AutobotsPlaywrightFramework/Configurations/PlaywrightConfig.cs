using NUnit.Framework;

namespace AutobotsPlaywrightFramework.Configurations;

/// <summary>
/// Represents Playwright execution settings loaded from NUnit runsettings.
/// </summary>
public sealed class PlaywrightConfig
{
    /// <summary>
    /// Gets the browser option selected for execution.
    /// Supported values: chrome, edge, bs_chrome, bs_edge.
    /// </summary>
    public required string BrowserType { get; init; }

    /// <summary>
    /// Gets the application base URL under test.
    /// </summary>
    public required string PageUrl { get; init; }

    /// <summary>
    /// Gets the BrowserStack or CDP connection string endpoint.
    /// </summary>
    public required string ConnectionString { get; init; }

    /// <summary>
    /// Gets the BrowserStack username.
    /// </summary>
    public required string BrowserStackUserName { get; init; }

    /// <summary>
    /// Gets the BrowserStack access key.
    /// </summary>
    public required string BrowserStackAccessKey { get; init; }

    /// <summary>
    /// Gets BrowserStack credentials in username:accessKey format.
    /// </summary>
    public string BrowserStackCredentials => $"{BrowserStackUserName}:{BrowserStackAccessKey}";

    /// <summary>
    /// Gets the default timeout in milliseconds.
    /// </summary>
    public int DefaultTimeoutMs { get; init; } = 60000;

    /// <summary>
    /// Gets the polling interval in milliseconds used by custom waits.
    /// </summary>
    public int PollingIntervalMs { get; init; } = 1000;

    /// <summary>
    /// Creates configuration from NUnit run settings parameters.
    /// </summary>
    public static PlaywrightConfig FromTestContext()
    {
        var parameters = TestContext.Parameters;

        return new PlaywrightConfig
        {
            BrowserType = parameters["BrowserOption"] ?? "chrome",
            PageUrl = parameters["PageUrl"] ?? string.Empty,
            ConnectionString = parameters["ConnectionString"] ?? "wss://cdp.browserstack.com/playwright",
            BrowserStackUserName = parameters["BrowserStackUserName"] ?? string.Empty,
            BrowserStackAccessKey = parameters["BrowserStackAccessKey"] ?? string.Empty,
            DefaultTimeoutMs = TryParseInt(parameters["DefaultTimeoutMs"], 60000),
            PollingIntervalMs = TryParseInt(parameters["PollingIntervalMs"], 1000)
        };
    }

    /// <summary>
    /// Ensures the loaded configuration is valid for the selected runtime.
    /// </summary>
    public void ValidateOrThrow()
    {
        if (string.IsNullOrWhiteSpace(BrowserType))
        {
            throw new InvalidOperationException("BrowserOption must be provided.");
        }

        if (string.IsNullOrWhiteSpace(PageUrl))
        {
            throw new InvalidOperationException("PageUrl must be provided.");
        }

        if (IsBrowserStackExecution())
        {
            if (string.IsNullOrWhiteSpace(BrowserStackUserName) || string.IsNullOrWhiteSpace(BrowserStackAccessKey))
            {
                throw new InvalidOperationException("BrowserStackUserName and BrowserStackAccessKey are required for BrowserStack execution.");
            }

            if (string.IsNullOrWhiteSpace(ConnectionString))
            {
                throw new InvalidOperationException("ConnectionString must be provided for BrowserStack execution.");
            }
        }
    }

    /// <summary>
    /// Indicates whether BrowserStack execution is requested.
    /// </summary>
    public bool IsBrowserStackExecution() =>
        BrowserType.Equals("bs_chrome", StringComparison.OrdinalIgnoreCase)
        || BrowserType.Equals("bs_edge", StringComparison.OrdinalIgnoreCase);

    private static int TryParseInt(string? value, int defaultValue)
    {
        return int.TryParse(value, out var parsed) && parsed > 0 ? parsed : defaultValue;
    }
}
