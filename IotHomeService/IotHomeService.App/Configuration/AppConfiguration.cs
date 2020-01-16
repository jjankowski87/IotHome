using System;

namespace IotHomeService.App.Configuration
{
    public class AppConfiguration
    {
        public AppConfiguration(RawAppConfiguration config)
        {
            ApplicationTimeZone = TimeZoneInfo.FindSystemTimeZoneById(config.ApplicationTimeZone);
        }

        public TimeZoneInfo ApplicationTimeZone { get; }
    }
}