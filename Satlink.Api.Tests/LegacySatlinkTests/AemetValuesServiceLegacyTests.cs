using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using NSubstitute;

using Satlink.Contracts.Dtos.Aemet;
using Satlink.Infrastructure;
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

        IAemetJsonSerializer serializer = new AemetJsonSerializer();
        AemetValuesService service = new AemetValuesService(client, serializer);

        Result<List<MarineZonePredictionDto>> result = await service.GetAemetMarineZonePredictionValuesAsync(
            "key",
            "http://not_a_valid_url_for_test",
            1);

        Assert.True(result.IsFailure);
    }
}
