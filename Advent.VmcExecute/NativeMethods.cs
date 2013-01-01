
using System;
using System.Runtime.InteropServices;

namespace Advent.VmcExecute
{
    internal class NativeMethods
    {
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, NativeMethods.WindowShowStyle nCmdShow);

        internal enum WindowShowStyle : uint
        {
            Hide = 0U,
            ShowNormal = 1U,
            ShowMinimized = 2U,
            Maximize = 3U,
            ShowMaximized = 3U,
            ShowNormalNoActivate = 4U,
            Show = 5U,
            Minimize = 6U,
            ShowMinNoActivate = 7U,
            ShowNoActivate = 8U,
            Restore = 9U,
            ShowDefault = 10U,
            ForceMinimized = 11U,
        }
    }
}
