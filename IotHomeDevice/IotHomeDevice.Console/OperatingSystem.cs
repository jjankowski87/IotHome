using System.Runtime.InteropServices;

namespace IotHomeDevice.Console
{
    public static class OperatingSystem
    {
        public static bool IsLinux() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    }
}
