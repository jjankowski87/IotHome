using System;
using System.Text.RegularExpressions;
using IotHomeDevice.Interface;
using IotHomeDevice.Interface.Sensor;
using IotHomeDevice.Model;
using IotHomeDevice.Model.Exception;

namespace IotHomeDevice.Implementation.Sensor
{
    public class DS18B20Thermometer : ISensor
    {
        private static readonly Regex TempRegex = new Regex(@"t=\d+", RegexOptions.Compiled);

        private readonly IShellHelper _shellHelper;
        private readonly string _deviceId;

        public DS18B20Thermometer(IShellHelper shellHelper, SensorSettings settings)
        {
            _shellHelper = shellHelper;
            _deviceId = settings.DeviceId;
            Name = settings.Name;
        }

        public ReadingType ReadingType => ReadingType.Temperature;

        public string Name { get; }

        public decimal ReadValue()
        {
            // 4b 01 4b 46 7f ff 05 10 e1 : crc=e1 YES
            // 4b 01 4b 46 7f ff 05 10 e1 t=20687
            var tempFileContent = _shellHelper.ExecuteCommandAsync($"cat /sys/bus/w1/devices/{_deviceId}/w1_slave").Result;
            var match = TempRegex.Match(tempFileContent);
            
            if (!match.Success)
            {
                throw new SensorException($"Unable to find t=XXXXX, sensor DS18B20 ({_deviceId})");
            }

            var stringTemp = match.Value.Replace("t=", string.Empty);
            if (int.TryParse(stringTemp, out var temp))
            {
                return Math.Round(temp / 1000m, 1);
            }

            throw new SensorException($"Unable to parse {stringTemp} to int, sensor DS18B20 ({_deviceId})");
        }
    }
}
