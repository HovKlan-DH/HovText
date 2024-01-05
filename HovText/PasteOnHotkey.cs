/*
##################################################################################################
PASTE ON HOTKEY (FORM)
----------------------

This is a form that will be shown when the user presses 
the hotkey for pasting cleartext. It will paste the 
cleartext clipboard to the active application and then 
restore the original clipboard after 250ms.

##################################################################################################
*/

using System.Timers;
using System.Windows.Forms;
using System.Threading;

namespace HovText
{
    public sealed partial class PasteOnHotkey : Form
    {

        // ###########################################################################################
        // Class variables
        // ###########################################################################################   
        private readonly static System.Timers.Timer timerPasteOnHotkey = new System.Timers.Timer();


        // ###########################################################################################
        // Form initialization
        // ###########################################################################################

        public PasteOnHotkey()
        {
            InitializeComponent();

            // Define a timer
            timerPasteOnHotkey.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            timerPasteOnHotkey.Interval = 250;
            timerPasteOnHotkey.Enabled = false;

            // Make this form invisible
            Size = new System.Drawing.Size(0, 0);
            Opacity = 0.0;
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

                Logging.Log($"Pasted cleartext clipboard [{HandleClipboard.threadSafeIndex - 1}]");
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

            bool hest = Settings.pasteOnHotkeySetCleartext;

            // As this application is Single Thread then launch a new thread to mess with the clipboard again
            // https://stackoverflow.com/a/23803659/2028935
            Thread thread = new Thread(() => HandleClipboard.SetClipboard(HandleClipboard.threadSafeIndex - 1));
            //Thread thread = new Thread(() => HandleClipboard.RestoreOriginal(HandleClipboard.threadSafeIndex - 1));
            thread.SetApartmentState(ApartmentState.STA); // set the thread to STA
            thread.Start();
            thread.Join();

            if (Settings.pasteOnHotkeySetCleartext)
            {
                Logging.Log($"Populated cleartext to clipboard [{HandleClipboard.threadSafeIndex - 1}]");
            } else
            {
                Logging.Log($"Populated original to clipboard [{HandleClipboard.threadSafeIndex - 1}]");
            }
            

            // We do no longer need to paste cleartext
            Settings.pasteOnHotkeySetCleartext = false;
        }


        // ###########################################################################################
    }
}
