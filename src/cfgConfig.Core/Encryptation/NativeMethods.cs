using System;
using System.Runtime.InteropServices;

namespace cfgConfig.Core.Encryptation
{
    internal static class NativeMethods
    {
        [DllImport("KERNEL32.dll", EntryPoint = "RtlZeroMemory")]
        public static extern bool ZeroMemory(IntPtr Destination, int Length);
    }
}
