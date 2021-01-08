using HovText.Properties;
using NHotkey.WindowsForms; // https://github.com/thomaslevesque/NHotkey
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

// ----------------------------------------------------------------------------
// Upload application to these places:
// https://www.microsoft.com/en-us/wdsi/filesubmission
// ----------------------------------------------------------------------------

// NuGet: "Costura.Fody" to merge the DLLs in to the EXE - to get only one EXE file for this application
// Incredible cool and simple compared to the other complex stuff I have seen! :-)
// https://stackoverflow.com/a/40786196/2028935

namespace HovText
{

    public partial class Settings : Form
    {
        // ###########################################################################################
        // Define "Settings" class variables - real spaghetti :-)
        // ###########################################################################################

        // History, default values
        public static string historyFontFamily = "Segoe UI";
        public static float historyFontSize = 11;
        public static int historyListElements = 8;
        private readonly static int[] historyElementsWidth = { 30, 30, 30, 35, 35, 35, 40, 40, 40, 40, 40, 40, 40, 40, 40, 40, 40, 40, 40, 40, 40, 40, 40, 40, 40, 40, 40, 40, 40, 40 }; // percentage (10-95%)
        private readonly static int[] historyElementsHeight = { 25, 30, 35, 40, 45, 50, 60, 70, 80, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 }; // percentage (10-95%)
        public static int historySizeWidth = historyElementsWidth[historyListElements - 1]; // percentage (10-95%)
        public static int historySizeHeight = historyElementsHeight[historyListElements - 1]; // percentage (10-95%)
        public static string historyColor = "Yellow";
        public static Dictionary<string, string> historyColorsTop = new Dictionary<string, string>() {
            { "Blue", "#dae1e7" },
            { "Brown", "#dac1a0" },
            { "Green", "#c1dac1" },
            { "White", "#e7e1c8" }, // not selectable but was a part of a previous release
            { "Yellow", "#e7e1c8" },
            { "Custom", registryHistoryColorCustomTop },
        };
        public static Dictionary<string, string> historyColorsBottom = new Dictionary<string, string>() {
            { "Blue", "#f5faff" },
            { "Brown", "#eee3d5" },
            { "Green", "#eaf2ea" },
            { "White", "#fff8dc" }, // not selectable but was a part of a previous release
            { "Yellow", "#fff8dc" },
            { "Custom", registryHistoryColorCustomBottom },
        };
        public static Dictionary<string, string> historyColorsText = new Dictionary<string, string>() {
            { "Blue", "#000000" },
            { "Brown", "#000000" },
            { "Green", "#000000" },
            { "White", "#000000" },
            { "Yellow", "#000000" },
            { "Custom", registryHistoryColorCustomText },
        };
        public static Dictionary<string, string> historyColorsBorder = new Dictionary<string, string>() {
            { "Blue", "#ff0000" },
            { "Brown", "#ff0000" },
            { "Green", "#ff0000" },
            { "White", "#ff0000" },
            { "Yellow", "#ff0000" },
            { "Custom", registryHistoryColorCustomBorder },
        };
        public static string historyLocation = "Right Bottom";
        public static bool historyBorder = true;

        // Registry, default values
        public readonly static string registryPath = "SOFTWARE\\HovText";
        private const string registryHotkeyToggleApplication = "Control + Oem5"; // hotkey "Toggle application on/off"
        private const string registryHotkeyGetOlderEntry = "Alt + H"; // hotkey "Show older entry"
        private const string registryHotkeyGetNewerEntry = "Shift + Alt + H"; // hotkey "Show newer entry"
        private const string registryHotkeyPasteOnHotkey = "Alt + O"; // hotkey "Paste on hotkey"
        private const string registryCheckUpdates = "1"; // 1 = check for updates
        private const string registryHotkeyBehaviour = "System"; // use system clipboard
        private const string registryCloseMinimizes = "1"; // 1 = minimize to tray
        private const string registryRestoreOriginal = "1"; // 1 = restore original
        private const string registryCopyImages = "1"; // 1 = copy images to history
        private const string registryEnableHistory = "1"; // 1 = enable history
        private const string registryPasteOnSelection = "0"; // 0 = do not paste selected entry when selected
        private const string registryTrimWhitespaces = "1"; // 1 = trim whitespaces
        private const string registryHistoryColorCustomTop = "#000000";
        private const string registryHistoryColorCustomBottom = "#555555";
        private const string registryHistoryColorCustomText = "#ffffff";
        private const string registryHistoryColorCustomBorder = "#ffffff";
        private const string registryHistoryBorder = "1";

        // UI elements
        public static bool isEnabledHistory;
        public static bool isEnabledPasteOnSelection;
        public static bool isEnabledTrimWhitespacing;
        public static bool isRestoreOriginal;
        public static bool isCopyImages;
        public static bool isCloseMinimizes;
        public static bool isClosedFromNotifyIcon;
        public static bool isHistoryHotkeyPressed;

        // Clipboard
        public static int entryIndex = -1;
        public static int entryCounter = -1;
        bool isClipboardText;
        bool isClipboardImage;
        string clipboardText = "";
        string clipboardTextLast = "";
        bool isClipboardImageTransparent;
        Image clipboardImage;
        string clipboardImageHashLast = "";
        IDataObject clipboardObject;
        public readonly static SortedDictionary<int, string> entriesApplication = new SortedDictionary<int, string>();
        public readonly static SortedDictionary<int, string> entriesText = new SortedDictionary<int, string>();
        public readonly static SortedDictionary<int, Image> entriesImage = new SortedDictionary<int, Image>();
        private readonly static SortedList<int, Dictionary<string, object>> entriesOriginal = new SortedList<int, Dictionary<string, object>>();
        public readonly static SortedDictionary<int, bool> entriesImageTransparent = new SortedDictionary<int, bool>();
        const int WM_CLIPBOARDUPDATE = 0x031D;
        string whoUpdatedClipboardName = "";
        public static bool pasteOnHotkeySetCleartext;

        // Misc
        private static IntPtr originatingHandle = IntPtr.Zero;
        private static bool isFirstCallAfterHotkey = true;
        public static bool isSettingsFormVisible;
        public static string hovtextPage = "https://hovtext.com/";
        public static string appDate = "";
        public static string appType = "";
        internal static Settings settings;
        readonly History history = new History();
        readonly Update update = new Update();
        readonly PasteOnHotkey pasteOnHotkey = new PasteOnHotkey();
        readonly HotkeyConflict hotkeyConflict = new HotkeyConflict();


        // ###########################################################################################
        // Main
        // ###########################################################################################

        public Settings()
        {
            // Refering to the current form - used in the history form
            settings = this;

            InitializeComponent();

            // Catch repaint event for this specific element (to draw the border)
            uiShowFontBottom.Paint += new System.Windows.Forms.PaintEventHandler(this.uiShowFontBottom_Paint);

            // Get information from assembly
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo verInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            int verTypeInt = verInfo.ProductPrivatePart;
            appType = verTypeInt == 1 ? "# DEVELOPMENT" : appType;
            appType = verTypeInt == 2 ? "# TEST" : appType;
            if (verTypeInt == 2)
            {
                timerTest.Enabled = true;
            }

            // Get assembly version
            Assembly assemblyInfo = Assembly.GetExecutingAssembly();
            string assemblyVersion = FileVersionInfo.GetVersionInfo(assemblyInfo.Location).FileVersion;
            string year = assemblyVersion.Substring(0, 4);
            string month = assemblyVersion.Substring(5, 2);
            string day = assemblyVersion.Substring(8, 2);
            switch (month)
            {
                case "01": month = "January"; break;
                case "02": month = "February"; break;
                case "03": month = "March"; break;
                case "04": month = "April"; break;
                case "05": month = "May"; break;
                case "06": month = "June"; break;
                case "07": month = "July"; break;
                case "08": month = "August"; break;
                case "09": month = "September"; break;
                case "10": month = "October"; break;
                case "11": month = "November"; break;
                case "12": month = "December"; break;
                default: month = "Unknown"; break;
            }
            day = day.TrimStart(new Char[] { '0' }); // remove leading zero
            day = day.TrimEnd(new Char[] { '.' }); // remove last dot
            string date = year + "-" + month + "-" + day;
            appDate = date;
            appVer.Text = "Version " + appDate + " " + appType;

            // Initialize registry and get its values for the various checkboxes
            InitializeRegistry();
            GetStartupSettings();

            Program.AddClipboardFormatListener(this.Handle);

            // Write text for the "About" page

            // Basic RTF formatting
            // "test \b text \b0 is good" for bold
            // "test \i text \i0 is good" for italic
            // "test \ul text\ulnone  is good" for underline
            var sb = new StringBuilder();
            sb.Append(@"{\rtf1\ansi");
            sb.Append(@" This application is open source and you can use it on any computer you want without any cost. You are of course encouraged to donate some $$$, if you use it and want to motivate for continues support :-) It is \ul not\ulnone  allowed to sell or distribute it in any commercial regard without written consent from the developer. \line ");
            sb.Append(@" \line ");
            sb.Append(@" Visit the HovText home page where you also can contact the developers: \line ");
            sb.Append(@" https://hovtext.com/ \line ");
            sb.Append(@" \line ");
            sb.Append(@" Kind regards, ");
            sb.Append(@" \line ");
            sb.Append(@" \line ");
            sb.Append(@" Jesper and Dennis ");
            sb.Append(@"}");
            aboutBox.Rtf = sb.ToString();

            // Set the initial text on the tray icon
            UpdateNotifyIconText();
        }


        // ###########################################################################################
        // Called when clipboard is changed
        // ###########################################################################################

