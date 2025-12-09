using System;
using Satlink.Logic;
using Satlink.Infrastructure;
using Satlink.Domain.Models;
using System.Collections.Generic;

namespace Satlink.Tests
{
    internal class AemetValuesServiceTests
    {
        public static int Run()
        {
            int failures = 0;

            try
            {
                var mockRepo = new DummyRepository();
                var service = new AemetValuesService(mockRepo);

                var result = service.GetAemetMarineZonePredictionValues("key", "http://not_a_valid_url_for_test", 1);
                if (!result.IsFailure) { failures++; Console.WriteLine("Expected failure in AemetValuesService test"); }

                var ok = Result.Ok();
                if (!ok.Success) { failures++; Console.WriteLine("Result.Ok failed"); }

                var fail = Result.Fail("err");
                if (!fail.IsFailure) { failures++; Console.WriteLine("Result.Fail failed"); }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                failures++;
            }

            return failures;
        }
    }

    class DummyRepository : IAemetRepository
    {
        public IEnumerable<Request> GetAllAemetItems() => new List<Request>();
        public System.Threading.Tasks.Task<Request> GetAemetItems(int id) => System.Threading.Tasks.Task.FromResult<Request>(null);
    }
}