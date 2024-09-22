using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

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
                MessageBox.Show($"No se ha podido escribir en el Log. Error: {ex.Message}. {ex.InnerException?.ToString()}", "ATENCIÓN", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
