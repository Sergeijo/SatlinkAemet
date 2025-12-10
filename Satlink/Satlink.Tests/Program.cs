using System;

namespace Satlink.Tests
{
    internal class Program
    {
        public static void Main()
        {
            TestRunner.RunAll();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
        }
    }
}
