using Microsoft.Extensions.Configuration;

namespace IotHomeService.ReadingsMonitor.Configuration
{
    public static class ConfigurationExtensions
    {
        public static T GetConfigSection<T>(this IConfiguration config, string section) where T : new()
        {
            var configSection = new T();
            config.GetSection(section).Bind(configSection);

            return configSection;
        }
    }
}