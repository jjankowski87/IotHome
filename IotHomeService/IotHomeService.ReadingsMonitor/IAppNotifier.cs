using System;
using System.Threading.Tasks;
using IotHomeService.Model;
using Microsoft.Extensions.Logging;

namespace IotHomeService.ReadingsMonitor
{
    public interface IAppNotifier
    {
        Task NotifyNewReadingAsync(DateTimeOffset date, Reading reading, ILogger logger);
    }
}