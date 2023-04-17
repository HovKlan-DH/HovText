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
        // Go to the HovText web page
        // ###########################################################################################

        private void GoToPage_Click(object sender, EventArgs e)
        {
            if (Settings.isTroubleshootEnabled) Logging.Log("Clicked the \"Go to web page\""); 
            Hide();
            System.Diagnostics.Process.Start(Settings.hovtextPage+"download/");
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
