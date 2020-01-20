using System;

namespace IotHomeDevice.Interface
{
    public interface ILogger
    {
        void LogInfo(string message);

        void LogError(Exception exception, string message);
    }
}