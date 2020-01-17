using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IotHomeService.Model;
using IotHomeService.Services.Interfaces;

namespace IotHomeService.Services
{
    public class StorageExplorer : IStorageExplorer
    {
        private readonly IStorageHelper _storageHelper;

        public StorageExplorer(IStorageHelper storageHelper)
        {
            _storageHelper = storageHelper;
        }

        public async Task<IEnumerable<IotMessage<Reading>>> ListSensorDetailsAsync(DateTimeOffset @from,
            DateTimeOffset to)
        {
            var dateDirectories = await _storageHelper.ListDateDirectoriesAsync();
            var matchingDirectories = dateDirectories.Where(d => d.Date >= from.Date && d.Date <= to.Date);

            var timeBlobs = await Task.WhenAll(matchingDirectories.Select(d => _storageHelper.ListTimeBlobsAsync(d)));
            var messages = await Task.WhenAll(timeBlobs.SelectMany(b => b)
                .Where(b => b.DateTime >= from && b.DateTime <= to).Select(b => _storageHelper.ListMessagesAsync(b)));

            return messages.SelectMany(m => m);
        }
    }
}