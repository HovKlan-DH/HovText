using HovText.Properties;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
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
        private static int entryNewestBox = -1; // 0-indexed array ID for the first box element to show (this is the LAST element in the list, as this is the most recent one added)
        private static int entryNewest = -1; // 0-indexed array ID for the first element in the full list
        private static int entryOldest = -1; // 0-indexed array ID for the last element in the full list
        private static int entryActive = -1; // 0-indexed array ID for the active entry
        private static int entryActiveLast = -1; // 0-indexed array ID for the last active entry (only used to determine if it should "flash")
        private static int entryActiveList = -1; // human-readable list number for the active entry (1 => showElements)
        private static bool isEntryAtTop = false; // is the active entry at the top position (newest entry)
        private static bool isEntryAtBottom = false; // is the active entry at the bottom position (oldest entry)
        public static int entriesInList;
        private static int entryInList = 0; // this is number X of Y (this stores the "X")
        private System.Windows.Forms.Timer _flashTimer;
        private Color _flashColor;


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

        public void SetupForm(bool keepHeaders = false)
        {

            bool hest = Settings.isHistoryHotkeyPressed;

            // "keepHeaders = true" equals that headline + search will be kept - but all other elements are deleted
            if (keepHeaders)
            {
                for (int i = this.Controls.Count - 1; i >= 2; i--)
                {
                    Control control = this.Controls[i];
                    this.Controls.Remove(control);
                    control.Dispose();
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
            this.Controls.Add(label);

            // Get the font size for the temporary label
            Size textSize = TextRenderer.MeasureText("Sample Text", label.Font);
            fontSizePixels = textSize.Height;
            headlineHeight = fontSizePixels + headlineHeightPadding;

            // Remove the temporary label again - no more use for it
            Control control1 = this.Controls["temporarylabel"];
            this.Controls.Remove(control1);
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
                this.Controls.Add(label);

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
                    this.Controls.Add(pictureBoxFav);
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
                    this.Controls.Add(textBox);
                    textBox.BringToFront();

                    // Define events for the input field
                    textBox.TextChanged += Search_TextChanged;
                    textBox.KeyDown += Search_KeyDown;
                }
            }

            // Get and set the height for "search" - this is a dynamic height, based on font size
            if (this.Controls.ContainsKey("search"))
            {
                TextBox search = this.Controls["search"] as TextBox; // get the reference to the textbox element
                searchlineHeight = fontSizePixels + searchlineHeightPadding;
                search.Height = searchlineHeight;
            }

            // Set the next vertical position
            int nextPosY = 0;
            nextPosY += headlineHeight + searchlineHeight;

            // Get the total amount of entries, depending which view this is
            //entriesInList = Settings.entriesText.Count;
            entriesInList = Settings.entriesShow.Count(kv => kv.Value == true);
            if (Settings.isEnabledFavorites && Settings.showFavoriteList)
            {
                int countFavorites = 0;
                for (int i = Settings.entriesText.ElementAt(0).Key; i <= Settings.entriesText.ElementAt(Settings.entriesText.Count - 1).Key; i++)
                {
                    /*
                    if (Settings.entriesText.ContainsKey(i))
                    {
                        bool isFavorite = Settings.entriesIsFavorite[i];
                        countFavorites += isFavorite ? 1 : 0;
                    }
                    */
                    // ChatGTP4 suggestion for code optimization for the above commented
                    if (Settings.entriesIsFavorite.TryGetValue(i, out bool isFavorite))
                    {
                        countFavorites += isFavorite ? 1 : 0;
                    }
                }
                entriesInList = countFavorites;
                Logging.Log("Opened the history list view [Favorite]");
            }
            else
            {
                Logging.Log("Opened the history list view [All]");
            }

            // Show a "warning" if we are in the favorite view but has no favorites
            if (Settings.isEnabledFavorites && entriesInList == 0 && Settings.showFavoriteList)
            {
                Height = height + SystemInformation.FrameBorderSize.Height + SystemInformation.Border3DSize.Height + 4;

                // Add a new label that will show the warning text
                label = new Label
                {
                    Name = "uiNoFavorites",
                    Width = width,
                    Height = height - (headlineHeight + searchlineHeight),
                    Location = new Point(0, headlineHeight + searchlineHeight),
                    BorderStyle = BorderStyle.FixedSingle,
                    Padding = new Padding(5),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Text = "You have no favorites set",
                    Font = new Font(Settings.historyFontFamily, Settings.historyFontSize),
                    BackColor = Color.WhiteSmoke,
                    ForeColor = Color.Black,
                    Visible = true
                };
                this.Controls.Add(label);
            }
            else
            {
                // We are now in one of the views, "All" or "Favorite" and it has one or more entries

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
                            Visible = false,
                            Text = ""
                        };
                        this.Controls.Add(label);

                        // Catch repaint event for this specific element (to draw the border)
                        this.Controls["historyLabel" + i].Paint += new System.Windows.Forms.PaintEventHandler(this.History_Paint);

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
                            Image = null
                        };
                        this.Controls.Add(pictureBox);

                        // Catch repaint event for this specific element (to draw the border)
                        this.Controls["historyPictureBox" + i].Paint += new System.Windows.Forms.PaintEventHandler(this.History_Paint);

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
                            this.Controls.Add(pictureBoxFavEntry);
                        }

                        nextPosY += boxHeight;
                
                    }
                } else
                {
                    this.Controls["uiHistoryHeadline"].Text = "0 entries found with this text";
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
            if (Settings.historyBorderThickness > 0 && entriesInList > 1)
            {
                // Set padding
                int padding = 0;

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
                                  ColorTranslator.FromHtml(Settings.historyColorsBorder[Settings.historyColorTheme]), Settings.historyBorderThickness, ButtonBorderStyle.Solid,
                                  ColorTranslator.FromHtml(Settings.historyColorsBorder[Settings.historyColorTheme]), Settings.historyBorderThickness, ButtonBorderStyle.Solid,
                                  ColorTranslator.FromHtml(Settings.historyColorsBorder[Settings.historyColorTheme]), Settings.historyBorderThickness, ButtonBorderStyle.Solid,
                                  ColorTranslator.FromHtml(Settings.historyColorsBorder[Settings.historyColorTheme]), Settings.historyBorderThickness, ButtonBorderStyle.Solid);
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
            entryNewestBox = entryNewestBox == -1 ? GetNewestIndex() : entryNewestBox;
            entryNewest = entryNewest == -1 ? GetNewestIndex() : entryNewest;
            entryOldest = GetOldestIndex();
            entryNewestBox = isEntryAtTop && direction == "up" ? GetNextIndex(direction, entryNewestBox) : entryNewestBox;
            entryNewestBox = isEntryAtBottom && direction == "down" ? GetNextIndex(direction, entryNewestBox) : entryNewestBox;

            // Set the active entry
            entryActive = entryActive == -1 ? entryNewestBox : GetNextIndex(direction, entryActive);

            // Get the amount of total entries in array - overwrite if we are showing the "Favorite" list
            entriesInList = Settings.entriesShow.Count(kv => kv.Value == true);
            if (Settings.showFavoriteList)
            {
                int countFavorites = 0;
                for (int i = entryNewest; i >= Settings.entriesText.ElementAt(0).Key; i--)
                {
                    /*
                    if (Settings.entriesText.ContainsKey(i))
                    {
                        bool isFavorite = Settings.entriesIsFavorite[i];
                        countFavorites += isFavorite ? 1 : 0;
                    }
                    */
                    // ChatGTP4 suggestion for code optimization
                    if (Settings.entriesIsFavorite.TryGetValue(i, out bool isFavorite))
                    {
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
                entryNewestBox = showElements == 1 ? entryActive : entryNewestBox;

                // Build the list of visible entry boxes
                for (int i = entryNewestBox; i >= 0 && shownElements < showElements; i--)
                {
                    Color favoriteBackgroundColor = Color.Red;
                    
                    // Check if the array ID exists
                    bool doesKeyExist = Settings.entriesText.ContainsKey(i);

                    // Check if the array ID should be shown
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
                                        c.BackColor = ColorTranslator.FromHtml(Settings.historyColorsActive[Settings.historyColorTheme]);
                                        c.ForeColor = ColorTranslator.FromHtml(Settings.historyColorsActiveText[Settings.historyColorTheme]);
                                        changeBorderElement = c.Name;
                                        c.Refresh();

                                        isEntryAtTop = shownElements == 1;
                                        isEntryAtBottom = shownElements == showElements;
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
                                        c.BackColor = ColorTranslator.FromHtml(Settings.historyColorsActive[Settings.historyColorTheme]);
                                        c.ForeColor = ColorTranslator.FromHtml(Settings.historyColorsActiveText[Settings.historyColorTheme]);
                                        changeBorderElement = c.Name;
                                        c.Refresh();

                                        //isEntryAtTop = shownElements == 1 ? true : false;
                                        isEntryAtTop = shownElements == 1;
                                        //isEntryAtBottom = shownElements == showElements ? true : false;
                                        isEntryAtBottom = shownElements == showElements;
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
                entryInList += direction == "down" && entryActive != entryOldest ? 1 : 0;
                entryInList -= direction == "up" && entryActive != entryNewest ? 1 : 0;
                entryInList = entryActive == entryOldest ? entriesInList : entryInList;
                entryInList = entryActive == entryNewest ? 1 : entryInList;

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

        private static int GetNewestIndex ()
        {
            int first = entryNewestBox == -1 ? Settings.entryIndex : entryNewestBox;
            for (int i = first; i >= Settings.entriesText.ElementAt(0).Key; i--)
            {
                // Proceed if the array ID exists
                if (Settings.entriesText.ContainsKey(i))
                {
                    if (!Settings.showFavoriteList && Settings.entriesShow[i])
                    {
                        return i;
                    }
                    if (Settings.showFavoriteList && Settings.entriesIsFavorite[i] && Settings.entriesShow[i])
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

        private static int GetOldestIndex()
        {
            int last = entryOldest == -1 ? Settings.entriesText.ElementAt(0).Key : entryOldest;
            for (int i = last; i <= Settings.entriesText.ElementAt(Settings.entriesText.Count - 1).Key; i++)
            {
                // Proceed if the array ID exists
                if (Settings.entriesText.ContainsKey(i))
                {
                    if (!Settings.showFavoriteList && Settings.entriesShow[i])
                    {
                        return i;
                    }
                    if (Settings.showFavoriteList && Settings.entriesIsFavorite[i] && Settings.entriesShow[i])
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
                    if (Settings.entriesText.ContainsKey(i) && Settings.entriesShow[i])
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
                if (entryActive != entryOldest)
                {
                    for (int i = entryKey - 1; i >= Settings.entriesText.ElementAt(0).Key; i--)
                    {
                        if (Settings.entriesText.ContainsKey(i) && Settings.entriesShow[i])
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
                SelectEntry();
            } else
            {
                // Check for other key combinations

                // For conversion from text to keys
                KeysConverter cvt = new KeysConverter();
                Keys key;

                // Proceed if we should toggle the list view
                string hotkeyToggleView = Settings.GetRegistryKey(Settings.registryPath, "HotkeyToggleView");
                if (Settings.isEnabledFavorites && hotkeyToggleView != "Not set")
                {
                    key = (Keys)cvt.ConvertFrom(hotkeyToggleView);
                    if (e.KeyCode == key)
                    {
                        if (Settings.showFavoriteList)
                        {
                            Settings.showFavoriteList = false;
                            Logging.Log("History list changed to [All]");
                        }
                        else
                        {
                            Settings.showFavoriteList = true;
                            Logging.Log("History list changed to [Favorite]");
                        }

                        // Reset some stuff
                        ResetVariables();
                        ResetForm();
                        SetupForm();
                        UpdateHistory("down");
                    }
                }

                // Proceed if we should toggle a favorite entry
                string hotkeyToggleFavorite = Settings.GetRegistryKey(Settings.registryPath, "HotkeyToggleFavorite");
                if (Settings.isEnabledFavorites && hotkeyToggleFavorite != "Not set")
                {
                    key = (Keys)cvt.ConvertFrom(hotkeyToggleFavorite);
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
                                Logging.Log("History favorite toggled [Off] on entry key [" + entryActive + "]");
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
                                Logging.Log("History favorite toggled [On] on entry key [" + entryActive + "]");
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

        /*
        
        Rewrote this as I introduced catching global exception - and I caught an exception,
        so I asked ChatGPT4 for help solving this - and below is suggested GPT4 code, compared
        to my (found on StackOverflow) code :-)

        The new ChatGPT4 code seems more straight forward, not needing to "invoke" anything, which
        I believe is not so ideal.

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

        */

        private void flashTimer_Tick(object sender, System.EventArgs e)
        {
            if (this.Controls.ContainsKey("uiHistoryHeadline"))
            {
                if (this.Controls["uiHistoryHeadline"].BackColor == _flashColor)
                {
                    this.Controls["uiHistoryHeadline"].BackColor = ColorTranslator.FromHtml(Settings.historyColorsHeader[Settings.historyColorTheme]);
                    _flashTimer.Stop(); // stop the Timer after the color has been toggled
                }
                else
                {
                    this.Controls["uiHistoryHeadline"].BackColor = _flashColor;
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
        //
        // ###########################################################################################

        private void SelectEntry ()
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
        // 
        // ###########################################################################################

        private void Search_TextChanged(object sender, System.EventArgs e)
        {

            TextBox textBox = (TextBox)sender; // Cast the sender to TextBox
            string searchText = textBox.Text;

            // Perform a case-insensitive wildcard search using LINQ
            var searchResults = Settings.entriesText
                .Where(entry => entry.Value.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

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
            if(e.KeyCode == Keys.Return)
            {
                Logging.Log("Pressed \"Enter\" key");
                
                // Reset so all entries will be visible again
                foreach (var entry in Settings.entriesText)
                {
                    Settings.entriesShow[entry.Key] = true;
                }

                SelectEntry();

                // Restore if we previously was in the favorite list
                Settings.showFavoriteList = Settings.showFavoriteListLast;

                // https://stackoverflow.com/a/3068797/2028935
                e.SuppressKeyPress = true;
            }

            // DELETE
            if (e.KeyCode == Keys.Delete)
            {
                Logging.Log("Pressed \"Delete\" key");
                Logging.Log("Deleted key ID ["+ entryActive  + "]");

                // Remove the chosen entry, so it does not show duplicates
                Settings.entriesText.Remove(entryActive);
                Settings.entriesImage.Remove(entryActive);
                Settings.entriesImageTransparent.Remove(entryActive);
                Settings.entriesIsFavorite.Remove(entryActive);
                Settings.entriesApplication.Remove(entryActive);
                Settings.entriesOriginal.Remove(entryActive);
                Settings.entriesShow.Remove(entryActive);

                Settings.GetEntryCounter();

                ResetVariables();
                SetupForm(true);
                if(Settings.entryCounter > 0)
                {
                    UpdateHistory("");
                } else
                {
//                    SendKeys.Send("{ESC}");
                    ActionEscape();
                }
                NativeMethods.SetForegroundWindow(this.Handle);

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
                    entryNewestBox = GetNextTopEntry(entryNewestBox,"getNewerEntries");
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
        }


        // ###########################################################################################
        // Action to take when ESCAPE is pressed
        // ###########################################################################################

        public void ActionEscape ()
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

            // Restore if we previously was in the favorite list
            Settings.showFavoriteList = Settings.showFavoriteListLast;
        }


        // ###########################################################################################
        // Find the next page.
        // Fully done by ChatGPT :-)
        // ###########################################################################################

        private int GetNextTopEntry (int currentIdTop, string direction)
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
