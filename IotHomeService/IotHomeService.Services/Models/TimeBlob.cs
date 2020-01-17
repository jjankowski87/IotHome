using System;
using Microsoft.WindowsAzure.Storage.Blob;

namespace IotHomeService.Services.Models
{
    public class TimeBlob
    {
        public TimeBlob(CloudBlockBlob blob, DateTimeOffset dateTime)
        {
            Blob = blob;
            DateTime = dateTime;
        }

        public CloudBlockBlob Blob { get; }

        public DateTimeOffset DateTime { get; }
    }
}