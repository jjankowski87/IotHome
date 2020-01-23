using System.Collections.Generic;
using System.Threading.Tasks;
using IotHomeService.Model;
using IotHomeService.Services.Models;

namespace IotHomeService.Services.Interfaces
{
    public interface IStorageHelper
    {
        Task<IEnumerable<DateDirectory>> ListDateDirectoriesAsync();

        Task<IEnumerable<TimeBlob>> ListTimeBlobsAsync(DateDirectory directory);

        Task<IEnumerable<IotMessage<Reading>>> ListMessagesAsync(TimeBlob blob);

        Task<IEnumerable<IotMessage<Reading>>> ListMessagesAsync(string path);
    }
}