using System;
using IotHomeDevice.Interface;

namespace IotHomeDevice.Console
{
    public class ConsoleLogger : ILogger
    {
        public void LogInfo(string message)
        {
            System.Console.WriteLine($"{DateTimeOffset.Now:yyyy-MM-dd hh:mm:ss zzz}: {message}");
        }

        public void LogError(Exception exception, string message)
        {
            System.Console.WriteLine($"ERROR: {message}, {exception.Message}");
        }
    }
}
