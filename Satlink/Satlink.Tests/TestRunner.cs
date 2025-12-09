using System;

namespace Satlink.Tests
{
    internal class TestRunner
    {
        public static int RunAll()
        {
            int failures = 0;
            failures += AemetValuesServiceTests.Run();
            failures += ResultTestsRunner.Run();
            failures += AemetRepositoryTestsRunner.Run();

            Console.WriteLine($"Total Failures: {failures}");
            return failures;
        }
    }
}