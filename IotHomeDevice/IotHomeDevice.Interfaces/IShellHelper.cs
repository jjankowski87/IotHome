using System.Threading.Tasks;

namespace IotHomeDevice.Interfaces
{
    public interface IShellHelper
    {
        Task<string> ExecuteCommandAsync(string command);
    }
}
