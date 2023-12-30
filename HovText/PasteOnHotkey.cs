﻿/*
##################################################################################################
PASTE ON HOTKEY
---------------

This is a form that will be shown when the user presses the hotkey for pasting cleartext. It 
will paste the cleartext clipboard to the active application and then restore the original 
clipboard after 250ms.

##################################################################################################
*/

using System.Threading;
using System.Timers;
using System.Windows.Forms;

namespace HovText
{
    public sealed partial class PasteOnHotkey : Form
    {

        // ###########################################################################################
        // Define "PasteOnHotkey" class variables - real spaghetti :-)
        // ###########################################################################################
        private readonly static System.Timers.Timer timerPasteOnHotkey = new System.Timers.Timer();


        // ###########################################################################################
        // Main
        // ###########################################################################################

        public PasteOnHotkey()
        {
            InitializeComponent();

            // Define a timer
            timerPasteOnHotkey.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            timerPasteOnHotkey.Interval = 250;
            timerPasteOnHotkey.Enabled = false;

            // Make this form invisible
            this.Size = new System.Drawing.Size(0, 0);
            this.Opacity = 0.0;
        }


        // ###########################################################################################
        // Catch keyboard input to react on modifier KEY UP
        // ###########################################################################################

        private void PasteOnHotkey_KeyUp(object sender, KeyEventArgs e)
        {
            // Check if modifier keys are pressed
            bool isShift = e.Shift;
            bool isAlt = e.Alt;
            bool isControl = e.Control;

            // Proceed if no modifier keys are pressed down
            if (!isShift && !isAlt && !isControl)
            {
                Hide();
                Settings.pasteOnHotkeySetCleartext = true;
                HandleClipboard.SetClipboard(HandleClipboard.threadSafeIndex - 1);
                Settings.ChangeFocusToOriginatingApplication();
                SendKeys.SendWait("^v"); // send "CTRL + v" (paste from clipboard)
                StartTimerToRestoreOriginal();

                Logging.Log("Pasted cleartext clipboard");
            }
        }


        // ###########################################################################################
        // What should happen when the timer has reached its time
        // ###########################################################################################

        public static void StartTimerToRestoreOriginal()
        {
            timerPasteOnHotkey.Enabled = true;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            // Stop the timer
            timerPasteOnHotkey.Enabled = false;

            // As this application is Single Thread then launch a new thread to mess with the clipboard again
            // https://stackoverflow.com/a/23803659/2028935
            Thread thread = new Thread(() => HandleClipboard.RestoreOriginal(HandleClipboard.threadSafeIndex - 1));
            thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            thread.Start();
            thread.Join();

            // We do no longer need to paste cleartext
            Settings.pasteOnHotkeySetCleartext = false;

            Logging.Log("Pasted original clipboard");
        }


        // ###########################################################################################
    }
}
