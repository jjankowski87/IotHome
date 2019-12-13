using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace IotHomeService.App.Data
{
    public class IotHubListener : IHostedService
    {
        private const int QueueSize = 50;

        private readonly Queue<MessageDetails> _messages = new Queue<MessageDetails>(QueueSize + 1);
        private readonly IList<Task> _tasks = new List<Task>();
        private readonly CancellationTokenSource _tokenSource;
        private readonly EventHubClient _client;

        public event Func<IEnumerable<MessageDetails>, Task> NotifyMessageAdded;

        public IotHubListener(IotSettings settings)
        {
            var connectionString = new EventHubsConnectionStringBuilder(new Uri(settings.EventHubCompatibleEndpoint),
                settings.IotHubsCompatiblePath, settings.IotHubSasKeyName, settings.IotHubSasKey);
            _client = EventHubClient.CreateFromConnectionString(connectionString.ToString());
            _tokenSource = new CancellationTokenSource();
        }

        public IEnumerable<MessageDetails> Messages => _messages;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _tasks.Clear();

            var runtimeInfo = await _client.GetRuntimeInformationAsync();
            foreach (var partition in runtimeInfo.PartitionIds)
            {
                _tasks.Add(ReceiveMessagesFromDeviceAsync(partition, cancellationToken));
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _tokenSource.Cancel();
            await Task.WhenAll(_tasks);
        }

        private async Task ReceiveMessagesFromDeviceAsync(string partition, CancellationToken token)
        {
            var eventHubReceiver = _client.CreateReceiver("$Default", partition,
                EventPosition.FromEnqueuedTime(DateTime.Now.AddHours(-1)));

            while (true)
            {
                if (token.IsCancellationRequested || _tokenSource.IsCancellationRequested)
                {
                    break;
                }

                var events = await eventHubReceiver.ReceiveAsync(QueueSize, TimeSpan.FromSeconds(30));
                if (events != null)
                {
                    var newMessages = events.Select(ReceiveMessage).ToList();
                    if (newMessages.Any())
                    {
                        foreach (var message in newMessages)
                        {
                            _messages.Enqueue(message);
                            if (_messages.Count > QueueSize)
                            {
                                _messages.Dequeue();
                            }
                        }

                        if (NotifyMessageAdded != null)
                        {
                            await NotifyMessageAdded.Invoke(newMessages);
                        }
                    }
                }
            }
        }

        private static MessageDetails ReceiveMessage(EventData eventData)
        {
            var data = Encoding.UTF8.GetString(eventData.Body.Array);

            try
            {
                var message = JsonConvert.DeserializeObject<Message>(data);
                eventData.SystemProperties.TryGetValue("iothub-connection-device-id", out var deviceId);

                return new MessageDetails
                {
                    DeviceId = deviceId?.ToString() ?? "unknown",
                    EnqueuedTime = eventData.SystemProperties.EnqueuedTimeUtc,
                    Message = message
                };
            }
            catch (Exception ex)
            {
                return new MessageDetails();
            }
        }
    }
}
