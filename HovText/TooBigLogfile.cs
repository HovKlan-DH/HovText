﻿/*
##################################################################################################
TOO BIG LOGFILE (FORM)
----------------------

This is an information popup that will be shown, if the 
troubleshoot logfile has become larger than 10Mb. The it 
offers the user to either disable troubleshoot logging or 
to keep logging but then truncate the logfile.

##################################################################################################
*/

using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace HovText
{
    public partial class TooBigLogfile : Form
    {

        // ###########################################################################################
        // Class variables
        // ###########################################################################################   
        private readonly Settings _settings;


        // ###########################################################################################
        // Form initialization
        // ###########################################################################################

        public TooBigLogfile(Settings mainForm)
        {
            InitializeComponent();
            _settings = mainForm;

            // Make sure this form gets active
            Shown += TooBigLogfile_Shown;
        }


        // ###########################################################################################
        // Make sure this form gets shown
        // ###########################################################################################

        private void TooBigLogfile_Shown(object sender, EventArgs e)
        {
            Activate();
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
        // Keep troubleshoot logging but truncate logfile.
        // Retry X times.
        // ###########################################################################################

        private void GuiTooBigKeep_Click(object sender, EventArgs e)
        {
            int retries = 5;
            int msDelay = 1000;

            if (File.Exists(Settings.pathAndLog))
            {
                for (int i = 1; i <= retries; i++)
                {
                    try
                    {
                        File.Delete(@Settings.pathAndLog);
                        Logging.Log("Keep troubleshoot logging enabled but truncate file");
                        break;
                    }
                    catch (Exception ex)
                    {
                        Logging.Log($"Error: Could not delete troubleshooting logfile as it is being used by another process - try [{i}/{retries}]");
                        Logging.LogException(ex);
                    }
                    Thread.Sleep(msDelay);
                }
            }
            Close();
        }


        // ###########################################################################################
    }
}
