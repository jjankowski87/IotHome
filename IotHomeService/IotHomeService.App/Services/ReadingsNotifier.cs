using System;
using System.Threading.Tasks;
using IotHomeService.App.Services.Interfaces;
using IotHomeService.Model;

namespace IotHomeService.App.Services
{
    public class ReadingsNotifier : IReadingsNotifier
    {
        public event Func<SensorDetails, SensorReading, Task> NewReadingsAppeared;

        public async Task NotifyClientsAsync(string sensor, string name, DateTimeOffset date, decimal value)
        {
            if (NewReadingsAppeared != null)
            {
                await NewReadingsAppeared.Invoke(new SensorDetails(sensor, name), new SensorReading(date, value));
            }
        }
    }
}