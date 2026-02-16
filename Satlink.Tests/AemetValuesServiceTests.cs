using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Satlink.Infrastructure;
using Satlink.Logic;
using Satlink.Contracts.Dtos.Aemet;

namespace Satlink.Tests
{
    internal class AemetValuesServiceTests
    {
        public static int Run()
        {
            int failures = 0;

            try
            {
                DummyOpenDataClient openDataClient = new DummyOpenDataClient();
                DummyJsonSerializer jsonSerializer = new DummyJsonSerializer();
                AemetValuesService service = new AemetValuesService(openDataClient, jsonSerializer);

                Result<List<MarineZonePredictionDto>> result = service.GetAemetMarineZonePredictionValuesAsync("key", "http://not_a_valid_url_for_test", 1)
                    .GetAwaiter()
                    .GetResult();

                if (!result.IsFailure)
                {
                    failures++;
                    Console.WriteLine("Expected failure in AemetValuesService test");
                }

                Result ok = Result.Ok();
                if (!ok.Success)
                {
                    failures++;
                    Console.WriteLine("Result.Ok failed");
                }

                Result fail = Result.Fail("err");
                if (!fail.IsFailure)
                {
                    failures++;
                    Console.WriteLine("Result.Fail failed");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                failures++;
            }

            return failures;
        }

        internal sealed class DummyJsonSerializer : IAemetJsonSerializer
        {
            public T? Deserialize<T>(string json)
            {
                return default;
            }
        }
    }

    internal sealed class DummyOpenDataClient : Satlink.Logic.IAemetOpenDataClient
    {
        public Task<string> GetMarineZoneDescriptorJsonAsync(string apiKey, string baseUrl, int zone, CancellationToken cancellationToken)
        {
            // Return invalid JSON so the service fails.
            return Task.FromResult("{");
        }

        public Task<string> DownloadJsonAsync(string url, CancellationToken cancellationToken)
        {
            return Task.FromResult(string.Empty);
        }
    }
}
