using System;
using IotHomeService.ReadingsMonitor;
using IotHomeService.ReadingsMonitor.Configuration;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace IotHomeService.ReadingsMonitor
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            RegisterConfiguration(builder.Services);
            RegisterDependencies(builder.Services);
        }

        private static void RegisterConfiguration(IServiceCollection services)
        {
            var config = BuildConfigurationRoot();

            var appSettings = config.GetConfigSection<AppSettings>("Values");

            services.AddSingleton(appSettings);
        }

        private static void RegisterDependencies(IServiceCollection services)
        {
            services.AddSingleton<IAppNotifier, AppNotifier>();
        }

        private static IConfiguration BuildConfigurationRoot()
        {
            var basePath = IsDevelopmentEnvironment()
                ? Environment.GetEnvironmentVariable("AzureWebJobsScriptRoot")
                : $"{Environment.GetEnvironmentVariable("HOME")}\\site\\wwwroot";

            return new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }

        private static bool IsDevelopmentEnvironment()
        {
            return Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT")?
                .Equals("Development", StringComparison.OrdinalIgnoreCase) == true;
        }
    }
}