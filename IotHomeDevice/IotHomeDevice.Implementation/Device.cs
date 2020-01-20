using System.Text;
using System.Threading.Tasks;
using IotHomeDevice.Interface;
using IotHomeDevice.Interface.Sensor;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

namespace IotHomeDevice.Console
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
            var sensorType = sensor.Type.ToString().ToLowerInvariant();
            var telemetryDataPoint = new
            {
                Sensor = sensorType,
                Name = sensor.Name,
                Value = sensor.ReadValue()
            };

            var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
            var message = new Message(Encoding.UTF8.GetBytes(messageString));

            message.Properties.Add("IsReading", "true");

            _logger.LogInfo($"Sending {sensorType} {telemetryDataPoint.Value} to IoT hub");

            await _client.SendEventAsync(message);
        }
    }
}