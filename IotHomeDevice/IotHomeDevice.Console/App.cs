using System.Threading.Tasks;
using IotHomeDevice.Console.Configuration;
using IotHomeDevice.Interfaces;

namespace IotHomeDevice.Console
{
    public class App
    {
        private readonly IDevice _device;
        private readonly AppSettings _appSettings;

        public App(IDevice device, AppSettings appSettings)
        {
            _device = device;
            _appSettings = appSettings;
        }

        public async Task ProcessAsync()
        {
            while (true)
            {
                await _device.ProcessAsync();
                await Task.Delay(_appSettings.ProcessingInterval);
            }
        }
    }
}