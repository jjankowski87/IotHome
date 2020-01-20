using IotHomeDevice.Model;

namespace IotHomeDevice.Interface.Sensor
{
    public interface ISensor
    {
        ReadingType ReadingType { get; }

        string Name { get; }

        decimal ReadValue();
    }
}
