using System;
using System.Collections.Generic;
using System.IO;

namespace Satlink
{
    public static class Log
    {
        public static void WriteLog(string message)
        {
            try
            {
                File.AppendAllText($"{System.AppDomain.CurrentDomain.BaseDirectory}\\Logs\\Log_{DateTime.Now.ToString("yyyyMMdd")}.txt", $"{Environment.NewLine}{DateTime.Now.ToString()}___{message}");
            }
            catch (Exception ex)
            {
                _ = ex;
            }
        }
    }
}
