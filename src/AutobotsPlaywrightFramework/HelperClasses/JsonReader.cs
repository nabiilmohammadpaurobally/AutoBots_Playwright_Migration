using Newtonsoft.Json;

namespace AutobotsPlaywrightFramework.HelperClasses;

/// <summary>
/// Reads strongly typed JSON files from the configured test data path.
/// </summary>
public static class JsonReader
{
    /// <summary>
    /// Fetches and deserializes JSON data from a file name (without extension) or relative path.
    /// </summary>
    /// <typeparam name="T">Target model type.</typeparam>
    /// <param name="jsonName">File name without extension, or relative path under the test data folder.</param>
    public static T FetchData<T>(string jsonName)
    {
        var filePath = Path.Combine(GetTestDataPath.GetPath(), $"{jsonName}.json");
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"JSON file not found: {filePath}");
        }

        var jsonContent = File.ReadAllText(filePath);
        var data = JsonConvert.DeserializeObject<T>(jsonContent);
        return data is not null ? data : throw new NullReferenceException("JSON data is null");
    }
}
