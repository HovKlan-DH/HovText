using System;
using System.IO.Pipes;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Controls;
//using static HovText.Settings;

namespace HovText
{
    static class Program
    {
        private static Settings Settings;
        public static string arg0 = "";

        private static Mutex _mutex;
        private static NamedPipeServerStream _pipeServer;

        [STAThread] // STAThreadAttribute indicates that the COM threading model for the application is single-threaded apartment, https://stackoverflow.com/a/1361048/2028935

        static int Main(string[] args)
        {
            // Ensure that application only run once - quote ChatGPT4:
            // "A mutex is a synchronization object that is used to ensure that only one thread at a time executes a particular block of code, in this case, to ensure that only one instance of the application runs at a time"
            bool newInstance;
            _mutex = new Mutex(true, "HovText", out newInstance);

            // Set troubleshooting boolean
            string regVal = Settings.GetRegistryKey(Settings.registryPath, "TroubleshootEnable");
            Settings.isTroubleshootEnabled = regVal == "1" ? true : false;

            // Get the argument passed (only one supported)
            if (args.Length > 0)
            {
                arg0 = args[0].ToLowerInvariant();

                // Do a quick "exit", if this is the case (but only if the application is not already launched)
                if (newInstance)
                {
                    if (arg0 == "--exit")
                    {
                        return 0;
                    }
                }
            }

            // Check if an application instance is already running
            if (!newInstance)
            {
                // This code will be trigged with the second instance, and it will instruct the first
                // instance to run the code from "SHOW_SETTINGS" - and then this second instance will
                // exit
                if (arg0 == "--cleanup-and-exit")
                {
                    // Do a clean-up and exit
                    using (var pipeClient = new NamedPipeClientStream(".", "HovText", PipeDirection.Out))
                    {
                        pipeClient.Connect();
                        using (var writer = new StreamWriter(pipeClient))
                        {
                            writer.WriteLine("CLEANUP_AND_EXIT");
                            return 0;
                        }
                    }
                }
                else if (arg0 == "--exit")
                {
                    // Do an application exit
                    using (var pipeClient = new NamedPipeClientStream(".", "HovText", PipeDirection.Out))
                    {
                        pipeClient.Connect();
                        using (var writer = new StreamWriter(pipeClient))
                        {
                            writer.WriteLine("EXIT");
                            return 0;
                        }
                    }
                }
                else
                {
                    // Show "Settings"
                    using (var pipeClient = new NamedPipeClientStream(".", "HovText", PipeDirection.Out))
                    {
                        pipeClient.Connect();
                        using (var writer = new StreamWriter(pipeClient))
                        {
                            writer.WriteLine("SHOW_SETTINGS");
                            return 0;
                        }
                    }
                }
//            } else
//                    Logging.Log("Application launched, but it hangs in its named pipe - is there running another non-working instance?");
            }

            // It will only come to here, if this is the first instance being run

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Settings = new Settings();

            // Start the named pipe server
            _pipeServer = new NamedPipeServerStream("HovText", PipeDirection.In);
            ThreadPool.QueueUserWorkItem(PipeServerThread);

            // Get the argument passed (only applicable here, if there is no instance running already)
            if (args.Length > 0)
            {
                if (arg0 == "--cleanup-and-exit")
                {
                    Settings.CleanupAndExit();
                    return 0;
                }
//                if (arg0 != "--start-minimized")
//                {
//                    return 0;
//                }
            }

            Application.Run(Settings);

            _mutex.ReleaseMutex();

            return 0;
        }


        // ###########################################################################################
        // Handle the named pipe "server".
        // This is handled within the first application launch - not the secondary launch.
        // First launch LISTEN - secondary launch SEND an event to the listener.
        // ###########################################################################################

        private static void PipeServerThread(object state)
        {
            while (true)
            {
                _pipeServer.WaitForConnection();

                try
                {
                    using (var ms = new MemoryStream())
                    {
                        _pipeServer.CopyTo(ms);
                        ms.Position = 0;
                        using (var reader = new StreamReader(ms))
                        {
                            var message = reader.ReadLine();
                                if (message == "SHOW_SETTINGS")
                                {
                                    // Show the settings page in the existing instance.
                                    Logging.Log("Application launched, with active application already running - showing \"Settings\"");
                                    Settings.Invoke(new Action(() => Settings.ShowSettingsForm()));
                                }
                                if (message == "EXIT")
                                {
                                    // Exit the application
                                    Settings.isClosedFromNotifyIcon = true;
                                    Logging.Log("Application launched, with application already running - started with commandline option [--exit]");
                                    Settings.Invoke(new Action(() => Settings.Close()));
                                }
                                if (message == "CLEANUP_AND_EXIT")
                                {
                                    // Clean-up and exit the application
                                    Settings.Invoke(new Action(() => Settings.CleanupAndExit()));
                                }
                        }
                    }
                }
                catch (IOException ex)
                {
                    // Handle any exceptions that might occur while reading from the pipe
                    Logging.Log("Exception #1 (Program):");
                    Logging.Log("  " + ex.Message);
                }
                finally
                {
                    _pipeServer.Disconnect();
                }
            }
        }


        // ###########################################################################################
        // Include native dependencies
        // ###########################################################################################

        public static class NativeMethods
        {
            // ###########################################################################################
            // Included for getting the process that has updated the clipboard
            // ###########################################################################################

            [DllImport("user32.dll")]
            public static extern IntPtr GetClipboardOwner();

            // ###########################################################################################
            // Included for getting and setting application focus
            // ###########################################################################################

            [DllImport("user32.dll")]
            public static extern bool SetForegroundWindow(IntPtr hWnd);

            [DllImport("user32.dll")]
            public static extern IntPtr GetForegroundWindow();

            [DllImport("user32.dll", SetLastError = true)]
            public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

            // ###########################################################################################
            // Included for registering application in the clipboard chain
            // ###########################################################################################

            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool AddClipboardFormatListener(IntPtr hwnd);

            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool RemoveClipboardFormatListener(IntPtr hwnd);
        }


    // ###########################################################################################
}
    }