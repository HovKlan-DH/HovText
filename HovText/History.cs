/*
##################################################################################################
HISTORY
-------

This is the history area shown for the clipboard entries. It is shown when the user presses the
different hotkeys for the history area.

##################################################################################################
*/

using HovText.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static HovText.Program;


namespace HovText
{
    public partial class History : Form
    {
        // ###########################################################################################
        // Define "History" class variables - real spaghetti :-)
        // ###########################################################################################
        string changeBorderElement = ""; // the UI name for the element coming to the paint redraw event
        private static int entryNewestBox = -1; // 0-indexed array key for the first box element to show (this is the LAST element in the list, as this is the most recent one added)
        private static int entryNewest = -1; // 0-indexed array key for the first element in the full list
        private static int entryOldest = -1; // 0-indexed array key for the last element in the full list
        private static int entryActive = -1; // 0-indexed array key for the active entry
        private static int entryActiveLast = -1; // 0-indexed array key for the last active entry (only used to determine if it should "flash")
        private static int entryActiveList = -1; // human-readable list number for the active entry (1 => showElements)
        private static bool isEntryAtTop = false; // is the active entry at the top position (newest entry)
        private static bool isEntryAtBottom = false; // is the active entry at the bottom position (oldest entry)
        public static int entriesInList = 0; // will contain the total visible amount of entries in the clipboard list (does not need to be the same as the total amount of clipboard entries as the list could show favorites only or a filtered list)
        private static int entryInList = 0; // "This is number X of Y" (this stores the "X")
        private Timer _flashTimer;
        private Color _flashColor;
        private Dictionary<string, Control> controlCache = new Dictionary<string, Control>();


        // ###########################################################################################
        // Main
        // ###########################################################################################

        public History()
        {
            InitializeComponent();

            // Catch the mousewheel event
            MouseWheel += new MouseEventHandler(Form_MouseWheel);
        }


        // ###########################################################################################
        // Handle mouse scroll events
        // ###########################################################################################

        private void Form_MouseWheel(object sender, MouseEventArgs e)
        {
            if (Visible)
            {
                // Convert screen coordinates to client coordinates
                Point clientCursorPos = PointToClient(Control.MousePosition);

                // Check if the cursor is within the bounds of the form
                if (ClientRectangle.Contains(clientCursorPos))
                {
                    if (e.Delta > 0)
                    {
                        ScrollUpFunction();
                    }
                    else if (e.Delta < 0)
                    {
                        ScrollDownFunction();
                    }
                }
            }
        }

        private void ScrollUpFunction()
        {
            Logging.Log("Scrolling up with mouse");
            UpdateHistory("up");
        }

        private void ScrollDownFunction()
        {
            Logging.Log("Scrolling down with mouse");
            UpdateHistory("down");
        }


        // ###########################################################################################
        // Select entry with mouse
        // ###########################################################################################

        private void Mouse_ClickInForm(object sender, EventArgs e)
        {
            // Control can either be a Label or a PictureBox
            Control clickedControl = sender as Control;
            if (clickedControl != null)
            {
                int index = clickedControl.Tag != null ? (int)clickedControl.Tag : -1;
                entryActive = index;
                Logging.Log("Selected entry with mouse");
                SelectEntry();
            }
        }


        // ###########################################################################################
        // Setup the history form for all its UI elements
        // ###########################################################################################

