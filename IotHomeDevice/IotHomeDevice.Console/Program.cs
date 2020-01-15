using System;
using System.Threading.Tasks;
using IotHomeDevice.Console.Configuration;
using IotHomeDevice.Interfaces;
using IotHomeDevice.Linux;
using IotHomeDevice.Windows;
using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IotHomeDevice.Console
{
    public class Program
    {
        private const string SettingsFileName = "appsettings.json";

        public static async Task Main(string[] args)
        {
            await CreateContainer().GetService<App>().ProcessAsync();

            System.Console.ReadKey();
        }

        private static ServiceProvider CreateContainer()
        {
            var settings = LoadApplicationSettings();
            var iotHubClient = DeviceClient.CreateFromConnectionString(settings.ConnectionString, TransportType.Mqtt);

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(settings);
            serviceCollection.AddSingleton(iotHubClient);
            serviceCollection.AddTransient<ILogger, ConsoleLogger>();
            serviceCollection.AddTransient<IDevice, Device>();
            serviceCollection.AddSingleton<App>();

            if (OperatingSystem.IsLinux())
            {
                serviceCollection.AddSingleton<IShellHelper, UnixShellHelper>();
                serviceCollection.AddTransient<IThermometer, RaspbianChipsetThermometer>();
            }
            else
            {
                serviceCollection.AddTransient<IThermometer, RandomThermometer>();
            }

            return serviceCollection.BuildServiceProvider();
        }

        private static AppSettings LoadApplicationSettings()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile(SettingsFileName);

            var configuration = builder.Build();
            return configuration.Get<AppSettings>();
        }
    }
}
