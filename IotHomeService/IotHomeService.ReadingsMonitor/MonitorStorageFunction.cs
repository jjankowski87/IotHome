using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IotHomeService.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace IotHomeService.ReadingsMonitor
{
    public class MonitorStorageFunction
    {
        private readonly IAppNotifier _appNotifier;

        public MonitorStorageFunction(IAppNotifier appNotifier)
        {
            _appNotifier = appNotifier;
        }

        [FunctionName("MonitorStorageFunction")]
        [StorageAccount("BlobStorageConnectionString")]
        public async Task Run([BlobTrigger("dev/IoTHomeServiceHub/{name}")]Stream myBlob, string name, ILogger log)
        {
            using (var streamReader = new StreamReader(myBlob))
            {
                var content = await streamReader.ReadToEndAsync();
                var messages = ReadingFileHelper.GetMessages(content).OrderBy(m => m.EnqueuedTimeUtc).ToList();

                if (messages.Any())
                {
                    await Task.WhenAll(messages.Select(m =>
                        _appNotifier.NotifyNewReadingAsync(m.EnqueuedTimeUtc, m.Body, log)));
                }
                else
                {
                    log.LogError($"Invalid file format, blob {name}");
                }
            }
        }
    }
}
