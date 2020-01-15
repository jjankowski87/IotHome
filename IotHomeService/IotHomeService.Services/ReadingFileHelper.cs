using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IotHomeService.Model;
using Newtonsoft.Json;

namespace IotHomeService.Services
{
    public static class ReadingFileHelper
    {
        public static IEnumerable<IotMessage<Reading>> GetMessages(string content)
        {
            return content.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .Select(m =>
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<IotMessage<string>>(m);
                    }
                    catch (JsonException)
                    {
                        return null;
                    }
                }).Where(m => m != null)
                .Select(m => new IotMessage<Reading>
                {
                    EnqueuedTimeUtc = m.EnqueuedTimeUtc,
                    SystemProperties = m.SystemProperties,
                    Body = DecodeBody(m.Body)
                }).Where(m => m.Body != null);
        }

        private static Reading DecodeBody(string body)
        {
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(body));

            try
            {
                return JsonConvert.DeserializeObject<Reading>(decoded);
            }
            catch (JsonException)
            {
                return null;
            }
        }
    }
}