using System;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using Satlink.Domain.Models;
using Satlink.Infrastructure.DI;

namespace Satlink.Api.Tests.LegacySatlinkTests;

/// <summary>
/// Legacy tests migrated from the original <c>Satlink.Tests</c> project.
/// </summary>
internal static class AemetRepositoryTestsRunner
{
    /// <summary>
    /// Executes legacy repository tests.
    /// </summary>
    /// <returns>Number of failures.</returns>
    public static int Run()
    {
        int failures = 0;

        DbContextOptions<AemetDbContext> options = new DbContextOptionsBuilder<AemetDbContext>()
            .UseInMemoryDatabase($"legacy-{Guid.NewGuid()}")
            .Options;

        using (AemetDbContext context = new AemetDbContext(options))
        {
            // Seed one value.
            context.zonePredictionsItems.Add(new Request { id = "1", nombre = "Test" });
            context.SaveChanges();

            // AemetRepository is internal to Satlink.Infrastructure; locate type via reflection.
            var assembly = typeof(AemetDbContext).Assembly;
            Type? repoType = assembly.GetTypes().FirstOrDefault(t => t.Name == "AemetRepository");
            if (repoType is null)
            {
                Console.WriteLine("AemetRepository not found");
                return 1;
            }

            object? repo = Activator.CreateInstance(repoType, context);

            var getAllMethod = repoType.GetMethod("GetAllAemetItems");
            var all = getAllMethod?.Invoke(repo, null) as System.Collections.IEnumerable;
            var enumerator = all?.GetEnumerator();
            if (enumerator is null || !enumerator.MoveNext())
            {
                failures++;
                Console.WriteLine("Expected items in repository");
            }

            var getOne = repoType.GetMethod("GetAemetItems");
            var task = getOne?.Invoke(repo, new object[] { 1 }) as System.Threading.Tasks.Task;
            if (task is null)
            {
                failures++;
                Console.WriteLine("Expected task from GetAemetItems");
            }
            else
            {
                task.Wait();
                var resultProperty = task.GetType().GetProperty("Result");
                var single = resultProperty?.GetValue(task);
                if (single is null)
                {
                    failures++;
                    Console.WriteLine("Expected single item");
                }
            }
        }

        return failures;
    }
}
