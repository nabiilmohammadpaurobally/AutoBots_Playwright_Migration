namespace AutobotsPlaywrightFramework.Configurations;

/// <summary>
/// Central NUnit settings for Playwright execution.
/// </summary>
public static class NUnitConfig
{
    /// <summary>
    /// Default timeout applied to action operations.
    /// </summary>
    public const int DefaultTimeoutMs = 60000;

    /// <summary>
    /// Default explicit wait timeout for custom extension methods.
    /// </summary>
    public const int ExplicitWaitTimeoutMs = 60000;

    /// <summary>
    /// Default polling interval used by custom waits.
    /// </summary>
    public const int PollingIntervalMs = 1000;
}
