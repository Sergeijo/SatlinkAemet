using System.Threading.Tasks;

using Xunit;

namespace Satlink.Api.Tests.LegacySatlinkTests;

/// <summary>
/// Bridges the legacy console-style tests into xUnit.
/// </summary>
public sealed class LegacySatlinkTestRunner
{
    [Fact]
    public async Task LegacyTests_RunAll_ReturnsZeroFailures()
    {
        int failures = 0;

        await new AemetValuesServiceLegacyTests().GetAemetMarineZonePredictionValuesAsync_InvalidUrl_ReturnsFailure();
        failures += ResultLegacyTests.Run();
        failures += AemetRepositoryTestsRunner.Run();

        Assert.True(failures == 0, $"Legacy test runner reported {failures} failures.");
    }
}
