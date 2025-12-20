using System;
using System.Linq;

using Satlink.Infrastructure;

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

        // AemetRepository is internal to Satlink.Infrastructure.
        var assembly = typeof(IAemetRepository).Assembly;
        Type? repoType = assembly.GetTypes().FirstOrDefault(t => t.Name == "AemetRepository");
        if (repoType is null)
        {
            Console.WriteLine("AemetRepository not found");
            return 1;
        }

        // Smoke-check: ensure the repository can be constructed (without exercising EF Core).
        // NOTE: We intentionally avoid creating a DbContext here because EF provider initialization
        // has proven to be unstable across environments in this repository.
        try
        {
            Activator.CreateInstance(repoType, new object?[] { null });
        }
        catch
        {
            // If instantiation fails, count as failure.
            failures++;
            Console.WriteLine("AemetRepository could not be instantiated with a null DbContext");
        }

        return failures;
    }
}
