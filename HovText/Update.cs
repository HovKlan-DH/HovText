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

            // Close form on ESCAPE
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MyForm_KeyDown);
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
            System.Diagnostics.Process.Start(Settings.hovtextPage+"download/"+ GuiAppVerOnline.Text +"/HovText.exe");
        }


        // ###########################################################################################
        // Skip this version
        // ###########################################################################################

        private void SkipVersion_Click(object sender, EventArgs e)
        {
            Logging.Log("Update popup: Clicked the \"Skip this version\""); 
            Hide();
            Settings.SetRegistryKey(Settings.registryPath, "VersionOnline", GuiAppVerOnline.Text);
        }


        // ###########################################################################################
        // Close form on ESCAPE
        // ###########################################################################################

        private void MyForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }


        // ###########################################################################################
    }
}
