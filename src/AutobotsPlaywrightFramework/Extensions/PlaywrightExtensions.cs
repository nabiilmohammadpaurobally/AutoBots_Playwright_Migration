using Microsoft.Playwright;
using NUnit.Framework;

namespace AutobotsPlaywrightFramework.Extensions;

/// <summary>
/// Extension methods that mirror Selenium helper behavior using Playwright APIs.
/// </summary>
public static class PlaywrightExtensions
{
    /// <summary>
    /// Waits until the locator is visible.
    /// </summary>
    public static async Task<ILocator> WaitUntilVisible(this IPage page, string locator, int secondsTimeout = 60, int secondsPollingInterval = 1)
    {
        var element = page.Locator(locator);
        await WaitUntilAsync(async () => await element.IsVisibleAsync().ConfigureAwait(false), secondsTimeout, secondsPollingInterval).ConfigureAwait(false);
        return element;
    }

    /// <summary>
    /// Waits until the locator is enabled.
    /// </summary>
    public static async Task<ILocator> WaitUntilClickable(this IPage page, string locator, int secondsTimeout = 60, int secondsPollingInterval = 1)
    {
        var element = page.Locator(locator);
        await WaitUntilAsync(async () => await element.IsVisibleAsync().ConfigureAwait(false) && await element.IsEnabledAsync().ConfigureAwait(false), secondsTimeout, secondsPollingInterval).ConfigureAwait(false);
        return element;
    }

    /// <summary>
    /// Waits until the locator is visible and contains the expected text.
    /// </summary>
    public static async Task<ILocator> WaitUntilTextVisible(this IPage page, string locator, string expectedText, int secondsTimeout = 60, int secondsPollingInterval = 1)
    {
        var element = page.Locator(locator);
        await WaitUntilAsync(async () =>
        {
            if (!await element.IsVisibleAsync().ConfigureAwait(false))
            {
                return false;
            }

            var currentText = (await element.InnerTextAsync().ConfigureAwait(false)).Trim();
            return currentText.Contains(expectedText, StringComparison.OrdinalIgnoreCase);
        }, secondsTimeout, secondsPollingInterval).ConfigureAwait(false);

        return element;
    }

    /// <summary>
    /// Waits until the locator is no longer visible or attached.
    /// </summary>
    public static Task WaitUntilElementNotFound(this IPage page, string locator, int secondsTimeout = 60)
    {
        return page.WaitForSelectorAsync(locator, new PageWaitForSelectorOptions
        {
            State = WaitForSelectorState.Hidden,
            Timeout = secondsTimeout * 1000
        });
    }

    /// <summary>
    /// Scrolls to the supplied locator and waits briefly.
    /// </summary>
    public static async Task ScrollToElementWait(this IPage page, string locator, int millisecondsToWaitFor = 500)
    {
        var element = page.Locator(locator);
        await element.ScrollIntoViewIfNeededAsync().ConfigureAwait(false);
        await page.WaitForTimeoutAsync(millisecondsToWaitFor).ConfigureAwait(false);
    }

    /// <summary>
    /// Clicks an element using JavaScript.
    /// </summary>
    public static Task JavaScriptClick(this IPage page, string locator)
    {
        return page.EvaluateAsync("selector => document.querySelector(selector)?.dispatchEvent(new MouseEvent('click', { bubbles: true }))", locator);
    }

    /// <summary>
    /// Clicks a locator directly with Playwright.
    /// </summary>
    public static Task ClickHandleAlert(this IPage page, string locator)
    {
        return page.Locator(locator).ClickAsync();
    }

