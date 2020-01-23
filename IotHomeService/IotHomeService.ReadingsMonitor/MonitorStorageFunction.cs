using System.Linq;
using System.Threading.Tasks;
using IotHomeService.ReadingsMonitor.Configuration;
using IotHomeService.Services.Interfaces;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace IotHomeService.ReadingsMonitor
{
    public class MonitorStorageFunction
    {
        private const string BlobCreated = "Microsoft.Storage.BlobCreated";
        private const string ContainerPrefix = "/blobServices/default/containers/";

        private readonly IAppNotifier _appNotifier;
        private readonly IStorageHelper _storageHelper;
        private readonly string _blobPrefix;

        public MonitorStorageFunction(IAppNotifier appNotifier, IStorageHelper storageHelper, AppSettings appSettings)
        {
            _appNotifier = appNotifier;
            _storageHelper = storageHelper;
            _blobPrefix = $"{appSettings.StorageContainerName}/blobs/{appSettings.StorageDirectory}";
        }

        [FunctionName("MonitorStorageFunction")]
        public async Task Run([EventGridTrigger]JObject eventGridEvent, ILogger log)
        {
            var e = eventGridEvent.ToObject<EventGridEvent>();
            if (e.EventType != BlobCreated)
            {
                log.Log(LogLevel.Information, $"Unsupported event {e.EventType}, skipping.");
                return;
            }

            var blobPath = e.Subject.Replace(ContainerPrefix, string.Empty);
            if (!blobPath.Contains(_blobPrefix))
            {
                log.Log(LogLevel.Information, $"Blob path {blobPath} doesn't match function configuration, skipping.");
                return;
            }

            var messages = await _storageHelper.ListMessagesAsync(blobPath.Replace(_blobPrefix, string.Empty));
            
            await Task.WhenAll(messages.Select(m =>
                _appNotifier.NotifyNewReadingAsync(m.EnqueuedTimeUtc, m.Body, log)));
        }
    }
}
