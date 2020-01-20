using IotHomeDevice.Model;

namespace IotHomeDevice.Interface.Sensor
{
    public interface ISensorFactory
    {
        ISensor CreateSensor(SensorSettings sensorSettings);
    }
}