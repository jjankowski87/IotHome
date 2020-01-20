using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IotHomeDevice.Console.Configuration;
using IotHomeDevice.Interface;
using IotHomeDevice.Interface.Sensor;

namespace IotHomeDevice.Console
{
    public class App
    {
        private readonly IDevice _device;
        private readonly AppSettings _appSettings;
        private readonly IEnumerable<ISensor> _sensors;
        private readonly ILogger _logger;

        public App(IDevice device, AppSettings appSettings, IEnumerable<ISensor> sensors, ILogger logger)
        {
            _device = device;
            _appSettings = appSettings;
            _sensors = sensors;
            _logger = logger;
        }

        public async Task ProcessAsync()
        {
            while (true)
            {
                foreach (var sensor in _sensors)
                {
                    try
                    {
                        await _device.ProcessSensorAsync(sensor);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Exception while processing data from {sensor.Type} sensor ({sensor.Name}).");
                    }
                }

                await Task.Delay(_appSettings.ProcessingInterval);
            }
        }
    }
}