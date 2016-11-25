using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace MinimizeOnCurrentMonitor
{
  
    class Window
    {
        public IntPtr HWnd;
        public String Title;
        public String ClassName;
        public NativeMethods.WINDOWINFO WindowInfo;
        public NativeMethods.WINDOWPLACEMENT WindowPlacement;
    }

    class DesktopWindows
    {
        private List<Window> Windows; 
        private bool AddWnd(int hwnd, int lparam)
        {
            if (NativeMethods.IsWindowVisible(hwnd))
            {
                // Get Title
                StringBuilder sbTitle = new StringBuilder(255);
                NativeMethods.GetWindowText(hwnd, sbTitle, sbTitle.Capacity);

                // Get ClassName
                string _ClassName = "";
                StringBuilder sbClassName = new StringBuilder(1024);
                if (NativeMethods.GetClassName((IntPtr)hwnd, sbClassName, sbClassName.Capacity) > 0)
                    _ClassName = sbClassName.ToString();

                // Find the current state of the Window
                NativeMethods.WINDOWPLACEMENT placement = new NativeMethods.WINDOWPLACEMENT();
                placement.length = Marshal.SizeOf(placement);
                NativeMethods.GetWindowPlacement((IntPtr)hwnd, ref placement);

                // Get additional info on the window
                NativeMethods.WINDOWINFO info = new NativeMethods.WINDOWINFO();
                NativeMethods.GetWindowInfo((IntPtr)hwnd, ref info);

                Windows.Add(new Window() {HWnd = (IntPtr)hwnd, Title =sbTitle.ToString(), ClassName = _ClassName, WindowInfo = info, WindowPlacement = placement});          
            }
            return true;
        }

        public List<Window> Scan()
        {
            Windows =  new List<Window>();
            //NativeMethods.EnumWindows(new NativeMethods.WindowEnumCallback(this.AddWnd), 0);
            NativeMethods.EnumDesktopWindows(IntPtr.Zero, new NativeMethods.WindowEnumCallback(this.AddWnd), 0);
            return Windows;
        }

    }

}
