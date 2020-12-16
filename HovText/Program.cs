﻿using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;

namespace HovText
{

    static class Program
    {
        private static Settings Settings;
        public static string arg0 = "";

        [STAThread] // STAThreadAttribute indicates that the COM threading model for the application is single-threaded apartment, https://stackoverflow.com/a/1361048/2028935

        static int Main(string[] args)
        {

            // Only run one instance
            // https://stackoverflow.com/a/184143/2028935
            bool createdNew = true;
            using (Mutex mutex = new Mutex(true, "HovText", out createdNew))
            {
                if (createdNew)
                {
                    // Get the argument passed (only one supported)
                    if (args.Length > 0)
                    {
                        arg0 = args[0].ToLower();
                    }

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Settings = new Settings();
                    Application.Run(Settings);

                } else
                {
                    MessageBox.Show("HovText is already running and cannot startup one more instance","HovText is already running", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }  

            return 0;
        }


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

        // https://stackoverflow.com/a/17345576/2028935
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);


        // ###########################################################################################
        // Register application in the clipboard chain
        // ###########################################################################################

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AddClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RemoveClipboardFormatListener(IntPtr hwnd);


        // ###########################################################################################
    }
}
