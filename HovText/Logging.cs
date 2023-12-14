/*
##################################################################################################
LOGGING
-------

This is where all logging is done. It is done both to a file and to the "Debug" console.
It has a lock so that multiple threads can write to the logfile without corrupting it.

##################################################################################################
*/

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Windows.Forms;

namespace HovText
{

    class Logging
    {
        // Lock that will hold and release logging of the logfile
        private static readonly object _logLock = new object();

        // ###########################################################################################
        // Main - Logging
        // ###########################################################################################

        public static void StartLogging()
        {
            // Get OS name and version
            // https://stackoverflow.com/a/50330392/2028935
            var os = (string)(from x in new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem").Get().Cast<ManagementObject>() select x.GetPropertyValue("Caption")).FirstOrDefault();
            string appVer = Settings.appVer.Trim();

            // Get .NET Framework version
            string dotNetVer = RuntimeInformation.FrameworkDescription;
            dotNetVer = dotNetVer.Replace(".NET Framework", "");
            dotNetVer = dotNetVer.Trim();

            // Get the name of the current keyboard layout as it appears in the regional settings of the operating system on the computer
            string langSetup = System.Windows.Forms.InputLanguage.CurrentInputLanguage.LayoutName;

            // Get the OS language
            CultureInfo ci = CultureInfo.CurrentUICulture; ;
            string osLang = ci.Name;

            // Output the architechture
            Architecture cpuArchitecture = RuntimeInformation.ProcessArchitecture;
            string architectureString;
            switch (cpuArchitecture)
            {
                case Architecture.X86:
                    architectureString = "x86/32bit";
                    break;
                case Architecture.X64:
                    architectureString = "x64/64bit";
                    break;
                case Architecture.Arm:
                    architectureString = "ARM/32bit";
                    break;
                case Architecture.Arm64:
                    architectureString = "ARM64/64bit";
                    break;
                default:
                    architectureString = "unknown";
                    break;
            }

            string adminOrNot;
            if (IsRunningAsAdministrator())
            {
                adminOrNot = "running with administrative privileges";
            }
            else
            {
                adminOrNot = "running as non-privileged";
            }

            Log("------------------------------------------------------------------------------");
            Log("Send this log to the developer via the \"Feedback\" tab, if you experience any problems.");
            Log("------------------------------------------------------------------------------");
            Log("Started HovText [" + appVer + "] logging");
            Log("Executable file [" + Settings.pathAndExe + "]");
            Log("Application privileges [" + adminOrNot + "]");
            Log("OS version = [" + os + "]");
            Log("OS language = [" + osLang + "]");
            Log("OS keyboard = [" + langSetup + "]");
            Log("OS .NET Framework version = [" + dotNetVer + "]");
            Log("CPU architecture = [" + architectureString + "]");
        }


        // ###########################################################################################
        // Put closure info, so we can see it has ended in a proper manner
        // ###########################################################################################

        public static void EndLogging()
        {
            Log("Ended HovText logging");
            Log("------------------------------------------------------------------------------");
            Log("");
        }


        // ###########################################################################################
        // Do the real logging to the file and "Debug" console
        // ###########################################################################################

        public static void Log(string logMessage)
        {
            if (Settings.isTroubleshootEnabled && Settings.hasWriteAccess)
            {
                lock (_logLock)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(logMessage))
                        {
                            File.AppendAllText(Settings.pathAndLog, Environment.NewLine);
                        }
                        else
                        {
                            string timestampedMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} {logMessage}{Environment.NewLine}";
                            File.AppendAllText(Settings.pathAndLog, timestampedMessage);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"Exception for logfile:\n\nMessage:\n{ex.Message}\n\nStackTrace:\n{ex.StackTrace}",
                            "Cannot write to troubleshooting logfile",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }

            Debug.WriteLine(logMessage);
        }


        // ###########################################################################################
        // Check if application is run as admin - could be useful for logging as paths may be different
        // ###########################################################################################

        public static bool IsRunningAsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }


        // ###########################################################################################
    }
}
