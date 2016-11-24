using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;


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
                if (ShouldIgnore(item))
                {
                    Log.WriteLine("Ignoring");
                    Log.WriteLine("  Title: " + item.Title);
                    Log.WriteLine("  Class: " + item.ClassName);
                }
                else
                {
                    // Detect which Screen the Window is on.
                    Screen s = Screen.FromHandle(item.HWnd);
                    // If it's on the same Screen as the Mouse it should be considered for minimizing.
                    if (s.DeviceName == MouseScreen.DeviceName)
                    {
                        // Find the current state of the Window
                        NativeMethods.WINDOWPLACEMENT placement = new NativeMethods.WINDOWPLACEMENT();
                        NativeMethods.GetWindowPlacement(item.HWnd, ref placement);
                        // If Window is not already minimized, then minimize it.
                        if (placement.showCmd != NativeMethods.SW_SHOWMINIMIZED)
                        {
                            Log.WriteLine("Minimizing");
                            Log.WriteLine("  Title: " + item.Title);
                            Log.WriteLine("  Class: " + item.ClassName);
                            NativeMethods.ShowWindowAsync(item.HWnd, NativeMethods.SW_SHOWMINIMIZED);
                        }
                        else
                        {
                            Log.WriteLine("Already minimized");
                            Log.WriteLine("  Title: " + item.Title);
                            Log.WriteLine("  Class: " + item.ClassName);

                        }
                    }
                    else
                    {
                        Log.WriteLine("Wrong Monitor");
                        Log.WriteLine("  Title: " + item.Title);
                        Log.WriteLine("  Class: " + item.ClassName);
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

            // ClassName: 
            // Progman : The Program Manager (the Desktop)
            // Windows.UI.Core.CoreWindow : Hidden built-in Apps, such as the calculator. 
            if (item.ClassName == "Progman" || item.ClassName == "Windows.UI.Core.CoreWindow")
                return true;

            // HwndWrapper[Twittalert.exe ....  : Popup from Twittalert
            if (item.ClassName.StartsWith("HwndWrapper[Twittalert.exe"))
                return true;

            return false;
        }

    }
}
