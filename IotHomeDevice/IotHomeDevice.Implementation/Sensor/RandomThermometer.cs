using System;
using IotHomeDevice.Interface;
using IotHomeDevice.Interface.Sensor;
using IotHomeDevice.Model;

namespace IotHomeDevice.Implementation.Sensor
{
    public class RandomThermometer : ISensor
    {
        private static readonly Random Random = new Random(DateTime.Now.Millisecond);

        public ReadingType ReadingType => ReadingType.Temperature;

        public string Name => "random";

        public decimal ReadValue()
        {
            return Math.Round(20m + (decimal) (Random.NextDouble() * 15d), 2);
        }
    }
}
