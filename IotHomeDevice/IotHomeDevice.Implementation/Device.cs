using System.Text;
using System.Threading.Tasks;
using IotHomeDevice.Interface;
using IotHomeDevice.Interface.Sensor;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

namespace IotHomeDevice.Implementation
{
    public class Device : IDevice
    {
        private readonly DeviceClient _client;
        private readonly ILogger _logger;

        public Device(DeviceClient client, ILogger logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task ProcessSensorAsync(ISensor sensor)
        {
            var readingType = sensor.ReadingType.ToString().ToLowerInvariant();
            var telemetryDataPoint = new
            {
                Sensor = readingType,
                Name = sensor.Name,
                Value = sensor.ReadValue()
            };

            var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
            var message = new Message(Encoding.UTF8.GetBytes(messageString));

            message.Properties.Add("IsReading", "true");

            _logger.LogInfo($"Sending {sensor.Name} {readingType} {telemetryDataPoint.Value} to IoT hub");

            await _client.SendEventAsync(message);
        }
    }
}