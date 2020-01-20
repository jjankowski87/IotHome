using IotHomeDevice.Model;

namespace IotHomeDevice.Interface.Sensor
{
    public interface ISensor
    {
        ReadingType Type { get; }

        string Name { get; }

        decimal ReadValue();
    }
}
