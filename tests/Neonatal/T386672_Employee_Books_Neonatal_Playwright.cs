using AutobotsPlaywrightFramework.HelperClasses;
using AutobotsPlaywrightFramework.Models.Neonatal;
using AutobotsPlaywrightFramework.PageObjects.Neonatal;
using AutobotsPlaywrightFramework.TestBase;
using NUnit.Framework;

namespace AutobotsPlaywrightFramework.Tests.Neonatal;

/// <summary>
/// Migrated Playwright sample for T386672 Employee Books Neonatal scenario.
/// </summary>
[TestFixture]
[Explicit("Sample migration test. Requires configured AUT URL and selectors.")]
public sealed class T386672_Employee_Books_Neonatal_Playwright : BasePlaywrightTest
{
    /// <summary>
    /// Verifies employee booking using JSON data with the migrated Playwright flow.
    /// </summary>
    [Test]
    public async Task Validate_Employee_Booking_With_Playwright()
    {
        var data = JsonReader.FetchData<EmployeeBookingData>("Neonatal/T386672_Employee_Books_Neonatal");

        var form = new PersonalDetailsFormPlaywright(Page);
        await form.FillAsync(data.Employee).ConfigureAwait(false);
        await form.SubmitAndValidateAlertAsync(data.ExpectedAlerts).ConfigureAwait(false);
    }
}
