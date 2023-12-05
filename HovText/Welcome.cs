/*
##################################################################################################
WELCOME GUIDE
-------------

This is a welcome guide that will be shown to the user the first time the application is launched.
It will not be shown if the "NotificationShown" has been set to "1" in registry.

##################################################################################################
*/

using System;
using System.Windows.Forms;

namespace HovText
{
    public partial class Welcome : Form
    {
        public Welcome()
        {
            InitializeComponent();

            guna2Button6.Focus();

            this.FormClosing += new FormClosingEventHandler(Welcome_FormClosing);
        }


        // ###########################################################################################
        // Make the "ESCAPE" key close the window.
        // ###########################################################################################

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true; // Indicate that you've handled the key press
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }


        // ###########################################################################################
        // Catch the click-event for the button
        // ###########################################################################################

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        // ###########################################################################################
        // Catch the "Form Close" event
        // ###########################################################################################

        private void Welcome_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.RegistryKeyCheckOrCreate(Settings.registryPath, "WelcomeShown", "1");
            int welcomeShown = int.Parse((string)Settings.GetRegistryKey(Settings.registryPath, "WelcomeShown"));
            if(welcomeShown == 0) {
                Settings.SetRegistryKey(Settings.registryPath, "WelcomeShown", "1");
            }

            Logging.Log("\"Welcome Guide\" was shown");
        }

        // ###########################################################################################
    }
}
