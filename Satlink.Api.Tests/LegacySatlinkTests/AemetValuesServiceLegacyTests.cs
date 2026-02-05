using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using NSubstitute;

using Satlink.Infrastructure;
using Satlink.Logic;

using Xunit;

namespace Satlink.Api.Tests.LegacySatlinkTests;

public sealed class AemetValuesServiceLegacyTests
{
    [Fact]
    public async Task GetAemetMarineZonePredictionValuesAsync_InvalidUrl_ReturnsFailure()
    {
        IAemetRepository repo = Substitute.For<IAemetRepository>();
        AemetValuesService service = new AemetValuesService(repo);

        Result result = await service.GetAemetMarineZonePredictionValuesAsync("key", "http://not_a_valid_url_for_test", 1);

        Assert.True(result.IsFailure);
    }
}
