using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IotHomeService.App.Services.Interfaces;
using IotHomeService.Model;
using IotHomeService.Services.Interfaces;

namespace IotHomeService.App.Services
{
    public class ReadingsService : IReadingsService
    {
        private const int SamplingWindow = 15;
        private const int HalfWindow = 7;

        private readonly IStorageExplorer _storageExplorer;

        public ReadingsService(IStorageExplorer storageExplorer)
        {
            _storageExplorer = storageExplorer;
        }

        public async Task<IDictionary<SensorDetails, IList<SensorReading>>> GetReadingsAsync(DateTime fromDate, DateTime toDate)
        {
            // TODO: validate date range
            var from = NormalizeDateTime(fromDate);
            var to = NormalizeDateTime(toDate);

            var readings = await _storageExplorer.ListSensorDetailsAsync(from, to);

            return readings.GroupBy(r => new SensorDetails(r.Body.Sensor, r.Body.Name))
                .ToDictionary(g => g.Key, g => (IList<SensorReading>)AdjustReadings(g.ToList(), from, to).ToList());
        }

        private static IEnumerable<SensorReading> AdjustReadings(IList<IotMessage<Reading>> readings, DateTime from, DateTime to)
        {
            var window = (to - from).TotalMinutes;

            var result = new List<SensorReading>();
            foreach (var readingTime in Enumerable.Range(0, (int)Math.Ceiling(window / SamplingWindow) + 1).Select(i => from.AddMinutes(i * SamplingWindow)))
            {
                var matching = readings.Where(r => Math.Abs((r.EnqueuedTimeUtc - readingTime).TotalMinutes) <= HalfWindow).ToList();

                result.Add(new SensorReading(new DateTimeOffset(readingTime),
                    matching.Any() ? matching.Average(m => m.Body.Value) : (decimal?) null));
            }

            return result;
        }

        private static DateTime NormalizeDateTime(DateTime dateTime)
        {
            var universalDateTime = dateTime.ToUniversalTime();
            var universalTime = universalDateTime.TimeOfDay;
            
            return universalDateTime.Date.AddMinutes(Math.Round(universalTime.TotalMinutes / SamplingWindow) * SamplingWindow);
        }
    }
}
