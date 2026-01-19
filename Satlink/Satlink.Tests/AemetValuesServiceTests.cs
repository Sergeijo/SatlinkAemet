using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Satlink.Domain.Models;
using Satlink.Infrastructure;
using Satlink.Logic;

namespace Satlink.Tests
{
    internal class AemetValuesServiceTests
    {
        public static int Run()
        {
            int failures = 0;

            try
            {
                DummyRepository mockRepo = new DummyRepository();
                AemetValuesService service = new AemetValuesService(mockRepo);

                Result result = service.GetAemetMarineZonePredictionValuesAsync("key", "http://not_a_valid_url_for_test", 1)
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
    }

    internal sealed class DummyRepository : IAemetRepository
    {
        public IEnumerable<Request> GetAllAemetItems() => new List<Request>();

        public Task<Request> GetAemetItems(int id) => Task.FromResult<Request>(null!);

        public Task<List<Request>> GetAllAemetItemsAsync(CancellationToken cancellationToken) => Task.FromResult(new List<Request>());

        public Task<Request?> GetAemetItemByIdAsync(int id, CancellationToken cancellationToken) => Task.FromResult<Request?>(null);
    }
}
