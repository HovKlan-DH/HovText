using System;
using System.Windows.Forms;

namespace HovText
{
    public partial class Update : Form
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
            Hide();
            System.Diagnostics.Process.Start(Settings.hovtextPage+"download/");
        }


        // ###########################################################################################
        // Skip this version
        // ###########################################################################################

        private void SkipVersion_Click(object sender, EventArgs e)
        {
            Hide();
            Settings.settings.SetRegistryKey(Settings.settings.registryPath, "CheckedVersion", uiAppVerOnline.Text);
        }


        // ###########################################################################################
    }
}
