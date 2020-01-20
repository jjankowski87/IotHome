using System.Threading.Tasks;
using IotHomeDevice.Interface.Sensor;

namespace IotHomeDevice.Interface
{
    public interface IDevice
    {
        Task ProcessSensorAsync(ISensor sensor);
    }
}