        protected override void WndProc(ref Message m)
        {
            // Clipboard chain
            switch (m.Msg)
            {
                case WM_CLIPBOARDUPDATE:

                    // Get the application name which updated the clipboard
                    IntPtr whoUpdatedClipboardHwnd = Program.GetClipboardOwner();
                    Program.GetWindowThreadProcessId(whoUpdatedClipboardHwnd, out uint thisProcessId);
                    whoUpdatedClipboardName = Process.GetProcessById((int)thisProcessId).ProcessName;
                    Debug.WriteLine("Clipboard UPDATE event from [" + whoUpdatedClipboardName + "]");

                    // I am not sure why some(?) applications are returned as "Idle" or "svchost" when coming from clipboard - in this case the get the active application and use that name instead
                    // This could potentially be a problem, if a process is correctly called "Idle" but not sure if this is realistic?
                    if (whoUpdatedClipboardName.ToLower() == "idle")
                    {
                        string activeProcessName = GetActiveApplication();
                        whoUpdatedClipboardName = activeProcessName;
                        Debug.WriteLine("Finding process name the secondary way, [" + activeProcessName + "]");
                    }

                    // Check if application is enabled
                    if (uiAppEnabled.Checked)
                    {
                        if (whoUpdatedClipboardName != "HovText")
                        {
                            ProcessClipboard();
                        }
                    }
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }
                        

        // ###########################################################################################
        // Process the clipboard
        // ###########################################################################################

        private void ProcessClipboard()
        {
            // Check which text formats are available
            isClipboardText = Clipboard.ContainsText();
            isClipboardImage = Clipboard.ContainsImage();

            // Get clipboard content
            clipboardText = isClipboardText ? Clipboard.GetText() : "";
            clipboardImage = isClipboardImage ? Clipboard.GetImage() : null;
            clipboardObject = Clipboard.GetDataObject();

            // Is clipboard text - also triggers when copying whitespaces only
            if (isClipboardText)
            {
                // Check if there are any formatted text entries in the clipboard
                bool isFormatted = false;
                foreach (var format in clipboardObject.GetFormats(false))
                {
                    // If the format is anything but "Text" then we can break and deem this as formatted
                    if (format.ToLower() != "text")
                    {
                        isFormatted = true;
                        break;
                    }
                }

                // Check if number of data formats equals 1 and this is unicode or if clipboard is different than last time
                if (clipboardText != clipboardTextLast || isFormatted)
                    {
                    // Trim the text
                    if (isEnabledTrimWhitespacing)
                    {
                        clipboardText = clipboardText.Trim();
                    }

                    if (!string.IsNullOrEmpty(clipboardText))
                    {
                        // Set the last clipboard text to be identical to this one
                        clipboardTextLast = clipboardText;

                        // Add text to the entries array and update the clipboard
                        AddEntry();
                        GetEntryCounter();
                        SetClipboard();
                        if(uiHotkeyBehaviourPaste.Checked)
                        {
                            RestoreOriginal(entryIndex);
                        }
                    }
                }
            }
            else
            if (isClipboardImage) // Is clipboard an image
            {
                // Only proceed if we should copy the images also
                if (isCopyImages)
                {
                    // Get hash value of picture in clipboard
                    ImageConverter converter = new ImageConverter();
                    byte[] byteArray = (byte[])converter.ConvertTo(clipboardImage, typeof(byte[]));
                    string clipboardImageHash = Convert.ToBase64String(byteArray);

                    if (clipboardImageHash != clipboardImageHashLast)
                    {
                        // Set the last clipboard image to be identical to this one
                        clipboardImageHashLast = clipboardImageHash;

                        // Check if picture is transparent
                        Image imgCopy = GetImageFromClipboard();
                        isClipboardImageTransparent = IsImageTransparent(imgCopy);

                        // Add the image to the entries array and update the clipboard
                        AddEntry();
                        GetEntryCounter();
                        SetClipboard();
                    }
                }
            }
            else
            // We have come in to here, if clipboard has changed but it does not contains a text or an image
            // It could be empty or e.g. a file copy

            if (string.IsNullOrEmpty(Clipboard.GetText()))
            {
                // If active application is EXCEL then restore last entry, as EXCEL clears clipboard when 
                if (entriesText.Count > 0 && whoUpdatedClipboardName.ToLower() == "excel")
                {
                    // Restore the last entry to the clipboard
                    SetClipboard();
                    clipboardTextLast = Clipboard.GetText();
                }
            }
        }


        // ###########################################################################################
        // Get the image from clipboard
        // https://github.com/skoshy/CopyTransparentImages/blob/304e383b8f3239496999087421545a9b4dc765e5/ConsoleApp2/Program.cs#L58
        // It will also get the transparency alpha channel from the clipboard image
        // ###########################################################################################

        private Image GetImageFromClipboard()
        {
            if (Clipboard.GetDataObject() == null) return null;
            if (Clipboard.GetDataObject().GetDataPresent(DataFormats.Dib))
            {
                var dib = ((System.IO.MemoryStream)Clipboard.GetData(DataFormats.Dib)).ToArray();
                var width = BitConverter.ToInt32(dib, 4);
                var height = BitConverter.ToInt32(dib, 8);
                var bpp = BitConverter.ToInt16(dib, 14);
                if (bpp == 32)
                {
                    var gch = GCHandle.Alloc(dib, GCHandleType.Pinned);
                    Bitmap bmp = null;
                    try
                    {
                        var ptr = new IntPtr((long)gch.AddrOfPinnedObject() + 40);
                        bmp = new Bitmap(width, height, width * 4, System.Drawing.Imaging.PixelFormat.Format32bppArgb, ptr);
                        return new Bitmap(bmp);
                    }
                    finally
                    {
                        gch.Free();
                        if (bmp != null) bmp.Dispose();
                    }
                }
            }
            return Clipboard.ContainsImage() ? Clipboard.GetImage() : null;
        }


        // ###########################################################################################
        // Check if the image contains any transparency
        // https://stackoverflow.com/a/2570002/2028935
        // ###########################################################################################

        private static bool IsImageTransparent(Image image)
        {
            Bitmap img = new Bitmap(image);
            for (int y = 0; y < img.Height; ++y)
            {
                for (int x = 0; x < img.Width; ++x)
                {
                    if (img.GetPixel(x, y).A != 255)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
 

        // ###########################################################################################
        // Restore the original clipboard (as original as I can get it at least)
        // ###########################################################################################

        public static void RestoreOriginal (int entryIndex)
        {
            try
            {
                // Snippet directly taken from legacy HovText - more or less the only thing? :-)
                // ---
                DataObject data = new DataObject();
                foreach (KeyValuePair<string, object> kvp in entriesOriginal[entryIndex])
                {
                    if (kvp.Value != null)
                    {
                        data.SetData(kvp.Key, kvp.Value);
                    }
                }
                Clipboard.Clear();
                Clipboard.SetDataObject(data, true);
                // ---
            }
            catch (Exception ex)
            {
                MessageBox.Show("EXCEPTION 5 - please report to developer: [" + ex.ToString() + "]");
            }
        }


        // ###########################################################################################
        // Check if clipboard content is already in the data arrays - if so then move the entry to top
        // ###########################################################################################

        private bool IsClipboardContentAlreadyInDataArrays()
        {
            if (entriesText.Count > 0)
            {
                for (int i = 0; i < entriesText.Count; i++)
                {

                    if (isClipboardText)
                    {
                        // If the two data are identical then move the entry to the top
                        if (entriesText.ElementAt(i).Value == clipboardText)
                        {
                            entryIndex = entriesText.ElementAt(i).Key;
                            MoveEntryToTop();
                            return true;
                        }
                    }
                    else
                        if (isClipboardImage)
                    {
                        // Get hash value of picture in clipboard
                        ImageConverter converter = new ImageConverter();
                        byte[] byteArray = (byte[])converter.ConvertTo(clipboardImage, typeof(byte[]));
                        string clipboardImageHash = Convert.ToBase64String(byteArray);

                        // Get hash value of picture in image array
                        converter = new ImageConverter();
                        byteArray = (byte[])converter.ConvertTo(entriesImage.ElementAt(i).Value, typeof(byte[]));
                        string clipboardImageArray = Convert.ToBase64String(byteArray);

                        // If the two data are identical then move the entry to the top
                        if (clipboardImageHash == clipboardImageArray)
                        {
                            entryIndex = entriesText.ElementAt(i).Key;
                            MoveEntryToTop();
                            return true;
                        }
                    }
                }
            }
            return false; // we did not find any identical data
        }


        // ###########################################################################################
        // Add the content from clipboard to the data arrays
        // ###########################################################################################

        private void AddEntry()
        {
            // Clear the dictionaries if we do not catch the history
            if (!isEnabledHistory)
            {
                entryIndex = -1;
                entriesText.Clear();
                entriesImage.Clear();
                entriesImageTransparent.Clear();
                entriesApplication.Clear();
                entriesOriginal.Clear();
            }

            // Proceed if the (cleartext) data is not already in the dictionary
            bool isAlreadyInDataArray = IsClipboardContentAlreadyInDataArrays();
            if (!isAlreadyInDataArray)
            {
                // If this is the first time then set the index to 0
                entryIndex = entryIndex >= 0 ? entriesText.Keys.Last() + 1 : 0;

                // Add the text and image to the entries array
                entriesText.Add(entryIndex, clipboardText);
                entriesImage.Add(entryIndex, clipboardImage);
                entriesImageTransparent.Add(entryIndex, isClipboardImageTransparent);

                // Walk through all (relevant) clipboard object formats and store them.
                // I am not sure how to do this differently as it does not work if I take everything!?
                // This seems to be something that needs constant changes!?
                var formats = clipboardObject.GetFormats(false);
                Dictionary<string, object> clipboardObjects = new Dictionary<string, object>();
                foreach (var format in formats)
                {
                    if (
                        format.Contains("Text") ||
                        format.Contains("HTML") ||
                        format.Contains("Csv") ||
                        format.Contains("Link") ||
                        format.Contains("Hyperlink") ||
                        format.Contains("Bitmap") ||
                        format.Contains("PNG") || // including transparent layer of PNG
                        format.Contains("Recipient") || // Outlook recipient
                        format.Contains("Format17") // picture format
                        )
                    {
                        clipboardObjects.Add(format, clipboardObject.GetData(format));
                    }
                }
                entriesOriginal.Add(entryIndex, clipboardObjects);

                // Add the process name that has updated the clipboard
                if (whoUpdatedClipboardName.Length == 0)
                {
                    whoUpdatedClipboardName = "(unknown)";
                }
                entriesApplication.Add(entryIndex, whoUpdatedClipboardName);
            }

            // Update the entries on the tray icon
            UpdateNotifyIconText();
        }


        // ###########################################################################################
        // Place data in the clipboard based on the entry index
        // ###########################################################################################

        public void SetClipboard()
        {
            string entryText = clipboardText;
            Image entryImage;
            bool isEntryText = isClipboardText;
            bool isEntryImage = isClipboardImage;

            if (isEnabledHistory)
            {
                if (entriesText.Count > 0)
                {
                    entryText = entriesText[entryIndex];
                    entryImage = entriesImage[entryIndex];
                    isEntryText = string.IsNullOrEmpty(entryText) ? false : true;
                    isEntryImage = entryImage == null ? false : true;
                }
            }

            // Put text to the clipboard
            if (isEntryText)
            {
                try
                {
                    if (uiHotkeyBehaviourPaste.Checked && !pasteOnHotkeySetCleartext)
                    {
                        RestoreOriginal(entryIndex);
                    }
                    else
                    {
                        if (uiTrimWhitespaces.Checked)
                        {
                            entryText = entryText.Trim();
                        }
                        Clipboard.Clear();
                        Clipboard.SetText(entryText, TextDataFormat.Text);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("EXCEPTION 1 - please report to developer: [" + ex.ToString() + "]");
                }
            }
            else
            if (isEntryImage) // Put an image to the clipboard
            {
                try
                {
                    RestoreOriginal(entryIndex);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("EXCEPTION 2 - please report to developer: [" + ex.ToString() + "]");
                }
            }
            else
            {
                MessageBox.Show("EXCEPTION 3 - please report to developer");
            }
        }


        // ###########################################################################################
        // Get the next older entry from history
        // ###########################################################################################

        public void GoEntryLowerNumber()
        {
            // Check if application is enabled
            if (uiAppEnabled.Checked && entryCounter > 0)
            {

                if(isFirstCallAfterHotkey)
                {
                    // Hide the "Settings" form if it is visible (it will be restored after key-up)
                    isSettingsFormVisible = this.Visible;
                    if(isSettingsFormVisible)
                    {
                        Hide();
                    }
                    GetActiveApplication();
                    history.SetupForm();
                }
                // Always change focus to HovText to ensure we can catch the key-up event
                ChangeFocusToThisApplication();

                // Only proceed if the entry counter is equal to or more than 0
                if (entryCounter > 0)
                {

                    // Check if this is the first call (we want to show the newest entry at the first keypress)
                    if (!isFirstCallAfterHotkey && entryCounter > 1)
                    {
                        var element = entriesText.ElementAt(entryCounter - 2);
                        entryIndex = element.Key;
                    }
                    isFirstCallAfterHotkey = false;

                    // Show history UI
                    GetEntryCounter();
                    history.UpdateHistory("down");
                }
            }
        }


        // ###########################################################################################
        // Get the next newer entry from history
        // ###########################################################################################

        public void GoEntryHigherNumber()
        {
            // Check if application is enabled
            if (uiAppEnabled.Checked && entryCounter > 0)
            {

                if(isFirstCallAfterHotkey)
                {
                    // Hide the "Settings" form if it is visible (it will be restored after key-up)
                    isSettingsFormVisible = this.Visible;
                    if (isSettingsFormVisible)
                    {
                        Hide();
                    }
                    GetActiveApplication();
                    history.SetupForm();
                }
                // Always change focus to HovText to ensure we can catch the key-up event
                ChangeFocusToThisApplication();

                // Only proceed if the entry counter is less than the total amount of entries
                if (entryCounter <= entriesText.Count)
                {

                    // Check if this is the first call (we want to show the newest entry at the first keypress)
                    if (!isFirstCallAfterHotkey && entryCounter < entriesText.Count)
                    {
                        var element = entriesText.ElementAt(entryCounter);
                        entryIndex = element.Key;
                    }
                    isFirstCallAfterHotkey = false;

                    // Show history UI
                    GetEntryCounter();
                    history.UpdateHistory("up");
                }
            }
        }


        // ###########################################################################################
        // Called when a history entry has been selected in the "HistoryList" form
        // ###########################################################################################

        public void SelectHistoryEntry()
        {
            isHistoryHotkeyPressed = false;

            // Check if application is enabled
            if (uiAppEnabled.Checked && entryCounter > 0)
            {
                MoveEntryToTop();

                pasteOnHotkeySetCleartext = true;

                // Set the clipboard with the new data
                SetClipboard();

                // Restore the original clipboard, if we are within the "Paste on hotkey only" mode
                if (uiHotkeyBehaviourPaste.Checked)
                {
                    PasteOnHotkey.StartTimerToRestoreOriginal();
                }

                // Set focus back to the originating application
                ChangeFocusToOriginatingApplication();

                // Reset some stuff
                isFirstCallAfterHotkey = true;
                entryIndex = entriesText.Keys.Last();
                GetEntryCounter();
            }
        }


        // ###########################################################################################
        // Convert the entry index to which logical number it is
        // ###########################################################################################

        private static void GetEntryCounter()
        {
            entryCounter = 0;

            foreach (var key in entriesText.Keys)
            {
                entryCounter++;
                if (key == entryIndex)
                {
                    break;
                }
            }
        }


        // ###########################################################################################
        // Move the active entry to the top of the data arrays (newest entry)
        // ###########################################################################################

        private void MoveEntryToTop()
        {
            clipboardTextLast = entriesText[entryIndex];

            int lastKey = entriesText.Keys.Last();
            int insertKey = lastKey + 1;

            // Copy the chosen entry to the top of the array lists (so it becomes the newest entry)
            entriesText.Add(insertKey, entriesText[entryIndex]);
            entriesImage.Add(insertKey, entriesImage[entryIndex]);
            entriesImageTransparent.Add(insertKey, entriesImageTransparent[entryIndex]);
            entriesApplication.Add(insertKey, entriesApplication[entryIndex]);
            entriesOriginal.Add(insertKey, entriesOriginal[entryIndex]);

            // Remove the chosen entry, so it does not show duplicates
            entriesText.Remove(entryIndex);
            entriesImage.Remove(entryIndex);
            entriesImageTransparent.Remove(entryIndex);
            entriesApplication.Remove(entryIndex);
            entriesOriginal.Remove(entryIndex);

            // Set the index to be the last one
            entryIndex = entriesText.Keys.Last();
        }


        // ###########################################################################################
        // Called when the "Enable application" checkbox is clicked with mouse or the "Toggle application on/off" hotkey is pressed
        // ###########################################################################################

        public void ToggleEnabled()
        {
            // Toggle the checkbox - this will also fire an "appEnabled_CheckedChanged" event
            uiAppEnabled.Checked = !uiAppEnabled.Checked;
        }


        // ###########################################################################################
        // Event that is triggede when application is toggled, either enabled or disabled
        // ###########################################################################################

        private void uiAppEnabled_CheckedChanged(object sender, EventArgs e)
        {
            // Check if application is enabled
            if (uiAppEnabled.Checked)
            {
                // Add this application to the clipboard chain again
                Program.AddClipboardFormatListener(this.Handle);

                ProcessClipboard();

                string hotkeyBehaviour = GetRegistryKey(registryPath, "HotkeyBehaviour");
                switch (hotkeyBehaviour)
                {
                    case "Paste":
                        uiHotkeyBehaviourPaste.Checked = true;
                        uiHotkeyPaste.Enabled = true;

                        // Change the icons to be the blue one
                        notifyIcon.Icon = Resources.Hotkey;
                        Icon = Resources.Hotkey;

                        break;
                    default:
                        uiHotkeyBehaviourSystem.Checked = true;
                        uiHotkeyPaste.Enabled = false;

                        // Change the icons to be green (active)
                        notifyIcon.Icon = Resources.Active;
                        Icon = Resources.Active;

                        break;
                }

                // Enable other checkboxes
                uiHistoryEnabled.Enabled = true;
                uiRestoreOriginal.Enabled = true;
                uiCopyImages.Enabled = true;
                uiTrimWhitespaces.Enabled = true;
                uiPasteOnSelection.Enabled = true;
                uiHotkeyOlder.Enabled = true;
                uiHotkeyNewer.Enabled = true;
                uiHotkeyBehaviourSystem.Enabled = true;
                uiHotkeyBehaviourPaste.Enabled = true;
            }
            else
            {
                // Remove this application from the clipboard chain
                Program.RemoveClipboardFormatListener(this.Handle);

                // Restore the original clipboard format
                if (isRestoreOriginal && entriesOriginal.Count > 0)
                {
                    RestoreOriginal(entryIndex);
                }

                // Change the icons to be red (inactive)
                notifyIcon.Icon = Resources.Inactive;
                Icon = Resources.Inactive;

                // Disable other checkboxes
                uiHistoryEnabled.Enabled = false;
                uiRestoreOriginal.Enabled = false;
                uiCopyImages.Enabled = false;
                uiTrimWhitespaces.Enabled = false;
                uiPasteOnSelection.Enabled = false;
                uiHotkeyOlder.Enabled = false;
                uiHotkeyNewer.Enabled = false;
                uiHotkeyPaste.Enabled = false;
                uiHotkeyBehaviourSystem.Enabled = false;
                uiHotkeyBehaviourPaste.Enabled = false;
            }
        }


        // ###########################################################################################
        // Called when application is minimized - it will hide the "Settings" form
        // ###########################################################################################

        private void MainWindow_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                ShowTrayNotification();
                Hide();
            }
        }


        // ###########################################################################################
        // Go to HovText web page when link is clicked
        // ###########################################################################################

        private void aboutBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }


        // ###########################################################################################
        // Unregister from the clipboard chain when application is closing down
        // ###########################################################################################

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            // In case windows is trying to shut down, don't hold up the process
            if (e.CloseReason == CloseReason.WindowsShutDown)
            {
                Program.RemoveClipboardFormatListener(this.Handle);
                RemoveAllHotkeys();
                return;
            }

            if (!isCloseMinimizes || isClosedFromNotifyIcon)
            {
                Program.RemoveClipboardFormatListener(this.Handle);
                RemoveAllHotkeys();
                return;
            }

            // Do not close as the X should minimize
            if (WindowState == FormWindowState.Normal)
            {
                ShowTrayNotification();
                Hide();
            }
            e.Cancel = true;
        }


        // ###########################################################################################
        // Get process info for active application (the one where the hotkey is pressed)
        // ###########################################################################################

        private static string GetActiveApplication()
        {
            // Get the active application
            originatingHandle = Program.GetForegroundWindow();

            // Get the process ID and find the name for that ID
            Program.GetWindowThreadProcessId(originatingHandle, out uint processId);
            string appProcessName = Process.GetProcessById((int)processId).ProcessName;
            Debug.WriteLine("Active application is ["+ appProcessName +"]");
            return appProcessName;
        }


        // ###########################################################################################
        // When pressing one of the history hotkeys then change focus to this application to prevent the keypresses go in to the active application
        // ###########################################################################################

        private void ChangeFocusToThisApplication()
        {
            Program.SetForegroundWindow(this.Handle);
            Debug.WriteLine("Set focus to HovText");
        }


        // ###########################################################################################
        // When an entry has been submitted to the clipboard then pass back focus to the originating application
        // ###########################################################################################

        public static void ChangeFocusToOriginatingApplication()
        {
            Program.SetForegroundWindow(originatingHandle);
            Debug.WriteLine("Set focus to originating application");
        }


        // ###########################################################################################
        // Check for HovText updates online
        // ###########################################################################################

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            updateTimer.Enabled = false;
            try
            {
                WebClient client = new WebClient();
                client.Headers.Add("user-agent", ("HovText "+ appDate +" "+ appType).Trim());
                string checkedVersion = client.DownloadString(hovtextPage +"autoupdate/");
                if (checkedVersion.Substring(0,7) == "Version")
                {
                    checkedVersion = checkedVersion.Substring(9);
                    update.uiAppVerYours.Text = appDate;
                    update.uiAppVerOnline.Text = checkedVersion;
                    string lastCheckedVersion = GetRegistryKey(registryPath, "CheckedVersion");
                    if (lastCheckedVersion != checkedVersion && checkedVersion != appDate)
                    {
                        update.Show();
                        update.Activate();
                    }
                }
            }
            catch (WebException ex)
            {
                Debug.WriteLine("No communication to HovText update checker page");
                Debug.WriteLine(ex.ToString());
            }
        }


