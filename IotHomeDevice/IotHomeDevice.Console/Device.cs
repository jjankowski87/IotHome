using System.Text;
using System.Threading.Tasks;
using IotHomeDevice.Interfaces;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

namespace IotHomeDevice.Console
{
    public class Device : IDevice
    {
        private readonly DeviceClient _client;
        private readonly IThermometer _thermometer;
        private readonly ILogger _logger;

        public Device(DeviceClient client, IThermometer thermometer, ILogger logger)
        {
            _client = client;
            _thermometer = thermometer;
            _logger = logger;
        }

        public async Task ProcessAsync()
        {
            var telemetryDataPoint = new
            {
                temperature = _thermometer.GetCurrentTemperature()
            };

            var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
            var message = new Message(Encoding.ASCII.GetBytes(messageString));

            message.Properties.Add("temperatureAlert", (telemetryDataPoint.temperature > 50) ? "true" : "false");

            _logger.LogInfo($"sending temperature {telemetryDataPoint.temperature} to IoT hub");

            await _client.SendEventAsync(message);
        }
    }
}