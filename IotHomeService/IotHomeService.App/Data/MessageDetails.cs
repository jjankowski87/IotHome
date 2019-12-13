using System;

namespace IotHomeService.App.Data
{
    public class MessageDetails
    {
        public string Id { get; } = Guid.NewGuid().ToString();

        public string DeviceId { get; set; }

        public DateTime EnqueuedTime { get; set; }

        public Message Message { get; set; }
    }
}