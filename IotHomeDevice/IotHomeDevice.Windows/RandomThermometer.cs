using System;
using IotHomeDevice.Interfaces;

namespace IotHomeDevice.Windows
{
    public class RandomThermometer : IThermometer
    {
        private static readonly Random Random = new Random(DateTime.Now.Millisecond);

        public decimal GetCurrentTemperature()
        {
            return Math.Round(20m + (decimal) (Random.NextDouble() * 15d), 2);
        }
    }
}
