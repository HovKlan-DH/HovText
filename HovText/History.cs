using System;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;

namespace HovText
{
    public partial class History : Form
    {
        // ###########################################################################################
        // Main
        // ###########################################################################################

        public History()
        {
            InitializeComponent();
        }


        // ###########################################################################################
        // Set the position and style (color and font) for the history area
        // ###########################################################################################

        public void SetHistoryPosition()
        {
            // Set this form width and height
            Width = Settings.historyWidth;
            Height = Settings.historyHeight;

            // Get history location
            int x;
            int y;
            switch (Settings.historyLocation)
            {
                case "Center":
                    x = (Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2;
                    y = (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2;
                    this.Left = x;
                    this.Top = y;
                    break;
                case "Top Left":
                    x = Screen.PrimaryScreen.WorkingArea.Left;
                    y = Screen.PrimaryScreen.WorkingArea.Top;
                    this.Left = x + 5;
                    this.Top = y + 5;
                    break;
                case "Top Right":
                    x = Screen.PrimaryScreen.WorkingArea.Right - this.Width;
                    y = Screen.PrimaryScreen.WorkingArea.Top;
                    this.Left = x - 5;
                    this.Top = y + 5;
                    break;
                case "Bottom Left":
                    x = Screen.PrimaryScreen.WorkingArea.Left;
                    y = Screen.PrimaryScreen.WorkingArea.Bottom - this.Height;
                    this.Left = x + 5;
                    this.Top = y - 5;
                    break;
                default: // Bottom Right
                    x = Screen.PrimaryScreen.WorkingArea.Right - this.Width;
                    y = Screen.PrimaryScreen.WorkingArea.Bottom - this.Height;
                    this.Left = x - 5;
                    this.Top = y - 5;
                    break;
            }

            topline.Width = Settings.historyWidth;
            textBoxHistory.Width = Settings.historyWidth;
            textBoxHistory.Height = Settings.historyHeight;
            pictureHistory.Width = Settings.historyWidth;
            pictureHistory.Height = Settings.historyHeight;
        }

        private void SetHistoryStyle()
        {
            // Set the font
            topline.Font = new Font(Settings.historyFontFamily, Settings.historyFontSize);
            textBoxHistory.Font = new Font(Settings.historyFontFamily, Settings.historyFontSize);

            // Set the background color
            topline.BackColor = ColorTranslator.FromHtml(Settings.historyColorTop);
            textBoxHistory.BackColor = ColorTranslator.FromHtml(Settings.historyColorBottom);
            pictureHistory.BackColor = ColorTranslator.FromHtml(Settings.historyColorBottom);
        }


        // ###########################################################################################
        // Show either the text or image history
        // ###########################################################################################

        public void ShowText(string strTop, string strBody)
        {
            SetHistoryPosition();
            SetHistoryStyle();

            topline.Text = strTop;
            textBoxHistory.Text = strBody;
            textBoxHistory.Visible = true;
            pictureHistory.Image = null;
            pictureHistory.Visible = false;
            TopMost = true; // make sure this form is the top most form
            Show();
        }

        public void ShowImage(string strTop, Image imgBody)
        {
            SetHistoryPosition();
            SetHistoryStyle();

            topline.Text = strTop + " (image)";
            pictureHistory.Image = imgBody;
            pictureHistory.Visible = true;
            textBoxHistory.Text = "";
            textBoxHistory.Visible = false;
            TopMost = true; // make sure this form is the top most form
            Show();
        }


        // ###########################################################################################
        // Catch keyboard input to react on ENTER or ESCAPE
        // ###########################################################################################

        private void History_KeyUp(object sender, KeyEventArgs e)
        {
            if (Settings.isHistoryHotkeyPressed)
            {

                // Get modifier keys
                bool isShift = e.Shift;
                bool isAlt = e.Alt;
                bool isControl = e.Control;

                // Select active entry, if no modifier keys are pressed down
                if (!isShift && !isAlt && !isControl)
                    {
                    Settings.settings.SelectHistoryEntry();
                    if (Settings.isEnabledPasteOnSelection)
                    {
                        SendKeys.Send("^v");
                    }
                    Console.WriteLine("Select entry");
                }
            }
        }


        // ###########################################################################################
        // Blink the top line in the history when it reaches oldest or newest element
        // https://stackoverflow.com/a/4147406/2028935
        // ###########################################################################################

        public void Flash(int interval)
        {
            Color color2 = textBoxHistory.BackColor;
            new Thread(() => FlashInternal(interval, color2)).Start();
        }

        private delegate void UpdateElementDelegate(Color originalColor);

        public void UpdateElement(Color color)
        {
            if (topline.InvokeRequired)
            {
                this.Invoke(new UpdateElementDelegate(UpdateElement), new object[] { color });
            }
            topline.BackColor = color;
        }

        private void FlashInternal(int interval, Color flashColor)
        {
            Color original = topline.BackColor;
            UpdateElement(flashColor);
            Thread.Sleep(interval / 2);
            UpdateElement(original);
            Thread.Sleep(interval / 2);
        }


        // ###########################################################################################
    }
}
