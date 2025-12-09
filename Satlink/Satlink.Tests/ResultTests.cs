using System;
using Satlink.Logic;

namespace Satlink.Tests
{
    internal class ResultTestsRunner
    {
        public static int Run()
        {
            int failures = 0;

            var r = Result.Ok<int>(5);
            if (!r.Success) { failures++; Console.WriteLine("Result Ok<T> not successful"); }
            if (!r.Value.Equals(5)) { failures++; Console.WriteLine("Result Ok<T> value mismatch"); }

            var rFail = Result.Fail<int>("err");
            if (!rFail.IsFailure) { failures++; Console.WriteLine("Result Fail<T> not failure"); }

            try
            {
                var ctor = typeof(Result).GetConstructor(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null, new[] { typeof(bool), typeof(string) }, null);
                ctor.Invoke(new object[] { true, "not empty" });
                failures++; Console.WriteLine("Expected InvalidOperationException on invalid construction");
            }
            catch (System.Reflection.TargetInvocationException tie) when (tie.InnerException is InvalidOperationException)
            {
            }
            catch (Exception ex)
            {
                failures++; Console.WriteLine("Unexpected exception: " + ex);
            }

            return failures;
        }
    }
}