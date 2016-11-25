using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;


namespace MinimizeOnCurrentMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            StreamWriter Log;
            Window DesktopWindow = null;

            // Find all windows.
            List<Window> w = (new DesktopWindows()).Scan();

            // Create a log-file. Will be overwritte on each run.
            Log = new StreamWriter(Path.Combine(Path.GetTempPath(), "MinimizeOnCurrentMonitor.log"));
            Log.WriteLine(DateTime.Now.ToString());

            // Detect which screen the mouse is on
            Screen MouseScreen = Screen.FromPoint(System.Windows.Forms.Cursor.Position);
            Log.WriteLine("Mouse on: " + MouseScreen.DeviceName);

            foreach (Window item in w)
            {
                Log.WriteLine("Window:");
                Log.WriteLine("  Title: " + item.Title);
                Log.WriteLine("  Class: " + item.ClassName);
                Log.WriteLine("  WindowStyle: " + item.WindowInfo.dwStyle.ToString("X8"));
                Log.WriteLine("  ExWindowsStyle: " + item.WindowInfo.dwExStyle.ToString("X8"));

                if (ShouldIgnore(item))
                {
                    Log.WriteLine("    * Ignoring");
                }
                else
                {
                    // Detect which Screen the Window is on.
                    Screen s = Screen.FromHandle(item.HWnd);
                    // If it's on the same Screen as the Mouse it should be considered for minimizing.
                    if (s.DeviceName == MouseScreen.DeviceName)
                    {
                        //If window is not already minimized, then minimize it.
                        if (item.WindowPlacement.showCmd != NativeMethods.SW_SHOWMINIMIZED)
                        {
                            
                            if ((item.WindowInfo.dwStyle & (uint)NativeMethods.WindowStyles.WS_POPUPWINDOW) != 0 && (item.WindowInfo.dwExStyle & (uint)NativeMethods.WindowStylesEx.WS_EX_TOPMOST) != 0)
                            {
                                // If window is a Popup and TopMost, don't minimize it, since there is a risk that it will minimize to just a caption-bar. 
                                Log.WriteLine("    * TopMost Popup");
                            }
                            else if ((item.WindowInfo.dwExStyle & (uint)(NativeMethods.WindowStylesEx.WS_EX_TOOLWINDOW | NativeMethods.WindowStylesEx.WS_EX_TOPMOST)) != 0)
                            {
                                // If window is a ToolWindow and TopMost, don't minimize it, since it's either a popup/toaster or a ToowWindows from another another program. 
                                Log.WriteLine("    * TopMost ToolWindow");
                            }
                            else
                            {
                                Log.WriteLine("    * Minimizing");
                                NativeMethods.ShowWindow(item.HWnd, NativeMethods.SW_MINIMIZE);
                            }
                        }
                        else
                        {
                            Log.WriteLine("    * Already minimized");
                        }
                    }
                    else
                    {
                        Log.WriteLine("    * Wrong Monitor");
                    }
                }

                if (item.ClassName == "Progman")
                {
                    // This is the Desktop "Window". Remember it so we can set focus at the end 
                    DesktopWindow = item;
                }
            }

            // Set focus on the Desktop
            if (DesktopWindow != null)
            {
                NativeMethods.SetForegroundWindow(DesktopWindow.HWnd);
            }
            Log.Close();

        }

        private static bool ShouldIgnore(Window item)
        {
            // Windows without title are most likely hidden windows, popup windows or part of the OS
            if (String.IsNullOrEmpty(item.Title))
                return true;

            // ClassName = Progman : The Program Manager (the Desktop)
            // Don't minimize this, since it will make the Desktop flicker (in best case) or crash.
            if (item.ClassName == "Progman")
                return true;

            // ClassName = Windows.UI.Core.CoreWindow : Hidden built-in Apps, such as the calculator. 
            if (item.ClassName == "Windows.UI.Core.CoreWindow")
                return true;

            //// HwndWrapper[Twittalert.exe ....  : Popup from Twittalert
            //if (item.ClassName.StartsWith("HwndWrapper[Twittalert.exe"))
            //    return true;

            return false;
        }

    }
}
