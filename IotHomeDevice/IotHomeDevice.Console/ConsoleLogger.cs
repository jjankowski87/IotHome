using System;
using IotHomeDevice.Interfaces;

namespace IotHomeDevice.Console
{
    public class ConsoleLogger : ILogger
    {
        public void LogInfo(string message)
        {
            System.Console.WriteLine($"{DateTimeOffset.Now:yyyy-MM-dd hh:mm:ss zzz}: {message}");
        }
    }
}
