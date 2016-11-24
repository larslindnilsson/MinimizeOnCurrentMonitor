using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinimizeOnCurrentMonitor
{
  
    class Window
    {
        public IntPtr HWnd;
        public String Title;
        public String ClassName;
    }

    class DesktopWindows
    {
        private List<Window> Windows; 
        private bool AddWnd(int hwnd, int lparam)
        {
            if (NativeMethods.IsWindowVisible(hwnd))
            {
                string _ClassName = "";
              StringBuilder sbTitle = new StringBuilder(255);
              NativeMethods.GetWindowText(hwnd, sbTitle, sbTitle.Capacity);
              StringBuilder sbClassName = new StringBuilder(1024);
              if (NativeMethods.GetClassName((IntPtr)hwnd, sbClassName, sbClassName.Capacity) > 0)
                  _ClassName = sbClassName.ToString();
              Windows.Add(new Window() {HWnd = (IntPtr)hwnd, Title =sbTitle.ToString(), ClassName = _ClassName});          
            }
            return true;
        }

        public List<Window> Scan()
        {
            Windows =  new List<Window>();
            NativeMethods.EnumWindows(new NativeMethods.WindowEnumCallback(this.AddWnd), 0);
            return Windows;
        }

    }

}
