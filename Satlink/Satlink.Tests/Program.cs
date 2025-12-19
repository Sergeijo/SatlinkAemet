using System;

namespace Satlink.Tests
{
    internal class Program
    {
        // Renamed to avoid CS0017 (multiple entry points when combined with WPF-generated entry point)
        public static void Run()
        {
            TestRunner.RunAll();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
        }
    }
}
