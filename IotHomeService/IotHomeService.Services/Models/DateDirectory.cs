using System;
using Microsoft.WindowsAzure.Storage.Blob;

namespace IotHomeService.Services.Models
{
    public class DateDirectory
    {
        public DateDirectory(CloudBlobDirectory directory, DateTime date)
        {
            Directory = directory;
            Date = date;
        }

        public CloudBlobDirectory Directory { get; }

        public DateTime Date { get; }
    }
}