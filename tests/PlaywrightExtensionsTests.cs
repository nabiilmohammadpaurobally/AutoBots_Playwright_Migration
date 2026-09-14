using AutobotsPlaywrightFramework.Extensions;
using NUnit.Framework;

namespace AutobotsPlaywrightFramework.Tests;

[TestFixture]
public class PlaywrightExtensionsTests
{
    [Test]
    public async Task WaitUntilAsync_Completes_WhenPredicateBecomesTrue()
    {
        var invocationCount = 0;

        await PlaywrightExtensions.WaitUntilAsync(() =>
        {
            invocationCount++;
            return Task.FromResult(invocationCount >= 2);
        }, timeoutSeconds: 2, pollingIntervalSeconds: 1);

        Assert.That(invocationCount, Is.GreaterThanOrEqualTo(2));
    }

    [Test]
    public void WaitUntilAsync_ThrowsTimeout_WhenPredicateNeverMatches()
    {
        Assert.ThrowsAsync<TimeoutException>(async () =>
            await PlaywrightExtensions.WaitUntilAsync(() => Task.FromResult(false), timeoutSeconds: 0, pollingIntervalSeconds: 1));
    }

    [Test]
    public async Task WaitUntilAsync_EvaluatesImmediately_WhenTimeoutIsZero()
    {
        await PlaywrightExtensions.WaitUntilAsync(() => Task.FromResult(true), timeoutSeconds: 0, pollingIntervalSeconds: 1);
        Assert.Pass();
    }

    [Test]
    public void WaitUntilAsync_ThrowsForNegativeTimeout()
    {
        Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            await PlaywrightExtensions.WaitUntilAsync(() => Task.FromResult(true), timeoutSeconds: -1, pollingIntervalSeconds: 1));
    }

    [Test]
    public void WaitUntilAsync_ThrowsForInvalidPollingInterval()
    {
        Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            await PlaywrightExtensions.WaitUntilAsync(() => Task.FromResult(false), timeoutSeconds: 1, pollingIntervalSeconds: 0));
    }
}
