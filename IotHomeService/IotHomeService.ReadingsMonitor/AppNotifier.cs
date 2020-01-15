using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using IotHomeService.Model;
using IotHomeService.ReadingsMonitor.Configuration;
using Microsoft.Extensions.Logging;

namespace IotHomeService.ReadingsMonitor
{
    public class AppNotifier : IAppNotifier, IDisposable
    {
        private readonly AppSettings _settings;

        private readonly HttpClient _client;

        public AppNotifier(AppSettings settings)
        {
            _settings = settings;
            _client = new HttpClient();
        }

        public async Task NotifyNewReadingAsync(DateTimeOffset date, Reading reading, ILogger logger)
        {
            if (date < DateTimeOffset.Now.AddMinutes(-30))
            {
                logger.LogInformation("Notification skipped, old file.");
                return;
            }

            var uri = new Uri(_settings.ServiceBaseAddress, $"api/newReadings/{reading.Sensor}?name={reading.Name}&date={date:u}&value={reading.Value.ToString(CultureInfo.InvariantCulture)}");
            try
            {
                (await _client.PostAsync(uri, null)).EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                logger.LogError(ex, $"Unable to send update to the app ({reading.Sensor} - {reading.Name}).");
            }
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
