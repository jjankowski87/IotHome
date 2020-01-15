using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IotHomeService.Model;

namespace IotHomeService.App.Services.Interfaces
{
    public interface IReadingsService
    {
        Task<IDictionary<SensorDetails, IList<SensorReading>>> GetReadingsAsync(DateTime fromDate, DateTime toDate);
    }
}
