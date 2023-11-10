using System;
using System.IO;
using System.Windows.Forms;

namespace HovText
{
    public partial class TooBigLogfile : Form
    {
        private readonly Settings _settings;


        // ###########################################################################################
        // Main - TooBigLogfile
        // ###########################################################################################

        public TooBigLogfile(Settings mainForm)
        {
            InitializeComponent();
            _settings = mainForm;

            // Make sure this form gets active
            this.Shown += TooBigLogfile_Shown;
        }


        // ###########################################################################################
        // Make sure this form gets shown
        // ###########################################################################################

        private void TooBigLogfile_Shown(object sender, EventArgs e)
        {
            this.Activate();
        }


        // ###########################################################################################
        // Disable troubleshoot logging
        // ###########################################################################################

        private void GuiTooBigDisable_Click(object sender, EventArgs e)
        {
            Logging.Log("Disable troubleshoot logging");
            Settings.SetRegistryKey(Settings.registryPath, "TroubleshootEnable", "0");
            _settings.UpdateTroubleshootDisabled(); // uncheck the "Enable troubleshoot logging" checkbox
            Close();
        }


        // ###########################################################################################
        // Keep troubleshoot logging but truncate logfile
        // ###########################################################################################

        private void GuiTooBigKeep_Click(object sender, EventArgs e)
        {
            if (File.Exists(Settings.pathAndLog))
            {
                File.Delete(@Settings.pathAndLog);
            }
            Logging.Log("Keep troubleshoot logging but truncate file");
            Close();
        }


        // ###########################################################################################
    }
}
