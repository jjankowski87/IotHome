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
using Serilog;

namespace IotHomeDevice.Console
{
    public class Program
    {
        private const string SettingsFileName = "appsettings.json";

        private static readonly IConfigurationRoot Configuration =
            new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile(SettingsFileName).Build();

        public static async Task Main(string[] args)
        {
            await CreateContainer().GetService<App>().ProcessAsync();

            System.Console.ReadKey();
        }

        private static ServiceProvider CreateContainer()
        {
            var services = new ServiceCollection();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .CreateLogger();

            services.AddLogging(logging =>
            {
                logging.AddSerilog();
            });

            var settings = Configuration.Get<AppSettings>();
            services.AddSingleton(settings);
            services.AddSingleton(CreateIotHubClient(settings));
            services.AddTransient<IDevice, Device>();
            services.AddSingleton<App>();
            services.AddSingleton<ISensorFactory, SensorFactory>();
            services.AddSingleton<IShellHelper, UnixShellHelper>();

            foreach (var sensorSetting in settings.SensorSettings.Where(ss => ss.IsEnabled))
            {
                services.AddTransient(p => p.GetService<ISensorFactory>().CreateSensor(sensorSetting));
            }

            return services.BuildServiceProvider();
        }

        private static DeviceClient CreateIotHubClient(AppSettings settings)
        {
            var iotHubClient = DeviceClient.CreateFromConnectionString(settings.ConnectionString, TransportType.Mqtt);
            iotHubClient.OperationTimeoutInMilliseconds = (uint)settings.IotHubSendInterval.TotalMilliseconds;

            return iotHubClient;
        }
    }
}
