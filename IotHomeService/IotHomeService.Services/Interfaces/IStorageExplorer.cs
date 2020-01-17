using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IotHomeService.Model;

namespace IotHomeService.Services.Interfaces
{
    public interface IStorageExplorer
    {
        Task<IEnumerable<IotMessage<Reading>>> ListSensorDetailsAsync(DateTimeOffset @from, DateTimeOffset to);
    }
}