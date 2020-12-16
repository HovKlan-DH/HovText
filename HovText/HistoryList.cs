using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HovText
{
    public partial class HistoryList : Form
    {

        int firstBox = 0;
        int entryIndexLast = 0;


        // ###########################################################################################
        // Main
        // ###########################################################################################

        public HistoryList()
        {
            InitializeComponent();
        }


        // ###########################################################################################
        // Show the history list (triggered on each history hotkey)
        // ###########################################################################################

        public void UpdateHistoryList(string direction)
        {
            SetHistoryPosition();

            int boxElements = Settings.historyListElements;
            int entries = Settings.entriesText.Count(); 
            int entryIndex = Settings.entryCounter; // 1 is the lowest value as a human readable number
            firstBox = firstBox == 0 ? entries : firstBox;
            int scopeHighest = firstBox; // "boxElements" is the lowest value as a human readable number (newest copy on top)
            int scopeLowest = scopeHighest - boxElements + 1; // 1 is the lowest value as a human readable number (oldest copy at bottom)

            if(direction == "down")
            {
                firstBox = entryIndex >= scopeLowest ? firstBox : firstBox - 1;
            }
            if (direction == "up")
            {
                firstBox = entryIndex <= scopeHighest ? firstBox : firstBox + 1;
            }

            if (entryIndex == entryIndexLast && (entryIndex == entries || entryIndex == 1))
            {
                Flash();
            }
            entryIndexLast = entryIndex;

            // Set this form width and element heights
            int boxHeight;
            int width;
            switch (Settings.historySize)
            {
                case "Small":
                    boxHeight = Settings.historyBoxHeightSmall[Settings.historyListElements - 1];
                    width = Settings.historyListWidthSmall;
                    break;
                case "Large":
                    boxHeight = Settings.historyBoxHeightLarge[Settings.historyListElements - 1];
                    width = Settings.historyListWidthLarge;
                    break;
                default: // Medium
                    boxHeight = Settings.historyBoxHeightMedium[Settings.historyListElements - 1];
                    width = Settings.historyListWidthMedium;
                    break;
            }

            // Hide if not all boxes in the list is populated with data
            if (boxElements > entries)
            {
                for (int i = boxElements; i <= entries; i++)
                {
                    HideBox(i);
                }
            }

            // Not sure why this is needed but will make the border visible
            int widthSubstract = 10;

            // Get application name of highlighted entry
            string entryApplication = Settings.entriesApplication.ElementAt(entryIndex - 1).Value;
            textBoxHeadline.Text = entryIndex + " of " + entries + " from \"" + entryApplication + "\"";
            textBoxHeadline.BackColor = ColorTranslator.FromHtml(Settings.historyColorTop);
            textBoxHeadline.Font = new Font(Settings.historyFontFamily, Settings.historyFontSize);
            textBoxHeadline.Width = width - widthSubstract;
            textBoxHeadline.Location = new Point(0, 0);

            // Walkthrough all "boxElements" boxes
            for (int i = 1; i <= boxElements; i++)
            {
                // Do not put more boxes than the max amount of entries
                if (i > entries)
                {
                    break;
                }

                string entryText = Settings.entriesText.ElementAt(firstBox - i).Value;
                Image entryImage = Settings.entriesImage.ElementAt(firstBox - i).Value;

                bool isEntryText = entryText == "" ? false : true;
                bool isEntryImage = entryImage == null ? false : true;

                switch (i)
                {

                    case 1:
                        textBox1.Width = width - widthSubstract;
                        pictureBox1.Width = width - widthSubstract;
                        textBox1.Height = boxHeight;
                        pictureBox1.Height = boxHeight;
                        textBox1.Location = new Point(0, textBoxHeadline.Height - 1);
                        pictureBox1.Location = new Point(0, textBoxHeadline.Height - 1);
                        textBox1.Font = new Font(Settings.historyFontFamily, Settings.historyFontSize);
                        textBox1.Text = entryText;
                        pictureBox1.Image = entryImage;
                        textBox1.BackColor = ColorTranslator.FromHtml(Settings.historyColorBottom);
                        pictureBox1.BackColor = ColorTranslator.FromHtml(Settings.historyColorBottom);
                        if (entryIndex == (firstBox - i + 1))
                        {
                            // Do not show the "active element" color, if only showing one element
                            if(Settings.historyListElements == 1)
                            {
                                textBox1.BackColor = ColorTranslator.FromHtml(Settings.historyColorBottom);
                                pictureBox1.BackColor = ColorTranslator.FromHtml(Settings.historyColorBottom);
                            } else
                            {
                                textBox1.BackColor = ColorTranslator.FromHtml(Settings.historyColorTop);
                                pictureBox1.BackColor = ColorTranslator.FromHtml(Settings.historyColorTop);
                            }
                        }
                        if (isEntryText)
                        {
                            textBox1.Visible = true;
                            pictureBox1.Visible = false;
                        }
                        else
                        {
                            textBox1.Visible = false; 
                            pictureBox1.Visible = true;
                        }
                        break;

                    case 2:
                        textBox2.Width = width - widthSubstract;
                        pictureBox2.Width = width - widthSubstract;
                        textBox2.Height = boxHeight;
                        pictureBox2.Height = boxHeight;
                        textBox2.Location = new Point(0, textBoxHeadline.Height + (1 * boxHeight) - 2);
                        pictureBox2.Location = new Point(0, textBoxHeadline.Height + (1 * boxHeight) - 2);
                        textBox2.Font = new Font(Settings.historyFontFamily, Settings.historyFontSize);
                        textBox2.Text = entryText;
                        pictureBox2.Image = entryImage;
                        textBox2.BackColor = ColorTranslator.FromHtml(Settings.historyColorBottom);
                        pictureBox2.BackColor = ColorTranslator.FromHtml(Settings.historyColorBottom);
                        if (entryIndex == (firstBox - i + 1))
                        {
                            textBox2.BackColor = ColorTranslator.FromHtml(Settings.historyColorTop);
                            pictureBox2.BackColor = ColorTranslator.FromHtml(Settings.historyColorTop);
                        }
                        if (isEntryText)
                        {
                            textBox2.Visible = true;
                            pictureBox2.Visible = false;
                        }
                        else
                        {
                            textBox2.Visible = false;
                            pictureBox2.Visible = true;
                        }
                        break;

                    case 3:
                        textBox3.Width = width - widthSubstract;
                        pictureBox3.Width = width - widthSubstract;
                        textBox3.Height = boxHeight;
                        pictureBox3.Height = boxHeight;
                        textBox3.Location = new Point(0, textBoxHeadline.Height + (2 * boxHeight) - 3);
                        pictureBox3.Location = new Point(0, textBoxHeadline.Height + (2 * boxHeight) - 3);
                        textBox3.Font = new Font(Settings.historyFontFamily, Settings.historyFontSize);
                        textBox3.Text = entryText;
                        pictureBox3.Image = entryImage;
                        textBox3.BackColor = ColorTranslator.FromHtml(Settings.historyColorBottom);
                        pictureBox3.BackColor = ColorTranslator.FromHtml(Settings.historyColorBottom);
                        if (entryIndex == (firstBox - i + 1))
                        {
                            textBox3.BackColor = ColorTranslator.FromHtml(Settings.historyColorTop);
                            pictureBox3.BackColor = ColorTranslator.FromHtml(Settings.historyColorTop);
                        }
                        if (isEntryText)
                        {
                            textBox3.Visible = true;
                            pictureBox3.Visible = false;
                        }
                        else
                        {
                            textBox3.Visible = false; 
                            pictureBox3.Visible = true;
                        }
                        break;

                    case 4:
                        textBox4.Width = width - widthSubstract;
                        pictureBox4.Width = width - widthSubstract;
                        textBox4.Height = boxHeight;
                        pictureBox4.Height = boxHeight;
                        textBox4.Location = new Point(0, textBoxHeadline.Height + (3 * boxHeight) - 4);
                        pictureBox4.Location = new Point(0, textBoxHeadline.Height + (3 * boxHeight) - 4);
                        textBox4.Font = new Font(Settings.historyFontFamily, Settings.historyFontSize);
                        textBox4.Text = entryText;
                        pictureBox4.Image = entryImage;
                        textBox4.BackColor = ColorTranslator.FromHtml(Settings.historyColorBottom);
                        pictureBox4.BackColor = ColorTranslator.FromHtml(Settings.historyColorBottom);
                        if (entryIndex == (firstBox - i + 1))
                        {
                            textBox4.BackColor = ColorTranslator.FromHtml(Settings.historyColorTop);
                            pictureBox4.BackColor = ColorTranslator.FromHtml(Settings.historyColorTop);
                        }
                        if (isEntryText)
                        {
                            textBox4.Visible = true;
                            pictureBox4.Visible = false;
                        }
                        else
                        {
                            textBox4.Visible = false; 
                            pictureBox4.Visible = true;
                        }
                        break;

                    case 5:
                        textBox5.Width = width - widthSubstract;
                        pictureBox5.Width = width - widthSubstract;
                        textBox5.Height = boxHeight;
                        pictureBox5.Height = boxHeight;
                        textBox5.Location = new Point(0, textBoxHeadline.Height + (4 * boxHeight) - 5);
                        pictureBox5.Location = new Point(0, textBoxHeadline.Height + (4 * boxHeight) - 5);
                        textBox5.Font = new Font(Settings.historyFontFamily, Settings.historyFontSize);
                        textBox5.Text = entryText;
                        pictureBox5.Image = entryImage;
                        textBox5.BackColor = ColorTranslator.FromHtml(Settings.historyColorBottom);
                        pictureBox5.BackColor = ColorTranslator.FromHtml(Settings.historyColorBottom);
                        if (entryIndex == (firstBox - i + 1))
                        {
                            textBox5.BackColor = ColorTranslator.FromHtml(Settings.historyColorTop);
                            pictureBox5.BackColor = ColorTranslator.FromHtml(Settings.historyColorTop);
                        }
                        if (isEntryText)
                        {
                            textBox5.Visible = true; 
                            pictureBox5.Visible = false;
                        }
                        else
                        {
                            textBox5.Visible = false; 
                            pictureBox5.Visible = true;
                        }
                        break;

                    case 6:
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        textBox6.Width = width - widthSubstract;
                        pictureBox6.Width = width - widthSubstract;
                        textBox6.Height = boxHeight;
                        pictureBox6.Height = boxHeight;
                        textBox6.Location = new Point(0, textBoxHeadline.Height + (5 * boxHeight) - 6);
                        pictureBox6.Location = new Point(0, textBoxHeadline.Height + (5 * boxHeight) - 6);
                        textBox6.Font = new Font(Settings.historyFontFamily, Settings.historyFontSize);
                        textBox6.Text = entryText;
                        pictureBox6.Image = entryImage;
                        textBox6.BackColor = ColorTranslator.FromHtml(Settings.historyColorBottom);
                        pictureBox6.BackColor = ColorTranslator.FromHtml(Settings.historyColorBottom);
                        if (entryIndex == (firstBox - i + 1))
                        {
                            textBox6.BackColor = ColorTranslator.FromHtml(Settings.historyColorTop);
                            pictureBox6.BackColor = ColorTranslator.FromHtml(Settings.historyColorTop);
                        }
                        if (isEntryText)
                        {
                            textBox6.Visible = true;
                            pictureBox6.Visible = false;
                        }
                        else
                        {
                            textBox6.Visible = false; 
                            pictureBox6.Visible = true;
                        }
                        break;

                    case 7:
                        textBox7.Width = width - widthSubstract;
                        pictureBox7.Width = width - widthSubstract;
                        textBox7.Height = boxHeight;
                        pictureBox7.Height = boxHeight;
                        textBox7.Location = new Point(0, textBoxHeadline.Height + (6 * boxHeight) - 7);
                        pictureBox7.Location = new Point(0, textBoxHeadline.Height + (6 * boxHeight) - 7);
                        textBox7.Font = new Font(Settings.historyFontFamily, Settings.historyFontSize);
                        textBox7.Text = entryText;
                        pictureBox7.Image = entryImage;
                        textBox7.BackColor = ColorTranslator.FromHtml(Settings.historyColorBottom);
                        pictureBox7.BackColor = ColorTranslator.FromHtml(Settings.historyColorBottom);
                        if (entryIndex == (firstBox - i + 1))
                        {
                            textBox7.BackColor = ColorTranslator.FromHtml(Settings.historyColorTop);
                            pictureBox7.BackColor = ColorTranslator.FromHtml(Settings.historyColorTop);
                        }
                        if (isEntryText)
                        {
                            textBox7.Visible = true;
                            pictureBox7.Visible = false;
                        }
                        else
                        {
                            textBox7.Visible = false; 
                            pictureBox7.Visible = true;
                        }
                        break;

                    case 8:
                        textBox8.Width = width - widthSubstract;
                        pictureBox8.Width = width - widthSubstract;
                        textBox8.Height = boxHeight;
                        pictureBox8.Height = boxHeight;
                        textBox8.Location = new Point(0, textBoxHeadline.Height + (7 * boxHeight) - 8);
                        pictureBox8.Location = new Point(0, textBoxHeadline.Height + (7 * boxHeight) - 8);
                        textBox8.Font = new Font(Settings.historyFontFamily, Settings.historyFontSize);
                        textBox8.Text = entryText;
                        pictureBox8.Image = entryImage;
                        textBox8.BackColor = ColorTranslator.FromHtml(Settings.historyColorBottom);
                        pictureBox8.BackColor = ColorTranslator.FromHtml(Settings.historyColorBottom);
                        if (entryIndex == (firstBox - i + 1))
                        {
                            textBox8.BackColor = ColorTranslator.FromHtml(Settings.historyColorTop);
                            pictureBox8.BackColor = ColorTranslator.FromHtml(Settings.historyColorTop);
                        }
                        if (isEntryText)
                        {
                            textBox8.Visible = true;
                            pictureBox8.Visible = false;
                        }
                        else
                        {
                            textBox8.Visible = false; 
                            pictureBox8.Visible = true;
                        }
                        break;

                    case 9:
                        textBox9.Width = width - widthSubstract;
                        pictureBox9.Width = width - widthSubstract;
                        textBox9.Height = boxHeight;
                        pictureBox9.Height = boxHeight;
                        textBox9.Location = new Point(0, textBoxHeadline.Height + (8 * boxHeight) - 9);
                        pictureBox9.Location = new Point(0, textBoxHeadline.Height + (8 * boxHeight) - 9);
                        textBox9.Font = new Font(Settings.historyFontFamily, Settings.historyFontSize);
                        textBox9.Text = entryText;
                        pictureBox9.Image = entryImage;
                        textBox9.BackColor = ColorTranslator.FromHtml(Settings.historyColorBottom);
                        pictureBox9.BackColor = ColorTranslator.FromHtml(Settings.historyColorBottom);
                        if (entryIndex == (firstBox - i + 1))
                        {
                            textBox9.BackColor = ColorTranslator.FromHtml(Settings.historyColorTop);
                            pictureBox9.BackColor = ColorTranslator.FromHtml(Settings.historyColorTop);
                        }
                        if (isEntryText)
                        {
                            textBox9.Visible = true;
                            pictureBox9.Visible = false;
                        }
                        else
                        {
                            textBox9.Visible = false; 
                            pictureBox9.Visible = true;
                        }
                        break;

                    case 10:
                        textBox10.Width = width - widthSubstract;
                        pictureBox10.Width = width - widthSubstract;
                        textBox10.Height = boxHeight;
                        pictureBox10.Height = boxHeight;
                        textBox10.Location = new Point(0, textBoxHeadline.Height + (9 * boxHeight) - 10);
                        pictureBox10.Location = new Point(0, textBoxHeadline.Height + (9 * boxHeight) - 10);
                        textBox10.Font = new Font(Settings.historyFontFamily, Settings.historyFontSize);
                        textBox10.Text = entryText;
                        pictureBox10.Image = entryImage;
                        textBox10.BackColor = ColorTranslator.FromHtml(Settings.historyColorBottom);
                        pictureBox10.BackColor = ColorTranslator.FromHtml(Settings.historyColorBottom);
                        if (entryIndex == (firstBox - i + 1))
                        {
                            textBox10.BackColor = ColorTranslator.FromHtml(Settings.historyColorTop);
                            pictureBox10.BackColor = ColorTranslator.FromHtml(Settings.historyColorTop);
                        }
                        if (isEntryText)
                        {
                            textBox10.Visible = true;
                            pictureBox10.Visible = false;
                        }
                        else
                        {
                            textBox10.Visible = false;
                            pictureBox10.Visible = true;
                        }
                        break;

                }

            }
            TopMost = true; // make sure this form is the top most form to catch the key-up
            Show();
        }


        // ###########################################################################################
        // Set the position for the history area
        // ###########################################################################################

        private void SetHistoryPosition()
        {
            // Get the history form width and the element height
            int boxHeight;
            int width;
            switch (Settings.historySize)
            {
                case "Small":
                    boxHeight = Settings.historyBoxHeightSmall[Settings.historyListElements - 1];
                    width = Settings.historyListWidthSmall;
                    break;
                case "Large":
                    boxHeight = Settings.historyBoxHeightLarge[Settings.historyListElements - 1];
                    width = Settings.historyListWidthLarge;
                    break;
                default: // Medium
                    boxHeight = Settings.historyBoxHeightMedium[Settings.historyListElements - 1];
                    width = Settings.historyListWidthMedium;
                    break;
            }

            // Determine if we should show max boxes or max entries
            int elements = Settings.historyListElements > Settings.entriesText.Count() ? Settings.entriesText.Count() : Settings.historyListElements;

            // Set the history form width and height
            Width = width;
            Height = (elements * boxHeight) + textBoxHeadline.Height + 3;

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
        // Catch keyboard input to react on modifier KEY UP
        // ###########################################################################################

        private void HistoryList_KeyUp(object sender, KeyEventArgs e)
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
                Console.WriteLine("Selected history entry");
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
            if (textBoxHeadline.InvokeRequired)
            {
                this.Invoke(new UpdateElementDelegate(UpdateElement), new object[] { color });
            }
            textBoxHeadline.BackColor = color;
        }

        private void FlashInternal()
        {
            int interval = 100;
            UpdateElement(ColorTranslator.FromHtml(Settings.historyColorBottom));
            Thread.Sleep(interval / 2);
            UpdateElement(ColorTranslator.FromHtml(Settings.historyColorTop));
            Thread.Sleep(interval / 2);
        }


        // ###########################################################################################
        // Hide a box (text and image)
        // ###########################################################################################

        public void HideBox(int box)
        {
            switch (box)
            {
                case 1:
                    textBox1.Visible = false;
                    pictureBox1.Visible = false;
                    break;
                case 2:
                    textBox2.Visible = false;
                    pictureBox2.Visible = false;
                    break;
                case 3:
                    textBox3.Visible = false;
                    pictureBox3.Visible = false;
                    break;
                case 4:
                    textBox4.Visible = false;
                    pictureBox4.Visible = false;
                    break;
                case 5:
                    textBox5.Visible = false;
                    pictureBox5.Visible = false;
                    break;
                case 6:
                    textBox6.Visible = false;
                    pictureBox6.Visible = false;
                    break;
                case 7:
                    textBox7.Visible = false;
                    pictureBox7.Visible = false;
                    break;
                case 8:
                    textBox8.Visible = false;
                    pictureBox8.Visible = false;
                    break;
                case 9:
                    textBox9.Visible = false;
                    pictureBox9.Visible = false;
                    break;
                case 10:
                    textBox10.Visible = false;
                    pictureBox10.Visible = false;
                    break;
            }
        }


        // ###########################################################################################
    }
}
