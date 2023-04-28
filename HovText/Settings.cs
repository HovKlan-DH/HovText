using HovText.Properties;
using NHotkey.WindowsForms; // https://github.com/thomaslevesque/NHotkey
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using static HovText.Program;
using Newtonsoft.Json;


// ----------------------------------------------------------------------------
// Upload application to these places:
// https://www.microsoft.com/en-us/wdsi/filesubmission
// ----------------------------------------------------------------------------

// NuGet: "Costura.Fody" to merge the DLLs in to the EXE - to get only one EXE file for this application
// Incredible cool and simple compared to the other complex stuff I have seen! :-)
// https://stackoverflow.com/a/40786196/2028935

namespace HovText
{

    public sealed partial class Settings : Form
    {
        // ###########################################################################################
        // Define "Settings" class variables - real spaghetti :-)
        // ###########################################################################################

        // History, default values
        public static string historyFontFamily = "Segoe UI";
        public static float historyFontSize = 11;
        public static int historyListElements = 6; // 1-30
        public static int historySizeWidth = 35; // percentage (10-100%)
        public static int historySizeHeight = 70; // percentage (10-100%)
        public static string historyColorTheme = "Yellow";
        public static Dictionary<string, string> historyColorsHeader = new Dictionary<string, string>() {
            { "Blue", "#dae1e7" },
            { "Brown", "#dac1a0" },
            { "Green", "#c1dac1" },
            { "Yellow", "#e7e1c8" },
            { "Contrast", "#000000" },
            { "Custom", "#cd5c5c" },
        };
        public static Dictionary<string, string> historyColorsHeaderText = new Dictionary<string, string>() {
            { "Blue", "#000000" },
            { "Brown", "#000000" },
            { "Green", "#000000" },
            { "Yellow", "#000000" },
            { "Contrast", "#ffffff" },
            { "Custom", "#ffff00" },
        };
        public static Dictionary<string, string> historyColorsActive = new Dictionary<string, string>() {
            { "Blue", "#dae1e7" },
            { "Brown", "#dac1a0" },
            { "Green", "#c1dac1" },
            { "Yellow", "#e7e1c8" },
            { "Contrast", "#000000" },
            { "Custom", "#ffd700" },
        };
        public static Dictionary<string, string> historyColorsActiveText = new Dictionary<string, string>() {
            { "Blue", "#000000" },
            { "Brown", "#000000" },
            { "Green", "#000000" },
            { "Yellow", "#000000" },
            { "Contrast", "#ffffff" },
            { "Custom", "#df00df" },
        };
        public static Dictionary<string, string> historyColorsEntry = new Dictionary<string, string>() {
            { "Blue", "#f5faff" },
            { "Brown", "#eee3d5" },
            { "Green", "#eaf2ea" },
            { "Yellow", "#fff8dc" },
            { "Contrast", "#ffffff" },
            { "Custom", "#8fbc8f" },
        };
        public static Dictionary<string, string> historyColorsEntryText = new Dictionary<string, string>() {
            { "Blue", "#000000" },
            { "Brown", "#000000" },
            { "Green", "#000000" },
            { "Yellow", "#000000" },
            { "Contrast", "#000000" },
            { "Custom", "#5e0086" },
        };
        public static string historyColorBorder = "#ff0000";
        public static string historyLocation = "Right Bottom";
        public static int historyBorderThickness = 1;

        // Registry, default values
        public const string registryPath = "SOFTWARE\\HovText";
        public const string registryPathDisplays = "SOFTWARE\\HovText\\DisplayLayout";
        public const string registryPathRun = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        private const string registryHotkeyToggleApplication = "Control + Oem5"; // hotkey "Toggle application on/off"
        private const string registryHotkeyGetOlderEntry = "Alt + H"; // hotkey "Show older entry"
        private const string registryHotkeyGetNewerEntry = "Shift + Alt + H"; // hotkey "Show newer entry"
        private const string registryHotkeyPasteOnHotkey = "Alt + O"; // hotkey "Paste on hotkey"
        private const string registryHotkeyToggleFavorite = "Space"; // hotkey "Toggle favorite entry"
        private const string registryHotkeyToggleView = "Q"; // hotkey "Toggle list view"
        private const string registryHotkeyBehaviour = "System"; // use system clipboard
        private const string registryCloseMinimizes = "1"; // 1 = minimize to tray
        private const string registryRestoreOriginal = "1"; // 1 = restore original
        private const string registryCopyImages = "1"; // 1 = copy images to history
        private const string registryEnableHistory = "1"; // 1 = enable history
        private const string registryEnableFavorites = "1"; // 1 = enable favorites
        private const string registryPasteOnSelection = "0"; // 0 = do not paste selected entry when selected
        private const string registryTrimWhitespaces = "1"; // 1 = trim whitespaces
        private const string registryTroubleshootEnable = "0"; // 0 = do not enable troubleshoot logging
        public static string iconSet = "Round"; // Round, SquareOld, SquareNew
        public static int historyMargin = 5;

        // UI elements
        public static bool isEnabledHistory;
        public static bool isEnabledFavorites;
        public static bool isEnabledPasteOnSelection;
        public static bool isEnabledTrimWhitespacing;
        public static bool isRestoreOriginal;
        public static bool isCopyImages;
        public static bool isCloseMinimizes;
        public static bool isClosedFromNotifyIcon;
        public static bool isHistoryHotkeyPressed;
        public static bool isHistoryMarginEnabled;

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
        System.Windows.Forms.IDataObject clipboardObject;
        public readonly static SortedDictionary<int, string> entriesApplication = new SortedDictionary<int, string>();
        public readonly static SortedDictionary<int, string> entriesText = new SortedDictionary<int, string>();
        public readonly static SortedDictionary<int, Image> entriesImage = new SortedDictionary<int, Image>();
        private readonly static SortedList<int, Dictionary<string, object>> entriesOriginal = new SortedList<int, Dictionary<string, object>>();
        public readonly static SortedDictionary<int, bool> entriesImageTransparent = new SortedDictionary<int, bool>();
        public readonly static SortedDictionary<int, bool> entriesIsFavorite = new SortedDictionary<int, bool>();
        const int WM_CLIPBOARDUPDATE = 0x031D;
        string whoUpdatedClipboardName = "";
        public static bool pasteOnHotkeySetCleartext;

        // Misc
        public static string appVer = "";
        private static IntPtr originatingHandle = IntPtr.Zero;
        private static bool isFirstCallAfterHotkey = true;
        public static bool isSettingsFormVisible;
        public static string hovtextPage = "https://hovtext.com/";
        internal static Settings settings;
        public static bool isTroubleshootEnabled;
        public static bool hasTroubleshootLogged;
        public static string troubleshootLogfile = "HovText-troubleshooting.txt";
        private static bool resetApp = false;
        public static bool showFavoriteList = false;
        readonly History history = new History();
        readonly Update update = new Update();
        readonly PasteOnHotkey pasteOnHotkey = new PasteOnHotkey();
        readonly HotkeyConflict hotkeyConflict = new HotkeyConflict();
        private static string originatingApplicationName = "";
        public static int activeDisplay; // selected display to show the history (default will be the main display)
        private static string hotkey; // needed for validating the keys as it is not set in the event
        private static bool firstTimeLaunch;
        private static string buildType = ""; // Debug, Release


        // ###########################################################################################
        // Main
        // ###########################################################################################

