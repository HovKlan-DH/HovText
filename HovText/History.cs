using System;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;

namespace HovText
{
    public partial class History : Form
    {
        public History()
        {
            InitializeComponent();
        }

        public void SetPosition()
        {

            double form2Size = Settings.form2Size;

            int form2Width = (int)(Settings.form2Width * form2Size);
            int form2Height = (int)(Settings.form2Height * form2Size);

            Width = form2Width;
            Height = form2Height;

            int x = Screen.PrimaryScreen.WorkingArea.Right - this.Width;
            int y = Screen.PrimaryScreen.WorkingArea.Bottom - this.Height;
            this.Left = x - 5;
            this.Top = y - 5;

            label1.Width = form2Width;
            textBoxHistory.Width = form2Width;
            textBoxHistory.Height = form2Height;
            pictureHistory.Width = form2Width;
            pictureHistory.Height = form2Height;

        }

        private void SetBackgroundColor()
        {
            // Set the font
            string fontFamily = Settings.fontFamily;
            float fontSize = Settings.fontSize;
            label1.Font = new Font(fontFamily, fontSize);
            //            historyIndexTxt.Font = new Font(fontFamily, fontSize);
            textBoxHistory.Font = new Font(fontFamily, fontSize);

            // Set the background color
            switch (Settings.themeColor)
            {
                case "Blue":
                    label1.BackColor = ColorTranslator.FromHtml(Settings.colorBlueTop);
                    textBoxHistory.BackColor = ColorTranslator.FromHtml(Settings.colorBlueBottom);
                    pictureHistory.BackColor = ColorTranslator.FromHtml(Settings.colorBlueBottom);
                    break;
                case "Brown":
                    label1.BackColor = ColorTranslator.FromHtml(Settings.colorBrownTop);
                    textBoxHistory.BackColor = ColorTranslator.FromHtml(Settings.colorBrownBottom);
                    pictureHistory.BackColor = ColorTranslator.FromHtml(Settings.colorBrownBottom);
                    break;
                case "Green":
                    label1.BackColor = ColorTranslator.FromHtml(Settings.colorGreenTop);
                    textBoxHistory.BackColor = ColorTranslator.FromHtml(Settings.colorGreenBottom);
                    pictureHistory.BackColor = ColorTranslator.FromHtml(Settings.colorGreenBottom);
                    break;
                case "Yellow":
                    label1.BackColor = ColorTranslator.FromHtml(Settings.colorYellowTop);
                    textBoxHistory.BackColor = ColorTranslator.FromHtml(Settings.colorYellowBottom);
                    pictureHistory.BackColor = ColorTranslator.FromHtml(Settings.colorYellowBottom);
                    break;
                case "White":
                    label1.BackColor = ColorTranslator.FromHtml(Settings.colorWhiteTop);
                    textBoxHistory.BackColor = ColorTranslator.FromHtml(Settings.colorWhiteBottom);
                    pictureHistory.BackColor = ColorTranslator.FromHtml(Settings.colorWhiteBottom);
                    break;
                default:
                    label1.BackColor = ColorTranslator.FromHtml(Settings.colorYellowTop);
                    textBoxHistory.BackColor = ColorTranslator.FromHtml(Settings.colorYellowBottom);
                    pictureHistory.BackColor = ColorTranslator.FromHtml(Settings.colorYellowBottom);
                    break;
            }
        }

        public void ShowText(string str, string str2, string application)
        {
            SetPosition();
            SetBackgroundColor();

            //            historyIndexTxt.Text = str2;
            label1.Text = str2;
            //            uiClipboardApplication.Text = application;
            textBoxHistory.Text = str; // this is actually a "label" and not a textbox
            textBoxHistory.Visible = true;
            pictureHistory.Image = null;
            pictureHistory.Visible = false;
            TopMost = true; // make sure this form is the top most form to ensure we can catch "ALT key up"
            Show();
        }


        // ###########################################################################################

        public void ShowImage(Image img, string str2, string application)
        {
            SetPosition();
            SetBackgroundColor();

            //            historyIndexTxt.Text = str2 +" (image)";
            label1.Text = str2 + " (image)";
            //            uiClipboardApplication.Text = application;
            pictureHistory.Image = img;
            pictureHistory.Visible = true;
            textBoxHistory.Text = ""; // this is actually a "label" and not a textbox
            textBoxHistory.Visible = false;
            TopMost = true; // make sure this form is the top most form to ensure we can catch "ALT key up"
            Show();
        }


        // ###########################################################################################

        private void History_KeyUp(object sender, KeyEventArgs e)
        {
            if (Settings.isAltPressedInThisApp)
            {
                if (e.KeyCode == Keys.Menu)
                {
                    Settings.isAltPressedInThisApp = false;
                    Settings.settings.ReleaseAltKey();
                    if (Settings.isEnabledPasteOnSelection)
                    {
                        SendKeys.Send("^v");
                    }
                    Console.WriteLine("ALT key up (FORM 2)");
                }
            }
        }


        // ###########################################################################################
        // https://stackoverflow.com/a/4147406/2028935

        public void Flash(int interval)
        {
            SetBackgroundColor();
            Color color2 = textBoxHistory.BackColor;
            new Thread(() => FlashInternal(interval, color2)).Start();
        }

        private delegate void UpdateElementDelegate(Color originalColor);

        public void UpdateElement(Color color)
        {
            if (label1.InvokeRequired)
            {
                this.Invoke(new UpdateElementDelegate(UpdateElement), new object[] { color });
            }
            label1.BackColor = color;
        }

        private void FlashInternal(int interval, Color flashColor)
        {
            Color original = label1.BackColor;
            UpdateElement(flashColor);
            Thread.Sleep(interval / 2);
            UpdateElement(original);
            Thread.Sleep(interval / 2);
        }


        // ###########################################################################################


    }
}
