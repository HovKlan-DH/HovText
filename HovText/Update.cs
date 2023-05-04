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
            Logging.Log("Update popup: Clicked the \"Go to web page\"");
            Hide();

            // Open file location for the executeable
            string appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            Settings.OpenExecuteableLocation(appPath);

            // Download executeable
            System.Diagnostics.Process.Start(Settings.hovtextPage+"download/"+ uiAppVerOnline.Text +"/HovText.exe");
        }


        // ###########################################################################################
        // Skip this version
        // ###########################################################################################

        private void SkipVersion_Click(object sender, EventArgs e)
        {
            Logging.Log("Update popup: Clicked the \"Skip this version\""); 
            Hide();
            Settings.SetRegistryKey(Settings.registryPath, "CheckedVersion", uiAppVerOnline.Text);
        }


        // ###########################################################################################
        // Auto-update
        // ###########################################################################################

        private void button3_Click(object sender, EventArgs e)
        {
            Logging.Log("Update popup: Clicked the \"Auto-update\"");
            Logging.Log("Auto-updating to new [STABLE] version:");
            Hide();
            Settings.DownloadInstall(uiAppVerOnline.Text);
        }


        // ###########################################################################################
    }
}
