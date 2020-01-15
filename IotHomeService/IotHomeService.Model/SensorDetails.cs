namespace IotHomeService.Model
{
    public struct SensorDetails
    {
        public SensorDetails(string type, string name)
        {
            Type = type;
            Name = name;
        }

        public string Type { get; }

        public string Name { get; }
    }
}