        public Settings()
        {
            
            // Get build type
            #if DEBUG
                 buildType = "Debug";
            #else
                buildType = "Release";
            #endif

            // Get application file version from assembly
            Assembly assemblyInfo = Assembly.GetExecutingAssembly();
            string assemblyVersion = FileVersionInfo.GetVersionInfo(assemblyInfo.Location).FileVersion;
            string year = assemblyVersion.Substring(0, 4);
            string month = assemblyVersion.Substring(5, 2);
            string day = assemblyVersion.Substring(8, 2);
            string rev = assemblyVersion.Substring(11); // will be ignored in RELEASE builds
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

            // Beautify revision and build-type 
            rev = "(rev. "+ rev +")";
            rev = buildType == "Debug" ? rev : "";
            string buildTypeTmp = buildType == "Debug" ? "# DEVELOPMENT " : "";

            // Set the application version
            appVer = (date + " " + buildTypeTmp + rev).Trim();

            // Start logging, if relevant
            Logging.StartLogging();
            hasTroubleshootLogged = isTroubleshootEnabled ? true : false;

            // Setup form and all elements
            InitializeComponent();

            // As the UI elements now have been initialized then we can setup the version
            uiAppVer.Text = "Version " + appVer;

            // Mark very clearly with a color, that this is not a normal version (red equals danger) :-)
            if (buildType == "Debug")
            {
                BackColor = Color.IndianRed;
                uiDeveloperVersion.Visible = true;
            } else
            {
                uiDeveloperVersion.Visible = false;
            }

            // Refering to the current form - used in the history form
            settings = this;

            // Catch repaint event for this specific element (to draw the border)
            uiShowFontActive.Paint += new System.Windows.Forms.PaintEventHandler(this.uiShowFontBottom_Paint);

            // Catch display change events (e.g. add/remove displays or change of main display)
            SystemEvents.DisplaySettingsChanged += new EventHandler(DisplayChangesEvent);

            // Initialize registry and get its values for the various checkboxes
            ConvertLegacyRegistry();
            InitializeRegistry();
            GetStartupSettings();

            NativeMethods.AddClipboardFormatListener(this.Handle);
            Logging.Log("Added HovText to clipboard chain");

            // Process clipboard at startup also (if application is enabled)
            if (uiAppEnabled.Checked)
            {
                ProcessClipboard();
            }

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

            // Write text (URL) for the "Privacy" page
            var sb2 = new StringBuilder();
            sb2.Append(@"{\rtf1\ansi");
            sb2.Append(@" https://hovtext.com/funfacts/ ");
            sb2.Append(@"}");
            urlFunfacts.Rtf = sb2.ToString();

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
                    IntPtr whoUpdatedClipboardHwnd = NativeMethods.GetClipboardOwner();
                    NativeMethods.GetWindowThreadProcessId(whoUpdatedClipboardHwnd, out uint thisProcessId);
                    whoUpdatedClipboardName = Process.GetProcessById((int)thisProcessId).ProcessName;

                    // Get the name for this HovText executable (it may not be "HovText.exe")
                    string exeFileNameWithPath = Process.GetCurrentProcess().MainModule.FileName;
//                    string exeFileName = System.IO.Path.GetFileName(exeFileNameWithPath);
                    string exeFileNameWithoutExtension = Path.GetFileNameWithoutExtension(exeFileNameWithPath);

                    // Do not process clipboard, if this is coming from HovText itself
                    if (whoUpdatedClipboardName != exeFileNameWithoutExtension)
                        {
                        Logging.Log("Clipboard [UPDATE] event from [" + whoUpdatedClipboardName + "]");

                        // I am not sure why some(?) applications are returned as "Idle" or "svchost" when coming from clipboard - in this case the get the active application and use that name instead
                        // This could potentially be a problem, if a process is correctly called "Idle" but not sure if this is realistic?
                        if (whoUpdatedClipboardName.ToLower() == "idle")
                        {
                            string activeProcessName = GetActiveApplication();
                            whoUpdatedClipboardName = activeProcessName;
                            Logging.Log("Finding process name the secondary way, [" + whoUpdatedClipboardName + "]");
                        }

                        // Check if application is enabled
                        if (uiAppEnabled.Checked)
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
            else if (isClipboardImage && isEnabledHistory) // Is clipboard an image
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
                        isClipboardImageTransparent = false;
                        if (imgCopy != null)
                        {
                            isClipboardImageTransparent = IsImageTransparent(imgCopy);
                        }

                        // Add the image to the entries array and update the clipboard
                        AddEntry();
                        GetEntryCounter();
                        SetClipboard();
                    }
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
                // Sometimes this fails - probably because clipboard handling also can be messy and complex
                byte[] dib;
                try
                {
                    dib = ((System.IO.MemoryStream)Clipboard.GetData(DataFormats.Dib)).ToArray();
                }
                catch (Exception ex)
                {
                    Logging.Log("Exception #10 raised (Settings):");
                    Logging.Log("  " + ex.Message);
                    Logging.Log("  Failed converting image to byte array - fetching \"GetImage()\" instead");
                    return Clipboard.ContainsImage() ? Clipboard.GetImage() : null;
                }
                
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
            Logging.Log("Restoring original content to clipboard:");

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
                        Logging.Log("  Adding format to clipboard, ["+ kvp.Key +"]");
                    }
                }
                Clipboard.Clear();
                Clipboard.SetDataObject(data, true);
                // ---
            }
            catch (Exception ex)
            {
                Logging.Log("Exception #1 raised (Settings):");
                Logging.Log("  " + ex.Message); 
                MessageBox.Show("EXCEPTION #1 - please enable troubleshooting log and report to developer");
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
                entriesIsFavorite.Clear();
                entriesApplication.Clear();
                entriesOriginal.Clear();
            }

            // Proceed if the (cleartext) data is not already in the dictionary
            bool isAlreadyInDataArray = IsClipboardContentAlreadyInDataArrays();
            if (!isAlreadyInDataArray)
            {
                if(isClipboardText)
                {
                    Logging.Log("Adding new [TEXT] clipboard to history:");
                }
                else
                {
                    Logging.Log("Adding new [IMAGE] clipboard to history:");
                }

                // If this is the first time then set the index to 0
                entryIndex = entryIndex >= 0 ? entriesText.Keys.Last() + 1 : 0;

                // Add the text and image to the entries array
                entriesText.Add(entryIndex, clipboardText);
                entriesImage.Add(entryIndex, clipboardImage);
                entriesImageTransparent.Add(entryIndex, isClipboardImageTransparent);
                entriesIsFavorite.Add(entryIndex, false);

                // Walk through all (relevant) clipboard object formats and store them.
                // I am not sure how to do this differently as it does not work if I take everything!?
                // This seems to be something that needs constant changes!?
                var formats = clipboardObject.GetFormats(false);
                Dictionary<string, object> clipboardObjects = new Dictionary<string, object>();
                foreach (var format in formats)
                {
                    if (
                        format.Contains("Text")
                        || format.Contains("HTML")
                        || format.Contains("Csv")
                        || format.Contains("Link")
                        || format.Contains("Hyperlink")
                        || format.Contains("Bitmap")
                        || format.Contains("PNG") // including transparent layer of PNG
                        || format.Contains("Recipient") // Outlook recipient
                        || format.Contains("Format17") // picture format
                        || format.Contains("GIF")
                        || format.Contains("JFIF")
                        || format.Contains("Office Drawing Shape Format")
                        || format.Contains("Preferred DropEffect") // used in e.g. animated GIF
                        || format.Contains("Shell IDList Array") // used in e.g. animated GIF
                        || format.Contains("FileDrop") // used in e.g. animated GIF
                        || format.Contains("FileContents") // used in e.g. animated GIF
                        || format.Contains("FileGroupDescriptorW") // used in e.g. animated GIF
                        || format.Contains("Image") // seen on "CorelPHOTOPAINT.Image.20" and "CorelPhotoPaint.Image.9"
                        || format.Contains("Color") // seen on "Corel.Color.20"
                        )
                    {
                        clipboardObjects.Add(format, clipboardObject.GetData(format));
                        Logging.Log("  Adding format ["+ format +"]");
                    }
                    else
                    {
                        Logging.Log("  Discarding format [" + format + "]");
                    }
                }
                entriesOriginal.Add(entryIndex, clipboardObjects);

                // Add the process name that has updated the clipboard
                if (whoUpdatedClipboardName.Length == 0)
                {
                    whoUpdatedClipboardName = "(unknown)";
                }
                entriesApplication.Add(entryIndex, whoUpdatedClipboardName);

                Logging.Log("Entries in history list is now [" + entriesText.Count + "]");
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
                        Clipboard.SetText(entryText, TextDataFormat.UnicodeText); // https://stackoverflow.com/a/14255608/2028935
                    }
                }
                catch (Exception ex)
                {
                    Logging.Log("Exception #2 raised (Settings):");
                    Logging.Log("  " + ex.Message);
                    MessageBox.Show("EXCEPTION #2 - please enable troubleshooting log and report to developer");
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
                    Logging.Log("Exception #3 raised (Settings):");
                    Logging.Log("  " + ex.Message);
                    MessageBox.Show("EXCEPTION #3 - please enable troubleshooting log and report to developer");
                }
            }
            else
            {
                Logging.Log("Exception #4 raised (Settings):");
                Logging.Log("  Clipboard triggered but is not [isEntryText] or [isEntryImage]");
                MessageBox.Show("EXCEPTION #4 - please enable troubleshooting log and report to developer");
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
                    originatingApplicationName = GetActiveApplication();
                    history.SetupForm();
                }
                // Always change focus to HovText to ensure we can catch the key-up event
                ChangeFocusToThisApplication();

                // Only proceed if the entry counter is equal to or more than 0
                if (entryCounter > 0)
                {
                    isFirstCallAfterHotkey = false;
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
                    originatingApplicationName = GetActiveApplication();
                    history.SetupForm();
                }
                // Always change focus to HovText to ensure we can catch the key-up event
               ChangeFocusToThisApplication();

                // Only proceed if the entry counter is less than the total amount of entries
                if (entryCounter <= entriesText.Count)
                {
                    isFirstCallAfterHotkey = false;
                    history.UpdateHistory("up");
                }
            }
        }


        // ###########################################################################################
        // Called when a history entry has been selected in the "History" form
        // ###########################################################################################

        public void SelectHistoryEntry()
        {
            isHistoryHotkeyPressed = false;

            // Check if application is enabled
            if (uiAppEnabled.Checked && entryCounter > 0)
            {
                // Only proceed if we should actually restore something (we could come here from an empty favorite list view)
                int entriesInList = History.entriesInList;
                if (entriesInList > 0)
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
            entriesIsFavorite.Add(insertKey, entriesIsFavorite[entryIndex]);
            entriesApplication.Add(insertKey, entriesApplication[entryIndex]);
            entriesOriginal.Add(insertKey, entriesOriginal[entryIndex]);

            // Remove the chosen entry, so it does not show duplicates
            entriesText.Remove(entryIndex);
            entriesImage.Remove(entryIndex);
            entriesImageTransparent.Remove(entryIndex);
            entriesIsFavorite.Remove(entryIndex);
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
                Logging.Log("Enabled HovText");

                // Add this application to the clipboard chain again
                NativeMethods.AddClipboardFormatListener(this.Handle);
                Logging.Log("Added HovText to clipboard chain");

                ProcessClipboard();

                string hotkeyBehaviour = GetRegistryKey(registryPath, "HotkeyBehaviour");
                switch (hotkeyBehaviour)
                {
                    case "Paste":
                        uiHotkeyBehaviourPaste.Checked = true;
                        uiHotkeyPaste.Enabled = true;

                        // Change the icons to be the blue one
                        if (iconSet == "SquareOld")
                        {
                            notifyIcon.Icon = Resources.Square_Old_Hotkey_16x16;
                            Icon = Resources.Square_Old_Hotkey_16x16;
                        }
                        else
                        {
                            notifyIcon.Icon = Resources.Round_Hotkey_48x48;
                            Icon = Resources.Round_Hotkey_48x48;
                        }

                        break;
                    default:
                        uiHotkeyBehaviourSystem.Checked = true;
                        uiHotkeyPaste.Enabled = false;

                        // Change the icons to be green (active)
                        if(iconSet == "SquareOld")
                        {
                            notifyIcon.Icon = Resources.Square_Old_Active_16x16;
                            Icon = Resources.Square_Old_Active_16x16;
                        } else
                        {
                            notifyIcon.Icon = Resources.Round_Active_48x48;
                            Icon = Resources.Round_Active_48x48;
                        }

                        break;
                }

                // Enable other checkboxes
                uiHistoryEnabled.Enabled = true;
                if (uiHistoryEnabled.Checked)
                {
                    uiCopyImages.Enabled = true;
                    uiPasteOnSelection.Enabled = true;
                    uiHotkeyOlder.Enabled = true;
                    uiHotkeyNewer.Enabled = true;
                    uiFavoritesEnabled.Enabled = true;
                    if (uiFavoritesEnabled.Checked)
                    {
                        uiHotkeyToggleFavorite.Enabled = true;
                        uiHotkeyToggleView.Enabled = true;
                    }
                }
                uiRestoreOriginal.Enabled = true;
                uiHotkeyBehaviourSystem.Enabled = true;
                uiHotkeyBehaviourPaste.Enabled = true;
                uiTrimWhitespaces.Enabled = true;
            }
            else
            {
                Logging.Log("Disabed HovText");

                // Remove this application from the clipboard chain
                NativeMethods.RemoveClipboardFormatListener(this.Handle);
                Logging.Log("Removed HovText from clipboard chain");

                // Restore the original clipboard format
                if (isRestoreOriginal && entriesOriginal.Count > 0)
                {
                    RestoreOriginal(entryIndex);
                }

                // Change the icons to be red (inactive)
                if (iconSet == "SquareOld")
                {
                    notifyIcon.Icon = Resources.Square_Old_Inactive_16x16;
                    Icon = Resources.Square_Old_Inactive_16x16;
                }
                else
                {
                    notifyIcon.Icon = Resources.Round_Inactive_48x48;
                    Icon = Resources.Round_Inactive_48x48;
                }

                // Disable other checkboxes
                uiHistoryEnabled.Enabled = false;
                uiFavoritesEnabled.Enabled = false;
                uiRestoreOriginal.Enabled = false;
                uiCopyImages.Enabled = false;
                uiTrimWhitespaces.Enabled = false;
                uiPasteOnSelection.Enabled = false;
                uiHotkeyOlder.Enabled = false;
                uiHotkeyNewer.Enabled = false;
                uiHotkeyPaste.Enabled = false;
                uiHotkeyBehaviourSystem.Enabled = false;
                uiHotkeyBehaviourPaste.Enabled = false;
                uiFavoritesEnabled.Enabled = false;
                uiHotkeyToggleFavorite.Enabled = false;
                uiHotkeyToggleView.Enabled = false;
            }
        }


        // ###########################################################################################
        // Hide the "Settings" form - called when application is minimized
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
        // Launch browser and go to HovText web page when link is clicked
        // ###########################################################################################

        private void aboutBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Logging.Log("Clicked the web page link in \"About\""); 
            System.Diagnostics.Process.Start(e.LinkText);
        }

        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Logging.Log("Clicked the web page link in \"Privacy\"");
            System.Diagnostics.Process.Start(e.LinkText);
        }


        // ###########################################################################################
        // Unregister from the clipboard chain, and remove hotkeys when application is closing down
        // ###########################################################################################

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            // In case windows is trying to shut down, don't hold up the process
            if (e.CloseReason == CloseReason.WindowsShutDown)
            {
                Logging.Log("Exit HovText");
                NativeMethods.RemoveClipboardFormatListener(this.Handle);
                Logging.Log("Removed HovText from clipboard chain");

                RemoveAllHotkeys();
                Logging.EndLogging();

                // Delete the troubleshooting logfile as the very last thing
                if (resetApp)
                {
                    if (File.Exists(troubleshootLogfile))
                    {
                        File.Delete(@troubleshootLogfile);
                    }
                }

                return;
            }

            if (!isCloseMinimizes || isClosedFromNotifyIcon)
            {
                Logging.Log("Exit HovText");
                NativeMethods.RemoveClipboardFormatListener(this.Handle);
                Logging.Log("Removed HovText from clipboard chain");

                RemoveAllHotkeys();
                Logging.EndLogging();

                // Delete the troubleshooting logfile as the very last thing
                if (resetApp)
                {
                    if (File.Exists(troubleshootLogfile))
                    {
                        File.Delete(@troubleshootLogfile);
                    }
                }

                return;
            }

            // Called when closing from "Clean up"
            if (resetApp)
            {
                NativeMethods.RemoveClipboardFormatListener(this.Handle);
                RemoveAllHotkeys();

                // Delete the troubleshooting logfile as the very last thing
                if (File.Exists(troubleshootLogfile))
                {
                    File.Delete(@troubleshootLogfile);
                }

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
            originatingHandle = NativeMethods.GetForegroundWindow();

            // Get the process ID and find the name for that ID
            NativeMethods.GetWindowThreadProcessId(originatingHandle, out uint processId);
            string appProcessName = Process.GetProcessById((int)processId).ProcessName;
            return appProcessName;
        }


        // ###########################################################################################
        // When pressing one of the history hotkeys then change focus to this application to prevent the keypresses go in to the active application
        // ###########################################################################################

        private void ChangeFocusToThisApplication()
        {
            NativeMethods.SetForegroundWindow(this.Handle);
            Logging.Log("Set focus to HovText");
        }


        // ###########################################################################################
        // When an entry has been submitted to the clipboard then pass back focus to the originating application
        // ###########################################################################################

        public static void ChangeFocusToOriginatingApplication()
        {
            NativeMethods.SetForegroundWindow(originatingHandle);
            Logging.Log("Set focus to originating application ["+ originatingApplicationName +"]");
        }


        // ###########################################################################################
        // Check for HovText updates online.
        // Stable versions will be notified via popup.
        // Development versions will be shown in "Advanced" tab only.
        // ###########################################################################################

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            updateTimer.Enabled = false;

            Logging.Log("Version check timer expired");
            Logging.Log("  User is running version = [" + appVer + "]");

            // Check for a new stable version
            try
            {
                WebClient webClient = new WebClient();
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
                webClient.Headers.Add("user-agent", ("HovText " + appVer).Trim());
                string checkedVersion = webClient.DownloadString(hovtextPage + "autoupdate/");
                if (checkedVersion.Substring(0, 7) == "Version")
                {
                    checkedVersion = checkedVersion.Substring(9);
                    Logging.Log("  Stable version available = [" + checkedVersion + "]");
                    update.uiAppVerYours.Text = appVer;
                    update.uiAppVerOnline.Text = checkedVersion;
                    string lastCheckedVersion = GetRegistryKey(registryPath, "CheckedVersion");
                    if (lastCheckedVersion != checkedVersion && checkedVersion != appVer)
                    {
                        update.Show();
                        update.Activate();
                        Logging.Log("  Notified on new version available");
                    }
                }
            }
            catch (WebException ex)
            {
                // Catch the exception though this is not so critical that we need to disturb the developer
                Logging.Log("Exception #11 raised (Settings):");
                Logging.Log("  " + ex.Message);
            }

            // Check for a new development version
            try
            {
                WebClient webClient = new WebClient();
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
                webClient.Headers.Add("user-agent", ("HovText " + uiAppVer.Text).Trim());
                string checkedVersion = webClient.DownloadString(hovtextPage + "autoupdate/development/");
                if (checkedVersion.Substring(0, 7) == "Version")
                {
                    checkedVersion = checkedVersion.Substring(9); 
                    uiDevelopmentVersion.Text = " "+ checkedVersion;
                    uiDevelopmentDownload.Enabled = true;
                    Logging.Log("  Development version available = [" + checkedVersion + "]");
                }
            }
            catch (WebException ex)
            {
                // Catch the exception though this is not so critical that we need to disturb the developer
                Logging.Log("Exception #14 raised (Settings):");
                Logging.Log("  " + ex.Message);
            }
        }


        // ###########################################################################################
        // Convert legacy/old registry entries
        // ###########################################################################################

        private void ConvertLegacyRegistry()
        {

            // Check if the "HovText" path exists in HKEY_CURRENT_USER\SOFTWARE registry
            using (RegistryKey registryPathExists = Registry.CurrentUser.OpenSubKey(registryPath))
            {
                if (registryPathExists == null)
                {
                    firstTimeLaunch = true;
                    Logging.Log("Registry [" + registryPath + "] does not exists, so this is a [first time] launch");
                }
                else
                {
                    firstTimeLaunch = false;
                    Logging.Log("Registry [" + registryPath + "] exists, so this is an [already initialized] launch");
                }
            }

            // Check or create "HovText" and "HovText\DisplayLayout" paths in HKEY_CURRENT_USER\SOFTWARE registry
            RegistryPathCheckOrCreate(registryPath);
            RegistryPathCheckOrCreate(registryPathDisplays);

            string regVal;

            // Check if the following registry entries exists, and if so convert them to new values

            // Convert "HistoryActiveBorder" => "HistoryBorderThickness"
            regVal = GetRegistryKey(registryPath, "HistoryActiveBorder");
            if (regVal != null)
            {
                if (regVal == "0")
                {
                    RegistryKeyCheckOrCreate(registryPath, "HistoryBorderThickness", regVal);
                } else
                {
                    RegistryKeyCheckOrCreate(registryPath, "HistoryBorderThickness", historyBorderThickness.ToString());
                }
                DeleteRegistryKey(registryPath, "HistoryActiveBorder");
            }

            // Convert "HistoryColor" => "HistoryColorTheme"
            regVal = GetRegistryKey(registryPath, "HistoryColor");
            if (regVal != null)
            {
                RegistryKeyCheckOrCreate(registryPath, "HistoryColorTheme", regVal);
                DeleteRegistryKey(registryPath, "HistoryColor");
            }

            // Convert "HistoryColorCustomTop" => "HistoryColorCustomHeader"
            regVal = GetRegistryKey(registryPath, "HistoryColorCustomTop");
            if (regVal != null)
            {
                RegistryKeyCheckOrCreate(registryPath, "HistoryColorCustomHeader", regVal);
                DeleteRegistryKey(registryPath, "HistoryColorCustomTop");
            }

            // Convert "HistoryColorCustomBottom" => "HistoryColorCustomEntry"
            regVal = GetRegistryKey(registryPath, "HistoryColorCustomBottom");
            if (regVal != null)
            {
                RegistryKeyCheckOrCreate(registryPath, "HistoryColorCustomEntry", regVal);
                DeleteRegistryKey(registryPath, "HistoryColorCustomBottom");
            }

            // Convert "HistoryColorCustomText" => "HistoryColorCustomEntryText"
            regVal = GetRegistryKey(registryPath, "HistoryColorCustomText");
            if (regVal != null)
            {
                RegistryKeyCheckOrCreate(registryPath, "HistoryColorCustomEntryText", regVal);
                DeleteRegistryKey(registryPath, "HistoryColorCustomText");
            }

            // Convert "ScreenSelection" => new unique screen setup identification
            regVal = GetRegistryKey(registryPath, "ScreenSelection");
            if (regVal != null)
            {
                string displaysId = GetUniqueDisplayLayout();
                RegistryKeyCheckOrCreate(registryPathDisplays, displaysId, regVal);
                DeleteRegistryKey(registryPath, "HistoryColorCustomText");
            }

            // Delete "CheckUpdates"
            regVal = GetRegistryKey(registryPath, "CheckUpdates");
            if (regVal != null)
            {
                DeleteRegistryKey(registryPath, "CheckUpdates");
            }

            // Delete "HistoryColorCustomBorder"
            regVal = GetRegistryKey(registryPath, "HistoryColorCustomBorder");
            if (regVal != null)
            {
                DeleteRegistryKey(registryPath, "HistoryColorCustomBorder");
            }
        }


        // ###########################################################################################
        // Initialize registry - check if all registry paths and keys have been created
        // ###########################################################################################

        private void InitializeRegistry()
        {

            string regVal;

            // Check if the following registry entries exists - if not, then create them with their default values

            // Screen - Hotkeys
            RegistryKeyCheckOrCreate(registryPath, "HotkeyBehaviour", registryHotkeyBehaviour);
            RegistryKeyCheckOrCreate(registryPath, "Hotkey1", registryHotkeyToggleApplication);
            RegistryKeyCheckOrCreate(registryPath, "Hotkey2", registryHotkeyGetOlderEntry);
            RegistryKeyCheckOrCreate(registryPath, "Hotkey3", registryHotkeyGetNewerEntry);
            RegistryKeyCheckOrCreate(registryPath, "Hotkey4", registryHotkeyPasteOnHotkey);
            RegistryKeyCheckOrCreate(registryPath, "Hotkey5", registryHotkeyToggleFavorite);
            RegistryKeyCheckOrCreate(registryPath, "Hotkey6", registryHotkeyToggleView);
            Logging.Log("  Hotkeys:");
            regVal = GetRegistryKey(registryPath, "HotkeyBehaviour");
            Logging.Log("    \"HotkeyBehaviour\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "Hotkey1");
            Logging.Log("    \"Hotkey1\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "Hotkey2");
            Logging.Log("    \"Hotkey2\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "Hotkey3");
            Logging.Log("    \"Hotkey3\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "Hotkey4");
            Logging.Log("    \"Hotkey4\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "Hotkey5");
            Logging.Log("    \"Hotkey5\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "Hotkey6");
            Logging.Log("    \"Hotkey6\" = [" + regVal + "]");

            // Screen - General
            if (firstTimeLaunch)
            {
                // Set "Start with Windows"
                RegistryKeyCheckOrCreate(registryPathRun, "HovText", "\"" + Application.ExecutablePath + "\" --start-minimized");
            }

            RegistryKeyCheckOrCreate(registryPath, "CheckedVersion", appVer);
            RegistryKeyCheckOrCreate(registryPath, "CloseMinimizes", registryCloseMinimizes);
            RegistryKeyCheckOrCreate(registryPath, "RestoreOriginal", registryRestoreOriginal);
            RegistryKeyCheckOrCreate(registryPath, "HistoryEnable", registryEnableHistory);
            RegistryKeyCheckOrCreate(registryPath, "FavoritesEnable", registryEnableFavorites);
            RegistryKeyCheckOrCreate(registryPath, "CopyImages", registryCopyImages);
            RegistryKeyCheckOrCreate(registryPath, "PasteOnSelection", registryPasteOnSelection);
            RegistryKeyCheckOrCreate(registryPath, "TrimWhitespaces", registryTrimWhitespaces);
            Logging.Log("Startup registry values:"); 
            Logging.Log("  General:");
            regVal = GetRegistryKey(registryPath, "CheckedVersion");
            Logging.Log("    \"CheckedVersion\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "CloseMinimizes");
            Logging.Log("    \"CloseMinimizes\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "RestoreOriginal");
            Logging.Log("    \"RestoreOriginal\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "HistoryEnable");
            Logging.Log("    \"HistoryEnable\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "FavoritesEnable");
            Logging.Log("    \"FavoritesEnable\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "CopyImages");
            Logging.Log("    \"CopyImages\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "PasteOnSelection");
            Logging.Log("    \"PasteOnSelection\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "TrimWhitespaces");
            Logging.Log("    \"TrimWhitespaces\" = [" + regVal + "]");

            // Get the main system display (0-indexed) and the unique identifier for the display setup
            activeDisplay = GetPrimaryDisplay();
            string displaysId = GetUniqueDisplayLayout();

            // Screen - Layout
            RegistryKeyCheckOrCreate(registryPath, "HistoryEntries", historyListElements.ToString());
            RegistryKeyCheckOrCreate(registryPath, "HistorySizeWidth", historySizeWidth.ToString());
            RegistryKeyCheckOrCreate(registryPath, "HistorySizeHeight", historySizeHeight.ToString());
            RegistryKeyCheckOrCreate(registryPath, "HistoryMargin", historyMargin.ToString());
            RegistryKeyCheckOrCreate(registryPath, "HistoryLocation", historyLocation);
            RegistryKeyCheckOrCreate(registryPathDisplays, displaysId, activeDisplay.ToString());
            Logging.Log("  Layout:");
            regVal = GetRegistryKey(registryPath, "HistoryEntries");
            Logging.Log("    \"HistoryEntries\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "HistorySizeWidth");
            Logging.Log("    \"HistorySizeWidth\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "HistorySizeHeight");
            Logging.Log("    \"HistorySizeHeight\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "HistoryMargin");
            Logging.Log("    \"HistoryMargin\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "HistoryLocation");
            Logging.Log("    \"HistoryLocation\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPathDisplays, displaysId);
            Logging.Log("    \"Display ID\" = [" + regVal + "]");

            // Screen - Style
            RegistryKeyCheckOrCreate(registryPath, "HistoryFontFamily", historyFontFamily);
            RegistryKeyCheckOrCreate(registryPath, "HistoryFontSize", historyFontSize.ToString());
            RegistryKeyCheckOrCreate(registryPath, "HistoryBorderThickness", historyBorderThickness.ToString());
            RegistryKeyCheckOrCreate(registryPath, "HistoryColorTheme", historyColorTheme);
            RegistryKeyCheckOrCreate(registryPath, "HistoryColorCustomHeader", historyColorsHeader["Custom"]);
            RegistryKeyCheckOrCreate(registryPath, "HistoryColorCustomHeaderText", historyColorsHeaderText["Custom"]);
            RegistryKeyCheckOrCreate(registryPath, "HistoryColorCustomEntry", historyColorsEntry["Custom"]);
            RegistryKeyCheckOrCreate(registryPath, "HistoryColorCustomEntryText", historyColorsEntryText["Custom"]);
            RegistryKeyCheckOrCreate(registryPath, "HistoryColorCustomActive", historyColorsActive["Custom"]);
            RegistryKeyCheckOrCreate(registryPath, "HistoryColorCustomActiveText", historyColorsActiveText["Custom"]);
            RegistryKeyCheckOrCreate(registryPath, "HistoryColorBorder", historyColorBorder);
            RegistryKeyCheckOrCreate(registryPath, "IconSet", iconSet);
            Logging.Log("  Style:");
            regVal = GetRegistryKey(registryPath, "HistoryFontFamily");
            Logging.Log("    \"HistoryFontFamily\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "HistoryFontSize");
            Logging.Log("    \"HistoryFontSize\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "HistoryBorderActive");
            Logging.Log("    \"HistoryBorderActive\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "HistoryColorTheme");
            Logging.Log("    \"HistoryColorTheme\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "HistoryColorCustomTop");
            Logging.Log("    \"HistoryColorCustomTop\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "HistoryColorCustomBottom");
            Logging.Log("    \"HistoryColorCustomBottom\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "HistoryColorCustomActive");
            Logging.Log("    \"HistoryColorCustomActive\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "HistoryColorCustomText");
            Logging.Log("    \"HistoryColorCustomText\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "HistoryColorCustomBorder");
            Logging.Log("    \"HistoryColorCustomBorder\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "IconSet");
            Logging.Log("    \"IconSet\" = [" + regVal + "]");

            // Advanced
            RegistryKeyCheckOrCreate(registryPath, "TroubleshootEnable", registryTroubleshootEnable);
            Logging.Log("  Advanced:");
            regVal = GetRegistryKey(registryPath, "TroubleshootEnable");
            Logging.Log("    \"TroubleshootEnable\" = [" + regVal + "]");

             // Misc
             RegistryKeyCheckOrCreate(registryPath, "NotificationShown", "0");
            Logging.Log("  Misc:");
            regVal = GetRegistryKey(registryPath, "NotificationShown");
            Logging.Log("    \"NotificationShown\" = [" + regVal + "]");
        }


        // ###########################################################################################
        // Check if the registry path exists - if not then create it
        // ###########################################################################################

        private static void RegistryPathCheckOrCreate(string regPath)
        {
            // Check if the path exists in HKEY_CURRENT_USER\SOFTWARE registry - if not, then create it
            using (RegistryKey registryPathExists = Registry.CurrentUser.OpenSubKey(regPath))
            {
                if (registryPathExists == null)
                {
                    Registry.CurrentUser.CreateSubKey(regPath);
                    Logging.Log("Created registry path [" + regPath + "]");
                }
            }

        }


        // ###########################################################################################
        // Check if the registry key exists - if not then create it and set default value.
        // It has two methods - one with a string or one with an array of strings
        // ###########################################################################################

        private static void RegistryKeyCheckOrCreate(string regPath, string regKey, string regValue)
        {
            // Check if the registry key is set - if not then set default value
            using (RegistryKey registryPathExists = Registry.CurrentUser.OpenSubKey(regPath, true))
            {
                if (registryPathExists.GetValue(regKey) == null)
                {
                    SetRegistryKey(regPath, regKey, regValue);
                }
            }
        }


        // ###########################################################################################
        // Get regsitry key value
        // ###########################################################################################

        public static string GetRegistryKey(string path, string key)
        {
            using (RegistryKey getKey = Registry.CurrentUser.OpenSubKey(path))
            {
                if (getKey != null)
                {
                    return (string)(getKey.GetValue(key));
                }
                else
                {
                    return "";
                }
            }
        }


        // ###########################################################################################
        // Set registry key value
        // ###########################################################################################

        public static void SetRegistryKey(string path, string key, string value)
        {
            using (RegistryKey setKey = Registry.CurrentUser.OpenSubKey(path, true))
            {
                // First check if the key is already created (only relevant for logging)
                string getKey = GetRegistryKey(path, key);

                // Create or modify the value
                setKey.SetValue(key, value);

                // Log it
                if (getKey == null)
                {
                    Logging.Log("Created registry key \"" + key + "\" with value [" + value + "] in [" + path + "]");
                }
                else
                {
                    // Only log if there really is a modification done
                    if (value != getKey)
                    {
                        Logging.Log("Modified registry key \"" + key + "\" to value [" + value + "] in [" + path + "]");
                    }
                }
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
                Logging.Log("Delete registry key \""+ key +"\" from ["+ path +"]");
            }
        }


        // ###########################################################################################
        // Set up everything when launching application
        // ###########################################################################################

        private void GetStartupSettings()
        {
            // ------------------------------------------
            // "Hotkeys" tab
            // ------------------------------------------

            // Hotkeys
            string hotkey1 = GetRegistryKey(registryPath, "Hotkey1");
            string hotkey2 = GetRegistryKey(registryPath, "Hotkey2");
            string hotkey3 = GetRegistryKey(registryPath, "Hotkey3");
            string hotkey4 = GetRegistryKey(registryPath, "Hotkey4");
            string hotkey5 = GetRegistryKey(registryPath, "Hotkey5");
            string hotkey6 = GetRegistryKey(registryPath, "Hotkey6");
            hotkey1 = hotkey1.Length == 0 ? "Not set" : hotkey1;
            hotkey2 = hotkey2.Length == 0 ? "Not set" : hotkey2;
            hotkey3 = hotkey3.Length == 0 ? "Not set" : hotkey3;
            hotkey4 = hotkey4.Length == 0 ? "Not set" : hotkey4;
            hotkey5 = hotkey5.Length == 0 ? "Not set" : hotkey5;
            hotkey6 = hotkey6.Length == 0 ? "Not set" : hotkey6;
            uiHotkeyEnable.Text = hotkey1;
            uiHotkeyOlder.Text = hotkey2;
            uiHotkeyNewer.Text = hotkey3;
            uiHotkeyPaste.Text = hotkey4;
            uiHotkeyToggleFavorite.Text = hotkey5;
            uiHotkeyToggleView.Text = hotkey6;
            SetHotkeys("Startup of application");

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


            // ------------------------------------------
            // "General" tab
            // ------------------------------------------

            // Start with Windows
            string getKey = GetRegistryKey(registryPathRun, "HovText");
            if (getKey == null)
            {
                uiStartWithWindows.Checked = false;
                Logging.Log("Start with Windows = [No]");
            }
            else
            {
                uiStartWithWindows.Checked = true;
                Logging.Log("Start with Windows = [Yes]");

                // Overwrite "Run" if it does not contain "HovText.exe" or "--start-minimized"
                string runEntry = GetRegistryKey(registryPathRun, "HovText");
                string thisEntry = "\"" + Application.ExecutablePath + "\" --start-minimized";
                if (runEntry != thisEntry)
                {
                    SetRegistryKey(registryPathRun, "HovText", "\"" + Application.ExecutablePath + "\" --start-minimized");
                }
            }

            // Check for updates online          
/*
            string checkUpdates = GetRegistryKey(registryPath, "CheckUpdates");
            uiCheckUpdates.Checked = checkUpdates == "1" ? true : false;
            uiCheckUpdates.Checked = true; // force check for new version - this is the price to pay for HovText :-)
            if (uiCheckUpdates.Checked)
            {
*/
                updateTimer.Enabled = true;
                uiDevelopmentVersion.Enabled = true;
                uiDevelopmentWarning.Enabled = true;
                uiDevelopmentVersion.Text = " Wait - checking ...";
                Logging.Log("Version check timer started");
//            }

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

            // Enable favorites
            int favoritesEnabled = int.Parse((string)GetRegistryKey(registryPath, "FavoritesEnable"));
            uiFavoritesEnabled.Checked = favoritesEnabled == 1 ? true : false;
            isEnabledFavorites = uiFavoritesEnabled.Checked;
            if (isEnabledFavorites)
            {
                uiHotkeyToggleFavorite.Enabled = true;
                uiHotkeyToggleView.Enabled = true;
            }
            else
            {
                uiHotkeyToggleFavorite.Enabled = false;
                uiHotkeyToggleView.Enabled = false;
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
                uiFavoritesEnabled.Enabled = true;
                if (uiFavoritesEnabled.Checked)
                {
                    uiHotkeyToggleFavorite.Enabled = true;
                    uiHotkeyToggleView.Enabled = true;
                }

            }
            else
            {
                uiHotkeyOlder.Enabled = false;
                uiHotkeyNewer.Enabled = false;
                uiPasteOnSelection.Enabled = false;
                uiFavoritesEnabled.Enabled = false;
                uiHotkeyToggleFavorite.Enabled = false;
                uiHotkeyToggleView.Enabled = false;
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
            // "Apperance" tab
            // ------------------------------------------

            // History entries
            historyListElements = Int32.Parse(GetRegistryKey(registryPath, "HistoryEntries"));
            uiHistoryElements.Value = historyListElements;
            labelHistoryElements.Text = historyListElements.ToString();

            // History area size
            historySizeWidth = Int32.Parse(GetRegistryKey(registryPath, "HistorySizeWidth"));
            historySizeHeight = Int32.Parse(GetRegistryKey(registryPath, "HistorySizeHeight"));
            historyMargin = Int32.Parse(GetRegistryKey(registryPath, "HistoryMargin"));
            uiHistorySizeWidth.Value = historySizeWidth;
            uiHistorySizeHeight.Value = historySizeHeight;
            uiHistoryMargin.Value = historyMargin;
            labelHistorySizeWidth.Text = historySizeWidth.ToString() + "%";
            labelHistorySizeHeight.Text = historySizeHeight.ToString() + "%";
            labelHistoryMargin.Text = historyMargin.ToString() + "px";
            CheckIfDisableHistoryMargin();

            // History border thickness
            historyBorderThickness = Int32.Parse(GetRegistryKey(registryPath, "HistoryBorderThickness"));
            uiBorderThickness.Value = historyBorderThickness;
            labelBorderThickness.Text = historyBorderThickness.ToString();

            // History color theme
            historyColorTheme = GetRegistryKey(registryPath, "HistoryColorTheme");
            historyColorsHeader["Custom"] = GetRegistryKey(registryPath, "HistoryColorCustomHeader");
            historyColorsHeaderText["Custom"] = GetRegistryKey(registryPath, "HistoryColorCustomHeaderText");
            historyColorsEntry["Custom"] = GetRegistryKey(registryPath, "HistoryColorCustomEntry");
            historyColorsEntryText["Custom"] = GetRegistryKey(registryPath, "HistoryColorCustomEntryText");
            historyColorsActive["Custom"] = GetRegistryKey(registryPath, "HistoryColorCustomActive");
            historyColorsActiveText["Custom"] = GetRegistryKey(registryPath, "HistoryColorCustomActiveText");
            historyColorBorder = GetRegistryKey(registryPath, "HistoryColorBorder");
            EnableDisableCustomColor();
            EnableDisableBorderColor();
            switch (historyColorTheme)
            {
                case "Blue":
                    uiHistoryColorThemeBlue.Checked = true;
                    break;
                case "Brown":
                    uiHistoryColorThemeBrown.Checked = true;
                    break;
                case "Green":
                    uiHistoryColorThemeGreen.Checked = true;
                    break;
                case "Contrast":
                    uiHistoryColorThemeContrast.Checked = true;
                    break;
                case "Custom":
                    uiHistoryColorThemeCustom.Checked = true;
                    EnableDisableCustomColor();
                    break;
                default: // Yellow
                    uiHistoryColorThemeYellow.Checked = true;
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
            uiShowFontHeader.Font = new Font(historyFontFamily, historyFontSize);
            uiShowFontActive.Font = new Font(historyFontFamily, historyFontSize);
            uiShowFontActive.Text = "Active entry\r\n" + historyFontFamily + ", " + historyFontSize;
            uiShowFontEntry.Font = new Font(historyFontFamily, historyFontSize);
            uiShowFontEntry.Text = "Entry\r\n" + historyFontFamily + ", " + historyFontSize;
            SetHistoryColors();

            // Icon set
            iconSet = GetRegistryKey(registryPath, "IconSet");
            if(iconSet == "SquareOld")
            {
                uiIconsSquareOld.Select();
            }

            // Display selection
            string displaysId = GetUniqueDisplayLayout();
            int displayReg = Int32.Parse(GetRegistryKey(registryPathDisplays, displaysId));
            PopulateDisplaySetup();
            if (IsDisplayValid(displayReg))
            {
                activeDisplay = displayReg;
                Logging.Log("History will be shown on display ID [" + activeDisplay + "] with registry entry ["+ displaysId +"]");
            }
            else
            {
                Logging.Log("History cannot be shown on display ID [" + displayReg + "] and will instead be shown on display ID ["+ activeDisplay +"] with registry entry ["+ displaysId +"]");
            }
            uiDisplayGroup.Controls["uiScreen" + activeDisplay].Select();


            // ------------------------------------------
            // "Advanced" tab
            // ------------------------------------------

            // Troubleshooting
            int troubleshootEnable = int.Parse((string)GetRegistryKey(registryPath, "TroubleshootEnable"));
            uiTroubleshootEnabled.Checked = troubleshootEnable == 1 ? true : false;
            if (buildType == "Debug")
            {
                // Enable troubleshoot logfile, if this is a DEVELOPMENT version
                uiTroubleshootEnabled.Checked = true;
            }
            string curFile = @troubleshootLogfile;
            if (File.Exists(curFile))
            {
                uiTroubleshootOpenLocation.Enabled = true;
                uiTroubleshootDeleteFile.Enabled = true;
            }
            else
            {
                uiTroubleshootOpenLocation.Enabled = false;
                uiTroubleshootDeleteFile.Enabled = false;
            }
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
                Logging.Log("Changed \"Start with Windows\" from [No] to [Yes]");
            }
            else
            {
                DeleteRegistryKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", "HovText");
                Logging.Log("Changed \"Start with Windows\" from [Yes] to [No]");
            }
        }


        // ###########################################################################################
        // Show the history font dialouge
        // ###########################################################################################

        private void uiChangeFont_Click(object sender, EventArgs e)
        {
            FontDialog fontDlg = new FontDialog();
            fontDlg.Font = uiShowFontActive.Font; // initialize the font dialouge with the font from "uiShowFont"
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
                uiShowFontHeader.Font = new Font(historyFontFamily, historyFontSize);
                uiShowFontActive.Text = historyFontFamily + ", " + historyFontSize;
                uiShowFontActive.Font = new Font(historyFontFamily, historyFontSize);
                uiShowFontEntry.Text = historyFontFamily + ", " + historyFontSize;
                uiShowFontEntry.Font = new Font(historyFontFamily, historyFontSize);
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
        }


        // ###########################################################################################
        // Disable history margin, if width or height is more tyhan 90%
        // ###########################################################################################

        private void CheckIfDisableHistoryMargin()
        {
            if (uiHistorySizeWidth.Value > 90 || uiHistorySizeHeight.Value > 90)
            {
                label16.Enabled = false;
                uiHistoryMargin.Enabled = false;
                labelHistoryMargin.Enabled = false;
                isHistoryMarginEnabled = false;
            }
            else
            {
                label16.Enabled = true;
                uiHistoryMargin.Enabled = true;
                labelHistoryMargin.Enabled = true;
                isHistoryMarginEnabled = true;
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

            // Disable margin, if above 90%
            CheckIfDisableHistoryMargin();
        }

        private void historySizeHeight_ValueChanged(object sender, EventArgs e)
        {
            labelHistorySizeHeight.Text = uiHistorySizeHeight.Value.ToString() + "%";
            historySizeHeight = uiHistorySizeHeight.Value;
            SetRegistryKey(registryPath, "HistorySizeHeight", historySizeHeight.ToString());

            // Disable margin, if above 90%
            CheckIfDisableHistoryMargin();
        }

        private void uiHistoryMargin_ValueChanged(object sender, EventArgs e)
        {
            labelHistoryMargin.Text = uiHistoryMargin.Value.ToString() + "px";
            historyMargin = uiHistoryMargin.Value;
            SetRegistryKey(registryPath, "HistoryMargin", historyMargin.ToString());
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
            if (uiHistoryColorThemeCustom.Checked)
            {
                uiCustomHeader.Enabled = true;
                uiCustomHeaderText.Enabled = true;
                uiCustomEntry.Enabled = true;
                uiCustomEntryText.Enabled = true;
                uiCustomActive.Enabled = true;
                uiCustomActiveText.Enabled = true;
                groupBoxCustom.Enabled = true;
                labelCustomHeader.Enabled = true;
                labelCustomHeaderText.Enabled = true;
                labelCustomEntry.Enabled = true;
                labelCustomEntryText.Enabled = true;
                labelCustomActive.Enabled = true;
                labelCustomActiveText.Enabled = true;

                uiCustomHeader.BackColor = ColorTranslator.FromHtml(historyColorsHeader["Custom"]);
                uiCustomHeaderText.BackColor = ColorTranslator.FromHtml(historyColorsHeaderText["Custom"]);
                uiCustomEntry.BackColor = ColorTranslator.FromHtml(historyColorsEntry["Custom"]);
                uiCustomEntryText.BackColor = ColorTranslator.FromHtml(historyColorsEntryText["Custom"]);
                uiCustomActive.BackColor = ColorTranslator.FromHtml(historyColorsActive["Custom"]);
                uiCustomActiveText.BackColor = ColorTranslator.FromHtml(historyColorsActiveText["Custom"]);
            }
            else
            {
                uiCustomHeader.Enabled = false;
                uiCustomHeaderText.Enabled = false;
                uiCustomEntry.Enabled = false;
                uiCustomEntryText.Enabled = false;
                uiCustomActive.Enabled = false;
                uiCustomActiveText.Enabled = false;
                groupBoxCustom.Enabled = false;
                labelCustomHeader.Enabled = false;
                labelCustomHeaderText.Enabled = false;
                labelCustomEntry.Enabled = false;
                labelCustomEntryText.Enabled = false;
                labelCustomActive.Enabled = false;
                labelCustomActiveText.Enabled = false;

                uiCustomHeader.BackColor = Color.WhiteSmoke;
                uiCustomHeaderText.BackColor = Color.WhiteSmoke;
                uiCustomEntry.BackColor = Color.WhiteSmoke;
                uiCustomEntryText.BackColor = Color.WhiteSmoke;
                uiCustomActive.BackColor = Color.WhiteSmoke;
                uiCustomActiveText.BackColor = Color.WhiteSmoke;
            }
        }


        // ###########################################################################################
        // Enable or disable the border color
        // ###########################################################################################

        private void EnableDisableBorderColor()
        {

            // Check if the "History border" checkbox is checked or if the element is enabled
            if (historyBorderThickness > 0)
            {
                uiCustomBorder.Enabled = true;
                labelCustomBorder.Enabled = true;
                uiCustomBorder.BackColor = ColorTranslator.FromHtml(historyColorBorder);
            }
            else
            {
                uiCustomBorder.Enabled = false;
                labelCustomBorder.Enabled = false;
                uiCustomBorder.BackColor = Color.WhiteSmoke;
            }
        }

                
        // ###########################################################################################
        // Change in the history color
        // ###########################################################################################

        private void uiHistoryColorTheme_CheckedChanged(object sender, EventArgs e)
        {
            // Get the text name for the clicked radio item
            historyColorTheme = (sender as RadioButton).Text;

            SetRegistryKey(registryPath, "HistoryColorTheme", historyColorTheme);
            SetHistoryColors();

            // Do something special for the custom
            if (historyColorTheme == "Custom")
            {
                EnableDisableCustomColor();
            }
        }


        // ###########################################################################################
        // Set the history colors (and selected value in UI)
        // ###########################################################################################

        private void SetHistoryColors()
        {
            uiShowFontHeader.BackColor = ColorTranslator.FromHtml(historyColorsHeader[historyColorTheme]);
            uiShowFontHeader.ForeColor = ColorTranslator.FromHtml(historyColorsHeaderText[historyColorTheme]);
            uiShowFontActive.BackColor = ColorTranslator.FromHtml(historyColorsActive[historyColorTheme]);
            uiShowFontActive.ForeColor = ColorTranslator.FromHtml(historyColorsActiveText[historyColorTheme]);
            uiShowFontEntry.BackColor = ColorTranslator.FromHtml(historyColorsEntry[historyColorTheme]);
            uiShowFontEntry.ForeColor = ColorTranslator.FromHtml(historyColorsEntryText[historyColorTheme]);
        }


        // ###########################################################################################
        // Changes in "Check for updates online"
        // ###########################################################################################

        private void uiCheckUpdates_CheckedChanged(object sender, EventArgs e)
        {
/*
            string status = uiCheckUpdates.Checked ? "1" : "0";
            SetRegistryKey(registryPath, "CheckUpdates", status);
            if (uiCheckUpdates.Checked)
            {            
                updateTimer.Enabled = true;
                Logging.Log("Version check timer started");
                uiDevelopmentVersion.Enabled = true;
                uiDevelopmentWarning.Enabled = true;
                uiDevelopmentVersion.Text = " Wait - checking ...";
            }
            else
            {
                updateTimer.Enabled = false;
                uiDevelopmentVersion.Enabled = false;
                uiDevelopmentWarning.Enabled = false;
                uiDevelopmentDownload.Enabled = false;
                uiDevelopmentVersion.Text = "  Enable \"Check for updates online\"";
            }
*/
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
                uiFavoritesEnabled.Enabled = true;
                if (uiFavoritesEnabled.Checked)
                {
                    uiHotkeyToggleFavorite.Enabled = true;
                    uiHotkeyToggleView.Enabled = true;
                }
            }
            else
            {
                uiHotkeyOlder.Enabled = false;
                uiHotkeyNewer.Enabled = false;
                uiCopyImages.Enabled = false;
                uiPasteOnSelection.Enabled = false;
                uiFavoritesEnabled.Enabled = false;
                uiHotkeyToggleFavorite.Enabled = false;
                uiHotkeyToggleView.Enabled = false;
                showFavoriteList = false;
            }

            // Enable/disable hotkeys
            SetHotkeys("Enable history change");

            // Update the tray icon
            UpdateNotifyIconText();
        }


        // ###########################################################################################
        // Changes in "Enable favorites"
        // ###########################################################################################

        private void uiFavoritesEnabled_CheckedChanged(object sender, EventArgs e)
        {
            string status = uiFavoritesEnabled.Checked ? "1" : "0";
            isEnabledFavorites = uiFavoritesEnabled.Checked;
            SetRegistryKey(registryPath, "FavoritesEnable", status);
            if (isEnabledFavorites)
            {
                uiHotkeyToggleFavorite.Enabled = true;
                uiHotkeyToggleView.Enabled = true;
            }
            else
            {
                uiHotkeyToggleFavorite.Enabled = false;
                uiHotkeyToggleView.Enabled = false;
                showFavoriteList = false;

                // Reset all favorites
                if (entriesIsFavorite.Count > 0)
                {
                    for (int i = 0; i <= entriesIsFavorite.ElementAt(Settings.entriesIsFavorite.Count - 1).Key; i++)
                    {
                        bool doesKeyExist = Settings.entriesText.ContainsKey(i);
                        if (doesKeyExist)
                        {
                            entriesIsFavorite[i] = false;
                        }
                    }
                }
            }
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
        // When application starts up then check if it should be minimized at once (when started with Windows)
        // https://stackoverflow.com/a/8486441/2028935
        // ###########################################################################################

        private void MainWindow_Shown(object sender, EventArgs e)
        {
            string arg0 = Program.arg0;
            if (arg0.Contains("--start-minimized"))
            {
                WindowState = FormWindowState.Minimized;
                Hide();
                Logging.Log("Started HovText minimized");
            }
            else
            {
                Logging.Log("Started HovText in window mode");
            }
        }


        // ###########################################################################################
        // When clicking the "About" in the tray icon menu
        // ###########################################################################################

        private void trayIconAbout_Click(object sender, EventArgs e)
        {
            Logging.Log("Clicked tray icon \"About\""); 
            ShowSettingsForm();
            tabControl.SelectedIndex = 6; // About
        }


        // ###########################################################################################
        // When clicking the "Settings" in the tray icon menu
        // ###########################################################################################

        private void trayIconSettings_Click(object sender, EventArgs e)
        {
            ShowSettingsForm();
            tabControl.SelectedIndex = 0; // General
            Logging.Log("Clicked tray icon \"Settings\"");
        }


        // ###########################################################################################
        // When clicking the "Exit" in the tray icon menu
        // ###########################################################################################

        private void trayIconExit_Click(object sender, EventArgs e)
        {
            Logging.Log("Clicked tray icon \"Exit\"");
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
            }
        }

        private void mouseClickTimer_Tick(object sender, EventArgs e)
        {
            Logging.Log("Tray icon single-clicked - toggling application enable/disable");
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
                Logging.Log("Tray icon double-clicked - opening \"Settings\"");
                
                // Cancel the single-click
                mouseClickTimer.Stop();

                ShowSettingsForm();
                tabControl.SelectedIndex = 0;
            }
        }


        // ###########################################################################################
        // Show the "Settings" form
        // https://stackoverflow.com/a/11941579/2028935
        // ###########################################################################################

        public void ShowSettingsForm()
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
            if (iconSet == "SquareOld")
            {
                notifyIcon.Icon = Resources.Square_Old_Active_16x16;
                Icon = Resources.Square_Old_Active_16x16;
            }
            else
            {
                notifyIcon.Icon = Resources.Round_Active_48x48;
                Icon = Resources.Round_Active_48x48;
            }
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
            if (iconSet == "SquareOld")
            {
                notifyIcon.Icon = Resources.Square_Old_Hotkey_16x16;
                Icon = Resources.Square_Old_Hotkey_16x16;
            } else
            {
                notifyIcon.Icon = Resources.Round_Hotkey_48x48;
                Icon = Resources.Round_Hotkey_48x48;
            }
        }


        // ###########################################################################################
        // Clicking the tab help button
        // ###########################################################################################

        private void uiHelp_Click(object sender, EventArgs e)
        {
            string selectedTab = tabControl.SelectedTab.AccessibilityObject.Name;

            // Show the specific help for a "Development" version
            string releaseTrain = "";
            releaseTrain += buildType == "Debug"? "-dev" : "";

            System.Diagnostics.Process.Start(hovtextPage +"documentation"+ releaseTrain +"/#"+ selectedTab);
            Logging.Log("Clicked the \"Help\" for \""+ selectedTab +"\"");
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
            ToggleEnabled();
            e.Handled = true;
        }


        // ###########################################################################################
        // Action for the "Show older entry" hotkey
        // ###########################################################################################

        private void HotkeyGetOlderEntry(object sender, NHotkey.HotkeyEventArgs e)
        {
            Logging.Log("Pressed the \"Get older history entry\" hotkey");
            GoEntryLowerNumber();
            isHistoryHotkeyPressed = true;
            e.Handled = true;
        }


        // ###########################################################################################
        // Action for the "Show newer entry" hotkey
        // ###########################################################################################

        private void HotkeyGetNewerEntry(object sender, NHotkey.HotkeyEventArgs e)
        {
            Logging.Log("Pressed the \"Get newer history entry\" hotkey");
            GoEntryHigherNumber();
            isHistoryHotkeyPressed = true;
            e.Handled = true;
        }


        // ###########################################################################################
        // Action for the "Paste on hotkey" hotkey
        // ###########################################################################################

        private void HotkeyPasteOnHotkey(object sender, NHotkey.HotkeyEventArgs e)
        {
            Logging.Log("Pressed the \"Paste only on hotkey\" hotkey");

            // Only proceed if there are history entries
            if (entriesText.Count > 0)
            {
                // Get active application and change focus to HovText
                originatingApplicationName = GetActiveApplication();
                ChangeFocusToThisApplication();

                // Show the invisible form, so we can catch the key-up event
                pasteOnHotkey.Show();
                pasteOnHotkey.Activate();
                pasteOnHotkey.TopMost = true;
            }

            e.Handled = true;
        }


        // ###########################################################################################
        // On "key down" in one of the hotkey fields then convert that in to a string
        // ###########################################################################################

        private static string ConvertKeyboardInputToString(KeyEventArgs e)
        {
            string thisHotkey = "";

            // Check if any of the modifiers have been pressed also
            bool isShift = e.Shift;
            bool isAlt = e.Alt;
            bool isControl = e.Control;

            string keyCode = e.KeyCode.ToString();

            // Invalidate various keys
            keyCode = keyCode == "Apps" ? "Unsupported" : keyCode;
            keyCode = keyCode == "LWin" ? "Unsupported" : keyCode;
            keyCode = keyCode == "RWin" ? "Unsupported" : keyCode;
            keyCode = keyCode == "Menu" ? "Unsupported" : keyCode;
            keyCode = keyCode == "ControlKey" ? "Unsupported" : keyCode;
            keyCode = keyCode == "ShiftKey" ? "Unsupported" : keyCode;
            keyCode = hotkey == "hotkeyToggleFavorite" && (isShift || isAlt || isControl) ? "Unsupported" : keyCode;
            keyCode = hotkey == "hotkeyToggleView" && (isShift || isAlt || isControl) ? "Unsupported" : keyCode;

            // Build the hotkey string
            thisHotkey = isShift ? thisHotkey + "Shift + " : thisHotkey;
            thisHotkey = isAlt ? thisHotkey + "Alt + " : thisHotkey;
            thisHotkey = isControl ? thisHotkey + "Control + " : thisHotkey;
            thisHotkey += keyCode;

            // Invalidate if the key is unspported
            thisHotkey = keyCode == "Unsupported" ? "Unsupported" : thisHotkey;

            // Mark the hotkey as deleted if pressing "Delete" og "Backspace"
            thisHotkey = (keyCode == "Delete" || keyCode == "Back") && !isShift && !isAlt && !isControl ? "Not set" : thisHotkey;

            return thisHotkey;
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

        private void hotkeyToggleFavorite_KeyDown(object sender, KeyEventArgs e)
        {
            string hotkey = ConvertKeyboardInputToString(e);
            uiHotkeyToggleFavorite.Text = hotkey;
            if (e.Alt)
            {
                // https://stackoverflow.com/a/3068797/2028935
                e.SuppressKeyPress = true;
            }
        }

        private void hotkeyToggleView_KeyDown(object sender, KeyEventArgs e)
        {
            string hotkey = ConvertKeyboardInputToString(e);
            uiHotkeyToggleView.Text = hotkey;
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
            hotkey = "hotkeyEnable";
            ModifyHotkey();
        }

        private void hotkeyOlder_Enter(object sender, EventArgs e)
        {
            hotkey = "hotkeyOlder";
            ModifyHotkey();
        }

        private void hotkeyNewer_Enter(object sender, EventArgs e)
        {
            hotkey = "hotkeyNewer";
            ModifyHotkey();
        }

        private void hotkeyPaste_Enter(object sender, EventArgs e)
        {
            hotkey = "hotkeyPaste";
            ModifyHotkey();
        }

        private void hotkeyToggleFavorite_Enter(object sender, EventArgs e)
        {
            hotkey = "hotkeyToggleFavorite";
            ModifyHotkey();
        }

        private void hotkeyToggleView_Enter(object sender, EventArgs e)
        {
            hotkey = "hotkeyToggleView";
            ModifyHotkey();
        }


        // ###########################################################################################
        // Color the hotkey field and enable the "Apply" and "Cancel" buttons
        // ###########################################################################################

        private void ModifyHotkey()
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
                case "hotkeyToggleFavorite":
                    uiHotkeyToggleFavorite.BackColor = SystemColors.Info;
                    break;
                case "hotkeyToggleView":
                    uiHotkeyToggleView.BackColor = SystemColors.Info;
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

        public static void RemoveAllHotkeys()
        {
            HotkeyManager.Current.Remove("ToggleApplication");
            HotkeyManager.Current.Remove("GetOlderEntry");
            HotkeyManager.Current.Remove("GetNewerEntry");
            HotkeyManager.Current.Remove("PasteOnHotkey");
            Logging.Log("[Hotkey1] removed");
            Logging.Log("[Hotkey2] removed");
            Logging.Log("[Hotkey3] removed");
            Logging.Log("[Hotkey4] removed");
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
            Logging.Log("Called \"SetHotkeys()\" from \"" + from +"\"");
            
            // Get all hotkey strings
            string hotkey1 = uiHotkeyEnable.Text;
            string hotkey2 = uiHotkeyOlder.Text;
            string hotkey3 = uiHotkeyNewer.Text;
            string hotkey4 = uiHotkeyPaste.Text;
            string hotkey5 = uiHotkeyToggleFavorite.Text;
            string hotkey6 = uiHotkeyToggleView.Text;

            string conflictText = "";
            KeysConverter cvt;
            Keys key;

            // Convert the strings to hotkey objects

            // Hotkey 1, "Application toggle"
            if (
                hotkey1 != "Unsupported"
                && hotkey1 != "Not set"
                && hotkey1 != "Hotkey conflicts"
                )
            {
                try
                {
                    cvt = new KeysConverter();
                    key = (Keys)cvt.ConvertFrom(hotkey1);
                    HotkeyManager.Current.AddOrReplace("ToggleApplication", key, HotkeyToggleApplication);
                    Logging.Log("[Hotkey1] added as global hotkey and set to ["+ hotkey1 +"]");
                }
                catch (Exception ex)
                {
                    Logging.Log("Exception #6 raised (Settings):");
                    Logging.Log("  Hotkey [Hotkey1] conflicts");
                    Logging.Log("  "+ ex.Message);
                    if (ex.Message.Contains("Hot key is already registered"))
                    {
                        hotkey1 = "Hotkey conflicts";
                        conflictText += "Hotkey for \"Toggle application on/off\" conflicts with another application\r\n";
                    }
                }
            }

            // Hotkey 2, "Get older entry"
            if (
                hotkey2 != "Unsupported"
                && hotkey2 != "Not set"
                && hotkey2 != "Hotkey conflicts"
                && uiHistoryEnabled.Checked
                )
            {
                try
                {
                    cvt = new KeysConverter();
                    key = (Keys)cvt.ConvertFrom(hotkey2);
                    HotkeyManager.Current.AddOrReplace("GetOlderEntry", key, HotkeyGetOlderEntry);
                    Logging.Log("[Hotkey2] added as global hotkey and set to [" + hotkey2 + "]");
                }
                catch (Exception ex)
                {
                    Logging.Log("Exception #7 raised (Settings):");
                    Logging.Log("  Hotkey [Hotkey2] conflicts");
                    Logging.Log("  " + ex.Message); 
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
                hotkey3 != "Unsupported"
                && hotkey3 != "Not set"
                && hotkey3 != "Hotkey conflicts"
                && uiHistoryEnabled.Checked
                )
            {
                try
                {
                    cvt = new KeysConverter();
                    key = (Keys)cvt.ConvertFrom(hotkey3);
                    HotkeyManager.Current.AddOrReplace("GetNewerEntry", key, HotkeyGetNewerEntry);
                    Logging.Log("[Hotkey3] added as global hotkey and set to [" + hotkey3 + "]");
                }
                catch (Exception ex)
                {
                    Logging.Log("Exception #8 raised (Settings):");
                    Logging.Log("  Hotkey [Hotkey3] conflicts");
                    Logging.Log("  " + ex.Message); 
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
                hotkey4 != "Unsupported"
                && hotkey4 != "Not set"
                && hotkey4 != "Hotkey conflicts"
                && uiHotkeyBehaviourPaste.Checked
                )
            {
                try
                {
                    cvt = new KeysConverter();
                    key = (Keys)cvt.ConvertFrom(hotkey4);

                    // Only (re)enable it, if the "Action only on hotkey" behaviour has been chosen
                    if (uiHotkeyBehaviourPaste.Checked)
                    {
                        HotkeyManager.Current.AddOrReplace("PasteOnHotkey", key, HotkeyPasteOnHotkey);
                        Logging.Log("[Hotkey4] added as global hotkey and set to [" + hotkey4 + "]");
                    }
                }
                catch (Exception ex)
                {
                    Logging.Log("Exception #9 raised (Settings):");
                    Logging.Log("  Hotkey [Hotkey4] conflicts");
                    Logging.Log("  " + ex.Message); 
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
                hotkey4 != "Unsupported" && hotkey4 != "Hotkey conflicts" &&
                hotkey5 != "Unsupported" && hotkey5 != "Hotkey conflicts" &&
                hotkey6 != "Unsupported" && hotkey6 != "Hotkey conflicts"
                )
            {
                SetRegistryKey(registryPath, "Hotkey1", hotkey1);
                SetRegistryKey(registryPath, "Hotkey2", hotkey2);
                SetRegistryKey(registryPath, "Hotkey3", hotkey3);
                SetRegistryKey(registryPath, "Hotkey4", hotkey4);
                SetRegistryKey(registryPath, "Hotkey5", hotkey5);
                SetRegistryKey(registryPath, "Hotkey6", hotkey6);
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

            // Hotkey 5, "Toggle favorite entry"
            if (hotkey5 == "Unsupported" || hotkey5 == "Hotkey conflicts")
            {
                hasError = true;
                uiHotkeyToggleFavorite.Text = hotkey5;
                uiHotkeyToggleFavorite.BackColor = Color.DarkSalmon;
            }
            else
            {
                uiHotkeyToggleFavorite.BackColor = SystemColors.Window;
            }

            // Hotkey 6, "Toggle list view"
            if (hotkey6 == "Unsupported" || hotkey6 == "Hotkey conflicts")
            {
                hasError = true;
                uiHotkeyToggleView.Text = hotkey6;
                uiHotkeyToggleView.BackColor = Color.DarkSalmon;
            }
            else
            {
                uiHotkeyToggleView.BackColor = SystemColors.Window;
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
            string hotkey5 = GetRegistryKey(registryPath, "Hotkey5");
            string hotkey6 = GetRegistryKey(registryPath, "Hotkey6");
            uiHotkeyEnable.Text = hotkey1;
            uiHotkeyOlder.Text = hotkey2;
            uiHotkeyNewer.Text = hotkey3;
            uiHotkeyPaste.Text = hotkey4;
            uiHotkeyToggleFavorite.Text = hotkey5;
            uiHotkeyToggleView.Text = hotkey6;
            SetHotkeys("Cancel hotkeys button press");
            Logging.Log("Cancelling hotkeys association and reverting to previous values");
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
        // Donate
        // ###########################################################################################

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.paypal.com/donate?hosted_button_id=U23UUA8YWABGU");
            Logging.Log("Clicked the \"Donate\" picture in \"About\"");
        }


        // ###########################################################################################
        // Get customer color inputs
        // ###########################################################################################

        private void uiCustomHeader_Enter(object sender, EventArgs e)
        {
            colorDialogHeader.Color = uiShowFontHeader.BackColor;
            colorDialogHeader.FullOpen = true;
            if (colorDialogHeader.ShowDialog() == DialogResult.OK)
            {
                string color = String.Format("#{0:X2}{1:X2}{2:X2}", colorDialogHeader.Color.R, colorDialogHeader.Color.G, colorDialogHeader.Color.B);
                SetRegistryKey(registryPath, "HistoryColorCustomHeader", color);
                historyColorsHeader["Custom"] = color;
                uiCustomHeader.BackColor = colorDialogHeader.Color;
                SetHistoryColors();
            }
            ActiveControl = null; // do not focus this form - so we can reclick the textbox again right after
        }

        private void uiCustomHeaderText_Enter(object sender, EventArgs e)
        {
            colorDialogEntryText.Color = uiShowFontHeader.ForeColor;
            colorDialogEntryText.FullOpen = true;
            if (colorDialogEntryText.ShowDialog() == DialogResult.OK)
            {
                string color = String.Format("#{0:X2}{1:X2}{2:X2}", colorDialogEntryText.Color.R, colorDialogEntryText.Color.G, colorDialogEntryText.Color.B);
                SetRegistryKey(registryPath, "HistoryColorCustomHeaderText", color);
                historyColorsHeaderText["Custom"] = color;
                uiCustomHeaderText.BackColor = colorDialogEntryText.Color;
                SetHistoryColors();
            }
            ActiveControl = null; // do not focus this form - so we can reclick the textbox again right after
        }

        private void uiCustomEntry_Enter(object sender, EventArgs e)
        {
            colorDialogEntry.Color = uiShowFontActive.BackColor;
            colorDialogEntry.FullOpen = true;
            if (colorDialogEntry.ShowDialog() == DialogResult.OK)
            {
                string color = String.Format("#{0:X2}{1:X2}{2:X2}", colorDialogEntry.Color.R, colorDialogEntry.Color.G, colorDialogEntry.Color.B);
                SetRegistryKey(registryPath, "HistoryColorCustomEntry", color);
                historyColorsEntry["Custom"] = color;
                uiCustomEntry.BackColor = colorDialogEntry.Color;
                SetHistoryColors();
            }
            ActiveControl = null; // do not focus this form - so we can reclick the textbox again right after
        }

        private void uiCustomEntryText_Enter(object sender, EventArgs e)
        {
            colorDialogEntryText.Color = uiShowFontHeader.ForeColor;
            colorDialogEntryText.FullOpen = true;
            if (colorDialogEntryText.ShowDialog() == DialogResult.OK)
            {
                string color = String.Format("#{0:X2}{1:X2}{2:X2}", colorDialogEntryText.Color.R, colorDialogEntryText.Color.G, colorDialogEntryText.Color.B);
                SetRegistryKey(registryPath, "HistoryColorCustomEntryText", color);
                historyColorsEntryText["Custom"] = color;
                uiCustomEntryText.BackColor = colorDialogEntryText.Color;
                SetHistoryColors();
            }
            ActiveControl = null; // do not focus this form - so we can reclick the textbox again right after
        }

        private void uiCustomActive_Enter(object sender, EventArgs e)
        {
            colorDialogActive.Color = uiShowFontActive.BackColor;
            colorDialogActive.FullOpen = true;
            if (colorDialogActive.ShowDialog() == DialogResult.OK)
            {
                string color = String.Format("#{0:X2}{1:X2}{2:X2}", colorDialogActive.Color.R, colorDialogActive.Color.G, colorDialogActive.Color.B);
                SetRegistryKey(registryPath, "HistoryColorCustomActive", color);
                historyColorsActive["Custom"] = color;
                uiCustomActive.BackColor = colorDialogActive.Color;
                SetHistoryColors();
            }
            ActiveControl = null; // do not focus this form - so we can reclick the textbox again right after
        }

        private void uiCustomActiveText_Enter(object sender, EventArgs e)
        {
            colorDialogEntryText.Color = uiShowFontHeader.ForeColor;
            colorDialogEntryText.FullOpen = true;
            if (colorDialogEntryText.ShowDialog() == DialogResult.OK)
            {
                string color = String.Format("#{0:X2}{1:X2}{2:X2}", colorDialogEntryText.Color.R, colorDialogEntryText.Color.G, colorDialogEntryText.Color.B);
                SetRegistryKey(registryPath, "HistoryColorCustomActiveText", color);
                historyColorsActiveText["Custom"] = color;
                uiCustomActiveText.BackColor = colorDialogEntryText.Color;
                SetHistoryColors();
            }
            ActiveControl = null; // do not focus this form - so we can reclick the textbox again right after
        }

        private void uiCustomBorder_Enter(object sender, EventArgs e)
        {
            colorDialogBorder.FullOpen = true;
            if (colorDialogBorder.ShowDialog() == DialogResult.OK)
            {
                string color = String.Format("#{0:X2}{1:X2}{2:X2}", colorDialogBorder.Color.R, colorDialogBorder.Color.G, colorDialogBorder.Color.B);
                SetRegistryKey(registryPath, "HistoryColorBorder", color);
                historyColorBorder = color;
                uiCustomBorder.BackColor = colorDialogBorder.Color;
                uiShowFontActive.Refresh(); // update/redraw the border
            }
            ActiveControl = null; // do not focus this form - so we can reclick the textbox again right after
        }


        // ###########################################################################################
        // Take over redraw for a specific UI element, "history border"
        // ###########################################################################################

        private void uiShowFontBottom_Paint(object sender, PaintEventArgs e)
        {
            if (historyBorderThickness > 0)
            {
                // Set padding
                if (historyBorderThickness >= 2)
                {
                    uiShowFontActive.Padding = new Padding(historyBorderThickness - 2);
                    uiShowFontEntry.Padding = new Padding(historyBorderThickness - 2);
                } else
                {
                    uiShowFontActive.Padding = new Padding(historyBorderThickness - 1);
                    uiShowFontEntry.Padding = new Padding(historyBorderThickness - 1);
                }

                // Redraw border with a solid color
                ControlPaint.DrawBorder(e.Graphics, e.ClipRectangle,
                                ColorTranslator.FromHtml(Settings.historyColorBorder), historyBorderThickness, ButtonBorderStyle.Solid,
                                ColorTranslator.FromHtml(Settings.historyColorBorder), historyBorderThickness, ButtonBorderStyle.Solid,
                                ColorTranslator.FromHtml(Settings.historyColorBorder), historyBorderThickness, ButtonBorderStyle.Solid,
                                ColorTranslator.FromHtml(Settings.historyColorBorder), historyBorderThickness, ButtonBorderStyle.Solid);
            }
            else
            {
                ControlPaint.DrawBorder(e.Graphics, e.ClipRectangle, this.BackColor, ButtonBorderStyle.None);
            }
        }
        

        // ###########################################################################################
        // Troubleshooting, enable or disable
        // ###########################################################################################

        private void uiTroubleshootEnabled_CheckedChanged(object sender, EventArgs e)
        {
            if (uiTroubleshootEnabled.Checked)
            {
                if (!hasTroubleshootLogged)
                {
                    Logging.StartLogging();
                }
                hasTroubleshootLogged = true;
            }
            else
            {
                Logging.EndLogging();
                hasTroubleshootLogged = false;
            }

            // Save it to the registry
            string status = uiTroubleshootEnabled.Checked ? "1" : "0";
            isTroubleshootEnabled = uiTroubleshootEnabled.Checked;
            SetRegistryKey(registryPath, "TroubleshootEnable", status);

            // If there is a logfile present then enable the UI fields for it
            if (File.Exists(troubleshootLogfile))
            {
                uiTroubleshootOpenLocation.Enabled = true;
                uiTroubleshootDeleteFile.Enabled = true;
            }
        }


        // ###########################################################################################
        // Troubleshooting, open explorer location for the logfile and highlight the file
        // ###########################################################################################

        private void uiTroubleshootOpenLocation_Click(object sender, EventArgs e)
        {
            if (File.Exists(troubleshootLogfile))
            {
                // https://stackoverflow.com/a/696144/2028935
                string argument = "/select, \"" + troubleshootLogfile + "\"";
                string folder = argument.Substring(6);
                Process.Start("explorer.exe", argument);
                Logging.Log("Clicked the \"Open logfile location\"");
            }
            else
            {
                MessageBox.Show(troubleshootLogfile +" does not exists!");
                uiTroubleshootOpenLocation.Enabled = false;
                uiTroubleshootDeleteFile.Enabled = false;
            }
        }


        // ###########################################################################################
        // Troubleshooting, delete the logfile
        // ###########################################################################################

        private void uiTroubleshootDeleteFile_Click(object sender, EventArgs e)
        {
            if (File.Exists(troubleshootLogfile))
            {
                File.Delete(@troubleshootLogfile);
                uiTroubleshootOpenLocation.Enabled = false;
                uiTroubleshootDeleteFile.Enabled = false;
            }
            else
            {
                MessageBox.Show(troubleshootLogfile + " does not exists!");
                uiTroubleshootOpenLocation.Enabled = false;
                uiTroubleshootDeleteFile.Enabled = false;
            }
        }


        // ###########################################################################################
        // Troubleshooting, refresh the buttons for the logfile, if viewing the "Advanced" tab
        // ###########################################################################################

        private void tabControl_Selected(object sender, TabControlEventArgs e)
        {
            // "Advanced" tab
            if (tabControl.SelectedTab.AccessibilityObject.Name == "Advanced")
            {
                if (File.Exists(troubleshootLogfile))
                {
                    uiTroubleshootOpenLocation.Enabled = true;
                    uiTroubleshootDeleteFile.Enabled = true;
                }
                else
                {
                    uiTroubleshootOpenLocation.Enabled = false;
                    uiTroubleshootDeleteFile.Enabled = false;
                }
            }

            // "Feedback" tab
            if (tabControl.SelectedTab.AccessibilityObject.Name == "Feedback")
            {
                if (uiTroubleshootEnabled.Checked)
                {
                    if (File.Exists(troubleshootLogfile))
                    {
                        uiAttachFile.Enabled = true;
                        FileInfo fileInfo = new FileInfo(troubleshootLogfile);
                        long fileSizeInBytes = fileInfo.Length;
                        double fileSizeInKilobytes = fileSizeInBytes / 1024.0;
                        double fileSizeInMegabytes = fileSizeInBytes / (1024.0 * 1024.0);
                        double fileSize = 0;
                        fileSize = fileSizeInBytes > 1000 ? fileSizeInKilobytes : fileSizeInBytes;
                        fileSize = fileSizeInKilobytes > 1000 ? fileSizeInMegabytes : fileSize;
                        fileSize = fileSize > 10 ? Math.Round(fileSize, 0) : fileSize;
                        string fileDescription = fileSizeInBytes > 1000 ? "KBytes" : "bytes";
                        fileDescription = fileSizeInKilobytes > 1000 ? "MBytes" : fileDescription;
                        bool hasDecimalPart = fileSize != Math.Truncate(fileSize);
                        fileSize = hasDecimalPart ? Math.Round(fileSize, 1) : fileSize;
                        uiAttachFile.Text = "Attach troubleshooting logfile (" + fileSize.ToString() + " " + fileDescription + ")";
                    }
                    else
                    {
                        uiAttachFile.Enabled = false;
                        uiAttachFile.Text = "Attach troubleshooting logfile (files does not exists)";
                    }
                }
                else
                {
                    uiAttachFile.Enabled = false;
                    uiAttachFile.Text = "Attach troubleshooting logfile (enable it under \"Advanced\")";
                }

            }

            // "Privacy" tab
            if (tabControl.SelectedTab.AccessibilityObject.Name == "Privacy")
            {
                try
                {
                    WebClient webClient = new WebClient();
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
                    webClient.Headers.Add("user-agent", ("HovText " + uiAppVer.Text).Trim());

                    // create a collection of name/value pairs to send to the server
                    var postData = new System.Collections.Specialized.NameValueCollection();
                    //postData.Add("version", uiAppVer.Text.Trim());
                    postData.Add("version", appVer);

                    // send the data to the server using the UploadValues method
                    byte[] responseBytes = webClient.UploadValues("https://hovtext.com/autoupdate/privacy/", postData);

                    // convert the response bytes to a string
                    string responseBody = Encoding.UTF8.GetString(responseBytes);

                    // parse the JSON response using JsonConvert.DeserializeObject
                    dynamic data = JsonConvert.DeserializeObject(responseBody);

                    // set the text of the two labels
                    label34.Text = data.timestamp;
                    label36.Text = data.ipaddr;
                    label35.Text = data.version;
                    label37.Text = data.countryCode;
                    label38.Text = data.countryName;
                }
                catch (WebException ex)
                {
                    // Catch the exception though this is not so critical that we need to disturb the developer
                    Logging.Log("Exception #13 raised (Settings):");
                    Logging.Log("  " + ex.Message);
                    MessageBox.Show("EXCEPTION #13 - please enable troubleshooting log and report to developer");
                }
            }
        }


        // ###########################################################################################
        // Troubleshooting, remove all registry keys and delete the logfile - this is resetting HovText to "factory default"
        // ###########################################################################################

        private void uiCleanUpExit_Click(object sender, EventArgs e)
        {
            CleanupAndExit();
        }

        public void CleanupAndExit()
        {
            isTroubleshootEnabled = false;

            // Remove HovText from starting up at Windows boot
            DeleteRegistryKey(registryPathRun, "HovText");

            // Delete the "HovText" folder in registry
            RegistryKey parentClass = Registry.CurrentUser;
            RegistryKey parentSoftware = parentClass.OpenSubKey("Software", true);
            parentSoftware.DeleteSubKeyTree("HovText");

            // Exit HovText
            resetApp = true;
            Close();
        }


        // ###########################################################################################
        // Send feedback to the developer
        // ###########################################################################################

        private void uiSendFeedback_Click(object sender, EventArgs e)
        {
            string email = uiEmailAddr.Text;
            string feedback = uiFeedbackText.Text;

            // Validate the email address
            bool isValidEmail = true; 
            if (email.Length > 0)
            {
                isValidEmail = IsValidEmail(email);
            }

            // Proceed if the email address is empty or valid
            if (isValidEmail)
            {
                try
                {
                    WebClient webClient = new WebClient();
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
                    webClient.Headers.Add("user-agent", ("HovText " + uiAppVer.Text).Trim());

                    // Build the data to send
                    string textFilePath = troubleshootLogfile;
                    string textFileContent = File.ReadAllText(textFilePath);
                    var data = new NameValueCollection();
                    data["version"] = uiAppVer.Text;
                    data["email"] = email;
                    if (uiAttachFile.Checked)
                    {
                        data["attachment"] = textFileContent;
                    }
                    data["feedback"] = feedback;

                    // Send it to the server
                    var response = webClient.UploadValues(hovtextPage +"contact/sendmail/", "POST", data);
                    string resultFromServer = Encoding.UTF8.GetString(response);
                    if (resultFromServer == "Success")
                    {
                        uiEmailAddr.Text = "";
                        uiFeedbackText.Text = "";
                        uiSendFeedback.Enabled = false;
                        if (email.Length > 0)
                        {
                            string txt = "Feedback sent - please allow for some time, if any response is required";
                            Logging.Log(txt);
                            Logging.Log("  Email used = [" + email + "]");
                            MessageBox.Show(txt);
                        }
                        else
                        {
                            string txt = "Feedback sent - no response will be given as you did not specify an email address";
                            Logging.Log(txt);
                            MessageBox.Show(txt);
                        }
                    }
                }
                catch (WebException ex)
                {
                    // Catch the exception though this is not so critical that we need to disturb the developer
                    Logging.Log("Exception #13 raised (Settings):");
                    Logging.Log("  " + ex.Message);
                    MessageBox.Show("EXCEPTION #13 - please enable troubleshooting log and report to developer");
                }

            }
            else
            {
                string txt = "Invalid email address ["+ email +"]";
                Logging.Log("EXCEPTION #12 raised:");
                Logging.Log("  "+ txt); 
                MessageBox.Show(txt);
            }
        }


        // ###########################################################################################
        // Check if the "Send feedback" button should be enabled or disabled
        // ###########################################################################################

        private void uiFeedbackText_TextChanged(object sender, EventArgs e)
        {
            if (uiFeedbackText.Text.Length > 0)
            {
                uiSendFeedback.Enabled = true;
            }
            else
            {
                uiSendFeedback.Enabled = false;
            }
        }


        // ###########################################################################################
        // Check if the email address typed in feedback is valid - not a very good check though!
        // ###########################################################################################

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void uiDevelopmentDownload_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(hovtextPage + "autoupdate/development/HovText.exe");
            Logging.Log("Clicked the \"Download\" development version");
        }


        // ###########################################################################################
        // Detect the main/primary display.
        // Return will be a 0-indexed screen/display
        // ###########################################################################################

        private int GetPrimaryDisplay()
        {
            // Walk through all displays
            int numDisplays = Screen.AllScreens.Length; // total number of displays
            for (int i = 0; i < numDisplays; i++)
            {
                bool isDisplayMain = Screen.AllScreens[i].Primary;
                if (isDisplayMain)
                {
                    return i;
                }
            }
            return 0;
        }


        // ###########################################################################################
        // Get the unique display layout.
        // Will get height, width, X/Y position and display internal name.
        // ###########################################################################################

        private string GetUniqueDisplayLayout()
        {
            StringBuilder sb = new StringBuilder();

            foreach (Screen display in Screen.AllScreens)
            {
                sb.Append('w');
                sb.Append(display.Bounds.Width);
                sb.Append(";h");
                sb.Append(display.Bounds.Height);
                sb.Append(";x");
                sb.Append(display.Bounds.X);
                sb.Append(";y");
                sb.Append(display.Bounds.Y);
                sb.Append(';');
                sb.Append(display.DeviceName);
                sb.Append(';');
            }

            string uniqueId = sb.ToString();
            if (uniqueId.EndsWith(";"))
            {
                uniqueId = uniqueId.TrimEnd(';');
            }

            return uniqueId;
        }
        

        // ###########################################################################################
        // Check if the selected display is valid
        // ###########################################################################################

        private bool IsDisplayValid (int display)
        {
            int numDisplays = Screen.AllScreens.Length; // total number of displays
            if (numDisplays >= display + 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        // ###########################################################################################
        // Populate the displays to the setup
        // Main contributor: FNI
        // ###########################################################################################

        private void PopulateDisplaySetup ()
        {
            // remove all elements
            uiDisplayGroup.Controls.Clear();

            // Build new radio buttons
            int numDisplays = Screen.AllScreens.Length; // total number of displays
            if (numDisplays > 1)
            {
                Logging.Log("Detected ["+ numDisplays + "] displays");
            }
            else
            {
                Logging.Log("Detected ["+ numDisplays + "] display");
            }

            // Walk through all displays
            int dividerSpace = 32;
            for (int i = 0; i < numDisplays; i++)
            {
                // Check if this display number is the main/primary one
                bool isDisplayMain = Screen.AllScreens[i].Primary;

                // Build a new radio UI element
                RadioButton display = new RadioButton();
                display.Name = "uiScreen" + i;
                display.Tag = i;
                if (isDisplayMain)
                {
                    display.Text = "Display " + (i + 1) + " (Main)";
                }
                else
                {
                    display.Text = "Display " + (i + 1);
                }
                display.Location = new Point(29, dividerSpace);
                display.AutoSize = true;
                display.CheckedChanged += new System.EventHandler(uiDisplayGroup_Changed);
                // Disable any possibility to select anything if we only have one display
                if (numDisplays <= 1)
                {
                    display.Enabled = false;
                }
                uiDisplayGroup.Controls.Add(display);
                dividerSpace += 31;
            }
        }


        // ###########################################################################################
        // Catch event when changing the display
        // Main contributor: FNI
        // ###########################################################################################

        private void uiDisplayGroup_Changed(object sender, EventArgs e)
        {
            RadioButton displaySelectedTag = (RadioButton)sender;
            int displaySelected = Convert.ToInt32(displaySelectedTag.Tag);
            if (displaySelected != activeDisplay)
            {
                activeDisplay = displaySelected;
                string displaysId = GetUniqueDisplayLayout();
                SetRegistryKey(registryPathDisplays, displaysId, activeDisplay.ToString());
            }
        }


        // ###########################################################################################
        // Detect events for changing display properties - e.g. adding/removing displays
        // ###########################################################################################
        private void DisplayChangesEvent(object sender, EventArgs e)
        {
            Logging.Log("Detected Windows display changes");

            PopulateDisplaySetup();

            // Get the unique identifier and value (if present) for the display setup
            string displaysId = GetUniqueDisplayLayout();
            string regVal;
            regVal = GetRegistryKey(registryPathDisplays, displaysId);

            // Check if we should show the clipboard history on a new display
            activeDisplay = Convert.ToInt32(regVal) != activeDisplay && regVal != null ? Convert.ToInt32(regVal) : activeDisplay;

            // Check if the new display settings still validates the display setup in HovText
            if (!IsDisplayValid(activeDisplay))
            {
                int activeDisplayOld = activeDisplay;
                activeDisplay = GetPrimaryDisplay();
                Logging.Log("History cannot be shown on display ID [" + activeDisplayOld + "] and will instead be shown on display ID [" + activeDisplay + "] with registry entry [\" + displaysId + \"]");
            }
            else
            {
                Logging.Log("History will be shown on display ID [" + activeDisplay + "] with registry entry [" + displaysId + "]");
            }
            uiDisplayGroup.Controls["uiScreen" + activeDisplay].Select();

            // Update the registry, as we do not know if it has been changed or not (too lazy to make variable for it)
            if (regVal == null)
            {
                SetRegistryKey(registryPathDisplays, displaysId, activeDisplay.ToString());
            }
        }


        // ###########################################################################################
        // Detect events for changing border thickness
        // ###########################################################################################

        private void uiBorderThickness_Scroll(object sender, EventArgs e)
        {
            labelBorderThickness.Text = uiBorderThickness.Value.ToString();
            historyBorderThickness = uiBorderThickness.Value;
            SetRegistryKey(registryPath, "HistoryBorderThickness", historyBorderThickness.ToString());
            uiShowFontActive.Refresh(); // update/redraw the border
            EnableDisableBorderColor(); // to enable/disable the border color
        }


        // ###########################################################################################
        // Detect events for changing icon set
        // ###########################################################################################

        private void uiIconsRound_CheckedChanged(object sender, EventArgs e)
        {
            if (!uiAppEnabled.Checked) // application is disabled
            {
                notifyIcon.Icon = Resources.Round_Inactive_48x48;
                Icon = Resources.Round_Inactive_48x48;
            } else if(uiHotkeyBehaviourPaste.Checked) // "Paste only on hotkey" checked
            {
                notifyIcon.Icon = Resources.Round_Hotkey_48x48;
                Icon = Resources.Round_Hotkey_48x48;
            } else // application is enabled and uses system clipboard
            {
                notifyIcon.Icon = Resources.Round_Active_48x48;
                Icon = Resources.Round_Active_48x48;
            }
            iconSet = "Round";
            SetRegistryKey(registryPath, "IconSet", iconSet);
        }

        private void uiIconsSquare_CheckedChanged(object sender, EventArgs e)
        {
            if (!uiAppEnabled.Checked) // application is disabled
            {
                notifyIcon.Icon = Resources.Square_Old_Inactive_16x16;
                Icon = Resources.Square_Old_Inactive_16x16;
            }
            else if (uiHotkeyBehaviourPaste.Checked) // "Paste only on hotkey" checked
            {
                notifyIcon.Icon = Resources.Square_Old_Hotkey_16x16;
                Icon = Resources.Square_Old_Hotkey_16x16;
            }
            else // application is enabled and uses system clipboard
            {
                notifyIcon.Icon = Resources.Square_Old_Active_16x16;
                Icon = Resources.Square_Old_Active_16x16;
            }
            iconSet = "SquareOld";
            SetRegistryKey(registryPath, "IconSet", iconSet);
        }

        private void uiIconsSquareNew_CheckedChanged(object sender, EventArgs e)
        {
            if (!uiAppEnabled.Checked) // application is disabled
            {
                notifyIcon.Icon = Resources.Square_Old_Inactive_16x16;
                Icon = Resources.Square_Old_Inactive_16x16;
            }
            else if (uiHotkeyBehaviourPaste.Checked) // "Paste only on hotkey" checked
            {
                notifyIcon.Icon = Resources.Square_Old_Hotkey_16x16;
                Icon = Resources.Square_Old_Hotkey_16x16;
            }
            else // application is enabled and uses system clipboard
            {
                notifyIcon.Icon = Resources.Square_New_Active_48x48;
                Icon = Resources.Square_New_Active_48x48;
            }
            iconSet = "SquareOld";
            SetRegistryKey(registryPath, "IconSet", iconSet);
        }


        // ###########################################################################################
    }
}