        public void SetupForm(bool keepHeaders = false)
        {

            // "keepHeaders = true" equals that headline + search will be kept - but all other elements are deleted
            if (keepHeaders)
            {
                for (int i = Controls.Count - 1; i >= 0; i--)
                {
                    Control control = Controls[i];
                    // Remove any control element that contains "historyLabel" or "historyPictureBox"
                    if (control.Name.Contains("historyLabel") || control.Name.Contains("historyPictureBox"))
                    {
                        Controls.Remove(control);
                        control.Dispose();
                    }
                }
            }

            // Define some heights and widths
            int headlineHeight = 0; // this will be calculated based on font size
            int headlineHeightPadding = 15; // additional height to give some space
            int searchlineHeight = 0; // this will be calculated based on font size
            int searchlineHeightPadding = 6; // additional height to give some space
            int fontSizePixels = 0; // this will be calculated based on font size
            int favoriteImageWidth = 18; // this is the "Favorite" icon width - optimize this by automatically get the width

            // If height is 100%, then reduce it to 99% (corner case and I really should investigate)
            int historySizeHeight = Settings.historySizeHeight;
            if (historySizeHeight > 99)
            {
                historySizeHeight = 99;
            }

            // Setup form width and height
            int workingAreaWidth = Screen.AllScreens[Settings.activeDisplay].WorkingArea.Width;
            int workingAreaHeight = Screen.AllScreens[Settings.activeDisplay].WorkingArea.Height;
            int width = (workingAreaWidth * Settings.historySizeWidth) / 100;
            int height = (workingAreaHeight * historySizeHeight) / 100;

            Label label;
            Width = width; // width of the form

            // Temporarily add a label to calculate the font size in pixels
            label = new Label
            {
                Name = "temporaryLabel",
                Width = 100,
                Height = 10,
                Font = new Font(Settings.historyFontFamily, Settings.historyFontSize),
                Visible = false
            };
            Controls.Add(label);

            // Get the font size for the temporary label
            Size textSize = TextRenderer.MeasureText("Sample Text", label.Font);
            fontSizePixels = textSize.Height;
            headlineHeight = fontSizePixels + headlineHeightPadding;

            // Remove the temporary label again - no more use for it
            Control control1 = Controls["temporarylabel"];
            Controls.Remove(control1);
            control1.Dispose();

            // Add the headline, favorite image and search
            if (!keepHeaders)
            {
                // Add the headline
                label = new Label
                {
                    Name = "uiHistoryHeadline",
                    Width = width,
                    Height = headlineHeight,
                    Location = new Point(0, 0),
                    BorderStyle = BorderStyle.FixedSingle,
                    Padding = new Padding(5),
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font(Settings.historyFontFamily, Settings.historyFontSize),
                    BackColor = ColorTranslator.FromHtml(Settings.historyColorsHeader[Settings.historyColorTheme]),
                    ForeColor = ColorTranslator.FromHtml(Settings.historyColorsHeaderText[Settings.historyColorTheme]),
                    Visible = true
                };
                Controls.Add(label);
                
                // Add the panel for headline
                Panel panel = new Panel
                {
                    Name = "uiHistoryHeadlinePanel",
                    Width = headlineHeight - 10,
                    Height = headlineHeight - 10,
                    Location = new Point(width - headlineHeight - 10, 5),
                    BackColor = ColorTranslator.FromHtml(Settings.historyColorsHeader[Settings.historyColorTheme]),
                    Visible = true
                };
                Controls.Add(panel);
                panel.BringToFront();

                // Add the application icon
                PictureBox iconbox = new PictureBox
                {
                    Name = "uiHistoryHeadlinePanelIcon",
                    Width = headlineHeight - 10,
                    Height = headlineHeight - 10,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    BackColor = ColorTranslator.FromHtml(Settings.historyColorsHeader[Settings.historyColorTheme]),
                    Visible = true
                };
                panel.Controls.Add(iconbox);
                iconbox.BringToFront();

                // Add the favorite image to the headline (PictureBox)
                if (Settings.isEnabledFavorites)
                {
                    PictureBox pictureBoxFav = new PictureBox
                    {
                        Name = "uiHistoryHeadlineFav",
                        Width = favoriteImageWidth,
                        Height = favoriteImageWidth,
                        Location = new Point(width - favoriteImageWidth - 10, 1),
                        BorderStyle = BorderStyle.None,
                        Visible = false,
                        Image = Resources.Favorite
                    };
                    Controls.Add(pictureBoxFav);
                }

                // Add the Search input field
                if (!Settings.isHistoryHotkeyPressed)
                {
                    TextBox textBox = new TextBox
                    {
                        Name = "search",
                        Width = width - 11,
                        Height = searchlineHeight,
                        Location = new Point(1, headlineHeight), // X, Y
                        BorderStyle = BorderStyle.FixedSingle,
                        Font = new Font(Settings.historyFontFamily, Settings.historyFontSize),
                        BackColor = ColorTranslator.FromHtml(Settings.historyColorsSearch[Settings.historyColorTheme]),
                        ForeColor = ColorTranslator.FromHtml(Settings.historyColorsSearchText[Settings.historyColorTheme]),
                        AutoSize = false,
                        Visible = true,
                    };
                    Controls.Add(textBox);
                    textBox.BringToFront();

                    // Define events for the input field
                    textBox.TextChanged += Search_TextChanged;
                    textBox.KeyDown += Search_KeyDown;
                }
            }

            // Get and set the height for "search" - this is a dynamic height, based on font size
            if (Controls.ContainsKey("search"))
            {
                TextBox search = Controls["search"] as TextBox; // get the reference to the textbox element
                searchlineHeight = fontSizePixels + searchlineHeightPadding;
                search.Height = searchlineHeight;
            }

            // Set the next vertical position
            int nextPosY = 0;
            nextPosY += headlineHeight + searchlineHeight;

            // Get the total amount of entries, depending which view this is
            entriesInList = Settings.entriesShow.Count(kv => kv.Value == true);
            
            // "showElements" determines how many boxes to show
            int showElements = Settings.historyListElements;
            showElements = showElements > entriesInList ? entriesInList : showElements;

            if (showElements > 0)
            {
                // Set this form height and element heights
                int boxHeight = (height - headlineHeight - searchlineHeight) / showElements;
                Height = (boxHeight * showElements) + headlineHeight + searchlineHeight + SystemInformation.FrameBorderSize.Height + SystemInformation.Border3DSize.Height + 4;

                // Setup all visible element boxes
                for (int i = 1; i <= showElements; i++)
                {
                    // Text entry (Label)
                    label = new Label
                    {
                        Name = "historyLabel" + i,
                        Width = width,
                        Height = boxHeight,
                        Location = new Point(0, nextPosY),
                        BorderStyle = BorderStyle.FixedSingle,
                        Padding = new Padding(Settings.historyBorderThickness - 2),
                        Font = new Font(Settings.historyFontFamily, Settings.historyFontSize),
                        TextAlign = ContentAlignment.TopLeft,
                        Visible = false,
                        Text = ""
                    };
                    Controls.Add(label);
                    label.Click += Mouse_ClickInForm;

                    // Catch repaint event for this specific element (to draw the border)
                    Controls["historyLabel" + i].Paint += new System.Windows.Forms.PaintEventHandler(History_Paint);

                    // Image entry (PictureBox)
                    PictureBox pictureBox = new PictureBox
                    {
                        Name = "historyPictureBox" + i,
                        Width = width,
                        Height = boxHeight,
                        Location = new Point(0, nextPosY),
                        BorderStyle = BorderStyle.FixedSingle,
                        Padding = new Padding(Settings.historyBorderThickness - 2),
                        Visible = false,
                        Image = null,
                        SizeMode = PictureBoxSizeMode.Zoom
                    };
                    Controls.Add(pictureBox);
                    pictureBox.Click += Mouse_ClickInForm;

                    // Catch repaint event for this specific element (to draw the border)
                    Controls["historyPictureBox" + i].Paint += new System.Windows.Forms.PaintEventHandler(History_Paint);

                    // Favorite image (PictureBox)
                    if (Settings.isEnabledFavorites)
                    {
                        PictureBox pictureBoxFavEntry = new PictureBox
                        {
                            Name = "historyPictureBoxFav" + i,
                            Width = favoriteImageWidth,
                            Height = favoriteImageWidth,
                            Location = new Point(width - favoriteImageWidth - 11, nextPosY + 2),
                            BorderStyle = BorderStyle.None,
                            Visible = false,
                            Image = Resources.Favorite
                        };
                        Controls.Add(pictureBoxFavEntry);
                    }

                    nextPosY += boxHeight;
                }
            }
            else
            {
                Controls["uiHistoryHeadline"].Text = "0 entries found with this text";
            }

            // If we are in the "favorite" view then show the icon for it in the "headline"
            if (Settings.isEnabledFavorites)
            {
                foreach (Control c in Controls.Find("uiHistoryHeadlineFav", true))
                {
                    c.Visible = false;
                }
            }

            // After all controls have been added to the form
            controlCache.Clear(); // Clear previous entries if any
            foreach (Control control in Controls)
            {
                controlCache.Add(control.Name, control);
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
            // Remove all form elements and hide the form
            Controls.Clear();
            Hide();
        }


        // ###########################################################################################
        // Repaint the entry border
        // ###########################################################################################

        private void History_Paint(object sender, PaintEventArgs e)
        {
            if (Settings.historyBorderThickness > 0 && entriesInList > 1)
            {
                // Set padding
                int padding = 0;

                if (Settings.historyBorderThickness >= 2)
                {
                    ((Control)sender).Padding = new Padding(Settings.historyBorderThickness - 2 + padding);
                }
                else
                {
                    ((Control)sender).Padding = new Padding(Settings.historyBorderThickness - 1 + padding);
                }

                // Redraw border with a solid color, if the update source event is larger than 18px (favorite icon) and if we are placed on the active entry
                if (changeBorderElement == ((Control)sender).Name && e.ClipRectangle.Width > 18)
                {
                    ControlPaint.DrawBorder(e.Graphics, e.ClipRectangle,
                                  ColorTranslator.FromHtml(Settings.historyColorsBorder[Settings.historyColorTheme]), Settings.historyBorderThickness, ButtonBorderStyle.Solid,
                                  ColorTranslator.FromHtml(Settings.historyColorsBorder[Settings.historyColorTheme]), Settings.historyBorderThickness, ButtonBorderStyle.Solid,
                                  ColorTranslator.FromHtml(Settings.historyColorsBorder[Settings.historyColorTheme]), Settings.historyBorderThickness, ButtonBorderStyle.Solid,
                                  ColorTranslator.FromHtml(Settings.historyColorsBorder[Settings.historyColorTheme]), Settings.historyBorderThickness, ButtonBorderStyle.Solid);
                }
                else
                {
                    ControlPaint.DrawBorder(e.Graphics, e.ClipRectangle, BackColor, ButtonBorderStyle.None);
                }
            }
            else
            {
                ControlPaint.DrawBorder(e.Graphics, e.ClipRectangle, BackColor, ButtonBorderStyle.None);
            }
        }


        // ###########################################################################################
        // Show content of the history list
        // ###########################################################################################

        public void UpdateHistory(string direction)
        {
            // Set the "first" and "last" index
            entryNewestBox = entryNewestBox == -1 ? GetNewestIndex() : entryNewestBox;
            entryNewest = entryNewest == -1 ? GetNewestIndex() : entryNewest;
            entryOldest = GetOldestIndex();
            entryNewestBox = isEntryAtTop && direction == "up" ? GetNextIndex(direction, entryNewestBox) : entryNewestBox;
            entryNewestBox = isEntryAtBottom && direction == "down" ? GetNextIndex(direction, entryNewestBox) : entryNewestBox;

            // Set the active entry
            entryActive = entryActive == -1 ? entryNewestBox : GetNextIndex(direction, entryActive);

            // Get the amount of total entries in array - overwrite if we are showing the "Favorite" list
            entriesInList = Settings.entriesShow.Count(kv => kv.Value == true);
    
            bool isTransparent;
            Image entryImage;
            Image entryImageTransparent;

            // Proceed if we have more than one entry in the list (it also comes to here, if we are showing an empty favorite list)
            if (entriesInList > 0)
            {
                // "showElements" determines how many boxes to show
                int showElements = Settings.historyListElements;
                showElements = showElements > entriesInList ? entriesInList : showElements;
                int shownElements = 0;

                // Move the first box element if there is only one to show
                entryNewestBox = showElements == 1 ? entryActive : entryNewestBox;

                // Build the list of visible entry boxes
                for (int i = entryNewestBox; i >= 0 && shownElements < showElements; i--)
                {
                    Color favoriteBackgroundColor = Color.Red;

                    // Check if the array index exists
                    bool doesKeyExist = Settings.entriesText.ContainsKey(i);

                    // Check if the array index should be shown
                    bool shouldEntryBeShown = false;
                    if (Settings.entriesShow.ContainsKey(i))
                    {
                        shouldEntryBeShown = Settings.entriesShow[i];
                    }

                    if (doesKeyExist && shouldEntryBeShown)
                    {
                        // Get the array data
                        string entryText = Settings.entriesText[i];
                        entryImage = Settings.entriesImage[i];
                        entryImageTransparent = Settings.entriesImageTrans[i];
                        bool isFavorite = Settings.entriesIsFavorite[i];
                        isTransparent = Settings.entriesIsTransparent[i];

                        // Proceed if this is a valid entry, depending on the list view
                        shownElements++;

                        // Check if it is a TEXT
                        if (!string.IsNullOrEmpty(entryText))
                        {
                            // Find a form element with a specific name
                            Control historyLabel;
                            if (controlCache.TryGetValue("historyLabel" + shownElements, out historyLabel))
                            {
                                // Truncate the text, if longer than 64K - a "Label" can max contain 65535 characters
                                int bytes = entryText.Length;
                                if(bytes > 65535)
                                {
                                    entryText = entryText.Substring(0, 65535);
                                }

                                historyLabel.Text = entryText;
                                if (!historyLabel.Visible)
                                {
                                    historyLabel.Visible = true;
                                }                                        
                                historyLabel.Tag = i;

                                // Set the colors
                                if (i == entryActive && showElements > 1)
                                {
                                    historyLabel.BackColor = ColorTranslator.FromHtml(Settings.historyColorsActive[Settings.historyColorTheme]);
                                    historyLabel.ForeColor = ColorTranslator.FromHtml(Settings.historyColorsActiveText[Settings.historyColorTheme]);
                                    changeBorderElement = historyLabel.Name;

                                    isEntryAtTop = shownElements == 1;
                                    isEntryAtBottom = shownElements == showElements;
                                    entryActiveList = shownElements;
                                }
                                else
                                {
                                    historyLabel.BackColor = ColorTranslator.FromHtml(Settings.historyColorsEntry[Settings.historyColorTheme]);
                                    historyLabel.ForeColor = ColorTranslator.FromHtml(Settings.historyColorsEntryText[Settings.historyColorTheme]);

                                    // Mark this as the active element, if there is only shown one entry
                                    if (i == entryActive && showElements == 1)
                                    {
                                        entryActiveList = shownElements;
                                    }
                                }
                                favoriteBackgroundColor = historyLabel.BackColor;
                            }
                        }
                        else
                        {
                            // Hide the label as it then must be an image
                            Controls["historyLabel" + shownElements].Visible = false;
                        }

                        // Check if it is an IMAGE
                        if (entryImage != null)
                        {
                            // Find a form element with a specific name
                            Control historyPictureBox;
                            if (controlCache.TryGetValue("historyPictureBox" + shownElements, out historyPictureBox))
                            {
                                // Check if the image is transparent - if so make the image background transparent
                                if (isTransparent)
                                {
                                    ((PictureBox)historyPictureBox).Image = entryImageTransparent;
                                }
                                else
                                {
                                    ((PictureBox)historyPictureBox).Image = entryImage;
                                }

                                historyPictureBox.Visible = true;
                                historyPictureBox.Tag = i;

                                // Set the colors
                                if (i == entryActive && showElements > 1)
                                {
                                    historyPictureBox.BackColor = ColorTranslator.FromHtml(Settings.historyColorsActive[Settings.historyColorTheme]);
                                    historyPictureBox.ForeColor = ColorTranslator.FromHtml(Settings.historyColorsActiveText[Settings.historyColorTheme]);
                                    changeBorderElement = historyPictureBox.Name;

                                    isEntryAtTop = shownElements == 1;
                                    isEntryAtBottom = shownElements == showElements;
                                    entryActiveList = shownElements;
                                }
                                else
                                {
                                    historyPictureBox.BackColor = ColorTranslator.FromHtml(Settings.historyColorsEntry[Settings.historyColorTheme]);
                                    historyPictureBox.ForeColor = ColorTranslator.FromHtml(Settings.historyColorsEntryText[Settings.historyColorTheme]);

                                    // Mark this as the active element, if there is only shown one entry
                                    if (i == entryActive && showElements == 1)
                                    {
                                        entryActiveList = shownElements;
                                    }
                                }
                                favoriteBackgroundColor = historyPictureBox.BackColor;
                            }
                        }
                        else
                        {
                            // Hide the pictureBox as it then must be a text
                            Controls["historyPictureBox" + shownElements].Visible = false;
                        }

                        // Find the "favorite" picture box
                        if (Settings.isEnabledFavorites)
                        {
                            Control historyPictureBoxFav;
                            // Find a form element with a specific name
                            if (controlCache.TryGetValue("historyPictureBoxFav" + shownElements, out historyPictureBoxFav))
                            {
                                if (isFavorite)
                                {
                                    historyPictureBoxFav.BackColor = favoriteBackgroundColor;
                                    historyPictureBoxFav.BringToFront();
                                    historyPictureBoxFav.Visible = true;
                                }
                                else
                                {
                                    historyPictureBoxFav.Visible = false;
                                }
                            }
                        }
                    }
                }

                // Set the "this is entry X of Y" number
                entryInList += direction == "down" && entryActive != entryOldest ? 1 : 0;
                entryInList -= direction == "up" && entryActive != entryNewest ? 1 : 0;
                entryInList = entryActive == entryOldest ? entriesInList : entryInList;
                entryInList = entryActive == entryNewest ? 1 : entryInList;

                Control historyPictureBoxFavA;
                if (controlCache.TryGetValue("uiHistoryHeadlinePanel", out historyPictureBoxFavA))
                {
                    // Find the control within the container
                    Control historyPictureBoxFavB = historyPictureBoxFavA.Controls.Find("uiHistoryHeadlinePanelIcon", true).FirstOrDefault();

                    if (historyPictureBoxFavB != null)
                    {
                        // Cast the Control to Guna2PictureBox before accessing the Image property
                        PictureBox pictureBox = historyPictureBoxFavB as PictureBox;
                        if (pictureBox != null)
                        {
                            if (Settings.entriesApplicationIcon[entryActive] != null)
                            {
                                pictureBox.Image = Settings.entriesApplicationIcon[entryActive];
                                Controls["uiHistoryHeadlinePanel"].Visible = true;
                            }
                            else
                            {
                                Controls["uiHistoryHeadlinePanel"].Visible = false;
                            }
                        }
                    }
                }

                // Set the headline
                string entryApplication = Settings.entriesApplication[entryActive];
                Controls["uiHistoryHeadline"].Text = entryInList + " of " + entriesInList + " from \"" + entryApplication + "\"";
                if (Settings.entriesIsImage[entryActive])
                {
                    isTransparent = Settings.entriesIsTransparent[entryActive];
                    if (isTransparent)
                    {
                        Controls["uiHistoryHeadline"].Text += " (transparent image)";
                    }
                    else
                    {
                        Controls["uiHistoryHeadline"].Text += " (image)";
                    }
                }

                // Flash the headline if we are all the way to the top or bottom
                if (entryActive == entryActiveLast)
                {
                    Flash();
                }
                entryActiveLast = entryActive;
            }

            // Make sure that we will catch the key-up event
            TopMost = true;
        }


        // ###########################################################################################
        // Get the first (newest) array index in the full array, depending on the view
        // ###########################################################################################

        private static int GetNewestIndex()
        {
            int returnValue = Settings.entriesOrder.Keys.LastOrDefault();
            return returnValue;
        }


        // ###########################################################################################
        // Get the last (oldest) array index in the full array, depending on the view
        // ###########################################################################################

        private static int GetOldestIndex()
        {
            int returnValue = Settings.entriesOrder.Keys.FirstOrDefault();
            return returnValue;
        }


        // ###########################################################################################
        // Get the next entry array index, depending on the view and the direction, up/down
        // ###########################################################################################

        public static int GetNextIndex(string direction, int entryKey)
        {
            if (direction == "up")
            {
                for (int i = entryKey + 1; i <= Settings.entriesText.ElementAt(Settings.entriesText.Count - 1).Key; i++)
                {
                    if (Settings.entriesText.ContainsKey(i) && Settings.entriesShow[i])
                    {
                        return i;
                    }
                }
            }

            if (direction == "down")
            {
                // Proceed if the active element is not the last element
                if (entryActive != entryOldest)
                {
                    for (int i = entryKey - 1; i >= Settings.entriesText.ElementAt(0).Key; i--)
                    {
                        if (Settings.entriesText.ContainsKey(i) && Settings.entriesShow[i])
                        {
                            return i;
                        }
                    }
                }
            }

            return entryKey;
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

            // Proceed if no modifier keys are pressed down - this equals that we have selected the entry
            if (!isShift && !isAlt && !isControl)
            {
                SelectEntry();
            }
            else
            {
                // Check for other key combinations

                // For conversion from text to keys
                KeysConverter cvt = new KeysConverter();
                Keys key;

                // Proceed if we should toggle a favorite entry
                string hotkeyToggleFavorite = Settings.GetRegistryKey(Settings.registryPath, "HotkeyToggleFavorite");
                if (Settings.isEnabledFavorites && hotkeyToggleFavorite != "Not set")
                {
                    key = (Keys)cvt.ConvertFrom(hotkeyToggleFavorite);
                    if (e.KeyCode == key)
                    {
                        MarkFavorite();
                    }
                }
            }
        }


        // ###########################################################################################
        // Do some reset stuff
        // ###########################################################################################

        private static void ResetVariables()
        {
            entryNewestBox = -1;
            entryNewest = -1;
            entryOldest = -1;
            entryActive = -1;
            entryActiveList = -1;
            isEntryAtTop = false;
            isEntryAtBottom = false;
            entryInList = 0;
        }


        // ###########################################################################################
        // Set the position for the history area
        // Contributor: FNI
        // ###########################################################################################

        private void SetHistoryPosition()
        {
            // Use local history margin
            int historyMargin = Settings.historyMargin;
            if (!Settings.isHistoryMarginEnabled)
            {
                historyMargin = 0;
            }

            // Set history location
            int x;
            int y;
            switch (Settings.historyLocation)
            {
                case "Left Top":
                    x = Screen.AllScreens[Settings.activeDisplay].WorkingArea.Left;
                    y = Screen.AllScreens[Settings.activeDisplay].WorkingArea.Top;
                    Left = x + historyMargin;
                    Top = y + historyMargin;
                    break;
                case "Left Bottom":
                    x = Screen.AllScreens[Settings.activeDisplay].WorkingArea.Left;
                    y = Screen.AllScreens[Settings.activeDisplay].WorkingArea.Bottom - Height;
                    Left = x + historyMargin;
                    Top = y - historyMargin;
                    break;
                case "Center":
                    x = Screen.AllScreens[Settings.activeDisplay].WorkingArea.Left + ((Screen.AllScreens[Settings.activeDisplay].WorkingArea.Width - Width) / 2);
                    y = Screen.AllScreens[Settings.activeDisplay].WorkingArea.Top + ((Screen.AllScreens[Settings.activeDisplay].WorkingArea.Height - Height) / 2);
                    Left = x;
                    Top = y;
                    break;
                case "Right Top":
                    x = Screen.AllScreens[Settings.activeDisplay].WorkingArea.Right - Width;
                    y = Screen.AllScreens[Settings.activeDisplay].WorkingArea.Top;
                    Left = x - historyMargin;
                    Top = y + historyMargin;
                    break;
                default: // Right Bottom
                    x = Screen.AllScreens[Settings.activeDisplay].WorkingArea.Right - Width;
                    y = Screen.AllScreens[Settings.activeDisplay].WorkingArea.Bottom - Height;
                    Left = x - historyMargin;
                    Top = y - historyMargin;
                    break;
            }
        }

        
        // ###########################################################################################
        // Blink the top line in the history when it reaches oldest or newest element
        // https://stackoverflow.com/a/4147406/2028935
        // ###########################################################################################

        private void flashTimer_Tick(object sender, System.EventArgs e)
        {
            if (Controls.ContainsKey("uiHistoryHeadline"))
            {
                if (Controls["uiHistoryHeadline"].BackColor == _flashColor)
                {
                    Controls["uiHistoryHeadline"].BackColor = ColorTranslator.FromHtml(Settings.historyColorsHeader[Settings.historyColorTheme]);
                    _flashTimer.Stop(); // stop the Timer after the color has been toggled
                }
                else
                {
                    Controls["uiHistoryHeadline"].BackColor = _flashColor;
                }
            }
        }

        private void Flash()
        {
            _flashColor = ColorTranslator.FromHtml(Settings.historyColorsEntry[Settings.historyColorTheme]);
            if (_flashTimer == null)
            {
                _flashTimer = new System.Windows.Forms.Timer();
                _flashTimer.Interval = 50;
                _flashTimer.Tick += flashTimer_Tick;
            }
            else if (_flashTimer.Enabled) // check if the Timer is already running
            {
                return; // if so, exit the method without starting the Timer again
            }
            _flashTimer.Start();
        }


        // ###########################################################################################
        // Select the active entry in the list
        // ###########################################################################################

        private void SelectEntry()
        {
            // Insert log depending if list is empty or not
            if (entriesInList == 0)
            {
                Logging.Log("Selected history entry element [none, as list is empty]");
            }
            else
            {
                Logging.Log("Selected history entry list element [" + entryActiveList + "] of [" + entriesInList + "] with key [" + entryActive + "]");
            }

            // Reset some stuff
            Settings.entryIndex = entryActive; // set the new "entryIndex" variable as it is now selected
            ResetVariables();

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

            ResetForm();
        }


        // ###########################################################################################
        // Handle input to the search box
        // ###########################################################################################
        // Search filter.
        // Filter keywords:
        //   :e[mail]
        //   :f[avorite] 
        //   :i[mage]
        //   :m[ail]
        //   :p[icture]
        //   :u[rl]
        //   :t[ransparent]
        //   :w[eb]
        // ###########################################################################################

        private void Search_TextChanged(object sender, System.EventArgs e)
        {
            TextBox textBox = (TextBox)sender; // typecast the sender as a "TextBox"
            string[] searchTexts = textBox.Text.Split(' '); // split the text by space

            // Check if the input contains a word starting with ":u" or ":e"
            string favoriteKeyword = searchTexts.FirstOrDefault(text => text.StartsWith(":f"));
            string urlKeyword1 = searchTexts.FirstOrDefault(text => text.StartsWith(":u"));
            string urlKeyword2 = searchTexts.FirstOrDefault(text => text.StartsWith(":w"));
            string emailKeyword1 = searchTexts.FirstOrDefault(text => text.StartsWith(":e"));
            string emailKeyword2 = searchTexts.FirstOrDefault(text => text.StartsWith(":m"));
            string imageKeyword1 = searchTexts.FirstOrDefault(text => text.StartsWith(":i"));
            string imageKeyword2 = searchTexts.FirstOrDefault(text => text.StartsWith(":p"));
            string transparentKeyword = searchTexts.FirstOrDefault(text => text.StartsWith(":t"));

            bool searchForFavorite = favoriteKeyword != null;
            bool searchForUrl = urlKeyword1 != null || urlKeyword2 != null;
            bool searchForEmail = emailKeyword1 != null || emailKeyword2 != null;
            bool searchForImage = imageKeyword1 != null || imageKeyword2 != null;
            bool searchForTransparent = transparentKeyword != null;

            // If any of the above shorts are used, then remove it from the "searchTexts" array
            if (searchForFavorite)
            {
                searchTexts = searchTexts.Where(text => text != favoriteKeyword).ToArray();
            }
            if (searchForUrl)
            {
                searchTexts = searchTexts.Where(text => text != urlKeyword1 && text != urlKeyword2).ToArray();
            }
            if (searchForEmail)
            {
                searchTexts = searchTexts.Where(text => text != emailKeyword1 && text != emailKeyword2).ToArray();
            }
            if (searchForImage)
            {
                searchTexts = searchTexts.Where(text => text != imageKeyword1 && text != imageKeyword2).ToArray();
            }
            if (searchForTransparent)
            {
                searchTexts = searchTexts.Where(text => text != transparentKeyword).ToArray();
            }

            // Remove terms that start with ":" (except for the special ones)
            searchTexts = searchTexts.Where(text => !text.StartsWith(":") || text == favoriteKeyword || text == urlKeyword1 || text == urlKeyword2 || text == emailKeyword1 || text == emailKeyword2 || text == imageKeyword1 || text == imageKeyword2 || text == transparentKeyword).ToArray();

            // Perform a case-insensitive wildcard search using LINQ
            var searchResults = Settings.entriesText
                .Where(entry =>
                    searchTexts.All(searchText =>
                        entry.Value.Split(' ')
                        .Any(word => word.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)))
                .ToList();

            // If we're searching for favorites, URLs, Emails or Images, filter the search results to include only those
            if (searchForFavorite)
            {
                searchResults = searchResults
                    .Where(entry => Settings.entriesIsFavorite.ContainsKey(entry.Key) && Settings.entriesIsFavorite[entry.Key] == true)
                    .ToList();
            }
            if (searchForUrl)
            {
                searchResults = searchResults
                    .Where(entry => Settings.entriesIsUrl.ContainsKey(entry.Key) && Settings.entriesIsUrl[entry.Key] == true)
                    .ToList();
            }
            if (searchForEmail)
            {
                searchResults = searchResults
                    .Where(entry => Settings.entriesIsEmail.ContainsKey(entry.Key) && Settings.entriesIsEmail[entry.Key] == true)
                    .ToList();
            }
            if (searchForImage)
            {
                searchResults = searchResults
                    .Where(entry => Settings.entriesIsImage.ContainsKey(entry.Key) && Settings.entriesIsImage[entry.Key] == true)
                    .ToList();
            }
            if (searchForTransparent)
            {
                searchResults = searchResults
                    .Where(entry => Settings.entriesIsTransparent.ContainsKey(entry.Key) && Settings.entriesIsTransparent[entry.Key] == true)
                    .ToList();
            }

            // Clear existing entries in entriesShow
            Settings.entriesShow.Clear();

            // Update entriesShow based on search results
            foreach (var entry in searchResults)
            {
                Settings.entriesShow[entry.Key] = true;
            }

            // Set the remaining entries in entriesShow to false
            foreach (var key in Settings.entriesText.Keys)
            {
                if (!Settings.entriesShow.ContainsKey(key))
                {
                    Settings.entriesShow[key] = false;
                }
            }

            ResetVariables();
            SetupForm(true);
            UpdateHistory("");
        }


        // ###########################################################################################
        // Mark as favorite (used two places)
        // ###########################################################################################

        private void MarkFavorite()
        {
            // Procced if the entry is marked as a favorite
            if (Settings.entriesIsFavorite[entryActive])
            {
                Settings.entriesIsFavorite[entryActive] = false;
                foreach (Control c in Controls.Find("historyPictureBoxFav" + entryActiveList, true))
                {
                    c.Visible = false;
                }
                Logging.Log("History favorite toggled [Off] on entry key [" + entryActive + "]");
            }
            else
            {
                // Get the background color for the active entry (this is either a "label" or a "pictureBox")
                Color favoriteBackgroundColor = Color.Red;
                foreach (Control c in Controls.Find("historyLabel" + entryActiveList, true))
                {
                    if (c.Visible)
                    {
                        favoriteBackgroundColor = c.BackColor;
                    }
                }
                foreach (Control c in Controls.Find("historyPictureBox" + entryActiveList, true))
                {
                    if (c.Visible)
                    {
                        favoriteBackgroundColor = c.BackColor;
                    }
                }

                // Show the entry with a favorite marking
                Settings.entriesIsFavorite[entryActive] = true;
                foreach (Control c in Controls.Find("historyPictureBoxFav" + entryActiveList, true))
                {
                    c.BackColor = favoriteBackgroundColor;
                    c.BringToFront();
                    c.Visible = true;
                }
                Logging.Log("History favorite toggled [On] on entry key [" + entryActive + "]");
            }

            HandleFiles.saveIndexAndFavoriteFiles = true;
        }


        // ###########################################################################################
        // Key pressed actions
        // ###########################################################################################

        private void Search_KeyDown(object sender, KeyEventArgs e)
        {

            // ESCAPE
            if (e.KeyCode == Keys.Escape)
            {
                Logging.Log("Pressed \"Escape\" key");

                ActionEscape();

                // https://stackoverflow.com/a/3068797/2028935
                e.SuppressKeyPress = true;
            }

            // ENTER
            if (e.KeyCode == Keys.Return)
            {
                Logging.Log("Pressed \"Enter\" key");

                // Reset so all entries will be visible again
                foreach (var entry in Settings.entriesText)
                {
                    Settings.entriesShow[entry.Key] = true;
                }

                SelectEntry();

//                // Restore if we previously was in the favorite list
//                Settings.showFavoriteList = Settings.showFavoriteListLast;

                // https://stackoverflow.com/a/3068797/2028935
                e.SuppressKeyPress = true;
            }

            // DELETE
            if (e.KeyCode == Keys.Delete)
            {
                Logging.Log("Pressed \"Delete\" key");
                Logging.Log("Deleted key index [" + entryActive + "]");

                int historyListElements = Settings.historyListElements;
                int entryNewActive = 0;

                if (entriesInList > 1)
                {
                    if (!isEntryAtBottom)
                    {
                        entryNewActive = GetNextIndex("down", entryActive);
                    }
                    else
                    {
                        entryNewActive = GetNextIndex("up", entryActive);
                        entryInList--;
                    }
                }

                // Remove the chosen entry, so it does not show duplicates
                Settings.entriesText.Remove(entryActive);
                Settings.entriesImage.Remove(entryActive);
                Settings.entriesImageTrans.Remove(entryActive);
                Settings.entriesChecksum.Remove(entryActive);
                Settings.entriesApplication.Remove(entryActive);
                Settings.entriesApplicationIcon.Remove(entryActive);
                Settings.entriesOriginal.Remove(entryActive);
                Settings.entriesShow.Remove(entryActive);
                Settings.entriesIsFavorite.Remove(entryActive);
                Settings.entriesIsUrl.Remove(entryActive);
                Settings.entriesIsEmail.Remove(entryActive);
                Settings.entriesIsImage.Remove(entryActive);
                Settings.entriesIsTransparent.Remove(entryActive);
                Settings.entriesOrder.Remove(entryActive);

                entryActive = entryNewActive;

                entriesInList--;
                
                Settings.GetEntryCounter();

                if (entriesInList > 1)
                {
                    entryNewest = Settings.entriesText.Keys.Last();
                    entryOldest = Settings.entriesText.Keys.First();
                }

                if(entriesInList > 0)
                {
                    int hest = entryNewestBox;
                    int hestCompare = 0;
                    bool goUp = false;
                    int showElements = historyListElements > entriesInList ? entriesInList : historyListElements;
                    for (int i=0; i < showElements; i++)
                    {
                        hest = GetNextIndex("down", hest);
                        if(hest == hestCompare)
                        {
                            goUp = true;
                        }
                        hestCompare = hest;
                    }

                    if ((entryActive == entryOldest && entryNewestBox < entryNewest) || goUp)
                    {
                        entryNewestBox = GetNextIndex("up", entryNewestBox);
                    }

                    if (entriesInList <= historyListElements)
                    {
                        SetupForm(true);
                    }
                
                    UpdateHistory("");
                } else
                {
                    ResetVariables();
                    ResetForm();
                    ActionEscape();
                }
                NativeMethods.SetForegroundWindow(Handle);

                // Save the new order of the entries
                HandleFiles.saveIndexAndFavoriteFiles = true;

                // https://stackoverflow.com/a/3068797/2028935
                e.SuppressKeyPress = true;
            }

            // UP
            if (e.KeyCode == Keys.Up)
            {
                Logging.Log("Pressed \"Up\" arrow key");
                Settings.settings.GoEntryHigherNumber();
                e.SuppressKeyPress = true;
            }

            // DOWN
            if (e.KeyCode == Keys.Down)
            {
                Logging.Log("Pressed \"Down\" arrow key");
                Settings.settings.GoEntryLowerNumber();
                e.SuppressKeyPress = true;
            }

            // LEFT or PAGEUP (new page, get newer entries)
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.PageUp)
            {
                Logging.Log("Pressed \"Left\" arrow or \"PageUp\" key");

                if (entriesInList > Settings.historyListElements)
                {
                    // Get the next page, showing NEWER entries
                    entryNewestBox = GetNextTopEntry(entryNewestBox, "getNewerEntries");
                    entryActive = entryNewestBox;

                    // Get the human readable index for the header
                    var filteredEntries = Settings.entriesShow.Where(entry => entry.Value).ToList();
                    var sortedEntries = filteredEntries.OrderByDescending(entry => entry.Key).ToList();
                    entryInList = sortedEntries.FindIndex(entry => entry.Key == entryNewestBox) + 1;
                }

                // Update the list elements
                UpdateHistory("");

                e.SuppressKeyPress = true;
            }

            // RIGHT or PAGEDOWN (new page, get older entries)
            if (e.KeyCode == Keys.Right || e.KeyCode == Keys.PageDown)
            {
                Logging.Log("Pressed \"Right\" arrow or \"PageDown\" key");

                if (entriesInList > Settings.historyListElements)
                {
                    // Get the next page, showing OLDER entries
                    entryNewestBox = GetNextTopEntry(entryNewestBox, "getOlderEntries");
                    entryActive = entryNewestBox;

                    // Get the human readable index for the header
                    var filteredEntries = Settings.entriesShow.Where(entry => entry.Value).ToList();
                    var sortedEntries = filteredEntries.OrderByDescending(entry => entry.Key).ToList();
                    entryInList = sortedEntries.FindIndex(entry => entry.Key == entryNewestBox) + 1;
                }

                // Update the list elements
                UpdateHistory("");

                e.SuppressKeyPress = true;
            }

            // END
            if (e.KeyCode == Keys.End)
            {
                entryActive = entryOldest;

                Logging.Log("Pressed \"End\" key");

                int counter = 0;
                foreach (var entry in Settings.entriesShow)
                {
                    if (entry.Value)
                    {
                        entryNewestBox = entry.Key;
                        counter++;
                    }
                    if (counter == Settings.historyListElements)
                    {
                        break;
                    }
                }

                // Update the list elements
                UpdateHistory("");

                e.SuppressKeyPress = true;
            }

            // HOME
            if (e.KeyCode == Keys.Home)
            {
                Logging.Log("Pressed \"Home\" key");

                entryActive = entryNewest;
                entryNewestBox = entryNewest;

                // Update the list elements
                UpdateHistory("");

                e.SuppressKeyPress = true;
            }

            // Favorite hotkey
            string hotkeyFavorite = Settings.GetRegistryKey(Settings.registryPath, "HotkeyToggleFavorite");
            if (Enum.TryParse(hotkeyFavorite, out Keys parsedKey) && e.KeyCode == parsedKey)
            {
                MarkFavorite();
                e.SuppressKeyPress = true;
            }
        }


