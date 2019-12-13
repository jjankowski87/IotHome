﻿using System;

namespace IotHomeDevice.Console.Configuration
{
    public class AppSettings
    {
        public string ConnectionString { get; set; }

        public int ProcessingIntervalInSeconds { get; set; }

        public TimeSpan ProcessingInterval => TimeSpan.FromSeconds(ProcessingIntervalInSeconds);
    }
}
