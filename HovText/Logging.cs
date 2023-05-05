using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;

namespace HovText
{

    class Logging
    {
        // ###########################################################################################
        // Put start-up information to the logfile
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

            Log("------------------------------------------------------------------------------");
            Log("Send this log to the developer via the \"Feedback\" tab, if you experience any problems.");
            Log("------------------------------------------------------------------------------");
            Log("Started HovText [" + appVer + "] logging");
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
            if (Settings.isTroubleshootEnabled)
            {
                if (logMessage == "")
                {
                    File.AppendAllText(Settings.troubleshootLogfile, Environment.NewLine);
                }
                else
                {
                    File.AppendAllText(Settings.troubleshootLogfile, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + logMessage + Environment.NewLine);
                }
            }

            // Output to the Visual Studio "Output" console
            Debug.WriteLine(logMessage);
        }


        // ###########################################################################################
    }
}
