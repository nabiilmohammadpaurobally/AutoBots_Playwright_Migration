using AutobotsPlaywrightFramework.HelperClasses;
using AutobotsPlaywrightFramework.Models.Neonatal;
using NUnit.Framework;

namespace AutobotsPlaywrightFramework.Tests;

[TestFixture]
public class JsonReaderTests
{
    [Test]
    public void FetchData_ReturnsTypedModel_WhenJsonExists()
    {
        var data = JsonReader.FetchData<EmployeeBookingData>("Neonatal/T386672_Employee_Books_Neonatal");

        Assert.That(data.Employee.FirstName, Is.EqualTo("John"));
        Assert.That(data.ExpectedAlerts, Is.Not.Empty);
    }

    [Test]
    public void FetchData_Throws_WhenJsonDoesNotExist()
    {
        Assert.Throws<FileNotFoundException>(() => JsonReader.FetchData<EmployeeBookingData>("Neonatal/does_not_exist"));
    }
}
