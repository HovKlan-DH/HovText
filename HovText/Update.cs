using System;
using System.Windows.Forms;

namespace HovText
{
    public sealed partial class Update : Form
    {
        // ###########################################################################################
        // Main
        // ###########################################################################################
        public Update()
        {
            InitializeComponent();
        }


        // ###########################################################################################
        // Go to the HovText download web page
        // ###########################################################################################

        private void GoToPage_Click(object sender, EventArgs e)
        {
            Logging.Log("Clicked the \"Go to web page\"");
            Hide();
            System.Diagnostics.Process.Start(Settings.hovtextPage+"download/");
            
/*
// EXPERIMENTAL AUTO-UPDATER:
// Does not work because Microsoft Defender blocks this.
// It is considered dangerous to dynamically create a script file and execute that - this is fair,
// as no one knows what could be triggered from there, so I need another approach.

                        string appUrl = Settings.hovtextPage + "/autoupdate/development/HovText.exe";
                        string appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                        string appDirectory = Path.GetDirectoryName(appPath);
                        string appFileName = Path.GetFileName(appPath);
                        string tempFilePath = Path.Combine(Path.GetTempPath(), "HovText_new.exe");
                        WebClient webClient = new WebClient();
                        webClient.DownloadFile(appUrl, tempFilePath);
                        string batchContents = @"
@echo off
:wait
timeout /t 1 >nul
tasklist /nh /fi ""imagename eq " + appFileName + @""" | find /i """ + appFileName + @""" >nul && goto :wait
echo Moving: """ + tempFilePath + @""" to """ + appPath + @"""
move /y """ + tempFilePath + @""" """ + appPath + @""" 2>move_error.log
IF %ERRORLEVEL% NEQ 0 (
echo Move failed. Check move_error.log for details.
pause
exit /b 1
)
echo Move successful.
start """" """ + appPath + @"""
del ""%~f0""
";

                        string batchFilePath = Path.Combine(Path.GetTempPath(), "HovText_update.cmd");
                        File.WriteAllText(batchFilePath, batchContents);
                        Process.Start(batchFilePath);
                        Environment.Exit(0);
*/
        }


        // ###########################################################################################
        // Skip this version
        // ###########################################################################################

        private void SkipVersion_Click(object sender, EventArgs e)
        {
            if (Settings.isTroubleshootEnabled) Logging.Log("Clicked the \"Skip this version\""); 
            Hide();
            Settings.SetRegistryKey(Settings.registryPath, "CheckedVersion", uiAppVerOnline.Text);
        }


        // ###########################################################################################
    }
}
