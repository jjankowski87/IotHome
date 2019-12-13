using System;
using System.Globalization;
using IotHomeDevice.Interfaces;

namespace IotHomeDevice.Linux
{
    public class RaspbianChipsetThermometer : IThermometer
    {
        private readonly IShellHelper _shellHelper;

        public RaspbianChipsetThermometer(IShellHelper shellHelper)
        {
            _shellHelper = shellHelper;
        }

        public decimal GetCurrentTemperature()
        {
            // temp=47.8'C
            var stringTemp = _shellHelper.ExecuteCommandAsync("/opt/vc/bin/vcgencmd measure_temp").Result;
            stringTemp = stringTemp.Replace("temp=", string.Empty).Replace("'C", string.Empty);

            return Convert.ToDecimal(stringTemp, CultureInfo.GetCultureInfo("en-gb"));
        }
    }
}
