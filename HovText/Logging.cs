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
/*
            string netVer = Get45or451FromRegistry();
*/
            string dotNetVer = RuntimeInformation.FrameworkDescription;
            dotNetVer = dotNetVer.Replace(".NET Framework", "");
            dotNetVer = dotNetVer.Trim();

            // Get the name of the current keyboard layout as it appears in the regional settings of the operating system on the computer
            string langSetup = System.Windows.Forms.InputLanguage.CurrentInputLanguage.LayoutName;

            // Get the OS language
            CultureInfo ci = CultureInfo.CurrentUICulture; ;
            string osLang = ci.Name;

            Log("-------------------------------------------------------------------------------");
            Log("Send this log to [dennis@hovtext.com] if you experience any problems.");
            Log("You can also copy/paste it to the \"Feedback\" tab, if it is not insanely large.");
            Log("-------------------------------------------------------------------------------");
            Log("Started HovText [" + appVer + "] logging");
            Log(".NET Framework version = [" + dotNetVer + "]");
            Log("OS version = [" + os + "]");
            Log("OS language = [" + osLang + "]");
            Log("Input language = [" + langSetup + "]");
        }


        // ###########################################################################################
        // Put closure info, so we can see it has ended in a proper manner
        // ###########################################################################################

        public static void EndLogging()
        {
            Log("Ended HovText logging");
            Log("-------------------------------------------------------------------------------");
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
                    File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + Settings.troubleshootLogfile, Environment.NewLine);
                }
                else
                {
                    File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + Settings.troubleshootLogfile, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + logMessage + Environment.NewLine);
                }
            }

            Debug.WriteLine(logMessage);
        }

/*
        // ###########################################################################################
        // Get the used .NET Framework version
        // https://stackoverflow.com/a/951915/2028935
        // ###########################################################################################

        private static string Get45or451FromRegistry()
        {
            using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\"))
            {
                int releaseKey = Convert.ToInt32(ndpKey.GetValue("Release"));
                if (true)
                {
                    return CheckFor45DotVersion(releaseKey);
                }
            }
        }

        // For updates to .NET Framework then view these:
        // https://github.com/dotnet/docs/blob/master/docs/framework/migration-guide/how-to-determine-which-versions-are-installed.md
        // .. and "What is new" in:
        // https://docs.microsoft.com/en-us/dotnet/framework/migration-guide/versions-and-dependencies
        private static string CheckFor45DotVersion(int releaseKey)
        {
            string returnStr;
            switch (releaseKey)
            {
                case 378389:
                    returnStr = "4.5";
                    break;
                case 378675:
                case 378758:
                    returnStr = "4.5.1";
                    break;
                case 379893:
                    returnStr = "4.5.2";
                    break;
                case 393295:
                case 393297:
                    returnStr = "4.6";
                    break;
                case 394254:
                case 394271:
                    returnStr = "4.6.1";
                    break;
                case 394802:
                case 394806:
                    returnStr = "4.6.2";
                    break;
                case 460798:
                case 460805:
                    returnStr = "4.7";
                    break;
                case 461308:
                case 461310:
                    returnStr = "4.7.1";
                    break;
                case 461808:
                case 461814:
                    returnStr = "4.7.2";
                    break;
                case 528040:
                case 528049:
                case 528372: // also set in "default"
                    returnStr = "4.8";
                    break;
                default:
                    if (releaseKey > 528372) // also set in "4.8"
                    {
                        returnStr = "newer than 4.8";
                    } else
                    {
                        returnStr = "older than 4.5";
                    }
                    break;
            }
            return returnStr;
        }
*/


        // ###########################################################################################
    }
}
