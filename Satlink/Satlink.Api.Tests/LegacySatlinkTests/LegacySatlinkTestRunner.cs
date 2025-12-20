using Xunit;

namespace Satlink.Api.Tests.LegacySatlinkTests;

/// <summary>
/// Bridges the legacy console-style tests into xUnit.
/// </summary>
public sealed class LegacySatlinkTestRunner
{
    [Fact]
    public void LegacyTests_RunAll_ReturnsZeroFailures()
    {
        int failures = 0;

        failures += AemetValuesServiceLegacyTests.Run();
        failures += ResultLegacyTests.Run();
        failures += AemetRepositoryTestsRunner.Run();

        Assert.True(failures == 0, $"Legacy test runner reported {failures} failures.");
    }
}