        // ###########################################################################################
        // Initialize registry - check if all registry keys have been created
        // ###########################################################################################

        private void InitializeRegistry()
        {
            // Check if the "HovText" path exists in SOFTWARE registry - if not then create it
            using (RegistryKey registryPathExists = Registry.CurrentUser.OpenSubKey(registryPath))
            {
                if (registryPathExists == null)
                {
                    Registry.CurrentUser.CreateSubKey(registryPath);
                }
            }

            // Check if the following registry keys exists - if not then create them with their default values

            // General
            RegistryCheckOrCreate("CheckUpdates", registryCheckUpdates); 
            RegistryCheckOrCreate("CheckedVersion", appDate);
            RegistryCheckOrCreate("CloseMinimizes", registryCloseMinimizes);
            RegistryCheckOrCreate("RestoreOriginal", registryRestoreOriginal);
            RegistryCheckOrCreate("HistoryEnable", registryEnableHistory);
            RegistryCheckOrCreate("CopyImages", registryCopyImages);
            RegistryCheckOrCreate("PasteOnSelection", registryPasteOnSelection);
            RegistryCheckOrCreate("TrimWhitespaces", registryTrimWhitespaces);

            // Hotkeys
            RegistryCheckOrCreate("HotkeyBehaviour", registryHotkeyBehaviour);
            RegistryCheckOrCreate("Hotkey1", registryHotkeyToggleApplication);
            RegistryCheckOrCreate("Hotkey2", registryHotkeyGetOlderEntry);
            RegistryCheckOrCreate("Hotkey3", registryHotkeyGetNewerEntry);
            RegistryCheckOrCreate("Hotkey4", registryHotkeyPasteOnHotkey);

            // Layout
            RegistryCheckOrCreate("HistoryEntries", historyListElements.ToString());
            RegistryCheckOrCreate("HistorySizeWidth", historySizeWidth.ToString());
            RegistryCheckOrCreate("HistorySizeHeight", historySizeHeight.ToString());
            RegistryCheckOrCreate("HistoryLocation", historyLocation);

            // Style
            RegistryCheckOrCreate("HistoryFontFamily", historyFontFamily);
            RegistryCheckOrCreate("HistoryFontSize", historyFontSize.ToString());
            RegistryCheckOrCreate("HistoryActiveBorder", registryHistoryBorder);
            RegistryCheckOrCreate("HistoryColor", historyColor);
            RegistryCheckOrCreate("HistoryColorCustomTop", registryHistoryColorCustomTop);
            RegistryCheckOrCreate("HistoryColorCustomBottom", registryHistoryColorCustomBottom);
            RegistryCheckOrCreate("HistoryColorCustomText", registryHistoryColorCustomText);
            RegistryCheckOrCreate("HistoryColorCustomBorder", registryHistoryColorCustomBorder);

            // Misc
            RegistryCheckOrCreate("NotificationShown", "0");
        }


