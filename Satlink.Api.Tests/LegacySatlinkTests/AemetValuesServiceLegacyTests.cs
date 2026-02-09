using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using NSubstitute;

using Satlink.Logic;

using Xunit;

namespace Satlink.Api.Tests.LegacySatlinkTests;

public sealed class AemetValuesServiceLegacyTests
{
    [Fact]
    public async Task GetAemetMarineZonePredictionValuesAsync_InvalidUrl_ReturnsFailure()
    {
        IAemetOpenDataClient client = Substitute.For<IAemetOpenDataClient>();
        client.GetMarineZoneDescriptorJsonAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult("{"));

        AemetValuesService service = new AemetValuesService(client);

        Result<List<Satlink.Domain.Models.Request>> result = await service.GetAemetMarineZonePredictionValuesAsync(
            "key",
            "http://not_a_valid_url_for_test",
            1);

        Assert.True(result.IsFailure);
    }
}
