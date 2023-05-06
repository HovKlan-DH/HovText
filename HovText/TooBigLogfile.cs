using System;
using System.IO;
using System.Windows.Forms;

namespace HovText
{
    public partial class TooBigLogfile : Form
    {
        private Settings _settings;

        public TooBigLogfile(Settings mainForm)
        {
            InitializeComponent();
            _settings = mainForm;

            // Make sure this form gets active
            this.Shown += TooBigLogfile_Shown;
        }

        private void TooBigLogfile_Shown(object sender, EventArgs e)
        {
            this.Activate();
        }

        private void GuiDev2StableDisable_Click(object sender, EventArgs e)
        {
            Logging.Log("Disable troubleshoot logging");
            Settings.SetRegistryKey(Settings.registryPath, "VersionLastUsed", Settings.appVer); 
            Settings.SetRegistryKey(Settings.registryPath, "TroubleshootEnable", "0");
            _settings.UpdateTroubleshootDisabled(); // uncheck the "Enable troubleshoot logging" checkbox
            Close();
        }

        private void GuiDev2StableKeep_Click(object sender, EventArgs e)
        {
            Settings.SetRegistryKey(Settings.registryPath, "VersionLastUsed", Settings.appVer);
            if (File.Exists(Settings.troubleshootLogfile))
            {
                File.Delete(@Settings.troubleshootLogfile);
            }
            Logging.Log("Keep troubleshoot logging but truncate file");
            Close();
        }
    }
}
