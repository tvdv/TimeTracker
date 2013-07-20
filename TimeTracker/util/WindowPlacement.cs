using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace TimeTracker.util
{
    /// <summary>
    /// Utility class to help with getting and setting a window's placement.
    /// Based on https://blogs.msdn.com/b/davidrickard/archive/2010/03/09/saving-window-size-and-location-in-wpf-and-winforms.aspx
    /// </summary>
    public class WindowPlacement
    {
        // RECT structure required by WINDOWPLACEMENT structure
        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                this.Left = left;
                this.Top = top;
                this.Right = right;
                this.Bottom = bottom;
            }
        }

        // POINT structure required by WINDOWPLACEMENT structure
        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }

        // WINDOWPLACEMENT stores the position, size, and state of a window
        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public POINT minPosition;
            public POINT maxPosition;
            public RECT normalPosition;
        }

        [DllImport("user32.dll")]
        private static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll")]
        private static extern bool GetWindowPlacement(IntPtr hWnd, out WINDOWPLACEMENT lpwndpl);

        public const int SW_SHOWNORMAL = 1;
        public const int SW_SHOWMINIMIZED = 2;

        public static WINDOWPLACEMENT GetWindowsPlacement(Window wnd)
        {
            IntPtr hwnd = new WindowInteropHelper(wnd).Handle;
            return GetWindowsPlacement(hwnd);
        }
        public static WINDOWPLACEMENT GetWindowsPlacement(IntPtr wndHanlde)
        {
            WINDOWPLACEMENT placement;
            GetWindowPlacement(wndHanlde, out placement);
            return placement;
        }

        public static bool SetWindowPlacement(Window wndHandle, WINDOWPLACEMENT placement)
        {
            IntPtr hwnd = new WindowInteropHelper(wndHandle).Handle;
            return SetWindowPlacement(hwnd, placement);
        }
        public static bool SetWindowPlacement(IntPtr wndHandle, WINDOWPLACEMENT placement)
        {
            placement.length = Marshal.SizeOf(typeof(WINDOWPLACEMENT));
            placement.flags = 0;
            placement.showCmd = (placement.showCmd == SW_SHOWMINIMIZED ? SW_SHOWNORMAL : placement.showCmd);
            var ret=SetWindowPlacement(wndHandle, ref placement);
            return ret;
        }
    }
}
