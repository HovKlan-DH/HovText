using System;
using System.Windows.Forms;

namespace HovText
{
    public partial class Update : Form
    {
        public Update()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Hide();
            System.Diagnostics.Process.Start("http://hovtext.com/");
            Settings.settings.SetRegistryKey(Settings.settings.registryPath, "CheckedVersion", uiAppVerOnline.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Hide();
            Settings.settings.SetRegistryKey(Settings.settings.registryPath, "CheckedVersion", uiAppVerOnline.Text);
        }

    }
}
