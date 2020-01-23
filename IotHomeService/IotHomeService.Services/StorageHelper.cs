using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IotHomeService.Model;
using IotHomeService.Services.Interfaces;
using IotHomeService.Services.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace IotHomeService.Services
{
    public class StorageHelper : IStorageHelper
    {
        private const string DateFormat = "yyyy-MM-dd";
        private const string TimeFormat = "hhmm";

        private readonly CloudBlobClient _blobClient;

        private readonly string _containerName;

        private readonly string _parentDirectory;

        public StorageHelper(StorageConfiguration configuration)
        {
            _containerName = configuration.ContainerName;
            _parentDirectory = configuration.ParentDirectory;

            var storageAccount = CloudStorageAccount.Parse(configuration.ConnectionString);
            _blobClient = storageAccount.CreateCloudBlobClient();
        }

        public async Task<IEnumerable<IotMessage<Reading>>> ListMessagesAsync(string blobPath)
        {
            var container = _blobClient.GetContainerReference(_containerName);

            var blob = container.GetBlockBlobReference(Path.Combine(_parentDirectory, blobPath));
            var content = await blob.DownloadTextAsync();

            return ReadingFileHelper.GetMessages(content);
        }

        public async Task<IEnumerable<DateDirectory>> ListDateDirectoriesAsync()
        {
            var container = _blobClient.GetContainerReference(_containerName);
            var segments = await container.ListBlobsSegmentedAsync(_parentDirectory, false, BlobListingDetails.None, null, null, null, null);
            var blobDirectories = segments.Results.OfType<CloudBlobDirectory>();

            var result = new List<DateDirectory>();
            foreach (var blobDirectory in blobDirectories)
            {
                var datePart = blobDirectory.Prefix.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
                if (DateTime.TryParseExact(datePart, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal,
                    out var date))
                {
                    result.Add(new DateDirectory(blobDirectory, date));
                }
            }

            return result;
        }

        public async Task<IEnumerable<TimeBlob>> ListTimeBlobsAsync(DateDirectory directory)
        {
            var blobs = await directory.Directory.ListBlobsSegmentedAsync(false, BlobListingDetails.None, null, null, null, null);

            var result = new List<TimeBlob>();
            foreach (var blob in blobs.Results.OfType<CloudBlockBlob>())
            {
                var timePart = blob.Name.Split('/').LastOrDefault()?.Split('-').FirstOrDefault();
                if (TimeSpan.TryParseExact(timePart, TimeFormat, CultureInfo.InvariantCulture, TimeSpanStyles.None,
                    out var time))
                {
                    result.Add(new TimeBlob(blob, new DateTimeOffset(directory.Date.Add(time), TimeSpan.Zero)));
                }
            }

            return result;
        }

        public async Task<IEnumerable<IotMessage<Reading>>> ListMessagesAsync(TimeBlob blob)
        {
            var content = await blob.Blob.DownloadTextAsync();
            return ReadingFileHelper.GetMessages(content);
        }
    }
}