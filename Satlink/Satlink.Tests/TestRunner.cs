using System;
using System.Collections.Generic;
using System.IO;

namespace Satlink.Tests
{
    internal class TestRunner
    {
        public static int RunAll()
        {
            int totalFailures = 0;

            var runners = new List<(string Name, Func<int> Run, string Project)>
            {
                ("AemetValuesServiceTests", () => AemetValuesServiceTests.Run(), "Satlink.Logic"),
                ("ResultTestsRunner", () => ResultTestsRunner.Run(), "Satlink.Logic"),
                ("AemetRepositoryTestsRunner", () => AemetRepositoryTestsRunner.Run(), "Satlink.Infrastructure / Satlink.Logic")
            };

            var failureDetails = new List<(string Project, string TestName, string Description)>();

            foreach (var runner in runners)
            {
                var originalOut = Console.Out;
                var sw = new StringWriter();
                try
                {
                    Console.SetOut(sw);
                    int failures = 0;
                    try
                    {
                        failures = runner.Run();
                    }
                    catch (Exception ex)
                    {
                        // In case a test runner throws, count as a single failure and capture exception
                        failures = 1;
                        sw.WriteLine($"Unhandled exception in {runner.Name}: {ex}");
                    }

                    var output = sw.ToString().Trim();

                    if (failures > 0)
                    {
                        totalFailures += failures;
                        var desc = string.IsNullOrEmpty(output) ? "No details provided." : output;
                        // Add one entry per failure with the same description
                        for (int i = 0; i < failures; i++)
                        {
                            failureDetails.Add((runner.Project, runner.Name, desc));
                        }
                    }
                }
                finally
                {
                    Console.SetOut(originalOut);
                    sw.Dispose();
                }
            }

            Console.WriteLine($"Total Failures: {totalFailures}");

            if (failureDetails.Count > 0)
            {
                Console.WriteLine("Failure details:");
                int idx = 1;
                foreach (var fd in failureDetails)
                {
                    Console.WriteLine($"{idx}. Project: {fd.Project} | Test: {fd.TestName}");
                    Console.WriteLine($"   Description: {fd.Description}");
                    idx++;
                }
            }

            return totalFailures;
        }
    }
}