        // ###########################################################################################
        // Check if the registry key exists - if not then create it and set default value
        // ###########################################################################################

        private static void RegistryCheckOrCreate(string regKey, string regValue)
        {
            // Check if the registry key is set - if not then set default value
            using (RegistryKey registryPathExists = Registry.CurrentUser.OpenSubKey(registryPath, true))
            {
                // NotificationShown
                if (registryPathExists.GetValue(regKey) == null)
                {
                    registryPathExists.SetValue(regKey, regValue);
                }
            }
        }


        // ###########################################################################################
        // Get regsitry key value
        // ###########################################################################################

        private static string GetRegistryKey(string path, string key)
        {
            using (RegistryKey getKey = Registry.CurrentUser.OpenSubKey(path))
            {
                return (string)(getKey.GetValue(key));
            }
        }


        // ###########################################################################################
        // Set registry key value
        // ###########################################################################################

        public static void SetRegistryKey(string path, string key, string value)
        {
            using (RegistryKey setKey = Registry.CurrentUser.OpenSubKey(path, true))
            {
                setKey.SetValue(key, value);
            }
        }


        // ###########################################################################################
        // Delete registry key
        // ###########################################################################################

        private static void DeleteRegistryKey(string path, string key)
        {
            using (RegistryKey deleteKey = Registry.CurrentUser.OpenSubKey(path, true))
            {
                deleteKey.DeleteValue(key, false);
            }
        }


        // ###########################################################################################
        // Set up everything when launching application
        // ###########################################################################################

        private void GetStartupSettings()
        {
            // ------------------------------------------
            // "General" tab
            // ------------------------------------------

            // Start with Windows
            string getKey = GetRegistryKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", "HovText");
            if (getKey == null)
            {
                uiStartWithWindows.Checked = false;
            }
            else
            {
                uiStartWithWindows.Checked = true;

                // Make sure the legacy HovText does not interfere - overwrite if it does not contain "HovText.exe" or "--start-minimized"
                string runEntry = GetRegistryKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", "HovText");
                if(!runEntry.Contains("HovText.exe") || !runEntry.Contains("--start-minimized"))
                {
                    SetRegistryKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", "HovText", "\"" + Application.ExecutablePath + "\" --start-minimized");
                }
            }

            // Check for updates online
            string checkUpdates = GetRegistryKey(registryPath, "CheckUpdates");
            uiCheckUpdates.Checked = checkUpdates == "1" ? true : false;
            if (uiCheckUpdates.Checked)
            {
                updateTimer.Enabled = true;
            }

            // Restore original when disabling application
            int restoreOriginal = int.Parse((string)GetRegistryKey(registryPath, "RestoreOriginal"));
            uiRestoreOriginal.Checked = restoreOriginal == 1 ? true : false;
            isRestoreOriginal = uiRestoreOriginal.Checked;

            // Do not copy images
            int copyImages = int.Parse((string)GetRegistryKey(registryPath, "CopyImages"));
            uiCopyImages.Checked = copyImages == 1 ? true : false;
            isCopyImages = uiCopyImages.Checked;

            // Close minimizes application to tray
            int closeMinimizes = int.Parse((string)GetRegistryKey(registryPath, "CloseMinimizes"));
            uiCloseMinimize.Checked = closeMinimizes == 1 ? true : false;
            isCloseMinimizes = uiCloseMinimize.Checked;
            if (isCloseMinimizes)
            {
                MinimizeBox = false;
            }
            else
            {
                MinimizeBox = true;
            }

            // Enable history
            int historyEnabled = int.Parse((string)GetRegistryKey(registryPath, "HistoryEnable"));
            uiHistoryEnabled.Checked = historyEnabled == 1 ? true : false;
            isEnabledHistory = uiHistoryEnabled.Checked;
            if (isEnabledHistory)
            {
                uiHotkeyOlder.Enabled = true;
                uiHotkeyNewer.Enabled = true;
                uiPasteOnSelection.Enabled = true;
            }
            else
            {
                uiHotkeyOlder.Enabled = false;
                uiHotkeyNewer.Enabled = false;
                uiPasteOnSelection.Enabled = false;
            }

            // Paste on history selection
            int pasteOnSelection = int.Parse((string)GetRegistryKey(registryPath, "PasteOnSelection"));
            uiPasteOnSelection.Checked = pasteOnSelection == 1 ? true : false;
            isEnabledPasteOnSelection = uiPasteOnSelection.Checked;

            // Trim whitespaces
            int trimWhitespaces = int.Parse((string)GetRegistryKey(registryPath, "TrimWhitespaces"));
            uiTrimWhitespaces.Checked = trimWhitespaces == 1 ? true : false;
            isEnabledTrimWhitespacing = uiTrimWhitespaces.Checked;

            // ------------------------------------------
            // "Hotkeys" tab
            // ------------------------------------------

            // Hotkey behaviour
            string hotkeyBehaviour = GetRegistryKey(registryPath, "HotkeyBehaviour");
            switch (hotkeyBehaviour)
            {
                case "Paste":
                    uiHotkeyBehaviourPaste.Checked = true;
                    uiHotkeyPaste.Enabled = true;
                    break;
                default:
                    uiHotkeyBehaviourSystem.Checked = true;
                    uiHotkeyPaste.Enabled = false;
                    break;
            }

