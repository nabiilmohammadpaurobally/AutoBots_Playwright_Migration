using AutobotsPlaywrightFramework.Extensions;
using AutobotsPlaywrightFramework.Models.Neonatal;
using Microsoft.Playwright;

namespace AutobotsPlaywrightFramework.PageObjects.Neonatal;

/// <summary>
/// Playwright page object for the Personal Details form.
/// </summary>
public sealed class PersonalDetailsFormPlaywright
{
    private readonly IPage _page;

    private const string FirstNameTextBox = "#firstName";
    private const string SurnameTextBox = "#surname";
    private const string EmailTextBox = "#email";
    private const string GenderFrame = "iframe#genderFrame";
    private const string MaleOption = "#genderMale";
    private const string ContinueButton = "#continue";

    /// <summary>
    /// Initializes a new instance of the <see cref="PersonalDetailsFormPlaywright"/> class.
    /// </summary>
    public PersonalDetailsFormPlaywright(IPage page)
    {
        _page = page;
    }

    /// <summary>
    /// Fills employee personal details.
    /// </summary>
    public async Task FillAsync(EmployeeDetails employee)
    {
        await (await _page.WaitUntilVisible(FirstNameTextBox).ConfigureAwait(false)).FillAsync(employee.FirstName).ConfigureAwait(false);
        await (await _page.WaitUntilVisible(SurnameTextBox).ConfigureAwait(false)).FillAsync(employee.Surname).ConfigureAwait(false);
        await (await _page.WaitUntilVisible(EmailTextBox).ConfigureAwait(false)).FillAsync(employee.Email).ConfigureAwait(false);
        await _page.FrameLocator(GenderFrame).Locator(MaleOption).ClickAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Submits the form and accepts the confirmation alert.
    /// </summary>
    public Task SubmitAndAcceptAlertAsync()
    {
        return _page.WaitForAlertAndAccept(() => _page.Locator(ContinueButton).ClickAsync());
    }
}
