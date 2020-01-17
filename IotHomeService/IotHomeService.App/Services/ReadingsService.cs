using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IotHomeService.App.Configuration;
using IotHomeService.App.Services.Interfaces;
using IotHomeService.Model;
using IotHomeService.Services.Interfaces;

namespace IotHomeService.App.Services
{
    public class ReadingsService : IReadingsService
    {
        // TODO: sampling should depend on window size
        // 15min, 30min, 1h etc
        private const int SamplingWindow = 15;
        private const int HalfWindow = 7;

        private readonly IStorageExplorer _storageExplorer;
        private readonly AppConfiguration _configuration;

        public ReadingsService(IStorageExplorer storageExplorer, AppConfiguration configuration)
        {
            _storageExplorer = storageExplorer;
            _configuration = configuration;
        }

        public async Task<IDictionary<SensorDetails, IList<SensorReading>>> GetReadingsAsync(DateTime fromDate, DateTime toDate)
        {
            // TODO: validate date range
            var from = NormalizeDateTime(fromDate);
            var to = NormalizeDateTime(toDate);

            var readings = await _storageExplorer.ListSensorDetailsAsync(@from, to);

            return readings.GroupBy(r => new SensorDetails(r.Body.Sensor, r.Body.Name))
                .ToDictionary(g => g.Key, g => (IList<SensorReading>)AdjustReadings(g.ToList(), from, to).ToList());
        }

        private IEnumerable<SensorReading> AdjustReadings(IList<IotMessage<Reading>> readings, DateTimeOffset from, DateTimeOffset to)
        {
            var window = (to - from).TotalMinutes;

            var result = new List<SensorReading>();
            foreach (var readingTime in Enumerable.Range(0, (int)Math.Ceiling(window / SamplingWindow) + 1).Select(i => from.AddMinutes(i * SamplingWindow)))
            {
                var matching = readings.Where(r => Math.Abs((r.EnqueuedTimeUtc - readingTime).TotalMinutes) <= HalfWindow).ToList();
                var localTime = TimeZoneInfo.ConvertTime(readingTime, _configuration.ApplicationTimeZone);

                result.Add(new SensorReading(localTime,
                    matching.Any() ? matching.Average(m => m.Body.Value) : (decimal?) null));
            }

            return result;
        }

        private DateTimeOffset NormalizeDateTime(DateTime dateTime)
        {
            var utc = dateTime.Kind == DateTimeKind.Utc
                ? new DateTimeOffset(dateTime, TimeSpan.Zero)
                : new DateTimeOffset(dateTime, _configuration.ApplicationTimeZone.GetUtcOffset(dateTime)).ToUniversalTime();

            var normalizedMinutes = Math.Round(utc.TimeOfDay.TotalMinutes / SamplingWindow) * SamplingWindow;

            return utc.Add(-utc.TimeOfDay).AddMinutes(normalizedMinutes);
        }
    }
}
