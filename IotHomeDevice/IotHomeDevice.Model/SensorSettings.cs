namespace IotHomeDevice.Model
{
    public class SensorSettings
    {
        public string Name { get; set; }

        public SensorType Type { get; set; }

        public bool IsEnabled { get; set; }
    }
}