using System.Threading.Tasks;

namespace IotHomeDevice.Interface
{
    public interface IShellHelper
    {
        Task<string> ExecuteCommandAsync(string command);
    }
}
