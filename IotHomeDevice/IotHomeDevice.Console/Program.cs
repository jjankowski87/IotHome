using System;
using System.Linq;
using System.Threading.Tasks;
using IotHomeDevice.Console.Configuration;
using IotHomeDevice.Implementation;
using IotHomeDevice.Implementation.Sensor;
using IotHomeDevice.Interface;
using IotHomeDevice.Interface.Sensor;
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
            serviceCollection.AddSingleton<ISensorFactory, SensorFactory>();
            serviceCollection.AddSingleton<IShellHelper, UnixShellHelper>();

            foreach (var sensorSetting in settings.SensorSettings.Where(ss => ss.IsEnabled))
            {
                serviceCollection.AddTransient(p => p.GetService<ISensorFactory>().CreateSensor(sensorSetting));
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
