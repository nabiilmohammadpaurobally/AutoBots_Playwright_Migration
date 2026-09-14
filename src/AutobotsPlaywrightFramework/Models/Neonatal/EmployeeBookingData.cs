namespace AutobotsPlaywrightFramework.Models.Neonatal;

/// <summary>
/// Sample model used by migrated Neonatal booking test data.
/// </summary>
public sealed class EmployeeBookingData
{
    /// <summary>
    /// Gets or sets employee details.
    /// </summary>
    public required EmployeeDetails Employee { get; init; }

    /// <summary>
    /// Gets or sets expected alert messages.
    /// </summary>
    public List<string> ExpectedAlerts { get; init; } = [];
}

/// <summary>
/// Employee details for booking flow.
/// </summary>
public sealed class EmployeeDetails
{
    /// <summary>
    /// Gets or sets first name.
    /// </summary>
    public required string FirstName { get; init; }

    /// <summary>
    /// Gets or sets surname.
    /// </summary>
    public required string Surname { get; init; }

    /// <summary>
    /// Gets or sets email.
    /// </summary>
    public required string Email { get; init; }
}
