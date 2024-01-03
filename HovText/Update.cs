/*
##################################################################################################
UPDATE (FORM)
-------------

This is an information popup that will be shown, if a 
new version of HovText is available. It will show the 
user the version number of the new version and offer 
the user to either download the new version or to skip 
this version.

##################################################################################################
*/

using System;
using System.Text;
using System.Windows.Forms;

namespace HovText
{
    public sealed partial class Update : Form
    {

        // ###########################################################################################
        // Form initialization
        // ###########################################################################################

        public Update()
        {
            InitializeComponent();

            // Update RTF text with a link
            var sb = new StringBuilder();
            sb.Append(@"{\rtf1\ansi");
            sb.Append(@" Check what has changed on the HovText download page, " + Settings.hovtextPageDownload + @" \line ");
            sb.Append('}');
            UpdateGoToHomepage.Rtf = sb.ToString();

            // Close form on ESCAPE
            KeyPreview = true;
            KeyDown += new System.Windows.Forms.KeyEventHandler(TooBigLogfile_KeyDown);
        }


        // ###########################################################################################
        // Auto-install
        // ###########################################################################################

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            Logging.Log("Update popup: Clicked the \"Auto-install\"");
            Logging.Log("Auto-install new [STABLE] version");
            Settings.AutoInstall("Stable");
            Close();
        }


        // ###########################################################################################
        // Download
        // ###########################################################################################

        private void Download_Click(object sender, EventArgs e)
        {
            Logging.Log("Update popup: Clicked the \"Download\"");

            // Open file location for the executeable
            string appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            Settings.OpenExecuteableLocation(appPath);

            // Download executeable
            System.Diagnostics.Process.Start(Settings.hovtextPage + "/download/" + GuiAppVerOnline.Text + "/HovText.exe");
            
            Close();
        }


        // ###########################################################################################
        // Skip this version
        // ###########################################################################################

        private void SkipVersion_Click(object sender, EventArgs e)
        {
            Logging.Log("Update popup: Clicked the \"Skip this version\"");
            Settings.SetRegistryKey(Settings.registryPath, "CheckedVersion", GuiAppVerOnline.Text);
            Close();
        }


        // ###########################################################################################
        // Close form on ESCAPE
        // ###########################################################################################

        private void TooBigLogfile_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }


        // ###########################################################################################
        // Open download page
        // ###########################################################################################

        private void UpdateGoToHomepage_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Logging.Log("Clicked the download page link in \"Update\"");
            System.Diagnostics.Process.Start(e.LinkText);
        }


        // ###########################################################################################
    }
}
