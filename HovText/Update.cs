using System;
using System.Text;
using System.Windows.Forms;

namespace HovText
{
    public sealed partial class Update : Form
    {

        // ###########################################################################################
        // Main - Update
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
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TooBigLogfile_KeyDown);
        }


        // ###########################################################################################
        // Auto-install
        // ###########################################################################################

        private void GuiUpdateButton3_Click(object sender, EventArgs e)
        {
            Logging.Log("Update popup: Clicked the \"Auto-install\"");
            Logging.Log("Auto-install new [STABLE] version");
            Hide();
            Settings.DownloadInstall(GuiAppVerOnline.Text);
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
            System.Diagnostics.Process.Start(Settings.hovtextPage +"/download/"+ GuiAppVerOnline.Text +"/HovText.exe");
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
        // Close form on ESCAPE
        // ###########################################################################################

        private void TooBigLogfile_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void UpdateGoToHomepage_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Logging.Log("Clicked the download page link in \"Update\"");
            System.Diagnostics.Process.Start(e.LinkText);
        }


        // ###########################################################################################
    }
}