    /// <summary>
    /// Waits for an alert/dialog and accepts it.
    /// </summary>
    public static async Task WaitForAlertAndAccept(this IPage page, int timeoutInSeconds = 10)
    {
        var dialog = await WaitForDialogAsync(page, timeoutInSeconds).ConfigureAwait(false);
        await dialog.AcceptAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Scrolls to the element and clicks.
    /// </summary>
    public static async Task ScrollToElementWaitAndClick(this IPage page, string locator)
    {
        await page.ScrollToElementWait(locator).ConfigureAwait(false);
        await page.Locator(locator).ClickAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Scrolls to the element and JavaScript-clicks.
    /// </summary>
    public static async Task ScrollToElementWaitAndJavaScriptClick(this IPage page, string locator)
    {
        await page.ScrollToElementWait(locator).ConfigureAwait(false);
        await page.JavaScriptClick(locator).ConfigureAwait(false);
        await page.SmartWaitPageLoader().ConfigureAwait(false);
    }

    /// <summary>
    /// Scrolls to the element, JavaScript-clicks and validates dialog text.
    /// </summary>
    public static async Task ScrollToElementWaitAndJavaScriptClick(this IPage page, string locator, List<string> alertMessage)
    {
        await page.ScrollToElementWait(locator).ConfigureAwait(false);
        await page.JavaScriptClick(locator).ConfigureAwait(false);
        await page.WaitForAlertContainsAndAccept(alertMessage).ConfigureAwait(false);
        await page.SmartWaitPageLoader().ConfigureAwait(false);
    }

    /// <summary>
    /// Scrolls to the element, clicks, validates dialog text and accepts.
    /// </summary>
    public static async Task ScrollToElementAndJavaScriptClickAlert(this IPage page, string locator, List<string> alertMessage)
    {
        await page.ScrollToElementWait(locator).ConfigureAwait(false);
        await page.ClickHandleAlert(locator).ConfigureAwait(false);
        await page.WaitForAlertContainsAndAccept(alertMessage).ConfigureAwait(false);
        await page.SmartWaitPageLoader().ConfigureAwait(false);
    }

    /// <summary>
    /// Scrolls to the element and JavaScript-clicks without smart wait.
    /// </summary>
    public static async Task ScrollToElementWaitAndJavaScriptClickWithoutSmartWait(this IPage page, string locator)
    {
        await page.ScrollToElementWait(locator).ConfigureAwait(false);
        await page.JavaScriptClick(locator).ConfigureAwait(false);
    }

    /// <summary>
    /// Determines whether a locator exists and is visible.
    /// </summary>
    public static async Task<bool> IsElementPresent(this IPage page, string locator)
    {
        await page.SmartWaitPageLoader().ConfigureAwait(false);
        return await page.Locator(locator).CountAsync().ConfigureAwait(false) > 0;
    }

    /// <summary>
    /// Determines whether a dialog appears within the timeout.
    /// </summary>
    public static async Task<bool> IsAlertPresent(this IPage page, TimeSpan timeout)
    {
        var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        async void Handler(object? _, IDialog dialog)
        {
            await dialog.DismissAsync().ConfigureAwait(false);
            tcs.TrySetResult(true);
        }

        page.Dialog += Handler;
        try
        {
            var delayTask = Task.Delay(timeout);
            var completed = await Task.WhenAny(tcs.Task, delayTask).ConfigureAwait(false);
            return completed == tcs.Task && tcs.Task.Result;
        }
        finally
        {
            page.Dialog -= Handler;
        }
    }

    /// <summary>
    /// Waits for the page and network to finish loading.
    /// </summary>
    public static async Task SmartWaitPageLoader(this IPage page, int timeoutInSeconds = 60)
    {
        var timeout = timeoutInSeconds * 1000;
        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded, new PageWaitForLoadStateOptions { Timeout = timeout }).ConfigureAwait(false);
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions { Timeout = timeout }).ConfigureAwait(false);
    }

    /// <summary>
    /// Explicit static wait in seconds.
    /// </summary>
    public static Task StaticWait(int seconds)
    {
        if (seconds < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(seconds), "Wait time must be non-negative.");
        }

        return Task.Delay(seconds * 1000);
    }

    /// <summary>
    /// Waits for an alert, validates message and accepts it.
    /// </summary>
    public static async Task WaitForAlertValidateMessageAndAccept(this IPage page, string expectedMessage, int timeoutInSeconds = 10)
    {
        var dialog = await WaitForDialogAsync(page, timeoutInSeconds).ConfigureAwait(false);
        Assert.That(dialog.Message, Does.Contain(expectedMessage), $"Expected alert to contain: '{expectedMessage}' but was: '{dialog.Message}'");
        await dialog.AcceptAsync().ConfigureAwait(false);
    }

    private static async Task WaitForAlertContainsAndAccept(this IPage page, IReadOnlyCollection<string> expectedMessages, int timeoutInSeconds = 10)
    {
        var dialog = await WaitForDialogAsync(page, timeoutInSeconds).ConfigureAwait(false);
        Assert.That(expectedMessages.Any(m => dialog.Message.Contains(m, StringComparison.OrdinalIgnoreCase)),
            $"Expected dialog message to contain one of [{string.Join(", ", expectedMessages)}], but was '{dialog.Message}'.");
        await dialog.AcceptAsync().ConfigureAwait(false);
    }

    private static async Task<IDialog> WaitForDialogAsync(IPage page, int timeoutInSeconds)
    {
        var tcs = new TaskCompletionSource<IDialog>(TaskCreationOptions.RunContinuationsAsynchronously);
        void Handler(object? _, IDialog dialog) => tcs.TrySetResult(dialog);

        page.Dialog += Handler;
        try
        {
            var completed = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(timeoutInSeconds))).ConfigureAwait(false);
            if (completed != tcs.Task)
            {
                throw new TimeoutException("No alert appeared within the specified timeout.");
            }

            return await tcs.Task.ConfigureAwait(false);
        }
        finally
        {
            page.Dialog -= Handler;
        }
    }

    private static async Task WaitUntilAsync(Func<Task<bool>> predicate, int timeoutSeconds, int pollingIntervalSeconds)
    {
        var timeout = DateTime.UtcNow.AddSeconds(timeoutSeconds);

        while (DateTime.UtcNow < timeout)
        {
            if (await predicate().ConfigureAwait(false))
            {
                return;
            }

            await Task.Delay(TimeSpan.FromSeconds(pollingIntervalSeconds)).ConfigureAwait(false);
        }

        throw new TimeoutException($"Condition was not met in {timeoutSeconds} second(s).");
    }
}
