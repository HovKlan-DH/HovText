using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace HovText
{
    public partial class History : Form
    {
        // ###########################################################################################
        // Define "History" class variables - real spaghetti :-)
        // ###########################################################################################
        int firstBox;
        int entryIndexLast;
        string changeBorderElement = "";


        // ###########################################################################################
        // Main
        // ###########################################################################################

        public History()
        {
            InitializeComponent();
        }


        // ###########################################################################################
        // Setup the history form for all its UI elements
        // ###########################################################################################

        public void SetupForm()
        {
            int entries = Settings.entriesText.Count;
            int showElements = Settings.historyListElements;
            showElements = showElements > entries ? entries : showElements;
            int width;
            int nextPosY = 0;

            // Set this form width and element heights
            int headlineHeight = 42;
            int workingAreaWidth = Screen.PrimaryScreen.WorkingArea.Width;
            int workingAreaHeight = Screen.PrimaryScreen.WorkingArea.Height;
            int height = (workingAreaHeight * Settings.historySizeHeight) / 100;
            int boxHeight = (height - headlineHeight) / showElements;
            width = (workingAreaWidth * Settings.historySizeWidth) / 100;

            // Setup form width and height
            Width = width;
            Height = (boxHeight * showElements) + headlineHeight + SystemInformation.FrameBorderSize.Height + SystemInformation.Border3DSize.Height + 4;

            // Add the headline, if not already created
            if (this.Controls.Count == 0)
            {
                Label label = new Label();
                label.Name = "uiHistoryHeadline";
                label.Width = width;
                label.Height = headlineHeight;
                label.Location = new Point(0, 0);
                label.BorderStyle = BorderStyle.FixedSingle;
                label.Padding = new Padding(5);
                label.TextAlign = ContentAlignment.MiddleLeft;
                label.Font = new Font(Settings.historyFontFamily, Settings.historyFontSize);
                label.BackColor = ColorTranslator.FromHtml(Settings.historyColorsTop[Settings.historyColor]);
                label.ForeColor = ColorTranslator.FromHtml(Settings.historyColorsText[Settings.historyColor]);
                label.Visible = true;
                this.Controls.Add(label);
            } else
            {
                // If created then only update the colors etc.
                this.Controls["uiHistoryHeadline"].Width = width;
                this.Controls["uiHistoryHeadline"].Height = headlineHeight;
                this.Controls["uiHistoryHeadline"].Font = new Font(Settings.historyFontFamily, Settings.historyFontSize);
                this.Controls["uiHistoryHeadline"].BackColor = ColorTranslator.FromHtml(Settings.historyColorsTop[Settings.historyColor]);
                this.Controls["uiHistoryHeadline"].ForeColor = ColorTranslator.FromHtml(Settings.historyColorsText[Settings.historyColor]);
            }

            nextPosY += headlineHeight;

            // Setup all visible element boxes
            int instance = 1;
            for (int i = 1; i <= showElements; i++)
            {
                // Label
                Label label = new Label();
                label.Name = "historyLabel" + i;
                label.Width = width;
                label.Height = boxHeight;
                label.Location = new Point(0, nextPosY);
                label.BorderStyle = BorderStyle.FixedSingle;
                label.Padding = new Padding(5);
                label.Font = new Font(Settings.historyFontFamily, Settings.historyFontSize);
                label.Visible = false;
                label.Text = "";
                this.Controls.Add(label);

                // Catch repaint event for this specific element (to draw the border)
                this.Controls[instance].Paint += new System.Windows.Forms.PaintEventHandler(this.History_Paint);
                instance++;

                // PictureBox
                PictureBox pictureBox = new PictureBox();
                pictureBox.Name = "historyPictureBox" + i;
                pictureBox.Width = width;
                pictureBox.Height = boxHeight;
                pictureBox.Location = new Point(0, nextPosY);
                pictureBox.BorderStyle = BorderStyle.FixedSingle;
                pictureBox.Padding = new Padding(10);
                pictureBox.Visible = false;
                pictureBox.Image = null;
                this.Controls.Add(pictureBox);

                // Catch repaint event for this specific element (to draw the border)
                this.Controls[instance].Paint += new System.Windows.Forms.PaintEventHandler(this.History_Paint);
                instance++;

                nextPosY += boxHeight;
            }

            SetHistoryPosition();
            Show();
            Activate();
        }


        // ###########################################################################################
        // Remove all form elements and make the form ready for the next layout setup
        // ###########################################################################################

        private void ResetForm()
        {
            // remove all form elements, except element [0] which is the "uiHistoryHeadline"
            int formElements = this.Controls.Count;
            for (int i = 0; i < formElements; i++)
            {
                if (i != 0)
                {
                    this.Controls.RemoveAt(1);
                }
            }
            Hide();
        }


        // ###########################################################################################
        // Update the form elements - set content and color
        // ###########################################################################################

        private void History_Paint(object sender, PaintEventArgs e)
        {
            if (Settings.historyBorder)
            {
                if (changeBorderElement == ((System.Windows.Forms.Control)sender).Name)
                {
                    ControlPaint.DrawBorder(e.Graphics, e.ClipRectangle,
                                  ColorTranslator.FromHtml(Settings.historyColorsBorder[Settings.historyColor]), 1, ButtonBorderStyle.Solid,
                                  ColorTranslator.FromHtml(Settings.historyColorsBorder[Settings.historyColor]), 1, ButtonBorderStyle.Solid,
                                  ColorTranslator.FromHtml(Settings.historyColorsBorder[Settings.historyColor]), 1, ButtonBorderStyle.Solid,
                                  ColorTranslator.FromHtml(Settings.historyColorsBorder[Settings.historyColor]), 1, ButtonBorderStyle.Solid);
                }
                else
                {
                    ControlPaint.DrawBorder(e.Graphics, e.ClipRectangle, this.BackColor, ButtonBorderStyle.None);
                }
            }
            else
            {
                ControlPaint.DrawBorder(e.Graphics, e.ClipRectangle, this.BackColor, ButtonBorderStyle.None);
            }
        }


        // ###########################################################################################
        // Show content of the history list
        // ###########################################################################################
                
        public void UpdateHistory(string direction)
        {

            int entries = Settings.entriesText.Count;
            int showElements = Settings.historyListElements;
            showElements = showElements > entries ? entries : showElements;
            int entryIndex = Settings.entryCounter; // 1 is the lowest value as a human readable number
            firstBox = firstBox == 0 ? entries : firstBox;
            int scopeHighest = firstBox; // "showElements" is the lowest value as a human readable number (newest copy on top)
            int scopeLowest = scopeHighest - showElements + 1; // 1 is the lowest value as a human readable number (oldest copy at bottom)
            scopeLowest = scopeLowest < 1 ? 1 : scopeLowest;

            // Set the "firstBox" depending if we are going down (older) or up (newer) in history
            if (direction == "down")
            {
                firstBox = entryIndex >= scopeLowest ? firstBox : firstBox - 1;
            }
            if (direction == "up")
            {
                firstBox = entryIndex <= scopeHighest ? firstBox : firstBox + 1;
            }

            // Flash the headline if we are all the way to the top or bottom
            if (entryIndex == entryIndexLast && (entryIndex == entries || entryIndex == 1))
            {
                Flash();
            }
            entryIndexLast = entryIndex;

            // Set the headline
            bool isTransparent;
            string entryApplication = Settings.entriesApplication.ElementAt(entryIndex - 1).Value;
            this.Controls["uiHistoryHeadline"].Text = entryIndex + " of " + entries + " from \"" + entryApplication + "\"";
            Image entryImage = Settings.entriesImage.ElementAt(entryIndex - 1).Value;
            if (entryImage != null)
            {
                isTransparent = Settings.entriesImageTransparent.ElementAt(entryIndex - 1).Value;
                if(isTransparent)
                {
                    this.Controls["uiHistoryHeadline"].Text += " (transparent image)";
                }
                else
                {
                    this.Controls["uiHistoryHeadline"].Text += " (image)";
                }
                
            }
            this.Controls["uiHistoryHeadline"].Refresh();

            // Update the history list view
            for (int i = 1; i <= showElements; i++)
            {
                int activeEntryKey = firstBox - i;

                // Get the data from the data arrays
                string entryText = Settings.entriesText.ElementAt(activeEntryKey).Value;
                entryImage = Settings.entriesImage.ElementAt(activeEntryKey).Value;

                // Check if it is a TEXT
                if (!string.IsNullOrEmpty(entryText))
                {
                    // Find a form element with a specific name
                    foreach (Control c in this.Controls.Find("historyLabel" + i, true))
                    {
                        c.Text = entryText;
                        c.Visible = true;

                        // Set the colors
                        if (entryIndex == activeEntryKey + 1 && showElements > 1)
                        {
                            c.BackColor = ColorTranslator.FromHtml(Settings.historyColorsTop[Settings.historyColor]);
                            c.ForeColor = ColorTranslator.FromHtml(Settings.historyColorsText[Settings.historyColor]);
                            changeBorderElement = c.Name;
                            c.Refresh();
                        }
                        else
                        {
                            c.BackColor = ColorTranslator.FromHtml(Settings.historyColorsBottom[Settings.historyColor]);
                            c.ForeColor = ColorTranslator.FromHtml(Settings.historyColorsText[Settings.historyColor]);
                        }
                        
                    }
                } else
                {
                    // Hide the label as it then must be an image
                    this.Controls["historyLabel" + i].Visible = false;
                }

                // Check if it is an IMAGE
                if (entryImage != null)
                {
                    // Find a form element with a specific name
                    foreach (Control c in this.Controls.Find("historyPictureBox" + i, true))
                    {
                        // Check if the image is transparent - if so make the image background transparent
                        isTransparent = Settings.entriesImageTransparent.ElementAt(activeEntryKey).Value;
                        if(isTransparent)
                        {
                            Bitmap bmp = new Bitmap(entryImage);
                            bmp.MakeTransparent(bmp.GetPixel(0, 0));
                            ((PictureBox)c).Image = (Image)bmp;
                        }
                        else
                        {
                            ((PictureBox)c).Image = entryImage;
                        }

                        c.Visible = true;

                        // Set the colors
                        if (entryIndex == activeEntryKey + 1 && showElements > 1)
                        {
                            c.BackColor = ColorTranslator.FromHtml(Settings.historyColorsTop[Settings.historyColor]);
                            c.ForeColor = ColorTranslator.FromHtml(Settings.historyColorsText[Settings.historyColor]);
                            changeBorderElement = c.Name;
                            c.Refresh();
                        }
                        else
                        {
                            c.BackColor = ColorTranslator.FromHtml(Settings.historyColorsBottom[Settings.historyColor]);
                            c.ForeColor = ColorTranslator.FromHtml(Settings.historyColorsText[Settings.historyColor]);
                        }
                    }
                } else
                {
                    // Hide the pictureBox as it then must be a text
                    this.Controls["historyPictureBox" + i].Visible = false;
                }
            }

            // Make sure that we will catch the key-up event
            TopMost = true;
        }

       

        // ###########################################################################################
        // Catch keyboard input to react on modifier KEY UP
        // ###########################################################################################

        private void History_KeyUp(object sender, KeyEventArgs e)
        {
            // Check if modifier keys are pressed
            bool isShift = e.Shift;
            bool isAlt = e.Alt;
            bool isControl = e.Control;

            // Proceed if no modifier keys are pressed down
            if (!isShift && !isAlt && !isControl)
            {
                // Reset if the entry is selected
                firstBox = 0;
                entryIndexLast = 0;

                Settings.settings.SelectHistoryEntry();
                if (Settings.isEnabledPasteOnSelection)
                {
                    SendKeys.Send("^v");
                }

                // Show the "Settings" form again, if it was visible before the hotkey keypress
                if (Settings.isSettingsFormVisible)
                {
                    Settings.settings.Show();
                }
                Debug.WriteLine("Selected history entry");

                ResetForm();
            }
        }


        // ###########################################################################################
        // Set the position for the history area
        // ###########################################################################################

        private void SetHistoryPosition()
        {
            
            // Set history location
            int x;
            int y;
            int airgab = 5;
            switch (Settings.historyLocation)
            {
                case "Left Top":
                    x = Screen.PrimaryScreen.WorkingArea.Left;
                    y = Screen.PrimaryScreen.WorkingArea.Top;
                    this.Left = x + airgab;
                    this.Top = y + airgab;
                    break;
                case "Left Bottom":
                    x = Screen.PrimaryScreen.WorkingArea.Left;
                    y = Screen.PrimaryScreen.WorkingArea.Bottom - this.Height;
                    this.Left = x + airgab;
                    this.Top = y - airgab;
                    break;
                case "Center":
                    x = (Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2;
                    y = (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2;
                    this.Left = x;
                    this.Top = y;
                    break;
                case "Right Top":
                    x = Screen.PrimaryScreen.WorkingArea.Right - this.Width;
                    y = Screen.PrimaryScreen.WorkingArea.Top;
                    this.Left = x - airgab;
                    this.Top = y + airgab;
                    break;
                default: // Right Bottom
                    x = Screen.PrimaryScreen.WorkingArea.Right - this.Width;
                    y = Screen.PrimaryScreen.WorkingArea.Bottom - this.Height;
                    this.Left = x - airgab;
                    this.Top = y - airgab;
                    break;
            }
        }


        // ###########################################################################################
        // Blink the top line in the history when it reaches oldest or newest element
        // https://stackoverflow.com/a/4147406/2028935
        // ###########################################################################################

        private void Flash()
        {
            new Thread(() => FlashInternal()).Start();
        }

        private delegate void UpdateElementDelegate(Color originalColor);

        private void UpdateElement(Color color)
        {
            if (this.Controls["uiHistoryHeadline"].InvokeRequired)
            {
                this.Invoke(new UpdateElementDelegate(UpdateElement), new object[] { color });
            }
            this.Controls["uiHistoryHeadline"].BackColor = color;
        }

        private void FlashInternal()
        {
            int interval = 100;
            UpdateElement(ColorTranslator.FromHtml(Settings.historyColorsBottom[Settings.historyColor]));
            Thread.Sleep(interval / 2);
            UpdateElement(ColorTranslator.FromHtml(Settings.historyColorsTop[Settings.historyColor]));
            Thread.Sleep(interval / 2);
        }


        // ###########################################################################################
    }
}
