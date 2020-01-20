using System;
using IotHomeDevice.Interface;
using IotHomeDevice.Interface.Sensor;
using IotHomeDevice.Model;

namespace IotHomeDevice.Implementation.Sensor
{
    public class DS18B20Thermometer : ISensor
    {
        private readonly IShellHelper _shellHelper;

        public DS18B20Thermometer(IShellHelper shellHelper, string name)
        {
            _shellHelper = shellHelper;
            Name = name;
        }

        public ReadingType Type => ReadingType.Temperature;

        public string Name { get; }

        public decimal ReadValue()
        {
            throw new NotImplementedException();
        }
    }
}
