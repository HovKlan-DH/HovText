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
        // Auto-install
        // ###########################################################################################

        private void GuiUpdateButton3_Click(object sender, EventArgs e)
        {
            Logging.Log("Update popup: Clicked the \"Auto-install\"");
            Logging.Log("Auto-install new [STABLE] version");
            Hide();
            Settings.DownloadInstall(GuiAppVerOnline.Text, this);
        }

        // ###########################################################################################
        // Download
        // ###########################################################################################

        private void Download_Click(object sender, EventArgs e)
        {
            Logging.Log("Update popup: Clicked the \"Download\"");
            Hide();

            // Open file location for the executeable
            string appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            Settings.OpenExecuteableLocation(appPath);

            // Download executeable
            System.Diagnostics.Process.Start(Settings.hovtextPage+"download/"+ GuiAppVerOnline.Text +"/HovText.exe");
        }


        // ###########################################################################################
        // Skip this version
        // ###########################################################################################

        private void SkipVersion_Click(object sender, EventArgs e)
        {
            Logging.Log("Update popup: Clicked the \"Skip this version\""); 
            Hide();
            Settings.SetRegistryKey(Settings.registryPath, "CheckedVersion", GuiAppVerOnline.Text);
        }


        // ###########################################################################################
    }
}