            // Hotkeys
            string hotkey1 = GetRegistryKey(registryPath, "Hotkey1");
            string hotkey2 = GetRegistryKey(registryPath, "Hotkey2");
            string hotkey3 = GetRegistryKey(registryPath, "Hotkey3");
            string hotkey4 = GetRegistryKey(registryPath, "Hotkey4");
            hotkey1 = hotkey1.Length == 0 ? "Not set" : hotkey1;
            hotkey2 = hotkey2.Length == 0 ? "Not set" : hotkey2;
            hotkey3 = hotkey3.Length == 0 ? "Not set" : hotkey3;
            hotkey4 = hotkey4.Length == 0 ? "Not set" : hotkey4;
            uiHotkeyEnable.Text = hotkey1;
            uiHotkeyOlder.Text = hotkey2;
            uiHotkeyNewer.Text = hotkey3;
            uiHotkeyPaste.Text = hotkey4;
            SetHotkeys("Startup of application");

            // ------------------------------------------
            // "Apperance" tab
            // ------------------------------------------

            // History entries
            historyListElements = Int32.Parse(GetRegistryKey(registryPath, "HistoryEntries"));
            uiHistoryElements.Value = historyListElements;
            labelHistoryElements.Text = historyListElements.ToString();

            // History area size
            historySizeWidth = Int32.Parse(GetRegistryKey(registryPath, "HistorySizeWidth"));
            historySizeHeight = Int32.Parse(GetRegistryKey(registryPath, "HistorySizeHeight"));
            uiHistorySizeWidth.Value = historySizeWidth;
            uiHistorySizeHeight.Value = historySizeHeight;
            labelHistorySizeWidth.Text = historySizeWidth.ToString() + "%";
            labelHistorySizeHeight.Text = historySizeHeight.ToString() + "%";

            // History active border
            int activeBorder = Int32.Parse(GetRegistryKey(registryPath, "HistoryActiveBorder"));
            if (activeBorder == 1)
            {
                uiHistoryBorder.Checked = true;
            }
            if (historyListElements > 1)
            {
                uiHistoryBorder.Enabled = true;
            }
            else
            {
                uiHistoryBorder.Enabled = false;
            }

            // History color theme
                historyColor = GetRegistryKey(registryPath, "HistoryColor");
            historyColorsTop["Custom"] = GetRegistryKey(registryPath, "HistoryColorCustomTop");
            historyColorsBottom["Custom"] = GetRegistryKey(registryPath, "HistoryColorCustomBottom");
            historyColorsText["Custom"] = GetRegistryKey(registryPath, "HistoryColorCustomText");
            historyColorsBorder["Custom"] = GetRegistryKey(registryPath, "HistoryColorCustomBorder");
            EnableDisableCustomColor();
            switch (historyColor)
            {
                case "Blue":
                    uiHistoryColorBlue.Checked = true;
                    break;
                case "Brown":
                    uiHistoryColorBrown.Checked = true;
                    break;
                case "Green":
                    uiHistoryColorGreen.Checked = true;
                    break;
                case "Custom":
                    uiHistoryColorCustom.Checked = true;
                    EnableDisableCustomColor();
                    break;
                default: // Yellow
                    uiHistoryColorYellow.Checked = true;
                    break;
            }
            SetHistoryColors();

            // History location
            historyLocation = GetRegistryKey(registryPath, "HistoryLocation");
            switch (historyLocation)
            {
                case "Left Top":
                    uiHistoryLocationRadioLeftTop.Checked = true;
                    break;
                case "Left Bottom":
                    uiHistoryLocationRadioLeftBottom.Checked = true;
                    break;
                case "Center":
                    uiHistoryLocationRadioCenter.Checked = true;
                    break;
                case "Right Top":
                    uiHistoryLocationRadioRightTop.Checked = true;
                    break;
                default: // Right Bottom
                    uiHistoryLocationRadioRightBottom.Checked = true;
                    break;
            }

            // History font
            historyFontFamily = GetRegistryKey(registryPath, "HistoryFontFamily");
            historyFontSize = float.Parse((string)GetRegistryKey(registryPath, "HistoryFontSize"));
            uiShowFontTop.Font = new Font(historyFontFamily, historyFontSize);
            uiShowFontBottom.Font = new Font(historyFontFamily, historyFontSize);
            uiShowFontBottom.Text = historyFontFamily + ", " + historyFontSize;
            SetHistoryColors();
        }


        // ###########################################################################################
        // Enable or disable that the application starts up with Windows
        // https://www.fluxbytes.com/csharp/start-application-at-windows-startup/
        // ###########################################################################################

