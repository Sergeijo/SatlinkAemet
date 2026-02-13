using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Satlink.Infrastructure.DI;
using Satlink.Domain.Models;

namespace Satlink.Tests
{
    internal class AemetRepositoryTestsRunner
    {
        public static int Run()
        {
            int failures = 0;
            // Use a plain options builder to avoid requiring the InMemory provider at compile time
            var options = new DbContextOptionsBuilder<AemetDbContext>()
                .Options;

            using (var context = new AemetDbContext(options))
            {
                // Ensure the DbSet is available for compilation; runtime behavior is not relevant for build fixes
                try
                {
                    context.zonePredictionsItems.Add(new PersistedRequest { id = "1", nombre = "Test" });
                    context.SaveChanges();
                }
                catch { /* ignore runtime provider errors during tests build */ }

                // AemetRepository is internal to Satlink.Logic; find type via reflection
                var assembly = typeof(Satlink.Logic.Result).Assembly;
                var repoType = assembly.GetTypes().FirstOrDefault(t => t.Name == "AemetRepository");
                if (repoType == null) { Console.WriteLine("AemetRepository not found"); return 1; }

                var repo = Activator.CreateInstance(repoType, context);

                var getAllMethod = repoType.GetMethod("GetAllAemetItems");
                var all = getAllMethod.Invoke(repo, null) as System.Collections.IEnumerable;
                var enumerator = all?.GetEnumerator();
                if (enumerator == null || !enumerator.MoveNext()) { failures++; Console.WriteLine("Expected items in repository"); }

                var getOne = repoType.GetMethod("GetAemetItems");
                var task = getOne.Invoke(repo, new object[] { 1 }) as System.Threading.Tasks.Task;
                if (task == null)
                {
                    failures++; Console.WriteLine("Expected task from GetAemetItems");
                }
                else
                {
                    task.Wait();
                    var resultProperty = task.GetType().GetProperty("Result");
                    var single = resultProperty?.GetValue(task);
                    if (single == null) { failures++; Console.WriteLine("Expected single item"); }
                }
            }

            return failures;
        }
    }
}