using System;

using Satlink.Logic;

namespace Satlink.Api.Tests.LegacySatlinkTests;

/// <summary>
/// Legacy tests migrated from the original <c>Satlink.Tests</c> project.
/// </summary>
internal static class ResultLegacyTests
{
    /// <summary>
    /// Executes legacy Result tests.
    /// </summary>
    /// <returns>Number of failures.</returns>
    public static int Run()
    {
        int failures = 0;

        Result<int> r = Result.Ok<int>(5);
        if (!r.Success)
        {
            failures++;
            Console.WriteLine("Result Ok<T> not successful");
        }

        if (!r.Value.Equals(5))
        {
            failures++;
            Console.WriteLine("Result Ok<T> value mismatch");
        }

        Result<int> rFail = Result.Fail<int>("err");
        if (!rFail.IsFailure)
        {
            failures++;
            Console.WriteLine("Result Fail<T> not failure");
        }

        try
        {
            var ctor = typeof(Result).GetConstructor(
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
                null,
                new[] { typeof(bool), typeof(string) },
                null);

            ctor!.Invoke(new object[] { true, "not empty" });

            failures++;
            Console.WriteLine("Expected InvalidOperationException on invalid construction");
        }
        catch (System.Reflection.TargetInvocationException tie) when (tie.InnerException is InvalidOperationException)
        {
        }
        catch (Exception ex)
        {
            failures++;
            Console.WriteLine("Unexpected exception: " + ex);
        }

        return failures;
    }
}