        private void uiStartWithWindows_CheckedChanged(object sender, EventArgs e)
        {
            if (uiStartWithWindows.Checked)
            {
                SetRegistryKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", "HovText", "\"" + Application.ExecutablePath + "\" --start-minimized");
            }
            else
            {
                DeleteRegistryKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", "HovText");
            }
        }


        // ###########################################################################################
        // Show the history font dialouge
        // ###########################################################################################

        private void uiChangeFont_Click(object sender, EventArgs e)
        {
            FontDialog fontDlg = new FontDialog();
            fontDlg.Font = uiShowFontBottom.Font; // initialize the font dialouge with the font from "uiShowFont"
            fontDlg.AllowVerticalFonts = false;
            fontDlg.FontMustExist = true;
            fontDlg.ShowColor = false;
            fontDlg.ShowApply = false;
            fontDlg.ShowEffects = false;
            fontDlg.ShowHelp = false;
            if (fontDlg.ShowDialog() != DialogResult.Cancel)
            {
                historyFontSize = fontDlg.Font.Size;
                historyFontFamily = fontDlg.Font.Name;
                uiShowFontTop.Font = new Font(historyFontFamily, historyFontSize);
                uiShowFontBottom.Text = historyFontFamily + ", " + historyFontSize;
                uiShowFontBottom.Font = new Font(historyFontFamily, historyFontSize);
                SetRegistryKey(registryPath, "HistoryFontFamily", historyFontFamily);
                SetRegistryKey(registryPath, "HistoryFontSize", historyFontSize.ToString());
            }
        }


        // ###########################################################################################
        // Changes in the number of visible history elements
        // ###########################################################################################

        private void historyElements_Scroll(object sender, EventArgs e)
        {
            labelHistoryElements.Text = uiHistoryElements.Value.ToString();
            historyListElements = uiHistoryElements.Value;
            SetRegistryKey(registryPath, "HistoryEntries", historyListElements.ToString());

            // Set the global width and height in percentage
            historySizeWidth = historyElementsWidth[historyListElements - 1];
            historySizeHeight = historyElementsHeight[historyListElements - 1];

            // Update the width and height sliders
            uiHistorySizeWidth.Value = historyElementsWidth[historyListElements - 1];
            uiHistorySizeHeight.Value = historyElementsHeight[historyListElements - 1];
            labelHistorySizeWidth.Text = historyElementsWidth[historyListElements - 1].ToString() + "%";
            labelHistorySizeHeight.Text = historyElementsHeight[historyListElements - 1].ToString() + "%";

            if (uiHistoryElements.Value > 1)
            {
                uiHistoryBorder.Enabled = true;
                EnableDisableCustomColor();
            }
            else
            {
                uiHistoryBorder.Enabled = false;
                EnableDisableCustomColor();
            }
        }


        // ###########################################################################################
        // Changes in the history (area) size
        // ###########################################################################################

        private void historySizeWidth_ValueChanged(object sender, EventArgs e)
        {
            labelHistorySizeWidth.Text = uiHistorySizeWidth.Value.ToString() + "%";
            historySizeWidth = uiHistorySizeWidth.Value;
            SetRegistryKey(registryPath, "HistorySizeWidth", historySizeWidth.ToString());
        }

        private void historySizeHeight_ValueChanged(object sender, EventArgs e)
        {
            labelHistorySizeHeight.Text = uiHistorySizeHeight.Value.ToString() + "%";
            historySizeHeight = uiHistorySizeHeight.Value;
            SetRegistryKey(registryPath, "HistorySizeHeight", historySizeHeight.ToString());
        }


        // ###########################################################################################
        // Change in the history location
        // ###########################################################################################

        private void uiHistoryLocationRadioLeftTop_CheckedChanged(object sender, EventArgs e)
        {
            if(uiHistoryLocationRadioLeftTop.Checked)
            {
                historyLocation = "Left Top";
                SetRegistryKey(registryPath, "HistoryLocation", historyLocation);
            }
        }

        private void uiHistoryLocationRadioLeftBottom_CheckedChanged(object sender, EventArgs e)
        {
            if(uiHistoryLocationRadioLeftBottom.Checked)
            {
                historyLocation = "Left Bottom";
                SetRegistryKey(registryPath, "HistoryLocation", historyLocation);
            }
        }

        private void uiHistoryLocationRadioCenter_CheckedChanged(object sender, EventArgs e)
        {
            if(uiHistoryLocationRadioCenter.Checked)
            {
                historyLocation = "Center";
                SetRegistryKey(registryPath, "HistoryLocation", historyLocation);
            }
        }

        private void uiHistoryLocationRadioRightTop_CheckedChanged(object sender, EventArgs e)
        {
            if(uiHistoryLocationRadioRightTop.Checked)
            {
                historyLocation = "Right Top";
                SetRegistryKey(registryPath, "HistoryLocation", historyLocation);
            }
        }

        private void uiHistoryLocationRadioRightBottom_CheckedChanged(object sender, EventArgs e)
        {
            if (uiHistoryLocationRadioRightBottom.Checked)
            {
                historyLocation = "Right Bottom";
                SetRegistryKey(registryPath, "HistoryLocation", historyLocation);
            }
        }


        // ###########################################################################################
        // Enable or disable the custom color
        // ###########################################################################################

        private void EnableDisableCustomColor()
        {
            // Check if the "Custom" color is checked
            if (uiHistoryColorCustom.Checked)
            {
                uiCustomTop.Enabled = true;
                uiCustomBottom.Enabled = true;
                uiCustomText.Enabled = true;
                groupBoxCustom.Enabled = true;
                labelCustomTop.Enabled = true;
                labelCustomBottom.Enabled = true;
                labelCustomText.Enabled = true;

                uiCustomTop.BackColor = ColorTranslator.FromHtml(historyColorsTop["Custom"]);
                uiCustomBottom.BackColor = ColorTranslator.FromHtml(historyColorsBottom["Custom"]);
                uiCustomText.BackColor = ColorTranslator.FromHtml(historyColorsText["Custom"]);
                uiCustomBorder.BackColor = ColorTranslator.FromHtml(historyColorsBorder["Custom"]);

                // Check if the "History border" checkbox is checked or if the element is enabled
                if (uiHistoryBorder.Checked && uiHistoryBorder.Enabled)
                {
                    uiCustomBorder.Enabled = true;
                    labelCustomBorder.Enabled = true;
                    uiCustomBorder.BackColor = ColorTranslator.FromHtml(historyColorsBorder["Custom"]);
                    historyBorder = true;
                }
                else
                {
                    uiCustomBorder.Enabled = false;
                    labelCustomBorder.Enabled = false;
                    uiCustomBorder.BackColor = Color.WhiteSmoke;
                    historyBorder = false;
                }

            }
            else
            {
                uiCustomTop.Enabled = false;
                uiCustomBottom.Enabled = false;
                uiCustomText.Enabled = false;
                uiCustomBorder.Enabled = false;
                groupBoxCustom.Enabled = false;
                labelCustomTop.Enabled = false;
                labelCustomBottom.Enabled = false;
                labelCustomText.Enabled = false;
                labelCustomBorder.Enabled = false;

                uiCustomTop.BackColor = Color.WhiteSmoke;
                uiCustomBottom.BackColor = Color.WhiteSmoke;
                uiCustomText.BackColor = Color.WhiteSmoke;
                uiCustomBorder.BackColor = Color.WhiteSmoke;

                // Check if the "History border" checkbox is checked or if the element is enabled
                if (uiHistoryBorder.Checked && uiHistoryBorder.Enabled)
                {
                    historyBorder = true;
                }
                else
                {
                    historyBorder = false;
                }
            }

        }


        // ###########################################################################################
        // Change in the history color
        // ###########################################################################################

        private void uiHistoryColor_CheckedChanged(object sender, EventArgs e)
        {
            // Get the text name for the clicked radio item
            historyColor = (sender as RadioButton).Text;

            SetRegistryKey(registryPath, "HistoryColor", historyColor);
            SetHistoryColors();

            // Do something special for the custom
            if (historyColor == "Custom")
            {
                EnableDisableCustomColor();
            }
        }


        // ###########################################################################################
        // Set the history colors (and selected value in UI)
        // ###########################################################################################

        private void SetHistoryColors()
        {
            uiShowFontTop.BackColor = ColorTranslator.FromHtml(historyColorsTop[historyColor]);
            uiShowFontTop.ForeColor = ColorTranslator.FromHtml(historyColorsText[historyColor]);
            uiShowFontBottom.BackColor = ColorTranslator.FromHtml(historyColorsBottom[historyColor]);
            uiShowFontBottom.ForeColor = ColorTranslator.FromHtml(historyColorsText[historyColor]);
        }


        // ###########################################################################################
        // Changes in "Check for updates online"
        // ###########################################################################################

        private void uiCheckUpdates_CheckedChanged(object sender, EventArgs e)
        {
            string status = uiCheckUpdates.Checked ? "1" : "0";
            SetRegistryKey(registryPath, "CheckUpdates", status);
        }
        
        
        // ###########################################################################################
        // Changes in "Close minimizes application to tray"
        // ###########################################################################################

        private void uiCloseMinimize_CheckedChanged(object sender, EventArgs e)
        {
            string status = uiCloseMinimize.Checked ? "1" : "0";
            isCloseMinimizes = uiCloseMinimize.Checked;
            SetRegistryKey(registryPath, "CloseMinimizes", status);
            if (isCloseMinimizes)
            {
                MinimizeBox = false;
            }
            else
            {
                MinimizeBox = true;
            }
        }


        // ###########################################################################################
        // Changes in "Restore original formatting when disabling application"
        // ###########################################################################################

        private void uiRestoreOriginal_CheckedChanged(object sender, EventArgs e)
        {
            // History enabled
            string status = uiRestoreOriginal.Checked ? "1" : "0";
            isRestoreOriginal = uiRestoreOriginal.Checked;
            SetRegistryKey(registryPath, "RestoreOriginal", status);
        }


        // ###########################################################################################
        // Changes in "Do not copy images"
        // ###########################################################################################

        private void uiCopyImages_CheckedChanged(object sender, EventArgs e)
        {
            string status = uiCopyImages.Checked ? "1" : "0";
            isCopyImages = uiCopyImages.Checked;
            SetRegistryKey(registryPath, "CopyImages", status);
        }


        // ###########################################################################################
        // Changes in "Enable history"
        // ###########################################################################################

        private void uiHistoryEnabled_CheckedChanged(object sender, EventArgs e)
        {
            // History enabled
            string status = uiHistoryEnabled.Checked ? "1" : "0";
            isEnabledHistory = uiHistoryEnabled.Checked;
            SetRegistryKey(registryPath, "HistoryEnable", status);
            if (isEnabledHistory)
            {
                uiHotkeyOlder.Enabled = true;
                uiHotkeyNewer.Enabled = true;
                uiCopyImages.Enabled = true;
                uiPasteOnSelection.Enabled = true;
            }
            else
            {
                uiHotkeyOlder.Enabled = false;
                uiHotkeyNewer.Enabled = false;
                uiCopyImages.Enabled = false;
                uiPasteOnSelection.Enabled = false;
            }

            // Enable/disable hotkeys
            SetHotkeys("Enable history change");

            // Update the tray icon
            UpdateNotifyIconText();
        }


        // ###########################################################################################
        // Changes in "Paste on history selection"
        // ###########################################################################################

        private void uiPasteOnSelection_CheckedChanged(object sender, EventArgs e)
        {
            string status = uiPasteOnSelection.Checked ? "1" : "0";
            isEnabledPasteOnSelection = uiPasteOnSelection.Checked;
            SetRegistryKey(registryPath, "PasteOnSelection", status);
        }


        // ###########################################################################################
        // Changes in "Trim whitespaces"
        // ###########################################################################################

        private void uiTrimWhitespaces_CheckedChanged(object sender, EventArgs e)
        {
            string status = uiTrimWhitespaces.Checked ? "1" : "0";
            isEnabledTrimWhitespacing = uiTrimWhitespaces.Checked;
            SetRegistryKey(registryPath, "TrimWhitespaces", status);
        }

        
        // ###########################################################################################
        // Changes in "Show active border"
        // ###########################################################################################

        private void uiHistoryBorder_CheckedChanged(object sender, EventArgs e)
        {
            string status = uiHistoryBorder.Checked ? "1" : "0";
            SetRegistryKey(registryPath, "HistoryActiveBorder", status);
            historyBorder = uiHistoryBorder.Checked ? true : false;
            EnableDisableCustomColor();
            uiShowFontBottom.Refresh(); // update/redraw the border
        }


        // ###########################################################################################
        // When application starts up then check if it should be minimized at once (when started with Windows)
        // https://stackoverflow.com/a/8486441/2028935
        // ###########################################################################################

        private void MainWindow_Shown(object sender, EventArgs e)
        {
            string arg0 = Program.arg0.ToLower();
            if (arg0.Contains("--start-minimized"))
            {
                WindowState = FormWindowState.Minimized;
                Hide();
            }
        }


        // ###########################################################################################
        // When clicking the "About" in the tray icon menu
        // ###########################################################################################

        private void trayIconAbout_Click(object sender, EventArgs e)
        {
            ShowSettingsForm();
            tabControl.SelectedIndex = 4; // About
        }


        // ###########################################################################################
        // When clicking the "Settings" in the tray icon menu
        // ###########################################################################################

        private void trayIconSettings_Click(object sender, EventArgs e)
        {
            ShowSettingsForm();
            tabControl.SelectedIndex = 0; // General
        }


        // ###########################################################################################
        // When clicking the "Exit" in the tray icon menu
        // ###########################################################################################

        private void trayIconExit_Click(object sender, EventArgs e)
        {
            isClosedFromNotifyIcon = true;
            Close();
        }


        // ###########################################################################################
        // When single-clicking with the mouse on the tray icon
        // This includes a mouse timer to prevent double-click also triggers a single-click
        // https://stackoverflow.com/a/30595638/2028935
        // ###########################################################################################

        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Give the double-click a chance to cancel this
                mouseClickTimer.Start();
                Debug.WriteLine("Start mouse double-click timer");
            }
        }

        private void mouseClickTimer_Tick(object sender, EventArgs e)
        {
            Debug.WriteLine("Mouse single-click detected");
            mouseClickTimer.Stop();
            ToggleEnabled();
        }


        // ###########################################################################################
        // When double-clicking with the mouse on the tray icon
        // ###########################################################################################

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Cancel the single-click
                mouseClickTimer.Stop();

                Debug.WriteLine("Mouse double-click detected");
                ShowSettingsForm();
                tabControl.SelectedIndex = 0;
            }
        }


        // ###########################################################################################
        // Show the "Settings" form
        // https://stackoverflow.com/a/11941579/2028935
        // ###########################################################################################

        private void ShowSettingsForm()
        {
            Show();
            Activate();
            WindowState = FormWindowState.Normal;
        }


        // ###########################################################################################
        // Changes in "Always action" hotkey behaviour
        // ###########################################################################################

        private void uiHotkeyBehaviourSystem_CheckedChanged(object sender, EventArgs e)
        {
            uiHotkeyPaste.Enabled = false;
            SetRegistryKey(registryPath, "HotkeyBehaviour", "System");
            uiRestoreOriginal.Enabled = true;

            // Change the tray icon to be green (active)
            notifyIcon.Icon = Resources.Active;
            Icon = Resources.Active;
        }


        // ###########################################################################################
        // Changes in "Action only on hotkey" hotkey behaviour
        // ###########################################################################################

        private void uiHotkeyBehaviourPaste_CheckedChanged(object sender, EventArgs e)
        {
            uiHotkeyPaste.Enabled = true;
            SetRegistryKey(registryPath, "HotkeyBehaviour", "Paste");
            SetHotkeys("Hotkey behaviour change");
            uiRestoreOriginal.Enabled = false;

            // Change the icon to be the blue one
            notifyIcon.Icon = Resources.Hotkey;
            Icon = Resources.Hotkey;
        }


        // ###########################################################################################
        // Clicking the tab help button
        // ###########################################################################################

        private void uiHelp_Click(object sender, EventArgs e)
        {
            string selectedTab = tabControl.SelectedTab.AccessibilityObject.Name;
            System.Diagnostics.Process.Start(hovtextPage +"documentation/#"+ selectedTab);
        }


        // ###########################################################################################
        // Hotkey handling - how do I get this in to its own file or class!? Hmmm... for later
        // I feel this code is clumsy and inefficient so probably need rewrite!
        // ###########################################################################################


        // ###########################################################################################
        // Action for the "Toggle application on/off" hotkey
        // ###########################################################################################

        private void HotkeyToggleApplication(object sender, NHotkey.HotkeyEventArgs e)
        {
            Debug.WriteLine("Pressed the [Enable/Disable application] hotkey");
            ToggleEnabled();
            e.Handled = true;
        }


        // ###########################################################################################
        // Action for the "Show older entry" hotkey
        // ###########################################################################################

        private void HotkeyGetOlderEntry(object sender, NHotkey.HotkeyEventArgs e)
        {
            Debug.WriteLine("Pressed the [Get older history entry] hotkey");
            GoEntryLowerNumber();
            isHistoryHotkeyPressed = true;
            e.Handled = true;
        }


        // ###########################################################################################
        // Action for the "Show newer entry" hotkey
        // ###########################################################################################

        private void HotkeyGetNewerEntry(object sender, NHotkey.HotkeyEventArgs e)
        {
            Debug.WriteLine("Pressed the [Get newer history entry] hotkey");
            GoEntryHigherNumber();
            isHistoryHotkeyPressed = true;
            e.Handled = true;
        }


        // ###########################################################################################
        // Action for the "Paste on hotkey" hotkey
        // ###########################################################################################

        private void HotkeyPasteOnHotkey(object sender, NHotkey.HotkeyEventArgs e)
        {
            Debug.WriteLine("Pressed the [Paste only on hotkey] hotkey");

            // Get active application and change focus to HovText
            GetActiveApplication();
            ChangeFocusToThisApplication();
            
            // Show the invisible form, so we can catch the key-up event
            pasteOnHotkey.Show();
            pasteOnHotkey.Activate();
            pasteOnHotkey.TopMost = true;

            e.Handled = true;
        }


        // ###########################################################################################
        // On "key down" in one of the hotkey fields then convert that in to a string
        // ###########################################################################################

        private static string ConvertKeyboardInputToString(KeyEventArgs e)
        {
            Debug.WriteLine("Set hotkey: [" + e.KeyData + "] [" + e.KeyCode + "] [" + e.Modifiers + "] [" + e.SuppressKeyPress + "]");

            string hotkey = "";

            // Check if any of the modifiers have been pressed also
            bool isShift = e.Shift;
            bool isAlt = e.Alt;
            bool isControl = e.Control;

            string keyCode = e.KeyCode.ToString();

            // Invalidate various keys
            keyCode = keyCode == "LWin" ? "Unsupported" : keyCode;
            keyCode = keyCode == "Menu" ? "Unsupported" : keyCode;
            keyCode = keyCode == "ControlKey" ? "Unsupported" : keyCode;
            keyCode = keyCode == "ShiftKey" ? "Unsupported" : keyCode;

            // Build the hotkey string
            hotkey = isShift ? hotkey + "Shift + " : hotkey;
            hotkey = isAlt ? hotkey + "Alt + " : hotkey;
            hotkey = isControl ? hotkey + "Control + " : hotkey;
            hotkey += keyCode;

            // Invalidate if no modifier has been pressed also - or only SHIFT
            hotkey = !isShift && !isAlt && !isControl ? "Unsupported" : hotkey;
            hotkey = isShift && !isAlt && !isControl ? "Unsupported" : hotkey;

            // Invalidate if the key is unspported
            hotkey = keyCode == "Unsupported" ? "Unsupported" : hotkey;

            // Mark the hotkey as deleted if pressing "Delete" og "Backspace"
            hotkey = (keyCode == "Delete" || keyCode == "Back") && !isShift && !isAlt && !isControl ? "Not set" : hotkey;

            return hotkey;
        }


        // ###########################################################################################
        // Catch keyboard input on the 4 hotkey fields
        // ###########################################################################################

        private void HotkeyEnable_KeyDown(object sender, KeyEventArgs e)
        {
            string hotkey = ConvertKeyboardInputToString(e);
            uiHotkeyEnable.Text = hotkey;
            if (e.Alt)
            {
                // https://stackoverflow.com/a/3068797/2028935
                e.SuppressKeyPress = true;
            }
        }

        private void hotkeyOlder_KeyDown(object sender, KeyEventArgs e)
        {
            string hotkey = ConvertKeyboardInputToString(e);
            uiHotkeyOlder.Text = hotkey;
            if (e.Alt)
            {
                // https://stackoverflow.com/a/3068797/2028935
                e.SuppressKeyPress = true;
            }
        }

        private void hotkeyNewer_KeyDown(object sender, KeyEventArgs e)
        {
            string hotkey = ConvertKeyboardInputToString(e);
            uiHotkeyNewer.Text = hotkey;
            if (e.Alt)
            {
                // https://stackoverflow.com/a/3068797/2028935
                e.SuppressKeyPress = true;
            }
        }

        private void hotkeyPaste_KeyDown(object sender, KeyEventArgs e)
        {
            string hotkey = ConvertKeyboardInputToString(e);
            uiHotkeyPaste.Text = hotkey;
            if (e.Alt)
            {
                // https://stackoverflow.com/a/3068797/2028935
                e.SuppressKeyPress = true;
            }
        }


        // ###########################################################################################
        // Mark hotkey field as modified when entering it
        // ###########################################################################################

        private void HotkeyEnable_Enter(object sender, EventArgs e)
        {
            ModifyHotkey("hotkeyEnable");
        }

        private void hotkeyOlder_Enter(object sender, EventArgs e)
        {
            ModifyHotkey("hotkeyOlder");
        }

        private void hotkeyNewer_Enter(object sender, EventArgs e)
        {
            ModifyHotkey("hotkeyNewer");
        }

        private void hotkeyPaste_Enter(object sender, EventArgs e)
        {
            ModifyHotkey("hotkeyPaste");
        }


        // ###########################################################################################
        // Color the hotkey field and enable the "Apply" and "Cancel" buttons
        // ###########################################################################################

        private void ModifyHotkey(string hotkey)
        {
            switch (hotkey)
            {
                case "hotkeyEnable":
                    uiHotkeyEnable.BackColor = SystemColors.Info;
                    break;
                case "hotkeyOlder":
                    uiHotkeyOlder.BackColor = SystemColors.Info;
                    break;
                case "hotkeyNewer":
                    uiHotkeyNewer.BackColor = SystemColors.Info;
                    break;
                case "hotkeyPaste":
                    uiHotkeyPaste.BackColor = SystemColors.Info;
                    break;
            }

            // Enable the two buttons, "Apply" and "Cancel"
            applyHotkey.Enabled = true;
            cancelHotkey.Enabled = true;

            // Make sure to remove all the active application hotkeys
            RemoveAllHotkeys();
        }


        // ###########################################################################################
        // Remove all the hotkeys
        // ###########################################################################################

        private static void RemoveAllHotkeys()
        {
            HotkeyManager.Current.Remove("ToggleApplication");
            HotkeyManager.Current.Remove("GetOlderEntry");
            HotkeyManager.Current.Remove("GetNewerEntry");
            HotkeyManager.Current.Remove("PasteOnHotkey");
        }


        // ###########################################################################################
        // Apply the hotkeys
        // ###########################################################################################

        private void ApplyHotkeys_Click(object sender, EventArgs e)
        {
            SetHotkeys("Apply hotkeys button press");
        }


        // ###########################################################################################
        // Set (or remove) the hotkeys and validate them
        // ###########################################################################################

        private void SetHotkeys(string from)
        {
            // Get all hotkey strings
            string hotkey1 = uiHotkeyEnable.Text;
            string hotkey2 = uiHotkeyOlder.Text;
            string hotkey3 = uiHotkeyNewer.Text;
            string hotkey4 = uiHotkeyPaste.Text;

            string conflictText = "";
            KeysConverter cvt;
            Keys key;

            // Convert the strings to hotkey objects

            // Hotkey 1, "Application toggle"
            if (
                hotkey1 != "Unsupported" && 
                hotkey1 != "Not set" && 
                hotkey1 != "Hotkey conflicts"
                )
            {
                cvt = new KeysConverter();
                key = (Keys)cvt.ConvertFrom(hotkey1);
                try
                {
                    HotkeyManager.Current.AddOrReplace("ToggleApplication", key, HotkeyToggleApplication);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Hot key is already registered"))
                    {
                        hotkey1 = "Hotkey conflicts";
                        conflictText += "Hotkey for \"Toggle application on/off\" conflicts with another application\r\n";
                    }
                }
            }

            // Hotkey 2, "Get older entry"
            if (
                hotkey2 != "Unsupported" &&
                hotkey2 != "Not set" &&
                hotkey2 != "Hotkey conflicts" &&
                uiHistoryEnabled.Checked
                )
            {
                cvt = new KeysConverter();
                key = (Keys)cvt.ConvertFrom(hotkey2);
                try
                {
                    HotkeyManager.Current.AddOrReplace("GetOlderEntry", key, HotkeyGetOlderEntry);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Hot key is already registered"))
                    {
                        hotkey2 = "Hotkey conflicts";
                        conflictText += "Hotkey for \"Show older entry\" conflicts with another application\r\n";
                    }
                }
            }
            else
            {
                // Remove the hotkey, if it is not available
                HotkeyManager.Current.Remove("GetOlderEntry");
            }

            // Hotkey 3, "Get newer entry"
            if (
                hotkey3 != "Unsupported" &&
                hotkey3 != "Not set" &&
                hotkey3 != "Hotkey conflicts" &&
                uiHistoryEnabled.Checked
                )
            {
                cvt = new KeysConverter();
                key = (Keys)cvt.ConvertFrom(hotkey3);
                try
                {
                    HotkeyManager.Current.AddOrReplace("GetNewerEntry", key, HotkeyGetNewerEntry);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Hot key is already registered"))
                    {
                        hotkey3 = "Hotkey conflicts";
                        conflictText += "Hotkey for \"Show newer entry\" conflicts with another application\r\n";
                    }
                }
            }
            else
            {
                // Remove the hotkey, if it is not available
                HotkeyManager.Current.Remove("GetNewerEntry");
            }

            // Hotkey 4, "Paste only on hotkey"
            if (
                hotkey4 != "Unsupported" && 
                hotkey4 != "Not set" && 
                hotkey4 != "Hotkey conflicts" &&
                uiHotkeyBehaviourPaste.Checked
                )
            {
                cvt = new KeysConverter();
                key = (Keys)cvt.ConvertFrom(hotkey4);
                try
                {
                    // Only (re)enable it, if the "Action only on hotkey" behaviour has been chosen
                    if (uiHotkeyBehaviourPaste.Checked)
                    {
                        HotkeyManager.Current.AddOrReplace("PasteOnHotkey", key, HotkeyPasteOnHotkey);
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Hot key is already registered"))
                    {
                        hotkey4 = "Hotkey conflicts";
                        conflictText += "Hotkey for \"Paste on hotkey\" conflicts with another application\r\n";
                    }
                }
            } else
            {
                // Remove the hotkey, if it is not available
                HotkeyManager.Current.Remove("PasteOnHotkey");
            }

            // If this is called from startup then show an error, if there is a conflict
            if (conflictText.Length > 0 && from == "Startup of application")
            {
                hotkeyConflict.label2.Text = conflictText;
                hotkeyConflict.Show();
                hotkeyConflict.Activate();
                tabControl.SelectedIndex = 1;
            }

            // Save the hotkeys to registry, if no erros
            if (
                hotkey1 != "Unsupported" && hotkey1 != "Hotkey conflicts" &&
                hotkey2 != "Unsupported" && hotkey2 != "Hotkey conflicts" &&
                hotkey3 != "Unsupported" && hotkey3 != "Hotkey conflicts" &&
                hotkey4 != "Unsupported" && hotkey4 != "Hotkey conflicts"
                )
            {
                SetRegistryKey(registryPath, "Hotkey1", hotkey1);
                SetRegistryKey(registryPath, "Hotkey2", hotkey2);
                SetRegistryKey(registryPath, "Hotkey3", hotkey3);
                SetRegistryKey(registryPath, "Hotkey4", hotkey4);
            }

            bool hasError = false;

            // Update the hotkey fields to reflect if they are good or bad

            // Hotkey 1, "Application toggle"
            if (hotkey1 == "Unsupported" || hotkey1 == "Hotkey conflicts")
            {
                hasError = true;
                uiHotkeyEnable.Text = hotkey1;
                uiHotkeyEnable.BackColor = Color.DarkSalmon;
            }
            else
            {
                uiHotkeyEnable.BackColor = SystemColors.Window;
            }

            // Hotkey 2, "Get older entry"
            if (hotkey2 == "Unsupported" || hotkey2 == "Hotkey conflicts")
            {
                hasError = true;
                uiHotkeyOlder.Text = hotkey2;
                uiHotkeyOlder.BackColor = Color.DarkSalmon;
            }
            else
            {
                uiHotkeyOlder.BackColor = SystemColors.Window;
            }

            // Hotkey 3, "Get newer entry"
            if (hotkey3 == "Unsupported" || hotkey3 == "Hotkey conflicts")
            {
                hasError = true;
                uiHotkeyNewer.Text = hotkey3;
                uiHotkeyNewer.BackColor = Color.DarkSalmon;
            }
            else
            {
                uiHotkeyNewer.BackColor = SystemColors.Window;
            }

            // Hotkey 4, "Paste only on hotkey"
            if (hotkey4 == "Unsupported" || hotkey4 == "Hotkey conflicts")
            {
                hasError = true;
                uiHotkeyPaste.Text = hotkey4;
                uiHotkeyPaste.BackColor = Color.DarkSalmon;
            }
            else
            {
                uiHotkeyPaste.BackColor = SystemColors.Window;
            }

            // Accept the changes and disable the two buttons again
            if (!hasError)
            {
                applyHotkey.Enabled = false;
                cancelHotkey.Enabled = false;
            }
        }


        // ###########################################################################################
        // Cancel the hotkeys and restore original content
        // ###########################################################################################

        private void cancelHotkey_Click(object sender, EventArgs e)
        {
            string hotkey1 = GetRegistryKey(registryPath, "Hotkey1");
            string hotkey2 = GetRegistryKey(registryPath, "Hotkey2");
            string hotkey3 = GetRegistryKey(registryPath, "Hotkey3");
            string hotkey4 = GetRegistryKey(registryPath, "Hotkey4");
            uiHotkeyEnable.Text = hotkey1;
            uiHotkeyOlder.Text = hotkey2;
            uiHotkeyNewer.Text = hotkey3;
            uiHotkeyPaste.Text = hotkey4;
            SetHotkeys("Cancel hotkeys button press");
        }


        // ###########################################################################################
        // Show a notification if the user closes the application to tray for the very first time
        // ###########################################################################################

        private void ShowTrayNotification()
        {
            // Show a tray notification if this is the first time the user close the form (without exiting the application)
            int notificationShown = int.Parse((string)GetRegistryKey(registryPath, "NotificationShown"));
            if (notificationShown == 0)
            {
                notifyIcon.Visible = true;
                notifyIcon.ShowBalloonTip(
                    10000,
                    "HovText is still running",
                    "HovText continues running in the background to perform its duties. You can see the icon in the tray area",
                    ToolTipIcon.Info
                    );

                // Mark that we now have shown this
                SetRegistryKey(registryPath, "NotificationShown", "1");
            }
        }


        // ###########################################################################################
        // Update the entries on the tray icon (when hovering the mouse over it)
        // ###########################################################################################

        private void UpdateNotifyIconText()
        {
            // Update the counter if the history is enabled
            if (isEnabledHistory)
            {
                int entries = entriesText.Count;
                if (entries == 1)
                {
                    notifyIcon.Text = "HovText (" + entries + " entry)";
                }
                else
                {
                    notifyIcon.Text = "HovText (" + entries + " entries)";
                }
            }
            else
            {
                notifyIcon.Text = "HovText";
            }
        }


        // ###########################################################################################
        // Quit test releases after 30 minutes - ideally I don't want them floating around as they are not mainstream
        // ###########################################################################################

        private void timerTest_Tick(object sender, EventArgs e)
        {
            timerTest.Enabled = false;
            MessageBox.Show("This is a limited test version that can run for a maximum of 30 minutes and HovText will now exit - please replace it with a real version", "HovText will terminate");
            System.Windows.Forms.Application.Exit();
        }


        // ###########################################################################################
        // Donate
        // ###########################################################################################

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.paypal.com/donate?hosted_button_id=U23UUA8YWABGU");
        }


        // ###########################################################################################
        // Get customer color inputs
        // ###########################################################################################

        private void uiCustomTop_Enter(object sender, EventArgs e)
        {
            colorDialogTop.Color = uiShowFontTop.BackColor;
            colorDialogTop.FullOpen = true;
            if (colorDialogTop.ShowDialog() == DialogResult.OK)
            {
                string color = String.Format("#{0:X2}{1:X2}{2:X2}", colorDialogTop.Color.R, colorDialogTop.Color.G, colorDialogTop.Color.B);
                SetRegistryKey(registryPath, "HistoryColorCustomTop", color);
                historyColorsTop["Custom"] = color;
                uiCustomTop.BackColor = colorDialogTop.Color;
                SetHistoryColors();
            }
            ActiveControl = null; // do not focus this form - so we can reclick the textbox again right after
        }

        private void uiCustomBottom_Enter(object sender, EventArgs e)
        {
            colorDialogBottom.Color = uiShowFontBottom.BackColor;
            colorDialogBottom.FullOpen = true;
            if (colorDialogBottom.ShowDialog() == DialogResult.OK)
            {
                string color = String.Format("#{0:X2}{1:X2}{2:X2}", colorDialogBottom.Color.R, colorDialogBottom.Color.G, colorDialogBottom.Color.B);
                SetRegistryKey(registryPath, "HistoryColorCustomBottom", color);
                historyColorsBottom["Custom"] = color;
                uiCustomBottom.BackColor = colorDialogBottom.Color;
                SetHistoryColors();
            }
            ActiveControl = null; // do not focus this form - so we can reclick the textbox again right after
        }

        private void uiCustomText_Enter(object sender, EventArgs e)
        {
            colorDialogText.Color = uiShowFontTop.ForeColor;
            colorDialogText.FullOpen = true;
            if (colorDialogText.ShowDialog() == DialogResult.OK)
            {
                string color = String.Format("#{0:X2}{1:X2}{2:X2}", colorDialogText.Color.R, colorDialogText.Color.G, colorDialogText.Color.B);
                SetRegistryKey(registryPath, "HistoryColorCustomText", color);
                historyColorsText["Custom"] = color;
                uiCustomText.BackColor = colorDialogText.Color;
                SetHistoryColors();
            }
            ActiveControl = null; // do not focus this form - so we can reclick the textbox again right after
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
//            colorDialogBorder.Color = uiShowFontTop.ForeColor;
            colorDialogBorder.FullOpen = true;
            if (colorDialogBorder.ShowDialog() == DialogResult.OK)
            {
                string color = String.Format("#{0:X2}{1:X2}{2:X2}", colorDialogBorder.Color.R, colorDialogBorder.Color.G, colorDialogBorder.Color.B);
                SetRegistryKey(registryPath, "HistoryColorCustomBorder", color);
                historyColorsBorder["Custom"] = color;
                uiCustomBorder.BackColor = colorDialogBorder.Color;
//                SetHistoryColors();
            }
            ActiveControl = null; // do not focus this form - so we can reclick the textbox again right after
        }


        // ###########################################################################################
        // Take over redraw for a specific UI element, "history border"
        // ###########################################################################################

        private void uiShowFontBottom_Paint(object sender, PaintEventArgs e)
        {
            if (historyBorder)
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


        // ###########################################################################################
    }
}
