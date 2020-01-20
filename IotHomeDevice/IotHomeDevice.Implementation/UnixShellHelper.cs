using System.Diagnostics;
using System.Threading.Tasks;
using IotHomeDevice.Interface;

namespace IotHomeDevice.Implementation
{
    public class UnixShellHelper : IShellHelper
    {
        public async Task<string> ExecuteCommandAsync(string command)
        {
            var escapedArgs = command.Replace("\"", "\\\"");
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            return await Task.Run(() =>
            {
                process.Start();
                var result = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                return result;
            });
        }
    }
}
