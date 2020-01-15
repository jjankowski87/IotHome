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
        private readonly IStorageExplorer _storageExplorer;

        public ReadingsService(IStorageExplorer storageExplorer)
        {
            _storageExplorer = storageExplorer;
        }

        public async Task<IDictionary<SensorDetails, IList<SensorReading>>> GetReadingsAsync(DateTime fromDate, DateTime toDate)
        {
            var readings = await _storageExplorer.ListSensorDetailsAsync(fromDate, toDate);

            return readings.GroupBy(r => new SensorDetails(r.Body.Sensor, r.Body.Name))
                .ToDictionary(g => g.Key, g => (IList<SensorReading>)AdjustReadings(g.ToList(), fromDate, toDate).ToList());
        }

        private static IEnumerable<SensorReading> AdjustReadings(IList<IotMessage<Reading>> readings, DateTime fromDate, DateTime toDate)
        {
            const int samplingWindow = 15;
            const int halfWindow = 7;

            var from = fromDate.ToUniversalTime();
            var to = toDate.ToUniversalTime();
            var window = (to - from).TotalMinutes;

            var result = new List<SensorReading>();
            foreach (var readingTime in Enumerable.Range(0, (int)Math.Ceiling(window / samplingWindow) + 1).Select(i => from.AddMinutes(i * samplingWindow)))
            {
                var matching = readings.Where(r => Math.Abs((r.EnqueuedTimeUtc - readingTime).TotalMinutes) <= halfWindow).ToList();

                result.Add(new SensorReading(new DateTimeOffset(readingTime),
                    matching.Any() ? matching.Average(m => m.Body.Value) : (decimal?) null));
            }

            return result;
        }
    }
}