        // ###########################################################################################
        // Action to take when ESCAPE is pressed
        // ###########################################################################################

        public void ActionEscape()
        {
            // Reset so all entries will be visible again
            foreach (var entry in Settings.entriesText)
            {
                Settings.entriesShow[entry.Key] = true;
            }

            ResetVariables();
            ResetForm();
            Settings.isFirstCallAfterHotkey = true;

            // Check if the "Settings" UI was visible before - if so, then show it again
            if (Settings.isSettingsFormVisible)
            {
                Settings.settings.Show();
            }

            // Set focus back to the originating application
            Settings.ChangeFocusToOriginatingApplication();
        }


        // ###########################################################################################
        // Find the next page.
        // Fully done by ChatGPT :-)
        // ###########################################################################################

        private int GetNextTopEntry(int currentIdTop, string direction)
        {
            // Resort the list so it follows the UI
            var filteredEntries = Settings.entriesShow.Where(entry => entry.Value).ToList();
            var sortedEntries = filteredEntries.OrderByDescending(entry => entry.Key).ToList();
            int currentIndex = sortedEntries.FindIndex(entry => entry.Key == currentIdTop);
            int newIndex;

            if (direction == "getNewerEntries")
            {
                newIndex = currentIndex - Settings.historyListElements;
                newIndex = Math.Max(newIndex, 0);
            }

            else if (direction == "getOlderEntries")
            {
                newIndex = currentIndex + Settings.historyListElements;
                newIndex = Math.Min(newIndex, sortedEntries.Count - Settings.historyListElements);
            }

            else
            {
                throw new ArgumentException("Invalid direction");
            }

            int newTopEntry = sortedEntries[newIndex].Key;
            return newTopEntry;
        }


        // ###########################################################################################
    }
}
