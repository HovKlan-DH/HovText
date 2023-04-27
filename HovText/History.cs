using HovText.Properties;
//using System;
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
        string changeBorderElement = ""; // the UI name for the element coming to the paint redraw event
        private static int entryFirstBox = -1; // 0-indexed array ID for the first box element to show
        private static int entryFirst = -1; // 0-indexed array ID for the first element in the full list
        private static int entryLast = -1; // 0-indexed array ID for the last element in the full list
        private static int entryActive = -1; // 0-indexed array ID for the active entry
        private static int entryActiveLast = -1; // 0-indexed array ID for the last active entry (only used to determine if it should "flash")
        private static int entryActiveList = -1; // human-readable list number for the active entry (1 => showElements)
        private static bool isEntryAtTop = false; // is the active entry at the top position (newest entry)
        private static bool isEntryAtBottom = false; // is the active entry at the bottom position (oldest entry)
        public static int entriesInList;
        private static int entryInList = 0; // this is number X of Y (this stores the "X")


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
            // Get the total amount of entries, depending which view this is
            entriesInList = Settings.entriesText.Count;
            if (Settings.isEnabledFavorites && Settings.showFavoriteList)
            {
                int countFavorites = 0;
                for (int i = Settings.entriesText.ElementAt(0).Key; i <= Settings.entriesText.ElementAt(Settings.entriesText.Count - 1).Key; i++)
                {
                    if (Settings.entriesText.ContainsKey(i))
                    {
                        bool isFavorite = Settings.entriesIsFavorite[i];
                        countFavorites += isFavorite ? 1 : 0;
                    }
                }
                entriesInList = countFavorites;
                if (Settings.isTroubleshootEnabled) Logging.Log("Opened the history list view [Favorite]");
            }
            else
            {
                if (Settings.isTroubleshootEnabled) Logging.Log("Opened the history list view [All]");
            }

            int headlineHeight = 42;
            int resourceWidth = 18; // this is the "Favorite" icon width - optimize this by automatically get the width
            Label label;

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
            Width = width;

            // Add the headline and favorite marker

            // Headline
            label = new Label();
            label.Name = "uiHistoryHeadline";
            label.Width = width;
            label.Height = headlineHeight;
            label.Location = new Point(0, 0);
            label.BorderStyle = BorderStyle.FixedSingle;
            label.Padding = new Padding(5);
            label.TextAlign = ContentAlignment.MiddleLeft;
            label.Font = new Font(Settings.historyFontFamily, Settings.historyFontSize);
            label.BackColor = ColorTranslator.FromHtml(Settings.historyColorsHeader[Settings.historyColorTheme]);
            label.ForeColor = ColorTranslator.FromHtml(Settings.historyColorsHeaderText[Settings.historyColorTheme]);
            label.Visible = true;
            this.Controls.Add(label);

            // Headline favorite image (PictureBox)
            if (Settings.isEnabledFavorites)
            {
                PictureBox pictureBoxFav = new PictureBox();
                pictureBoxFav.Name = "uiHistoryHeadlineFav";
                pictureBoxFav.Width = resourceWidth;
                pictureBoxFav.Height = resourceWidth;
                pictureBoxFav.Location = new Point(width - resourceWidth - 10, 1);
                pictureBoxFav.BorderStyle = BorderStyle.None;
                pictureBoxFav.Visible = false;
                pictureBoxFav.Image = Resources.Favorite;
                this.Controls.Add(pictureBoxFav);
            }

            // Set the next vertical position
            int nextPosY = 0;
            nextPosY += headlineHeight;

            // Show a "warning" if we are in the favorite view but has no favorites
            if (Settings.isEnabledFavorites && entriesInList == 0 && Settings.showFavoriteList)
            {
                Height = height + SystemInformation.FrameBorderSize.Height + SystemInformation.Border3DSize.Height + 4;

                // Add a new label that will show the warning text
                label = new Label();
                label.Name = "uiNoFavorites";
                label.Width = width;
                label.Height = height - headlineHeight;
                label.Location = new Point(0, headlineHeight);
                label.BorderStyle = BorderStyle.FixedSingle;
                label.Padding = new Padding(5);
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.Text = "You have no favorites set";
                label.Font = new Font(Settings.historyFontFamily, Settings.historyFontSize);
                label.BackColor = Color.WhiteSmoke;
                label.ForeColor = Color.Black;
                label.Visible = true;
                this.Controls.Add(label);
            }
            else
            {
                // We are now in one of the views, "All" or "Favorite" and it has one or more entries

                // "showElements" determines how many boxes to show
                int showElements = Settings.historyListElements;
                showElements = showElements > entriesInList ? entriesInList : showElements;

                // Set this form height and element heights
                int boxHeight = (height - headlineHeight) / showElements;
                Height = (boxHeight * showElements) + headlineHeight + SystemInformation.FrameBorderSize.Height + SystemInformation.Border3DSize.Height + 4;

                // Setup all visible element boxes
                for (int i = 1; i <= showElements; i++)
                {
                    // Text entry (Label)
                    label = new Label();
                    label.Name = "historyLabel" + i;
                    label.Width = width;
                    label.Height = boxHeight;
                    label.Location = new Point(0, nextPosY);
                    label.BorderStyle = BorderStyle.FixedSingle;
//                    label.Padding = new Padding(5);
                    label.Padding = new Padding(Settings.historyBorderThickness - 2);
                    label.Font = new Font(Settings.historyFontFamily, Settings.historyFontSize);
                    label.Visible = false;
                    label.Text = "";
                    this.Controls.Add(label);

                    // Catch repaint event for this specific element (to draw the border)
                    this.Controls["historyLabel" + i].Paint += new System.Windows.Forms.PaintEventHandler(this.History_Paint);

                    // Image entry (PictureBox)
                    PictureBox pictureBox = new PictureBox();
                    pictureBox.Name = "historyPictureBox" + i;
                    pictureBox.Width = width;
                    pictureBox.Height = boxHeight;
                    pictureBox.Location = new Point(0, nextPosY);
                    pictureBox.BorderStyle = BorderStyle.FixedSingle;
//                    pictureBox.Padding = new Padding(10);
                    pictureBox.Padding = new Padding(Settings.historyBorderThickness - 2);
                    pictureBox.Visible = false;
                    pictureBox.Image = null;
                    this.Controls.Add(pictureBox);

                    // Catch repaint event for this specific element (to draw the border)
                    this.Controls["historyPictureBox" + i].Paint += new System.Windows.Forms.PaintEventHandler(this.History_Paint);

                    // Favorite image (PictureBox)
                    if (Settings.isEnabledFavorites)
                    {
                        PictureBox pictureBoxFavEntry = new PictureBox();
                        pictureBoxFavEntry.Name = "historyPictureBoxFav" + i;
                        pictureBoxFavEntry.Width = resourceWidth;
                        pictureBoxFavEntry.Height = resourceWidth;
                        pictureBoxFavEntry.Location = new Point(width - resourceWidth - 11, nextPosY + 2);
                        pictureBoxFavEntry.BorderStyle = BorderStyle.None;
                        pictureBoxFavEntry.Visible = false;
                        pictureBoxFavEntry.Image = Resources.Favorite;
                        this.Controls.Add(pictureBoxFavEntry);
                    }

                    nextPosY += boxHeight;
                }
            }

            // If we are in the "favorite" view then show the icon for it in the "headline"
            if (Settings.isEnabledFavorites)
            {
                foreach (Control c in this.Controls.Find("uiHistoryHeadlineFav", true))
                {
                    if (Settings.showFavoriteList)
                    {
                        c.BackColor = this.Controls["uiHistoryHeadline"].BackColor;
                        c.BringToFront();
                        c.Visible = true;
                    }
                    else
                    {
                        c.Visible = false;
                    }
                }
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
            this.Controls.Clear();
            Hide();
        }


        // ###########################################################################################
        // Repaint the entry border
        // ###########################################################################################

        private void History_Paint(object sender, PaintEventArgs e)
        {
            if (Settings.historyBorderThickness > 0)
            {
                // Set padding
                string hest = ((System.Windows.Forms.Control)sender).Name;
                bool containsSubstring = hest.Contains("historyPictureBox");
                int padding = containsSubstring ? 10 : 5;
                padding = 0;
                System.Console.WriteLine("Padding=" + padding.ToString());

                if (Settings.historyBorderThickness >= 2)
                {
                    ((System.Windows.Forms.Control)sender).Padding = new Padding(Settings.historyBorderThickness - 2 + padding);
                } else
                {
                    ((System.Windows.Forms.Control)sender).Padding = new Padding(Settings.historyBorderThickness - 1 + padding);
                }

                // Redraw border with a solid color, if the update source event is larger than 18px (favorite icon) and if we are placed on the active entry
                if (changeBorderElement == ((System.Windows.Forms.Control)sender).Name && e.ClipRectangle.Width > 18)
                    {
                    ControlPaint.DrawBorder(e.Graphics, e.ClipRectangle,
                                  ColorTranslator.FromHtml(Settings.historyColorBorder), Settings.historyBorderThickness, ButtonBorderStyle.Solid,
                                  ColorTranslator.FromHtml(Settings.historyColorBorder), Settings.historyBorderThickness, ButtonBorderStyle.Solid,
                                  ColorTranslator.FromHtml(Settings.historyColorBorder), Settings.historyBorderThickness, ButtonBorderStyle.Solid,
                                  ColorTranslator.FromHtml(Settings.historyColorBorder), Settings.historyBorderThickness, ButtonBorderStyle.Solid);
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
            // Set the "first" and "last" IDs
            entryFirstBox = entryFirstBox == -1 ? GetFirstIndex() : entryFirstBox;
            entryFirst = entryFirst == -1 ? GetFirstIndex() : entryFirst;
            entryLast = entryLast == -1 ? GetLastIndex() : entryLast;
            entryFirstBox = isEntryAtTop && direction == "up" ? GetNextIndex(direction, entryFirstBox) : entryFirstBox;
            entryFirstBox = isEntryAtBottom && direction == "down" ? GetNextIndex(direction, entryFirstBox) : entryFirstBox;

            // Set the active entry
            entryActive = entryActive == -1 ? entryFirstBox : GetNextIndex(direction, entryActive);

            // Get the amount of total entries in array - overwrite if we are showing the "Favorite" list
            entriesInList = Settings.entriesText.Count;
            if (Settings.showFavoriteList)
            {
                int countFavorites = 0;
                for (int i = entryFirst; i >= Settings.entriesText.ElementAt(0).Key; i--)
                {
                    if (Settings.entriesText.ContainsKey(i))
                    {
                        bool isFavorite = Settings.entriesIsFavorite[i];
                        countFavorites += isFavorite ? 1 : 0;
                    }
                }
                entriesInList = countFavorites < entriesInList ? countFavorites : entriesInList;
            }

            bool isTransparent;
            Image entryImage;

            // Proceed if we have more than one entry in the list (it also comes to here, if we are showing an empty favorite list)
            if (entriesInList > 0)
            {
                // "showElements" determines how many boxes to show
                int showElements = Settings.historyListElements;
                showElements = showElements > entriesInList ? entriesInList : showElements;
                int shownElements = 0;

                // Move the first box element if there is only one to show
                entryFirstBox = showElements == 1 ? entryActive : entryFirstBox;

                // Build the list of visible entry boxes
                for (int i = entryFirstBox; i >= 0 && shownElements < showElements; i--)
                {
                    Color favoriteBackgroundColor = Color.Red;
                    
                    // Check if the array ID exists
                    bool doesKeyExist = Settings.entriesText.ContainsKey(i);
                    if (doesKeyExist)
                    {
                        // Get the array data
                        string entryText = Settings.entriesText[i];
                        entryImage = Settings.entriesImage[i];
                        bool isFavorite = Settings.entriesIsFavorite[i];

                        // Proceed if this is a valid entry, depending on the list view
                        if (!Settings.showFavoriteList || (Settings.showFavoriteList && isFavorite))
                        {
                            shownElements++;

                            // Check if it is a TEXT
                            if (!string.IsNullOrEmpty(entryText))
                            {
                                // Find a form element with a specific name
                                foreach (Control c in this.Controls.Find("historyLabel" + shownElements, true))
                                {
                                    c.Text = entryText;
                                    c.Visible = true;

                                    // Set the colors
                                    if (i == entryActive && showElements > 1)
                                    {
//                                        c.BackColor = ColorTranslator.FromHtml(Settings.historyColorsHeader[Settings.historyColorTheme]);
//                                        c.ForeColor = ColorTranslator.FromHtml(Settings.historyColorsHeaderText[Settings.historyColorTheme]);
                                        c.BackColor = ColorTranslator.FromHtml(Settings.historyColorsActive[Settings.historyColorTheme]);
                                        c.ForeColor = ColorTranslator.FromHtml(Settings.historyColorsActiveText[Settings.historyColorTheme]);
                                        changeBorderElement = c.Name;
                                        c.Refresh();

                                        isEntryAtTop = shownElements == 1 ? true : false;
                                        isEntryAtBottom = shownElements == showElements ? true : false;
                                        entryActiveList = shownElements;
                                    }
                                    else
                                    {
                                        c.BackColor = ColorTranslator.FromHtml(Settings.historyColorsEntry[Settings.historyColorTheme]);
                                        c.ForeColor = ColorTranslator.FromHtml(Settings.historyColorsEntryText[Settings.historyColorTheme]);

                                        // Mark this as the active element, if there is only shown one entry
                                        if (i == entryActive && showElements == 1)
                                        {
                                            entryActiveList = shownElements;
                                        }
                                    }
                                    favoriteBackgroundColor = c.BackColor;
                                }
                            }
                            else
                            {
                                // Hide the label as it then must be an image
                                this.Controls["historyLabel" + shownElements].Visible = false;
                            }

                            // Check if it is an IMAGE
                            if (entryImage != null)
                            {
                                // Find a form element with a specific name
                                foreach (Control c in this.Controls.Find("historyPictureBox" + shownElements, true))
                                {
                                    // Check if the image is transparent - if so make the image background transparent
                                    isTransparent = Settings.entriesImageTransparent[i];
                                    if (isTransparent)
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
                                    if (i == entryActive && showElements > 1)
                                    {
//                                        c.BackColor = ColorTranslator.FromHtml(Settings.historyColorsHeader[Settings.historyColorTheme]);
//                                        c.ForeColor = ColorTranslator.FromHtml(Settings.historyColorsHeaderText[Settings.historyColorTheme]);
                                        c.BackColor = ColorTranslator.FromHtml(Settings.historyColorsActive[Settings.historyColorTheme]);
                                        c.ForeColor = ColorTranslator.FromHtml(Settings.historyColorsActiveText[Settings.historyColorTheme]);
                                        changeBorderElement = c.Name;
                                        c.Refresh();

                                        isEntryAtTop = shownElements == 1 ? true : false;
                                        isEntryAtBottom = shownElements == showElements ? true : false;
                                        entryActiveList = shownElements;
                                    }
                                    else
                                    {
                                        c.BackColor = ColorTranslator.FromHtml(Settings.historyColorsEntry[Settings.historyColorTheme]);
                                        c.ForeColor = ColorTranslator.FromHtml(Settings.historyColorsEntryText[Settings.historyColorTheme]);

                                        // Mark this as the active element, if there is only shown one entry
                                        if (i == entryActive && showElements == 1)
                                        {
                                            entryActiveList = shownElements;
                                        }
                                    }
                                    favoriteBackgroundColor = c.BackColor;
                                }
                            }
                            else
                            {
                                // Hide the pictureBox as it then must be a text
                                this.Controls["historyPictureBox" + shownElements].Visible = false;
                            }

                            // Find the "favorite" picture box
                            if (Settings.isEnabledFavorites)
                            {
                                foreach (Control c in this.Controls.Find("historyPictureBoxFav" + shownElements, true))
                                {
                                    if (isFavorite)
                                    {
                                        c.BackColor = favoriteBackgroundColor;
                                        c.BringToFront();
                                        c.Visible = true;
                                    }
                                    else
                                    {
                                        c.Visible = false;

                                    }
                                }
                            }
                        }
                    }
                }

                // Set the "this is entry X of Y" number
                entryInList += direction == "down" && entryActive != entryLast ? 1 : 0;
                entryInList -= direction == "up" && entryActive != entryFirst ? 1 : 0;
                entryInList = entryActive == entryLast ? entriesInList : entryInList;
                entryInList = entryActive == entryFirst ? 1 : entryInList;

                // Set the headline
                string entryApplication = Settings.entriesApplication[entryActive];
                this.Controls["uiHistoryHeadline"].Text = entryInList + " of " + entriesInList + " from \"" + entryApplication + "\"";
                entryImage = Settings.entriesImage[entryActive];
                if (entryImage != null)
                {
                    isTransparent = Settings.entriesImageTransparent[entryActive];
                    if (isTransparent)
                    {
                        this.Controls["uiHistoryHeadline"].Text += " (transparent image)";
                    }
                    else
                    {
                        this.Controls["uiHistoryHeadline"].Text += " (image)";
                    }
                }

                // Flash the headline if we are all the way to the top or bottom
                if (entryActive == entryActiveLast)
                {
                    Flash();
                }
                entryActiveLast = entryActive;
            }

            // Set a special headline text, if there is no favorites
            if (Settings.showFavoriteList && entriesInList == 0)
            {
                this.Controls["uiHistoryHeadline"].Text = "No data - change view";
            }
            this.Controls["uiHistoryHeadline"].Refresh();

            // Make sure that we will catch the key-up event
            TopMost = true;
        }


        // ###########################################################################################
        // Get the first (newest) array ID in the full array, depending on the view
        // ###########################################################################################

        private static int GetFirstIndex ()
        {
            int first = entryFirstBox == -1 ? Settings.entryIndex : entryFirstBox;
            for (int i = first; i >= Settings.entriesText.ElementAt(0).Key; i--)
            {
                // Proceed if the array ID exists
                if (Settings.entriesText.ContainsKey(i))
                {
                    if (!Settings.showFavoriteList)
                    {
                        return i;
                    }
                    if (Settings.showFavoriteList && Settings.entriesIsFavorite[i])
                    {
                        return i;
                    }
                }
            }
            return 0;
        }


        // ###########################################################################################
        // Get the last (oldest) array ID in the full array, depending on the view
        // ###########################################################################################

        private static int GetLastIndex()
        {
            int last = entryLast == -1 ? Settings.entriesText.ElementAt(0).Key : entryLast;
            for (int i = last; i <= Settings.entriesText.ElementAt(Settings.entriesText.Count - 1).Key; i++)
            {
                // Proceed if the array ID exists
                if (Settings.entriesText.ContainsKey(i))
                {
                    if (!Settings.showFavoriteList)
                    {
                        return i;
                    }
                    if (Settings.showFavoriteList && Settings.entriesIsFavorite[i])
                    {
                        return i;
                    }
                }
            }
            return 0;
        }


        // ###########################################################################################
        // Get the next entry array ID, depending on the view and the direction, up/down
        // ###########################################################################################

        public static int GetNextIndex(string direction, int entryKey)
        {
            if (direction == "up")
            {
                for (int i = entryKey + 1; i <= Settings.entriesText.ElementAt(Settings.entriesText.Count - 1).Key; i++)
                {
                    if (Settings.entriesText.ContainsKey(i))
                    {
                        if (!Settings.showFavoriteList)
                        {
                            return i;
                        }

                        if (Settings.showFavoriteList && Settings.entriesIsFavorite[i])
                        {
                            return i;
                        }

                    }
                }
            }
                        
            if (direction == "down")
            {
                // Proceed if the active element is not the last element
                if (entryActive != entryLast)
                {
                    for (int i = entryKey - 1; i >= Settings.entriesText.ElementAt(0).Key; i--)
                    {
                        if (Settings.entriesText.ContainsKey(i))
                        {
                            if (!Settings.showFavoriteList)
                            {
                                return i;
                            }
                            if (Settings.showFavoriteList && Settings.entriesIsFavorite[i])
                            {
                                return i;
                            }
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
                // Insert log depending if list is empty or not
                if (entriesInList == 0)
                {
                    if (Settings.isTroubleshootEnabled) Logging.Log("Selected history entry element [none, as list is empty]");
                }
                else
                {
                    if (Settings.isTroubleshootEnabled) Logging.Log("Selected history entry list element [" + entryActiveList + "] of ["+ entriesInList +"] with key [" + entryActive + "]");
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
            } else
            {
                // Check for other key combinations

                // For conversion from text to keys
                KeysConverter cvt = new KeysConverter();
                Keys key;

                // Proceed if we should toggle the list view
                string hotkey6 = Settings.GetRegistryKey(Settings.registryPath, "Hotkey6");
                if (Settings.isEnabledFavorites && hotkey6 != "Not set")
                {
                    key = (Keys)cvt.ConvertFrom(hotkey6);
                    if (e.KeyCode == key)
                    {
                        if (Settings.showFavoriteList)
                        {
                            Settings.showFavoriteList = false;
                            if (Settings.isTroubleshootEnabled) Logging.Log("History list changed to [All]");
                        }
                        else
                        {
                            Settings.showFavoriteList = true;
                            if (Settings.isTroubleshootEnabled) Logging.Log("History list changed to [Favorite]");
                        }

                        // Reset some stuff
                        ResetVariables();
                        ResetForm();
                        SetupForm();
                        UpdateHistory("down");
                    }
                }

                // Proceed if we should toggle a favorite entry
                string hotkey5 = Settings.GetRegistryKey(Settings.registryPath, "Hotkey5");
                if (Settings.isEnabledFavorites && hotkey5 != "Not set")
                {
                    key = (Keys)cvt.ConvertFrom(hotkey5);
                    if (e.KeyCode == key)
                    {
                        // Only relevant when we are in the "favorite" view
                        bool isFavoriteEmpty = false;
                        foreach (Control c in this.Controls.Find("uiNoFavorites", true))
                        {
                            isFavoriteEmpty = true; // if this label exists then we know the favorite is empty
                        }

                        // Proceed if there is at least one entry in the favorite list
                        if (!Settings.showFavoriteList || !isFavoriteEmpty)
                        {

                            // Procced if the entry already is marked as a favorite
                            if (Settings.entriesIsFavorite[entryActive])
                            {
                                Settings.entriesIsFavorite[entryActive] = false;
                                foreach (Control c in this.Controls.Find("historyPictureBoxFav" + entryActiveList, true))
                                {
                                    c.Visible = false;
                                }
                                if (Settings.isTroubleshootEnabled) Logging.Log("History favorite toggled [Off] on entry key [" + entryActive + "]");
                            }
                            else
                            {
                                // Get the background color for the active entry (this is either a "label" or a "pictureBox")
                                Color favoriteBackgroundColor = Color.Red;
                                foreach (Control c in this.Controls.Find("historyLabel" + entryActiveList, true))
                                {
                                    if (c.Visible)
                                    {
                                        favoriteBackgroundColor = c.BackColor;
                                    }
                                }
                                foreach (Control c in this.Controls.Find("historyPictureBox" + entryActiveList, true))
                                {
                                    if (c.Visible)
                                    {
                                        favoriteBackgroundColor = c.BackColor;
                                    }
                                }

                                // Show the entry with a favorite marking
                                Settings.entriesIsFavorite[entryActive] = true;
                                foreach (Control c in this.Controls.Find("historyPictureBoxFav" + entryActiveList, true))
                                {
                                    c.BackColor = favoriteBackgroundColor;
                                    c.BringToFront();
                                    c.Visible = true;
                                }
                                if (Settings.isTroubleshootEnabled) Logging.Log("History favorite toggled [On] on entry key [" + entryActive + "]");
                            }

                            if (Settings.showFavoriteList)
                            {
                                // Reset some stuff
                                ResetVariables();
                                ResetForm();
                                SetupForm();
                                UpdateHistory("down");
                            }
                        }
                    }
                }
            }
        }

        private static void ResetVariables ()
        {
            entryFirstBox = -1;
            entryFirst = -1;
            entryLast = -1;
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
            if(!Settings.isHistoryMarginEnabled)
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
                    this.Left = x + historyMargin;
                    this.Top = y + historyMargin;
                    break;
                case "Left Bottom":
                    x = Screen.AllScreens[Settings.activeDisplay].WorkingArea.Left;
                    y = Screen.AllScreens[Settings.activeDisplay].WorkingArea.Bottom - this.Height;
                    this.Left = x + historyMargin;
                    this.Top = y - historyMargin;
                    break;
                case "Center":
                    x = Screen.AllScreens[Settings.activeDisplay].WorkingArea.Left + ((Screen.AllScreens[Settings.activeDisplay].WorkingArea.Width - this.Width) / 2);
                    y = Screen.AllScreens[Settings.activeDisplay].WorkingArea.Top + ((Screen.AllScreens[Settings.activeDisplay].WorkingArea.Height - this.Height) / 2);
                    this.Left = x;
                    this.Top = y;
                    break;
                case "Right Top":
                    x = Screen.AllScreens[Settings.activeDisplay].WorkingArea.Right - this.Width;
                    y = Screen.AllScreens[Settings.activeDisplay].WorkingArea.Top;
                    this.Left = x - historyMargin;
                    this.Top = y + historyMargin;
                    break;
                default: // Right Bottom
                    x = Screen.AllScreens[Settings.activeDisplay].WorkingArea.Right - this.Width;
                    y = Screen.AllScreens[Settings.activeDisplay].WorkingArea.Bottom - this.Height;
                    this.Left = x - historyMargin;
                    this.Top = y - historyMargin;
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
            UpdateElement(ColorTranslator.FromHtml(Settings.historyColorsEntry[Settings.historyColorTheme]));
            Thread.Sleep(interval / 2);
            UpdateElement(ColorTranslator.FromHtml(Settings.historyColorsHeader[Settings.historyColorTheme]));
            Thread.Sleep(interval / 2);
        }


        // ###########################################################################################
    }
}
