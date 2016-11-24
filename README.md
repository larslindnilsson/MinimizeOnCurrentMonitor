# MinimizeOnCurrentMonitor
Small Windows tool to minimize windows on current monitor only

## Installation

1. Make sure you have Microsoft .NET 4.0 framework installed
2. Download the latest released version from [Releases](https://github.com/larslindnilsson/MinimizeOnCurrentMonitor/releases/latest)
3. Unzip the downloaded zip-file, and place the MinimizeOnCurrentMonitor.exe file where you want to run it from

## Usage

When run, the program will minimize all windows on the monitor where the mouse is located.

A log-file (MinimizeOnCurrentMonitor.log) will be written to the user's Temp folder on each run. 
The log-file contains debugging information that can be used to improve the program if it misses 
some windows or minimizes windows that should be skipped.

The program can be used together with [AutoHotKey](https://autohotkey.com/). Use the follow script line to change 
the Win-M shortcut (which normally minimizes all windows) to only minimize on current monitor

`#m::Run C:\<path to folder>\MinimizeOnCurrentMonitor.exe,,hide`

## Known Issues

* For now only tested on Windows 10.
* May wrongly minimize some popup/toaster windows, if these are shown when the program is run.
