using NUnit.Framework;

namespace AutobotsPlaywrightFramework.HelperClasses;

/// <summary>
/// Resolves the test data root folder for JSON-driven tests.
/// </summary>
public static class GetTestDataPath
{
    /// <summary>
    /// Returns the configured test data directory.
    /// </summary>
    public static string GetPath()
    {
        var configuredPath = TestContext.Parameters["TestDataPath"];
        if (!string.IsNullOrWhiteSpace(configuredPath))
        {
            return configuredPath;
        }

        return Path.Combine(TestContext.CurrentContext.WorkDirectory, "TestData");
    }
}
