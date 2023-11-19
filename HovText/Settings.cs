/*
##################################################################################################
SETTINGS
--------

This is the main form for the HovText application.

##################################################################################################
*/

using HovText.Properties;
using Microsoft.Win32;
using NHotkey.WindowsForms; // https://github.com/thomaslevesque/NHotkey
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static HovText.Program;


// ----------------------------------------------------------------------------
// Upload application to these places:
// https://www.microsoft.com/en-us/wdsi/filesubmission
// https://virustotal.com
// ----------------------------------------------------------------------------

// NuGet: "Costura.Fody" to merge the DLLs in to the EXE - to get only one EXE file for this application
// Incredible cool and simple compared to the other complex stuff I have seen! :-)
// https://stackoverflow.com/a/40786196/2028935

namespace HovText
{

    public sealed partial class Settings : Form
    {
        // ###########################################################################################
        // Define "Settings" class variables - real spaghetti - but it works :-)
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
        public static Dictionary<string, string> historyColorsSearch = new Dictionary<string, string>() {
            { "Blue", "#ffffff" },
            { "Brown", "#ffffff" },
            { "Green", "#ffffff" },
            { "Yellow", "#ffffff" },
            { "Contrast", "#ffffff" },
            { "Custom", "#ffffd9" },
        };
        public static Dictionary<string, string> historyColorsSearchText = new Dictionary<string, string>() {
            { "Blue", "#000000" },
            { "Brown", "#000000" },
            { "Green", "#000000" },
            { "Yellow", "#000000" },
            { "Contrast", "#000000" },
            { "Custom", "#787878" },
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
        public static Dictionary<string, string> historyColorsBorder = new Dictionary<string, string>() {
            { "Blue", "#ff0000" },
            { "Brown", "#ff0000" },
            { "Green", "#ff0000" },
            { "Yellow", "#ff0000" },
            { "Contrast", "#ff0000" },
            { "Custom", "#ff0000" },
        };
        public static string historyLocation = "Right Bottom";
        public static int historyBorderThickness = 1;

        // Registry, default values
        public const string registryPath = "SOFTWARE\\HovText";
        public const string registryPathDisplays = "SOFTWARE\\HovText\\DisplayLayout";
        public const string registryPathRun = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        private const string registryHotkeyToggleApplication = "Control + Oem5"; // hotkey "Toggle application on/off"
        private const string registryHotkeyGetOlderEntry = "Alt + H"; // hotkey "Show older entry"
        private const string registryHotkeyGetNewerEntry = "Shift + Alt + H"; // hotkey "Show newer entry"
        private const string registryHotkeySearch = "Alt + S"; // hotkey "Search"
        private const string registryHotkeyPasteOnHotkey = "Alt + O"; // hotkey "Paste on hotkey"
        private const string registryHotkeyToggleFavorite = "Oem5"; // hotkey "Toggle favorite entry"
        private const string registryHotkeyToggleView = "Q"; // hotkey "Toggle list view"
        private const string registryHotkeyBehaviour = "System"; // use system clipboard
        private const string registryStorageSaveOnExit = "0"; // 0 = do not save clipboards at exit, 1 = save clipboards on exit
        private const string registryStorageLoadOnLaunch = "1"; // 0 = do not load clipboards, 1 = load clipboards on launch
        private const string registryStorageSaveType = "All"; // "All" or "Favorites"
        private const string registryStorageSaveEntries = "100"; // number of clipboard entries to save
        private const string registryCloseMinimizes = "1"; // 0 = terminates, 1 = minimize to tray
        private const string registryStartDisabled = "0"; // 0 = start active, 1 = start disabled
        private const string registryRestoreOriginal = "1"; // 1 = restore original
        private const string registryCopyImages = "1"; // 1 = copy images to history
        private const string registryHistorySearch = "1"; // 1 = enable history and Search
        private const string registryHistoryInstantSelect = "0"; // 1 = enable history and Instant-select
        private const string registryEnableFavorites = "0"; // 0 = do not enable favorites
        private const string registryPasteOnSelection = "0"; // 0 = do not paste selected entry when selected
        private const string registryAlwaysPasteOriginal = "0"; // 1 = always paste original (formatted) text
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
        public static bool isStartDisabled;
        public static bool isCopyImages;
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
        IDataObject clipboardObject;
        public static SortedDictionary<int, string> entriesApplication = new SortedDictionary<int, string>();
        public static SortedDictionary<int, string> entriesApplication2 = new SortedDictionary<int, string>();
        public static SortedDictionary<int, string> entriesText = new SortedDictionary<int, string>();
        public static SortedDictionary<int, bool> entriesImageTransparent = new SortedDictionary<int, bool>();
        public static SortedDictionary<int, bool> entriesShow = new SortedDictionary<int, bool>();
        public static SortedDictionary<int, bool> entriesIsFavorite = new SortedDictionary<int, bool>();
        public static SortedDictionary<int, bool> entriesIsFavorite2 = new SortedDictionary<int, bool>();
        public static SortedDictionary<int, bool> entriesIsUrl = new SortedDictionary<int, bool>();
        public static SortedDictionary<int, bool> entriesIsEmail = new SortedDictionary<int, bool>();
        public static SortedDictionary<int, bool> entriesIsImage = new SortedDictionary<int, bool>();
        public static SortedDictionary<int, Image> entriesImage = new SortedDictionary<int, Image>();
        public static SortedList<int, Dictionary<string, object>> entriesOriginal = new SortedList<int, Dictionary<string, object>>();
        public static SortedList<int, Dictionary<string, object>> entriesOriginal2 = new SortedList<int, Dictionary<string, object>>();
        private static Dictionary<string, object> clipboardData;
        const int WM_CLIPBOARDUPDATE = 0x031D;
        string whoUpdatedClipboardName = "";
        public static bool pasteOnHotkeySetCleartext;

        // Misc
        public static string appVer = "";
        private static IntPtr originatingHandle = IntPtr.Zero;
        public static bool isFirstCallAfterHotkey = true;
        public static bool isSettingsFormVisible;
        public static string hovtextPage = "https://hovtext.com";
        public static string hovtextPageDownload = "https://hovtext.com/download";
        internal static Settings settings;
        public static bool isTroubleshootEnabled;
        public static bool hasTroubleshootLogged;
        private static bool resetApp = false;
        public static bool showFavoriteList = false; // will be true if the favorite list should be used/shown
        public static bool showFavoriteListLast = false; // used when doing search - to store if we previously was in the favorite list or not
        private static bool isClipboardLoadingFromFile; // "true" when initial loading clipboard content from file
        private int isClipboardLoadingFromFileKey;
        readonly History history = new History();
        readonly Update update = new Update();
        readonly TooBigLogfile tooBigLogfile;
        readonly PasteOnHotkey pasteOnHotkey = new PasteOnHotkey();
        readonly HotkeyConflict hotkeyConflict = new HotkeyConflict();
        private static string originatingApplicationName = "";
        public static int activeDisplay; // selected display to show the history (default will be the main display)
        private static string hotkey; // needed for validating the keys as it is not set in the event
        public static string cpuArchitecture;
        public static string osVersion;
        private static bool firstTimeLaunch;
        private static string buildType = ""; // Debug, Release
        private static string baseDirectory;
        private static string exeOnly;
        public static string pathAndExe;
        public static string pathAndData;
        public static string pathAndLog;
        private static string pathAndSpecial;
        private static string pathAndTempExe;
        private static string pathAndTempCmd;
        private static string pathAndTempLog;
        readonly string dataFile = "HovText.bin";
        readonly string troubleshootLog = "HovText-troubleshooting.txt";
        readonly string saveContentFileExist = "HovText-save-content-in-logfile.txt"; // should ONLY be used by Dennis!
        static readonly string tempExe = "HovText-new.exe";
        static readonly string tempCmd = "HovText-batch-update.cmd";
        static readonly string tempLog = "HovText-batch-update-log.txt";
        private bool isApplicationEnabled = true;
        public static byte[] encryptionKey;
        public static byte[] encryptionInitializationVector;


        // ###########################################################################################
        // Main - Settings
        // ###########################################################################################

        public Settings()
        {
            // Setup form and all elements
            InitializeComponent();

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
            rev = "(rev. " + rev + ")";
            rev = buildType == "Debug" ? rev : "";
            string buildTypeTmp = buildType == "Debug" ? "# DEVELOPMENT " : "";

            // Set the application version
            appVer = (date + " " + buildTypeTmp + rev).Trim();

            // Get OS name
            // https://stackoverflow.com/a/50330392/2028935
            osVersion = (string)(from x in new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem").Get().Cast<ManagementObject>() select x.GetPropertyValue("Caption")).FirstOrDefault();

            // Get the CPU architechture
            Architecture cpuArchitectureObj = RuntimeInformation.ProcessArchitecture;
            switch (cpuArchitectureObj)
            {
                case Architecture.X86:
                    cpuArchitecture = "x86/32bit";
                    break;
                case Architecture.X64:
                    cpuArchitecture = "x64/64bit";
                    break;
                case Architecture.Arm:
                    cpuArchitecture = "ARM/32bit";
                    break;
                case Architecture.Arm64:
                    cpuArchitecture = "ARM64/64bit";
                    break;
                default:
                    cpuArchitecture = "unknown";
                    break;
            }

            // Get paths and files
            baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            exeOnly = System.AppDomain.CurrentDomain.FriendlyName;
            pathAndExe = Path.Combine(baseDirectory, exeOnly);
            pathAndData = Path.Combine(baseDirectory, dataFile);
            pathAndLog = Path.Combine(baseDirectory, troubleshootLog);
            pathAndSpecial = Path.Combine(baseDirectory, saveContentFileExist); // should ONLY be used by Dennis!

            // Start logging, if relevant
            Logging.StartLogging();
            hasTroubleshootLogged = isTroubleshootEnabled;

            // Instantiate the "TooBigLogfile" form
            this.Load += Settings_Load;
            tooBigLogfile = new TooBigLogfile(this);

            // As the UI elements now have been initialized then we can setup the version
            AboutLabelVersion.Text = "Version " + appVer + " (64-bit)";

            // Mark very clearly with a color, that this is not a normal version (red equals danger) :-)
            if (buildType == "Debug")
            {
                BackColor = Color.IndianRed;
                AboutLabelDevelopment.Visible = true;
            }
            else
            {
                AboutLabelDevelopment.Visible = false;
            }

            // Refering to the current form - used in the history form
            settings = this;

            // Catch repaint event for this specific element (to draw the border)
            GuiShowFontActive.Paint += new System.Windows.Forms.PaintEventHandler(this.GuiShowFontBottom_Paint);
            GuiShowFont.Paint += new System.Windows.Forms.PaintEventHandler(this.GuiShowFontBottom_Paint);

            // Catch display change events (e.g. add/remove displays or change of main display)
            SystemEvents.DisplaySettingsChanged += new EventHandler(DisplayChangesEvent);

            // Initialize registry and get its values for the various checkboxes
            ConvertLegacyRegistry();
            InitializeRegistry();
            GetStartupSettings();

            // Should we start in "disabled" mode?
            if (isStartDisabled)
            {
                ToggleEnabled();
            }

            // Set the notify icon
            SetNotifyIcon();

            NativeMethods.AddClipboardFormatListener(this.Handle);
            Logging.Log("Added HovText to clipboard chain");

            // Write text for the "About" page

            // Basic RTF formatting
            // "test \b text \b0 is good" for bold
            // "test \i text \i0 is good" for italic
            // "test \ul text\ulnone  is good" for underline
            var sb = new StringBuilder();
            sb.Append(@"{\rtf1\ansi");
            sb.Append(@" This application is open source and you can use it on any computer you want at no cost. You are of course encouraged to donate some $$$, if you use it and want to motivate for continues support :-) There are a couple of things we would like to look more deeply into; e.g. code signing certificate and a more modern UI and these comes with a hefty price tag for us. \line\line");
            sb.Append(@" It is \ul not\ulnone  allowed to sell or distribute it in any commercial regard without written consent from the developer. \line ");
            sb.Append(@" \line ");
            sb.Append(@" Visit the HovText home page: \line ");
            sb.Append(@" " + hovtextPage + @" \line ");
            sb.Append(@" \line ");
            sb.Append(@" Kind regards, ");
            sb.Append(@" \line ");
            sb.Append(@" \line ");
            sb.Append(@" Jesper and Dennis ");
            sb.Append('}');
            AboutLabelDescription.Rtf = sb.ToString();

            GuiShowFontActive.Text = "Active entry\r\nLine 2\r\nLine 3\r\nLine 4\r\nLine 5\r\nLine 6\r\nLine 7";
            GuiShowFontEntry.Text = "Entry\r\nLine 2\r\nLine 3\r\nLine 4\r\nLine 5\r\nLine 6\r\nLine 7";

            // Set the initial text on the tray icon
            UpdateNotifyIconText();

            // Update the location of the dynamic labels
            UpdateLocationDynamicLabels();

            GuiLoadingPanel.BackColor = Color.FromArgb(128, 255, 255, 255);

            // Should we load the clipboard data file?
            if (GuiStorageLoadClipboard.Checked)
            {
                if (File.Exists(pathAndData))
                {
                    isClipboardLoadingFromFile = true; // calling many of the same functions, but somewhere we need to do special stuff when this is loaded from start

                    // Show the "Loading" panel
                    GuiLoadingPanel.Dock = DockStyle.Fill;
                    GuiLoadingPanel.Visible = true;
                    if(buildType == "Debug")
                    {
                        GuiLoadingPanel.BackColor = Color.IndianRed;
                    } else
                    {
                        GuiLoadingPanel.BackColor = Color.WhiteSmoke;
                    }
                    GuiLoadingPanel.BringToFront();

                    // Calculate and set the label's location to center it within panel1
                    GuiLoadingText.AutoSize = false;
                    GuiLoadingText.Width = GuiLoadingPanel.ClientSize.Width;
                    GuiLoadingText.Height = 30; // Set this to an appropriate value for your label
                    GuiLoadingText.TextAlign = ContentAlignment.MiddleCenter;
                    int x = (GuiLoadingPanel.ClientSize.Width - GuiLoadingText.Width) / 2;
                    int y = (GuiLoadingPanel.ClientSize.Height - GuiLoadingText.Height) / 2;
                    GuiLoadingText.Location = new Point(x, y);

                    LoadEntriesFromFile();
                }
            }

            // Update meory status and clipboard entries in "Advanced" tab
            LogMemoryConsumed();
            UpdateAdvancedStatus();
        }
                

        // ###########################################################################################
        // Save history to a data file
        // ###########################################################################################

        public void SaveEntriesToFile()
        {
            if (GuiStorageChooseFavorites.Checked)
            {
                // Temporary list to hold keys to be removed
                List<int> keysToRemove = new List<int>();

                // Walk through the original entries and check if they are favorites
                foreach (var entry in entriesOriginal)
                {
                    int key = entry.Key;

                    // Check if the key exists in entriesIsFavorite and if its value is true
                    if (entriesIsFavorite.TryGetValue(key, out bool isFavorite) && isFavorite)
                    {
                        Logging.Log("Entry index [" + key + "] is a favorite and saved");
                    }
                    else
                    {
                        keysToRemove.Add(key); // Add key to the removal list

                    }
                }

                // Remove the entries
                foreach (var key in keysToRemove)
                {
                    entriesOriginal.Remove(key);
                    entriesIsFavorite.Remove(key);
                    entriesApplication.Remove(key);
                }
            }

            // Number of entries to keep
            int keepCount = GuiStorageEntries.Value;

            // Remove entries if there are more than the number of entries to keep
            if (entriesOriginal.Count > keepCount)
            {
                int removeCount = entriesOriginal.Count - keepCount; // number of entries to remove

                // Get the keys of the entries to remove
                List<int> keysToRemove = entriesOriginal.Keys.Take(removeCount).ToList();

                // Remove the entries not needed
                foreach (var key in keysToRemove)
                {
                    entriesOriginal.Remove(key);
                    entriesIsFavorite.Remove(key);
                    entriesApplication.Remove(key);
                }
            }

            // Create a composite object (containing two dictionaries) to serialize
            var dataToSerialize = new Tuple<
                SortedList<int, Dictionary<string, object>>,
                SortedDictionary<int, bool>,
                SortedDictionary<int, string>
            >(entriesOriginal, entriesIsFavorite, entriesApplication);

            // Serialize and encrypt the composite object
            var binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, dataToSerialize);
                var serializedData = memoryStream.ToArray();
                var encryptedData = Encryption.EncryptStringToBytes_Aes(serializedData, encryptionKey, encryptionInitializationVector);
                using (var fileStream = new FileStream(pathAndData, FileMode.Create))
                {
                    fileStream.Write(encryptedData, 0, encryptedData.Length);
                    Logging.Log("Entries and favorites saved successfully.");
                }
            }
        }


        // ###########################################################################################
        // Load history from data file
        // ###########################################################################################

        public void LoadEntriesFromFile()
        {
            // Parallel processing the loading of the file
            Task.Run(() =>
            {
                try
                {
                    using (var fileStream = new FileStream(pathAndData, FileMode.Open))
                    {
                        Logging.Log("Reading encrypted data file:");

                        // Read the encrypted data from the file
                        byte[] encryptedData = new byte[fileStream.Length];
                        fileStream.Read(encryptedData, 0, encryptedData.Length);

                        // Decrypt the data
                        var decryptedData = Encryption.DecryptStringFromBytes_Aes(encryptedData, encryptionKey, encryptionInitializationVector);

                        // Check if decryption was successful
                        if (decryptedData == null)
                        {
                            Logging.Log("Encryption mismatches - data file ignored and will be overwritten at exit");
                        }
                        else
                        {
                            try
                            {
                                using (var memoryStream = new MemoryStream(decryptedData))
                                {
                                    var binaryFormatter = new BinaryFormatter();
                                    //var loadedData = (Tuple<SortedList<int, Dictionary<string, object>>, SortedDictionary<int, bool>>)binaryFormatter.Deserialize(memoryStream);
                                    var loadedData = (Tuple<
                                        SortedList<int, Dictionary<string, object>>,
                                        SortedDictionary<int, bool>,
                                        SortedDictionary<int, string>
                                    >)binaryFormatter.Deserialize(memoryStream);

                                    // Extract the dictionaries from the tuple into temporary dictionaries
                                    entriesOriginal2 = loadedData.Item1;
                                    entriesIsFavorite2 = loadedData.Item2;
                                    entriesApplication2 = loadedData.Item3;
                                }
                            }
                            catch (SerializationException ex)
                            {
                                Logging.Log($"Error during deserialization: {ex.Message}");
                                entriesOriginal2 = null;
                                entriesIsFavorite2 = null;
                                entriesApplication2 = null;
                            }

                            // Process entries if decryption was successful
                            if (entriesOriginal2 != null)
                            {
                                Logging.Log("---");
                                foreach (var entry in entriesOriginal2)
                                {
                                    try
                                    {
                                        clipboardData = entry.Value;
                                        Logging.Log($"Processing entry index [{entry.Key}] with [{clipboardData.Count}] formats");
                                        isClipboardLoadingFromFileKey = entry.Key;
                                        ProcessClipboard();
                                        clipboardData = null;
                                    }
                                    catch (Exception ex)
                                    {
                                        Logging.Log($"ERROR: Error processing entry {entry.Key}: {ex.Message}");
                                    }
                                }
                                Logging.Log("---");
                                Logging.Log($"Loaded [{entriesOriginal2.Count}] entries from data file");

                                entriesOriginal2 = null;
                                entriesIsFavorite2 = null;
                                entriesApplication2 = null;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle other exceptions
                    Logging.Log("Error during file loading: " + ex.Message);
                }
                finally
                {
                    // This code will run regardless of decryption success or failure
                    UpdatePanelVisibility();
                }
            });
        }


        // ###########################################################################################
        // Update "loading" panel visibility
        // ###########################################################################################

        private void UpdatePanelVisibility()
        {
            if (GuiLoadingPanel.InvokeRequired)
            {
                GuiLoadingPanel.Invoke(new Action(() =>
                {
                    GuiLoadingPanel.Visible = false;
                    isClipboardLoadingFromFile = false;
                }));
            }
            else
            {
                GuiLoadingPanel.Visible = false;
                isClipboardLoadingFromFile = false;
            }
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
                    uint threadId = NativeMethods.GetWindowThreadProcessId(whoUpdatedClipboardHwnd, out uint thisProcessId);
                    whoUpdatedClipboardName = Process.GetProcessById((int)thisProcessId).ProcessName;

                    // Get the name for this HovText executable (it may not be "HovText.exe")
                    string exeFileNameWithPath = Process.GetCurrentProcess().MainModule.FileName;
                    string exeFileNameWithoutExtension = Path.GetFileNameWithoutExtension(exeFileNameWithPath);

                    // Do not process clipboard, if this is coming from HovText itself
                    if (whoUpdatedClipboardName != exeFileNameWithoutExtension)
                    {
                        Logging.Log("Clipboard [UPDATE] event from application [" + whoUpdatedClipboardName + "]");

                        // I am not sure why some(?) applications are returned as "Idle" or "svchost" when coming from clipboard - in this case the get the active application and use that name instead
                        // This could potentially be a problem, if a process is correctly called "Idle" but not sure if this is realistic?
                        if (whoUpdatedClipboardName.ToLower() == "idle")
                        {
                            string activeProcessName = GetActiveApplication();
                            whoUpdatedClipboardName = activeProcessName;
                            Logging.Log("Finding process name the secondary way, [" + whoUpdatedClipboardName + "]");
                        }

                        // Check if application is enabled
                        if (isApplicationEnabled)
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
            // This can come from either the clipboard chain or from the "LoadEntriesFromFile" function
            if (clipboardData != null) // this then comes from the "LoadEntriesFromFile"
            {
                isClipboardText = clipboardData.ContainsKey(DataFormats.Text) ||
                    clipboardData.ContainsKey(DataFormats.UnicodeText) ||
                    clipboardData.ContainsKey(DataFormats.Rtf);
                isClipboardImage = clipboardData.ContainsKey(DataFormats.Bitmap);

                clipboardImage = isClipboardImage ? clipboardData[DataFormats.Bitmap] as Image : null;

                // Get TEXT clipboard content
                if (clipboardData.ContainsKey(DataFormats.Text))
                {
                    clipboardText = clipboardData[DataFormats.Text] as string;
                }
                else if (clipboardData.ContainsKey(DataFormats.UnicodeText))
                {
                    clipboardText = clipboardData[DataFormats.UnicodeText] as string;
                }
                else if (clipboardData.ContainsKey(DataFormats.Rtf))
                {
                    clipboardText = clipboardData[DataFormats.Rtf] as string;
                }
                else
                {
                    clipboardText = ""; // no supported text format found
                }

                DataObject dataObject = new DataObject();

                // Add each item from clipboardData to the DataObject
                foreach (var item in clipboardData)
                {
                    dataObject.SetData(item.Key, item.Value);
                }

                // Now dataObject is equivalent to what you would get from Clipboard.GetDataObject()
                clipboardObject = dataObject;
            }
            else // .. and this comes from the normal clipboard chain
            {
                // Check which text formats are available
                isClipboardText = Clipboard.ContainsText();
                isClipboardImage = Clipboard.ContainsImage();

                // Get clipboard content
                clipboardText = isClipboardText ? Clipboard.GetText() : "";
                clipboardImage = isClipboardImage ? Clipboard.GetImage() : null;
                clipboardObject = Clipboard.GetDataObject();
            }

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
                        if (!GuiAlwaysPasteOriginal.Checked)
                        {
                            if (clipboardData == null) // this is when we are loading from file
                            {
                                SetClipboard();
                            }
//    /*if (GuiHotkeyBehaviourPaste.Checked)*/
//                            if (GuiAlwaysPasteOriginal.Checked)
//                            {
//                                RestoreOriginal(entryIndex);
//                            }
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
//                        if (clipboardData == null)
//                        {
//                            SetClipboard();
//                        }
                    }
                }
            }
        }


        // ###########################################################################################
        // Get the image from clipboard
        // https://github.com/skoshy/CopyTransparentImages/blob/304e383b8f3239496999087421545a9b4dc765e5/ConsoleApp2/Program.cs#L58
        // It will also get the transparency alpha channel from the clipboard image
        // ###########################################################################################

        private static Image GetImageFromClipboard()
        {
            if (Clipboard.GetDataObject() == null) return null;
            if (Clipboard.GetDataObject().GetDataPresent(DataFormats.Dib))
            {
                // Sometimes this fails - probably because clipboard handling also can be messy and complex
                byte[] dib;
                try
                {
                    dib = ((MemoryStream)Clipboard.GetData(DataFormats.Dib)).ToArray();
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
                        bmp?.Dispose(); // same as "if (bmp != null) bmp.Dispose();"
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

        public static void RestoreOriginal(int entryIndex)
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
                        Logging.Log("  Adding format to clipboard, [" + kvp.Key + "]");
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
                MessageBox.Show("EXCEPTION #1 - please enable troubleshooting log and report to developer",
                    "ERROR",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
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
        // Clear all the history - as-if we just launched the application
        // ###########################################################################################

        private void ClearHistory()
        {
            entryCounter = -1;
            entryIndex = -1;
            entriesText.Clear();
            entriesImage.Clear();
            entriesImageTransparent.Clear();
            entriesApplication.Clear();
            entriesOriginal.Clear();
            entriesShow.Clear();
            entriesIsFavorite.Clear();
            entriesIsUrl.Clear();
            entriesIsEmail.Clear();
            entriesIsImage.Clear();
        }


        // ###########################################################################################
        // Add the content from clipboard to the data arrays
        // ###########################################################################################

        private void AddEntry()
        {
            // Clear the dictionaries if we do not catch the history
            if (!isEnabledHistory)
            {
                /*
                entryIndex = -1;
                entriesText.Clear();
                entriesImage.Clear();
                entriesImageTransparent.Clear();
                entriesApplication.Clear();
                entriesOriginal.Clear();
                entriesShow.Clear();
                entriesIsFavorite.Clear();
                entriesIsUrl.Clear();
                entriesIsEmail.Clear();
                entriesIsImage.Clear();
                */
                ///*
                ClearHistory();
                //*/
            }

            // Proceed if the (cleartext) data is not already in the dictionary

            bool isAlreadyInDataArray = false;
            if (!isClipboardLoadingFromFile) // we know when content is coming from file then it is already unique (and this function is slow!)
            {
                isAlreadyInDataArray = IsClipboardContentAlreadyInDataArrays();
            }
            if (!isAlreadyInDataArray)
            {
                if (isClipboardText)
                {
                    Logging.Log("Adding new [TEXT] clipboard to history from application [" + whoUpdatedClipboardName + "]:");
                }
                else
                {
                    Logging.Log("Adding new [IMAGE] clipboard to history from application [" + whoUpdatedClipboardName + "]:");
                }
                // If this is the first time then set the index to 0
                entryIndex = entryIndex >= 0 ? entriesText.Keys.Last() + 1 : 0;

                // Add the text and image to the entries array
                entriesText.Add(entryIndex, clipboardText);
                entriesImage.Add(entryIndex, clipboardImage);
                entriesImageTransparent.Add(entryIndex, isClipboardImageTransparent);
                entriesShow.Add(entryIndex, true);
                if (isClipboardLoadingFromFile)
                {
                    bool hest = entriesIsFavorite2[isClipboardLoadingFromFileKey];
                    entriesIsFavorite.Add(entryIndex, hest);
                }
                else
                {
                    entriesIsFavorite.Add(entryIndex, false);
                }

                // Filter lists (HTTP and HTTPS)
                bool isUrl = Uri.TryCreate(clipboardText, UriKind.Absolute, out Uri myUri) && (myUri.Scheme == Uri.UriSchemeHttp || myUri.Scheme == Uri.UriSchemeHttps || myUri.Scheme == Uri.UriSchemeFtp || myUri.Scheme == "ws" || myUri.Scheme == "wss");
                if (isUrl)
                {
                    entriesIsUrl.Add(entryIndex, true);
                }
                else
                {
                    entriesIsUrl.Add(entryIndex, false);
                }
                bool isEmail = Regex.IsMatch(clipboardText, @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$");
                if (isEmail)
                {
                    entriesIsEmail.Add(entryIndex, true);
                }
                else
                {
                    entriesIsEmail.Add(entryIndex, false);
                }
                if (!isClipboardText)
                {
                    entriesIsImage.Add(entryIndex, true);
                }
                else
                {
                    entriesIsImage.Add(entryIndex, false);
                }

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

                        // Do special content logging in development version oply - should ONLY be used by Dennis!
                        if (File.Exists(pathAndSpecial) && buildType == "Debug")
                        {
                            // WILL SAVE CONTENT COPIED TO CLIPBOARD!!! Used for debugging purpose
                            Logging.Log("  Adding format [" + format + "] with value [" + clipboardObject.GetData(format) + "]");
                        }
                        else
                        {
                            // Standard logging for everyone else - do NOT save any content to the logfile! :-)
                            Logging.Log("  Adding format [" + format + "]");
                        }
                    }
                    else
                    {
                        //Logging.Log("  Discarding format [" + format + "]");
                    }
                }
                entriesOriginal.Add(entryIndex, clipboardObjects);

                // Add the process name that has updated the clipboard
                if (whoUpdatedClipboardName.Length == 0)
                {
                    whoUpdatedClipboardName = "(unknown)";
                }
                if (isClipboardLoadingFromFile)
                {
                    string hest = entriesApplication2[isClipboardLoadingFromFileKey];
                    entriesApplication.Add(entryIndex, hest);
                }
                else
                {
                    entriesApplication.Add(entryIndex, whoUpdatedClipboardName);
                }

                Logging.Log("Entries in history list is now [" + entriesText.Count + "]");
            }

            // Update the entries on the tray icon
            UpdateNotifyIconText();

            LogMemoryConsumed();
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

            if (entriesText.Count > 0)
            {
                if (isEnabledHistory)
                {
                    entryText = entriesText[entryIndex];
                    entryImage = entriesImage[entryIndex];
                    isEntryText = !string.IsNullOrEmpty(entryText);
                    isEntryImage = entryImage != null;
                }

                // Put text to the clipboard
                if (isEntryText)
                {
                    try
                    {
                        if ((GuiHotkeyBehaviourPaste.Checked && !pasteOnHotkeySetCleartext) || (GuiAlwaysPasteOriginal.Enabled && GuiAlwaysPasteOriginal.Checked))
                        {
                            RestoreOriginal(entryIndex);
                        }
                        else
                        {
                            if (GuiTrimWhitespaces.Checked)
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
                        MessageBox.Show("EXCEPTION #2 - please enable troubleshooting log and report to developer",
                            "ERROR",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
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
                        MessageBox.Show("EXCEPTION #3 - please enable troubleshooting log and report to developer",
                            "ERROR",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
                else
                {
                    Logging.Log("Exception #4 raised (Settings):");
                    Logging.Log("  Clipboard triggered but is not [isEntryText] or [isEntryImage]");
                    MessageBox.Show("EXCEPTION #4 - please enable troubleshooting log and report to developer",
                            "ERROR",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                }
            }
        }


        // ###########################################################################################
        // Get the next older entry from history
        // ###########################################################################################

        public void GoEntryLowerNumber()
        {
            // Check if application is enabled
            if (isApplicationEnabled && entryCounter > 0)
            {

                if (isFirstCallAfterHotkey)
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
                ChangeFocusToHovText();

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
            if (isApplicationEnabled && entryCounter > 0)
            {

                if (isFirstCallAfterHotkey)
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
                ChangeFocusToHovText();

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
            if (isApplicationEnabled && entryCounter > 0)
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
                    if (GuiHotkeyBehaviourPaste.Checked)
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

        public static void GetEntryCounter()
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

            // Reset counter
            if (entryCounter == 0)
            {
                entryIndex = -1;
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
            entriesShow.Add(insertKey, entriesShow[entryIndex]);
            entriesIsFavorite.Add(insertKey, entriesIsFavorite[entryIndex]);
            entriesIsUrl.Add(insertKey, entriesIsUrl[entryIndex]);
            entriesIsEmail.Add(insertKey, entriesIsEmail[entryIndex]);
            entriesIsImage.Add(insertKey, entriesIsImage[entryIndex]);

            // Remove the chosen entry, so it does not show duplicates
            entriesText.Remove(entryIndex);
            entriesImage.Remove(entryIndex);
            entriesImageTransparent.Remove(entryIndex);
            entriesApplication.Remove(entryIndex);
            entriesOriginal.Remove(entryIndex);
            entriesShow.Remove(entryIndex);
            entriesIsFavorite.Remove(entryIndex);
            entriesIsUrl.Remove(entryIndex);
            entriesIsEmail.Remove(entryIndex);
            entriesIsImage.Remove(entryIndex);

            // Set the index to be the last one
            entryIndex = entriesText.Keys.Last();
        }


        // ###########################################################################################
        // Called when the "Enable application" checkbox is clicked with mouse or the "Toggle application on/off" hotkey is pressed
        // ###########################################################################################

        public void ToggleEnabled()
        {
            // Toggle the checkbox - this will also fire an "appEnabled_CheckedChanged" event
            isApplicationEnabled = !isApplicationEnabled;

            if (isApplicationEnabled)
            {
                Logging.Log("Enabled HovText");

                // Add this application to the clipboard chain again
                NativeMethods.AddClipboardFormatListener(this.Handle);
                Logging.Log("Added HovText to clipboard chain");

                string hotkeyBehaviour = GetRegistryKey(registryPath, "HotkeyBehaviour");
                switch (hotkeyBehaviour)
                {
                    case "Paste":
                        GuiHotkeyBehaviourPaste.Checked = true;
                        GuiHotkeyPaste.Enabled = true;
                        break;
                    default:
                        GuiHotkeyBehaviourSystem.Checked = true;
                        GuiHotkeyPaste.Enabled = false;
                        break;
                }

                GuiSearch.Enabled = true;
                GuiInstantSelect.Enabled = true;
                GuiHotkeyEnable.Enabled = true;

                SetFieldsBasedOnHistoryEnabled();

                GuiFavoritesEnabled.Enabled = true;
                GuiRestoreOriginal.Enabled = true;
                GuiHotkeyBehaviourSystem.Enabled = true;
                GuiHotkeyBehaviourPaste.Enabled = true;
                GuiTrimWhitespaces.Enabled = true;
            }
            else
            {
                Logging.Log("Disabled HovText");

                // Remove this application from the clipboard chain
                NativeMethods.RemoveClipboardFormatListener(this.Handle);
                Logging.Log("Removed HovText from clipboard chain");

                // Restore the original clipboard format
                if (isRestoreOriginal && entriesOriginal.Count > 0)
                {
                    RestoreOriginal(entryIndex);
                }

                // Disable other checkboxes
                GuiSearch.Enabled = false;
                GuiHotkeySearch.Enabled = false;
                GuiInstantSelect.Enabled = false;
                GuiFavoritesEnabled.Enabled = false;
                GuiRestoreOriginal.Enabled = false;
                GuiCopyImages.Enabled = false;
                GuiTrimWhitespaces.Enabled = false;
                GuiPasteOnSelection.Enabled = false;
                GuiAlwaysPasteOriginal.Enabled = false;
                GuiHotkeyEnable.Enabled = false;
                GuiHotkeyOlder.Enabled = false;
                GuiHotkeyNewer.Enabled = false;
                GuiHotkeyPaste.Enabled = false;
                GuiHotkeyBehaviourSystem.Enabled = false;
                GuiHotkeyBehaviourPaste.Enabled = false;
                GuiHotkeyToggleFavorite.Enabled = false;
                GuiHotkeyToggleView.Enabled = false;
            }

            SetNotifyIcon();
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

        private void AboutBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Logging.Log("Clicked the web page link in \"About\"");
            System.Diagnostics.Process.Start(e.LinkText);
        }


        // ###########################################################################################
        // Delete troubleshoot logfile and clipboard data file
        // ###########################################################################################

        private void DeleteFiles()
        {
            // Troubleshoot logfile
            if (File.Exists(pathAndLog))
            {
                File.Delete(@pathAndLog);
            }
            // Clipboard data file
            if (File.Exists(pathAndData))
            {
                File.Delete(@pathAndData);
            }
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

                // Should we save the clipboard to data file?
                if (GuiStorageSaveClipboard.Checked)
                {
                    SaveEntriesToFile();
                }

                Logging.EndLogging();

                // Delete the troubleshooting logfile as the very last thing
                if (resetApp)
                {
                    DeleteFiles();
                }

                return;
            }

            if (!GuiCloseMinimize.Checked || isClosedFromNotifyIcon)
            {
                Logging.Log("Exit HovText");
                NativeMethods.RemoveClipboardFormatListener(this.Handle);
                Logging.Log("Removed HovText from clipboard chain");

                RemoveAllHotkeys();

                // Should we save the clipboard to data file?
                if (GuiStorageSaveClipboard.Checked)
                {
                    SaveEntriesToFile();
                }

                Logging.EndLogging();

                // Delete the troubleshooting logfile as the very last thing
                if (resetApp)
                {
                    DeleteFiles();
                }

                return;
            }

            // Called when closing from "Clean up"
            if (resetApp)
            {
                NativeMethods.RemoveClipboardFormatListener(this.Handle);
                RemoveAllHotkeys();

                DeleteFiles();

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

        public static string GetActiveApplication()
        {
            // Get the active application
            originatingHandle = NativeMethods.GetForegroundWindow();

            // Get the process ID and find the name for that ID
            uint threadId = NativeMethods.GetWindowThreadProcessId(originatingHandle, out uint processId);
            string appProcessName = Process.GetProcessById((int)processId).ProcessName;
            return appProcessName;
        }


        // ###########################################################################################
        // When pressing one of the history hotkeys then change focus to this application to prevent the keypresses go in to the active application
        // ###########################################################################################

        private void ChangeFocusToHovText()
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
            Logging.Log("Set focus to originating application [" + originatingApplicationName + "]");
        }


        // ###########################################################################################
        // Check for HovText updates online.
        // Stable versions will be notified via popup.
        // Development versions will be shown in "Advanced" tab only.
        // ###########################################################################################

        private void UpdateTimer_Tick(object sender, EventArgs e)
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

                // Prepare the POST data
                var postData = new System.Collections.Specialized.NameValueCollection
                {
                    { "osVersion", osVersion },
                    { "cpuArchitecture", cpuArchitecture }
                };

                // Send the POST data to the server
                byte[] responseBytes = webClient.UploadValues(hovtextPage + "/autoupdate/", postData);

                // Convert the response bytes to a string
                string checkedVersion = Encoding.UTF8.GetString(responseBytes);

                // Download the new stable version
                if (checkedVersion.Substring(0, 7) == "Version")
                {
                    checkedVersion = checkedVersion.Substring(9);
                    Logging.Log("  Stable version available = [" + checkedVersion + "]");
                    update.GuiAppVerYours.Text = appVer;
                    update.GuiAppVerOnline.Text = checkedVersion;
                    string lastCheckedVersion = GetRegistryKey(registryPath, "CheckedVersion");
                    if (lastCheckedVersion != checkedVersion && checkedVersion != appVer)
                    {
                        string notificationShown = GetRegistryKey(registryPath, "NotificationShown");
                        if (notificationShown == "1")
                        {
                            update.Show();
                            update.Activate();
                            Logging.Log("  Notified on new [STABLE] version available");
                        } else
                        {
                            Logging.Log("  Did not notify on new [STABLE] version available, as \"Notification\" has not yet been shown");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Catch the exception though this is not so critical that we need to disturb the developer
                Logging.Log("Exception raised (Settings):");
                Logging.Log("  Cannot connect with server to get information about newest available [STABLE] version:");
                Logging.Log("  " + ex.Message);
            }

            // Show the development version (if any)
            FetchInfoForDevelopment();
        }


        // ###########################################################################################
        // Enforce an application termination after 5 seconds.
        // In some cases the application does not exit(???) when auto-installing.
        // ###########################################################################################

        private void TerminateTimer_Tick(object sender, EventArgs e)
        {
            Logging.Log("Forcing application termination after 5 seconds timeout!!!");
            Logging.EndLogging();
            Environment.Exit(0);
        }


        // ###########################################################################################
        // Convert legacy/old registry entries
        // ###########################################################################################

        private static void ConvertLegacyRegistry()
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

            // Check or create the main "HovText" and "HovText\DisplayLayout" paths in HKEY_CURRENT_USER\SOFTWARE registry
            RegistryPathCheckOrCreate(registryPath);
            RegistryPathCheckOrCreate(registryPathDisplays);

            string regVal;

            /*

            
                        // ------------------------------------------------------------------------------------
                        // Below is the chronological order for the changes needed.
                        // New versions will need to go at the bottom of this.
                        // ------------------------------------------------------------------------------------

                        // ------------------------------------------------------------------------------------
                        // Changes for the version 2023-May-17
                        // ------------------------------------------------------------------------------------

                        // Check if the following registry entries exists, and if so convert them to new values

                        // Convert "Hotkey1" => "HotkeyToggleApplication"
                        regVal = GetRegistryKey(registryPath, "Hotkey1");
                        if (regVal != null || regVal?.Length == 0)
                        {
                            RegistryKeyCheckOrCreate(registryPath, "HotkeyToggleApplication", regVal);
                            DeleteRegistryKey(registryPath, "Hotkey1");
                        }

                        // Convert "Hotkey2" => "HotkeyGetOlderEntry"
                        regVal = GetRegistryKey(registryPath, "Hotkey2");
                        if (regVal != null || regVal?.Length == 0)
                        {
                            RegistryKeyCheckOrCreate(registryPath, "HotkeyGetOlderEntry", regVal);
                            DeleteRegistryKey(registryPath, "Hotkey2");
                        }

                        // Convert "Hotkey3" => "HotkeyGetNewerEntry"
                        regVal = GetRegistryKey(registryPath, "Hotkey3");
                        if (regVal != null || regVal?.Length == 0)
                        {
                            RegistryKeyCheckOrCreate(registryPath, "HotkeyGetNewerEntry", regVal);
                            DeleteRegistryKey(registryPath, "Hotkey3");
                        }

                        // Convert "Hotkey4" => "HotkeyPasteOnHotkey"
                        regVal = GetRegistryKey(registryPath, "Hotkey4");
                        if (regVal != null || regVal?.Length == 0)
                        {
                            RegistryKeyCheckOrCreate(registryPath, "HotkeyPasteOnHotkey", regVal);
                            DeleteRegistryKey(registryPath, "Hotkey4");
                        }

                        // Convert "Hotkey5" => "HotkeyToggleFavorite"
                        regVal = GetRegistryKey(registryPath, "Hotkey5");
                        if (regVal != null || regVal?.Length == 0)
                        {
                            RegistryKeyCheckOrCreate(registryPath, "HotkeyToggleFavorite", regVal);
                            DeleteRegistryKey(registryPath, "Hotkey5");
                        }

                        // Convert "Hotkey6" => "HotkeyToggleView"
                        regVal = GetRegistryKey(registryPath, "Hotkey6");
                        if (regVal != null || regVal?.Length == 0)
                        {
                            RegistryKeyCheckOrCreate(registryPath, "HotkeyToggleView", regVal);
                            DeleteRegistryKey(registryPath, "Hotkey6");
                        }

                        // Convert "HistoryActiveBorder" => "HistoryBorderThickness"
                        regVal = GetRegistryKey(registryPath, "HistoryActiveBorder");
                        if (regVal != null || regVal?.Length == 0)
                        {
                            if (regVal == "0")
                            {
                                RegistryKeyCheckOrCreate(registryPath, "HistoryBorderThickness", regVal);
                            }
                            else
                            {
                                RegistryKeyCheckOrCreate(registryPath, "HistoryBorderThickness", historyBorderThickness.ToString());
                            }
                            DeleteRegistryKey(registryPath, "HistoryActiveBorder");
                        }

                        // Convert "HistoryColor" => "HistoryColorTheme"
                        regVal = GetRegistryKey(registryPath, "HistoryColor");
                        if (regVal != null || regVal?.Length == 0)
                        {
                            RegistryKeyCheckOrCreate(registryPath, "HistoryColorTheme", regVal);
                            DeleteRegistryKey(registryPath, "HistoryColor");
                        }

                        // Convert "HistoryColorCustomTop" => "HistoryColorCustomHeader"
                        regVal = GetRegistryKey(registryPath, "HistoryColorCustomTop");
                        if (regVal != null || regVal?.Length == 0)
                        {
                            RegistryKeyCheckOrCreate(registryPath, "HistoryColorCustomHeader", regVal);
                            DeleteRegistryKey(registryPath, "HistoryColorCustomTop");
                        }

                        // Convert "HistoryColorCustomBottom" => "HistoryColorCustomEntry"
                        regVal = GetRegistryKey(registryPath, "HistoryColorCustomBottom");
                        if (regVal != null || regVal?.Length == 0)
                        {
                            RegistryKeyCheckOrCreate(registryPath, "HistoryColorCustomEntry", regVal);
                            DeleteRegistryKey(registryPath, "HistoryColorCustomBottom");
                        }

                        // Convert "HistoryColorCustomText" => "HistoryColorCustomEntryText"
                        regVal = GetRegistryKey(registryPath, "HistoryColorCustomText");
                        if (regVal != null || regVal?.Length == 0)
                        {
                            RegistryKeyCheckOrCreate(registryPath, "HistoryColorCustomEntryText", regVal);
                            DeleteRegistryKey(registryPath, "HistoryColorCustomText");
                        }

                        // Convert "ScreenSelection" => new unique screen setup identification
                        regVal = GetRegistryKey(registryPath, "ScreenSelection");
                        if (regVal != null || regVal?.Length == 0)
                        {
                            string displaysId = GetUniqueDisplayLayout();
                            RegistryKeyCheckOrCreate(registryPathDisplays, displaysId, regVal);
                            DeleteRegistryKey(registryPath, "HistoryColorCustomText");
                        }

                        // Delete "CheckUpdates"
                        regVal = GetRegistryKey(registryPath, "CheckUpdates");
                        if (regVal != null || regVal?.Length == 0)
                        {
                            DeleteRegistryKey(registryPath, "CheckUpdates");
                        }

                        // Delete "VersionOnline" - it was something used in development versions until I reverted back to "CheckedVersion"
                        regVal = GetRegistryKey(registryPath, "VersionOnline");
                        if (regVal != null || regVal?.Length == 0)
                        {
                            DeleteRegistryKey(registryPath, "VersionOnline");
                        }

                        // Delete "HistoryColorCustomBorder"
                        regVal = GetRegistryKey(registryPath, "HistoryColorCustomBorder");
                        if (regVal != null || regVal?.Length == 0)
                        {
                            DeleteRegistryKey(registryPath, "HistoryColorCustomBorder");
                        }

                        // ------------------------------------------------------------------------------------
                        // Changes for the version 2023-September-20
                        // ------------------------------------------------------------------------------------

                        // Convert "HistoryEnable" => "HistoryInstantSelect"
                        regVal = GetRegistryKey(registryPath, "HistoryEnable");
                        if (regVal != null || regVal?.Length == 0)
                        {
                            RegistryKeyCheckOrCreate(registryPath, "HistoryInstantSelect", regVal);
                            DeleteRegistryKey(registryPath, "HistoryEnable");
                        }

                        // Convert "HistoryColorBorder" => "HistoryColorCustomBorder"
                        regVal = GetRegistryKey(registryPath, "HistoryColorBorder");
                        if (regVal != null || regVal?.Length == 0)
                        {
                            RegistryKeyCheckOrCreate(registryPath, "HistoryColorCustomBorder", regVal);
                            DeleteRegistryKey(registryPath, "HistoryColorBorder");
                        }

                        // Convert "CloseMinimizes" => "CloseTerminates"
                        regVal = GetRegistryKey(registryPath, "CloseMinimizes");
                        if (regVal != null || regVal?.Length == 0)
                        {
                            regVal = regVal == "1" ? "0" : "1"; // set the oppesite of before
                            RegistryKeyCheckOrCreate(registryPath, "CloseTerminates", regVal);
                            DeleteRegistryKey(registryPath, "CloseMinimizes");
                        }
            */

            // ------------------------------------------------------------------------------------
            // Changes for the version 2024-????????-??
            // ------------------------------------------------------------------------------------

            // Convert "CloseTerminates" => "CloseMinimizes" (back and forth it seems, ha)
            regVal = GetRegistryKey(registryPath, "CloseTerminates");
            if (regVal != null || regVal?.Length == 0)
            {
                regVal = regVal == "1" ? "0" : "1"; // set the oppesite of before
                RegistryKeyCheckOrCreate(registryPath, "CloseMinimizes", regVal);
                DeleteRegistryKey(registryPath, "CloseTerminates");
            }

            // Convert "HotkeyToggleFavorite" if it has "Space" as default key AND the "Search" interface is enabled
            regVal = GetRegistryKey(registryPath, "HistorySearch");
            if (regVal != null || regVal == "1")
            {
                regVal = GetRegistryKey(registryPath, "HotkeyToggleFavorite");
                if (regVal != null || regVal?.Length == 0)
                {
                    // Build an array containing not-allowed characters
                    string[] alphaKeys = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray().Select(c => c.ToString()).ToArray();
                    string[] numericKeys = Enumerable.Range(0, 10).Select(n => $"D{n}").ToArray(); // Correct representation for numeric keys
                    string[] allKeys = alphaKeys.Concat(numericKeys).ToArray();

                    // Convert the value from registry, if it is within the "not allowed" array
                    if (regVal == "Space" || allKeys.Contains(regVal))
                    {
                        SetRegistryKey(registryPath, "HotkeyToggleFavorite", registryHotkeyToggleFavorite);
                    }
                }
            }
        }


        // ###########################################################################################
        // Initialize registry - check if all relevant registry paths and keys have been created
        // ###########################################################################################

        private static void InitializeRegistry()
        {

            string regVal;

            // Check if the following registry entries exists - if not, then create them with their default values

            Logging.Log("Startup registry values:");

            // General
            // -------
            Logging.Log("  General:");

            // Set "Start with Windows"
            if (firstTimeLaunch)
            {
                RegistryKeyCheckOrCreate(registryPathRun, "HovText", "\"" + System.Windows.Forms.Application.ExecutablePath + "\" --start-minimized");
            }
            regVal = GetRegistryKey(registryPathRun, "HovText");
            if (regVal != null || regVal?.Length > 0)
            {
                Logging.Log("    \"StartWithWindows\" = [Yes]");
            }
            else
            {
                Logging.Log("    \"StartWithWindows\" = [No]");

            }

            RegistryKeyCheckOrCreate(registryPath, "StartDisabled", registryStartDisabled);
            RegistryKeyCheckOrCreate(registryPath, "RestoreOriginal", registryRestoreOriginal);
            RegistryKeyCheckOrCreate(registryPath, "TrimWhitespaces", registryTrimWhitespaces);
            RegistryKeyCheckOrCreate(registryPath, "CloseMinimizes", registryCloseMinimizes);
            RegistryKeyCheckOrCreate(registryPath, "HistorySearch", registryHistorySearch);
            RegistryKeyCheckOrCreate(registryPath, "HistoryInstantSelect", registryHistoryInstantSelect);
            RegistryKeyCheckOrCreate(registryPath, "FavoritesEnable", registryEnableFavorites);
            RegistryKeyCheckOrCreate(registryPath, "CopyImages", registryCopyImages);
            RegistryKeyCheckOrCreate(registryPath, "PasteOnSelection", registryPasteOnSelection);
            RegistryKeyCheckOrCreate(registryPath, "AlwaysPasteOriginal", registryAlwaysPasteOriginal);

            regVal = GetRegistryKey(registryPath, "StartDisabled");
            Logging.Log("    \"StartDisabled\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "RestoreOriginal");
            Logging.Log("    \"RestoreOriginal\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "TrimWhitespaces");
            Logging.Log("    \"TrimWhitespaces\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "CloseMinimizes");
            Logging.Log("    \"CloseMinimizes\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "HistorySearch");
            Logging.Log("    \"HistorySearch\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "HistoryInstantSelect");
            Logging.Log("    \"HistoryInstantSelect\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "FavoritesEnable");
            Logging.Log("    \"FavoritesEnable\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "CopyImages");
            Logging.Log("    \"CopyImages\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "PasteOnSelection");
            Logging.Log("    \"PasteOnSelection\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "AlwaysPasteOriginal");
            Logging.Log("    \"AlwaysPasteOriginal\" = [" + regVal + "]");

            // Storage
            // -------
            Logging.Log("  Storage:");

            RegistryKeyCheckOrCreate(registryPath, "StorageSaveOnExit", registryStorageSaveOnExit);
            RegistryKeyCheckOrCreate(registryPath, "StorageLoadOnLaunch", registryStorageLoadOnLaunch);
            RegistryKeyCheckOrCreate(registryPath, "StorageSaveType", registryStorageSaveType);
            RegistryKeyCheckOrCreate(registryPath, "StorageSaveEntries", registryStorageSaveEntries);

            regVal = GetRegistryKey(registryPath, "StorageSaveOnExit");
            Logging.Log("    \"StorageSaveOnExit\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "StorageLoadOnLaunch");
            Logging.Log("    \"StorageLoadOnLaunch\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "StorageSaveType");
            Logging.Log("    \"StorageSaveType\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "StorageSaveEntries");
            Logging.Log("    \"StorageSaveEntries\" = [" + regVal + "]");

            // Encryption
            byte[] key;
            byte[] iv;
            Encryption.GenerateKeyAndIV(out key, out iv);
            string strKey = Convert.ToBase64String(key);
            string strIv = Convert.ToBase64String(iv);
            RegistryKeyCheckOrCreate(registryPath, "EncryptionKey", strKey);
            RegistryKeyCheckOrCreate(registryPath, "EncryptionInitializationVector", strIv);
            strKey = GetRegistryKey(registryPath, "EncryptionKey");
            strIv = GetRegistryKey(registryPath, "EncryptionInitializationVector");
            try
            {
                key = Convert.FromBase64String(strKey);
            }
            catch (FormatException)
            {
                key = new byte[1]; // invalidate this byte array
            }
            try
            {
                iv = Convert.FromBase64String(strIv);
            }
            catch (FormatException)
            {
                iv = new byte[1]; // invalidate this byte array
            }

            if (buildType == "Debug")
            {
                Logging.Log("    \"EncryptionKey\" = [" + strKey + "]");
                Logging.Log("    \"EncryptionInitializationVector\" = [" + strIv + "]");
            }
            else
            {
                Logging.Log("    \"EncryptionKey\" = [not logged in stable version]");
                Logging.Log("    \"EncryptionInitializationVector\" = [not logged in stable version]");
            }

            key = key.Length != 32 ? null : key;
            iv = iv.Length != 16 ? null : iv;
            if (key == null || iv == null)
            {
                Encryption.GenerateKeyAndIV(out key, out iv);
                strKey = Convert.ToBase64String(key);
                strIv = Convert.ToBase64String(iv);
                SetRegistryKey(registryPath, "EncryptionKey", strKey);
                SetRegistryKey(registryPath, "EncryptionInitializationVector", strIv);
                Logging.Log("    \"EncryptionKey\" has been reset as it was invalid");
                Logging.Log("    \"EncryptionInitializationVector\" has been reset as it was invalid");

            }

            // Hotkeys
            // -------
            Logging.Log("  Hotkeys:");

            RegistryKeyCheckOrCreate(registryPath, "HotkeyBehaviour", registryHotkeyBehaviour);
            RegistryKeyCheckOrCreate(registryPath, "HotkeyToggleApplication", registryHotkeyToggleApplication);
            RegistryKeyCheckOrCreate(registryPath, "HotkeySearch", registryHotkeySearch);
            RegistryKeyCheckOrCreate(registryPath, "HotkeyGetOlderEntry", registryHotkeyGetOlderEntry);
            RegistryKeyCheckOrCreate(registryPath, "HotkeyGetNewerEntry", registryHotkeyGetNewerEntry);
            RegistryKeyCheckOrCreate(registryPath, "HotkeyToggleFavorite", registryHotkeyToggleFavorite);
            RegistryKeyCheckOrCreate(registryPath, "HotkeyToggleView", registryHotkeyToggleView);
            RegistryKeyCheckOrCreate(registryPath, "HotkeyPasteOnHotkey", registryHotkeyPasteOnHotkey);

            regVal = GetRegistryKey(registryPath, "HotkeyBehaviour");
            Logging.Log("    \"HotkeyBehaviour\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "HotkeyToggleApplication");
            Logging.Log("    \"HotkeyToggleApplication\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "HotkeySearch");
            Logging.Log("    \"HotkeySearch\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "HotkeyGetOlderEntry");
            Logging.Log("    \"HotkeyGetOlderEntry\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "HotkeyGetNewerEntry");
            Logging.Log("    \"HotkeyGetNewerEntry\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "HotkeyToggleFavorite");
            Logging.Log("    \"HotkeyToggleFavorite\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "HotkeyToggleView");
            Logging.Log("    \"HotkeyToggleView\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "HotkeyPasteOnHotkey");
            Logging.Log("    \"HotkeyPasteOnHotkey\" = [" + regVal + "]");

            // Layout
            // ------
            Logging.Log("  Layout:");

            // Get the main system display (0-indexed) and the unique identifier for the display setup
            activeDisplay = GetPrimaryDisplay();
            string displaysId = GetUniqueDisplayLayout();

            RegistryKeyCheckOrCreate(registryPath, "HistoryEntries", historyListElements.ToString());
            RegistryKeyCheckOrCreate(registryPath, "HistorySizeWidth", historySizeWidth.ToString());
            RegistryKeyCheckOrCreate(registryPath, "HistorySizeHeight", historySizeHeight.ToString());
            RegistryKeyCheckOrCreate(registryPath, "HistoryMargin", historyMargin.ToString());
            RegistryKeyCheckOrCreate(registryPath, "HistoryLocation", historyLocation);
            RegistryKeyCheckOrCreate(registryPathDisplays, displaysId, activeDisplay.ToString());

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

            // Style
            // -----
            Logging.Log("  Style:");

            RegistryKeyCheckOrCreate(registryPath, "HistoryFontFamily", historyFontFamily);
            RegistryKeyCheckOrCreate(registryPath, "HistoryFontSize", historyFontSize.ToString());
            RegistryKeyCheckOrCreate(registryPath, "HistoryBorderThickness", historyBorderThickness.ToString());
            RegistryKeyCheckOrCreate(registryPath, "HistoryColorTheme", historyColorTheme);
            RegistryKeyCheckOrCreate(registryPath, "IconSet", iconSet);

            regVal = GetRegistryKey(registryPath, "HistoryFontFamily");
            Logging.Log("    \"HistoryFontFamily\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "HistoryFontSize");
            Logging.Log("    \"HistoryFontSize\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "HistoryBorderThickness");
            Logging.Log("    \"HistoryBorderThickness\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "IconSet");
            Logging.Log("    \"IconSet\" = [" + regVal + "]");

            // Colors
            // -----
            Logging.Log("  Colors:");

            RegistryKeyCheckOrCreate(registryPath, "HistoryColorCustomHeader", historyColorsHeader["Custom"]);
            RegistryKeyCheckOrCreate(registryPath, "HistoryColorCustomHeaderText", historyColorsHeaderText["Custom"]);
            RegistryKeyCheckOrCreate(registryPath, "HistoryColorCustomSearch", historyColorsSearch["Custom"]);
            RegistryKeyCheckOrCreate(registryPath, "HistoryColorCustomSearchText", historyColorsSearchText["Custom"]);
            RegistryKeyCheckOrCreate(registryPath, "HistoryColorCustomActive", historyColorsActive["Custom"]);
            RegistryKeyCheckOrCreate(registryPath, "HistoryColorCustomActiveText", historyColorsActiveText["Custom"]);
            RegistryKeyCheckOrCreate(registryPath, "HistoryColorCustomEntry", historyColorsEntry["Custom"]);
            RegistryKeyCheckOrCreate(registryPath, "HistoryColorCustomEntryText", historyColorsEntryText["Custom"]);
            RegistryKeyCheckOrCreate(registryPath, "HistoryColorCustomBorder", historyColorsBorder["Custom"]);

            regVal = GetRegistryKey(registryPath, "HistoryColorTheme");
            Logging.Log("    \"HistoryColorTheme\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "HistoryColorCustomHeader");
            Logging.Log("    \"HistoryColorCustomHeader\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "HistoryColorCustomHeaderText");
            Logging.Log("    \"HistoryColorCustomHeaderText\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "HistoryColorCustomSearch");
            Logging.Log("    \"HistoryColorCustomSearch\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "HistoryColorCustomSearchText");
            Logging.Log("    \"HistoryColorCustomSearchText\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "HistoryColorCustomActive");
            Logging.Log("    \"HistoryColorCustomActive\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "HistoryColorCustomActiveText");
            Logging.Log("    \"HistoryColorCustomActiveText\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "HistoryColorCustomEntry");
            Logging.Log("    \"HistoryColorCustomEntry\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "HistoryColorCustomEntryText");
            Logging.Log("    \"HistoryColorCustomEntryText\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "HistoryColorCustomBorder");
            Logging.Log("    \"HistoryColorCustomBorder\" = [" + regVal + "]");

            // Advanced
            // --------
            Logging.Log("  Advanced:");

            RegistryKeyCheckOrCreate(registryPath, "TroubleshootEnable", registryTroubleshootEnable);

            regVal = GetRegistryKey(registryPath, "TroubleshootEnable");
            Logging.Log("    \"TroubleshootEnable\" = [" + regVal + "]");

            // Misc
            // ----
            Logging.Log("  Misc:");
            RegistryKeyCheckOrCreate(registryPath, "NotificationShown", "0");
            RegistryKeyCheckOrCreate(registryPath, "CheckedVersion", appVer);

            regVal = GetRegistryKey(registryPath, "NotificationShown");
            Logging.Log("    \"NotificationShown\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "checkedVersion");
            Logging.Log("    \"CheckedVersion\" = [" + regVal + "]");
        }


        // ###########################################################################################
        // Check if the main registry path exists - if not then create it
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
        // Get registry key value (string)
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
        // Get registry key value (byte[])
        // ###########################################################################################

        public static byte[] GetRegistryKeyBytes(string path, string key)
        {
            try
            {
                using (RegistryKey getKey = Registry.CurrentUser.OpenSubKey(path))
                {
                    if (getKey != null)
                    {
                        object value = getKey.GetValue(key);
                        if (value != null)
                        {
                            return Convert.FromBase64String(value.ToString());
                        }
                    }
                }
            }
            catch (FormatException ex)
            {
                // Handle the case where the string is not a valid Base64 string
                // Example: Log the exception
                Logging.Log("Invalid Base64 string: " + ex.Message);
            }
            catch (Exception ex)
            {
                // Handle other types of exceptions if necessary
                // Example: Log the exception
                Logging.Log("An error occurred - needs more investigation: " + ex.Message);
            }
            return null;
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
                Logging.Log("Delete registry key \"" + key + "\" from [" + path + "]");
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
            string hotkeyToggleApplication = GetRegistryKey(registryPath, "HotkeyToggleApplication");
            string hotkeySearch = GetRegistryKey(registryPath, "HotkeySearch");
            string hotkeyGetOlderEntry = GetRegistryKey(registryPath, "HotkeyGetOlderEntry");
            string hotkeyGetNewerEntry = GetRegistryKey(registryPath, "HotkeyGetNewerEntry");
            string hotkeyPasteOnHotkey = GetRegistryKey(registryPath, "HotkeyPasteOnHotkey");
            string hotkeyToggleFavorite = GetRegistryKey(registryPath, "HotkeyToggleFavorite");
            string hotkeyToggleView = GetRegistryKey(registryPath, "HotkeyToggleView");
            hotkeyToggleApplication = hotkeyToggleApplication.Length == 0 ? "Not set" : hotkeyToggleApplication;
            hotkeySearch = hotkeySearch.Length == 0 ? "Not set" : hotkeySearch;
            hotkeyGetOlderEntry = hotkeyGetOlderEntry.Length == 0 ? "Not set" : hotkeyGetOlderEntry;
            hotkeyGetNewerEntry = hotkeyGetNewerEntry.Length == 0 ? "Not set" : hotkeyGetNewerEntry;
            hotkeyPasteOnHotkey = hotkeyPasteOnHotkey.Length == 0 ? "Not set" : hotkeyPasteOnHotkey;
            hotkeyToggleFavorite = hotkeyToggleFavorite.Length == 0 ? "Not set" : hotkeyToggleFavorite;
            hotkeyToggleView = hotkeyToggleView.Length == 0 ? "Not set" : hotkeyToggleView;
            GuiHotkeyEnable.Text = hotkeyToggleApplication;
            GuiHotkeySearch.Text = hotkeySearch;
            GuiHotkeyOlder.Text = hotkeyGetOlderEntry;
            GuiHotkeyNewer.Text = hotkeyGetNewerEntry;
            GuiHotkeyPaste.Text = hotkeyPasteOnHotkey;
            GuiHotkeyToggleFavorite.Text = hotkeyToggleFavorite;
            GuiHotkeyToggleView.Text = hotkeyToggleView;
            SetHotkeys("Startup of application");

            // Hotkey behaviour
            string hotkeyBehaviour = GetRegistryKey(registryPath, "HotkeyBehaviour");
            switch (hotkeyBehaviour)
            {
                case "Paste":
                    GuiHotkeyPaste.Enabled = true;
                    GuiHotkeyBehaviourPaste.Checked = true;
                    break;
                default:
                    GuiHotkeyPaste.Enabled = false;
                    GuiHotkeyBehaviourSystem.Checked = true;
                    break;
            }

            // ------------------------------------------
            // "General" tab
            // ------------------------------------------

            // Start with Windows
            string getKey = GetRegistryKey(registryPathRun, "HovText");
            if (getKey == null)
            {
                GuiStartWithWindows.Checked = false;
            }
            else
            {
                GuiStartWithWindows.Checked = true;

                // Overwrite "Run" if it does not contain "HovText.exe" or "--start-minimized"
                string runEntry = GetRegistryKey(registryPathRun, "HovText");
                string thisEntry = "\"" + System.Windows.Forms.Application.ExecutablePath + "\" --start-minimized";
                if (runEntry != thisEntry)
                {
                    SetRegistryKey(registryPathRun, "HovText", "\"" + System.Windows.Forms.Application.ExecutablePath + "\" --start-minimized");
                }
            }

            // Update timer
            updateTimer.Enabled = true;
            AdvancedLabelDevelopmentVersion.Enabled = true;
            AdvancedLabelInfoDevelopment.Enabled = true;
            AdvancedLabelDevelopmentVersion.Text = "Please wait ...";
            Logging.Log("Version check timer started");

            // Startup disabled
            int startDisabled = int.Parse((string)GetRegistryKey(registryPath, "StartDisabled"));
            GuiStartDisabled.Checked = startDisabled == 1;
            isStartDisabled = GuiStartDisabled.Checked;

            // Restore original when disabling application
            int restoreOriginal = int.Parse((string)GetRegistryKey(registryPath, "RestoreOriginal"));
            GuiRestoreOriginal.Checked = restoreOriginal == 1;
            isRestoreOriginal = GuiRestoreOriginal.Checked;

            // Do not copy images
            int copyImages = int.Parse((string)GetRegistryKey(registryPath, "CopyImages"));
            GuiCopyImages.Checked = copyImages == 1;
            isCopyImages = GuiCopyImages.Checked;

            // Close minimizes HovText
            int closeMinimizes = int.Parse((string)GetRegistryKey(registryPath, "CloseMinimizes"));
            GuiCloseMinimize.Checked = closeMinimizes == 1;
            if (GuiCloseMinimize.Checked)
            {
                MinimizeBox = false;
            }
            else
            {
                MinimizeBox = true;
            }

            // Enable history
            int historySearch = int.Parse((string)GetRegistryKey(registryPath, "HistorySearch"));
            int historyInstantSelect = int.Parse((string)GetRegistryKey(registryPath, "HistoryInstantSelect"));
            isEnabledHistory = historySearch == 1 || historyInstantSelect == 1;
            GuiSearch.Checked = historySearch == 1;
            GuiInstantSelect.Checked = historyInstantSelect == 1;

            // Enable favorites
            int favoritesEnabled = int.Parse((string)GetRegistryKey(registryPath, "FavoritesEnable"));
            GuiFavoritesEnabled.Checked = favoritesEnabled == 1;
            isEnabledFavorites = GuiFavoritesEnabled.Checked;
            if (isEnabledFavorites)
            {
                GuiHotkeyToggleFavorite.Enabled = true;
                GuiStorageChooseFavorites.Enabled = true;
            }
            else
            {
                GuiHotkeyToggleFavorite.Enabled = false;
                GuiStorageChooseFavorites.Enabled = false;
            }

            // Paste on history selection
            int pasteOnSelection = int.Parse((string)GetRegistryKey(registryPath, "PasteOnSelection"));
            GuiPasteOnSelection.Checked = pasteOnSelection == 1;
            isEnabledPasteOnSelection = GuiPasteOnSelection.Checked;

            // Paste on history selection
            int alwaysPasteOriginal = int.Parse((string)GetRegistryKey(registryPath, "AlwaysPasteOriginal"));
            GuiAlwaysPasteOriginal.Checked = alwaysPasteOriginal == 1;

            // Trim whitespaces
            int trimWhitespaces = int.Parse((string)GetRegistryKey(registryPath, "TrimWhitespaces"));
            GuiTrimWhitespaces.Checked = trimWhitespaces == 1;
            isEnabledTrimWhitespacing = GuiTrimWhitespaces.Checked;


            // ------------------------------------------
            // "Storage" tab
            // ------------------------------------------

            // Save clipboards on exit
            int saveOnExit = int.Parse((string)GetRegistryKey(registryPath, "StorageSaveOnExit"));
            GuiStorageSaveClipboard.Checked = saveOnExit == 1;

            // Load clipboards on launch
            int loadOnLaunch = int.Parse((string)GetRegistryKey(registryPath, "StorageLoadOnLaunch"));
            GuiStorageLoadClipboard.Checked = loadOnLaunch == 1;

            // Save type (all or favorites only)
            string saveType = GetRegistryKey(registryPath, "StorageSaveType");
            switch (saveType)
            {
                case "All":
                    GuiStorageChooseAll.Checked = true;
                    break;
                default: // Favorites
                    if(isEnabledFavorites)
                    {
                        GuiStorageChooseFavorites.Checked = true;
                    } else
                    {
                        GuiStorageChooseAll.Checked = true;
                        GuiStorageChooseFavorites.Enabled = false;
                    }

                    break;
            }

            // Save number of entries
            int entries = Int32.Parse(GetRegistryKey(registryPath, "StorageSaveEntries"));
            GuiStorageEntries.Value = entries;
            GuiStorageEntriesText.Text = entries.ToString();


            // ------------------------------------------
            // Encryption
            // ------------------------------------------
            encryptionKey = GetRegistryKeyBytes(registryPath, "EncryptionKey");
            encryptionInitializationVector = GetRegistryKeyBytes(registryPath, "EncryptionInitializationVector");


            // ------------------------------------------
            // "Apperance" tab
            // ------------------------------------------

            // Clipboard entries
            historyListElements = Int32.Parse(GetRegistryKey(registryPath, "HistoryEntries"));
            GuiHistoryElements.Value = historyListElements;
            LabelHistoryElements.Text = historyListElements.ToString();

            // History area size
            historySizeWidth = Int32.Parse(GetRegistryKey(registryPath, "HistorySizeWidth"));
            historySizeHeight = Int32.Parse(GetRegistryKey(registryPath, "HistorySizeHeight"));
            historyMargin = Int32.Parse(GetRegistryKey(registryPath, "HistoryMargin"));
            GuiHistorySizeWidth.Value = historySizeWidth;
            GuiHistorySizeHeight.Value = historySizeHeight;
            GuiHistoryMargin.Value = historyMargin;
            LabelHistorySizeWidth.Text = historySizeWidth.ToString() + "%";
            LabelHistorySizeHeight.Text = historySizeHeight.ToString() + "%";
            LabelHistoryMargin.Text = historyMargin.ToString() + "px";
            CheckIfDisableHistoryMargin();

            // History border thickness
            historyBorderThickness = Int32.Parse(GetRegistryKey(registryPath, "HistoryBorderThickness"));
            GuiBorderThickness.Value = historyBorderThickness;
            LabelBorderThickness.Text = historyBorderThickness.ToString();

            // History color theme
            historyColorTheme = GetRegistryKey(registryPath, "HistoryColorTheme");
            historyColorsHeader["Custom"] = GetRegistryKey(registryPath, "HistoryColorCustomHeader");
            historyColorsHeaderText["Custom"] = GetRegistryKey(registryPath, "HistoryColorCustomHeaderText");
            historyColorsSearch["Custom"] = GetRegistryKey(registryPath, "HistoryColorCustomSearch");
            historyColorsSearchText["Custom"] = GetRegistryKey(registryPath, "HistoryColorCustomSearchText");
            historyColorsActive["Custom"] = GetRegistryKey(registryPath, "HistoryColorCustomActive");
            historyColorsActiveText["Custom"] = GetRegistryKey(registryPath, "HistoryColorCustomActiveText");
            historyColorsEntry["Custom"] = GetRegistryKey(registryPath, "HistoryColorCustomEntry");
            historyColorsEntryText["Custom"] = GetRegistryKey(registryPath, "HistoryColorCustomEntryText");
            historyColorsBorder["Custom"] = GetRegistryKey(registryPath, "HistoryColorCustomBorder");
            switch (historyColorTheme)
            {
                case "Blue":
                    GuiHistoryColorThemeBlue.Checked = true;
                    break;
                case "Brown":
                    GuiHistoryColorThemeBrown.Checked = true;
                    break;
                case "Green":
                    GuiHistoryColorThemeGreen.Checked = true;
                    break;
                case "Contrast":
                    GuiHistoryColorThemeContrast.Checked = true;
                    break;
                case "Custom":
                    GuiHistoryColorThemeCustom.Checked = true;
                    EnableDisableCustomColor();
                    break;
                default: // Yellow
                    GuiHistoryColorThemeYellow.Checked = true;
                    break;
            }
            EnableDisableCustomColor();
            SetHistoryColors();

            // History location
            historyLocation = GetRegistryKey(registryPath, "HistoryLocation");
            switch (historyLocation)
            {
                case "Left Top":
                    GuiHistoryLocationRadioLeftTop.Checked = true;
                    break;
                case "Left Bottom":
                    GuiHistoryLocationRadioLeftBottom.Checked = true;
                    break;
                case "Center":
                    GuiHistoryLocationRadioCenter.Checked = true;
                    break;
                case "Right Top":
                    GuiHistoryLocationRadioRightTop.Checked = true;
                    break;
                default: // Right Bottom
                    GuiHistoryLocationRadioRightBottom.Checked = true;
                    break;
            }

            // History font
            historyFontFamily = GetRegistryKey(registryPath, "HistoryFontFamily");
            historyFontSize = float.Parse((string)GetRegistryKey(registryPath, "HistoryFontSize"));
            GuiShowFontHeader.Font = new Font(historyFontFamily, historyFontSize);
            GuiShowFontSearch.Font = new Font(historyFontFamily, historyFontSize);
            GuiShowFont.Text = historyFontFamily + "\r\n" + historyFontSize;
            GuiShowFont.Font = new Font(historyFontFamily, historyFontSize);
            GuiShowFontActive.Font = new Font(historyFontFamily, historyFontSize);
            GuiShowFontEntry.Font = new Font(historyFontFamily, historyFontSize);
            SetHistoryColors();

            // Icon set
            iconSet = GetRegistryKey(registryPath, "IconSet");
            if (iconSet == "SquareOld")
            {
                GuiIconsSquareOld.Select();
            }
            else if (iconSet == "SquareNew")
            {
                GuiIconsSquareNew.Select();
            }

            // Display selection
            string displaysId = GetUniqueDisplayLayout();
            int displayReg = Int32.Parse(GetRegistryKey(registryPathDisplays, displaysId));
            PopulateDisplaySetup();
            if (IsDisplayValid(displayReg))
            {
                activeDisplay = displayReg;
                Logging.Log("History will be shown on display ID [" + activeDisplay + "] with registry entry [" + displaysId + "]");
            }
            else
            {
                Logging.Log("History cannot be shown on display ID [" + displayReg + "] and will instead be shown on display ID [" + activeDisplay + "] with registry entry [" + displaysId + "]");
            }
            GuiLayoutGroup3.Controls["uiScreen" + activeDisplay].Select();


            // ------------------------------------------
            // "Advanced" tab
            // ------------------------------------------

            // Troubleshooting
            int troubleshootEnable = int.Parse((string)GetRegistryKey(registryPath, "TroubleshootEnable"));
            GuiTroubleshootEnabled.Checked = troubleshootEnable == 1;
        }


        // ###########################################################################################
        // Enable or disable that the application starts up with Windows
        // https://www.fluxbytes.com/csharp/start-application-at-windows-startup/
        // ###########################################################################################

        private void GuiStartWithWindows_CheckedChanged(object sender, EventArgs e)
        {
            if (GuiStartWithWindows.Checked)
            {
                SetRegistryKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", "HovText", "\"" + System.Windows.Forms.Application.ExecutablePath + "\" --start-minimized");
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

        private void GuiChangeFont_Click(object sender, EventArgs e)
        {
            FontDialog fontDlg = new FontDialog
            {
                Font = GuiShowFont.Font, // initialize the font dialogue with the font from "uiShowFont"
                AllowVerticalFonts = false,
                FontMustExist = true,
                ShowColor = false,
                ShowApply = false,
                ShowEffects = false,
                ShowHelp = false
            };

            if (fontDlg.ShowDialog() != DialogResult.Cancel)
            {
                historyFontSize = fontDlg.Font.Size;
                historyFontFamily = fontDlg.Font.Name;
                GuiShowFontHeader.Font = new Font(historyFontFamily, historyFontSize);
                GuiShowFontSearch.Font = new Font(historyFontFamily, historyFontSize);
                GuiShowFont.Text = historyFontFamily + "\r\n" + historyFontSize;
                GuiShowFont.Font = new Font(historyFontFamily, historyFontSize);
                GuiShowFontActive.Font = new Font(historyFontFamily, historyFontSize);
                GuiShowFontEntry.Font = new Font(historyFontFamily, historyFontSize);
                SetRegistryKey(registryPath, "HistoryFontFamily", historyFontFamily);
                SetRegistryKey(registryPath, "HistoryFontSize", historyFontSize.ToString());
            }
        }


        // ###########################################################################################
        // Changes in the number of visible history elements
        // ###########################################################################################

        private void HistoryElements_Scroll(object sender, EventArgs e)
        {
            LabelHistoryElements.Text = GuiHistoryElements.Value.ToString();
            historyListElements = GuiHistoryElements.Value;
            SetRegistryKey(registryPath, "HistoryEntries", historyListElements.ToString());
        }


        // ###########################################################################################
        // Disable history margin, if width or height is more tyhan 90%
        // ###########################################################################################

        private void CheckIfDisableHistoryMargin()
        {
            if (GuiHistorySizeWidth.Value > 90 || GuiHistorySizeHeight.Value > 90)
            {
                LayoutLabelMargin.Enabled = false;
                GuiHistoryMargin.Enabled = false;
                LabelHistoryMargin.Enabled = false;
                isHistoryMarginEnabled = false;
            }
            else
            {
                LayoutLabelMargin.Enabled = true;
                GuiHistoryMargin.Enabled = true;
                LabelHistoryMargin.Enabled = true;
                isHistoryMarginEnabled = true;
            }
        }


        // ###########################################################################################
        // Changes in the history (area) size
        // ###########################################################################################

        private void HistorySizeWidth_ValueChanged(object sender, EventArgs e)
        {
            LabelHistorySizeWidth.Text = GuiHistorySizeWidth.Value.ToString() + "%";
            historySizeWidth = GuiHistorySizeWidth.Value;
            SetRegistryKey(registryPath, "HistorySizeWidth", historySizeWidth.ToString());

            // Disable margin, if above 90%
            CheckIfDisableHistoryMargin();
        }

        private void HistorySizeHeight_ValueChanged(object sender, EventArgs e)
        {
            LabelHistorySizeHeight.Text = GuiHistorySizeHeight.Value.ToString() + "%";
            historySizeHeight = GuiHistorySizeHeight.Value;
            SetRegistryKey(registryPath, "HistorySizeHeight", historySizeHeight.ToString());

            // Disable margin, if above 90%
            CheckIfDisableHistoryMargin();
        }

        private void GuiHistoryMargin_ValueChanged(object sender, EventArgs e)
        {
            LabelHistoryMargin.Text = GuiHistoryMargin.Value.ToString() + "px";
            historyMargin = GuiHistoryMargin.Value;
            SetRegistryKey(registryPath, "HistoryMargin", historyMargin.ToString());
        }


        // ###########################################################################################
        // Change in the history location
        // ###########################################################################################

        private void GuiHistoryLocationRadioLeftTop_CheckedChanged(object sender, EventArgs e)
        {
            if (GuiHistoryLocationRadioLeftTop.Checked)
            {
                historyLocation = "Left Top";
                SetRegistryKey(registryPath, "HistoryLocation", historyLocation);
            }
        }

        private void GuiHistoryLocationRadioLeftBottom_CheckedChanged(object sender, EventArgs e)
        {
            if (GuiHistoryLocationRadioLeftBottom.Checked)
            {
                historyLocation = "Left Bottom";
                SetRegistryKey(registryPath, "HistoryLocation", historyLocation);
            }
        }

        private void GuiHistoryLocationRadioCenter_CheckedChanged(object sender, EventArgs e)
        {
            if (GuiHistoryLocationRadioCenter.Checked)
            {
                historyLocation = "Center";
                SetRegistryKey(registryPath, "HistoryLocation", historyLocation);
            }
        }

        private void GuiHistoryLocationRadioRightTop_CheckedChanged(object sender, EventArgs e)
        {
            if (GuiHistoryLocationRadioRightTop.Checked)
            {
                historyLocation = "Right Top";
                SetRegistryKey(registryPath, "HistoryLocation", historyLocation);
            }
        }

        private void GuiHistoryLocationRadioRightBottom_CheckedChanged(object sender, EventArgs e)
        {
            if (GuiHistoryLocationRadioRightBottom.Checked)
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
            if (GuiHistoryColorThemeCustom.Checked)
            {
                GuiStyleGroup4.Enabled = true;
                GuiCustomHeader.Enabled = true;
                GuiCustomHeaderText.Enabled = true;
                GuiCustomSearch.Enabled = true;
                GuiCustomSearchText.Enabled = true;
                GuiCustomEntry.Enabled = true;
                GuiCustomEntryText.Enabled = true;
                GuiCustomActive.Enabled = true;
                GuiCustomActiveText.Enabled = true;
                GuiCustomBorder.Enabled = true;

                StyleLabelHeaderColorBackground.Enabled = true;
                StyleLabelHeaderColorText.Enabled = true;
                StyleLabelSearchColorBackground.Enabled = true;
                StyleLabelSearchColorText.Enabled = true;
                StyleLabelEntryColorBackground.Enabled = true;
                StyleLabelEntryColorText.Enabled = true;
                StyleLabelActiveColorBackground.Enabled = true;
                StyleLabelActiveColorText.Enabled = true;
                StyleLabelBorderColor.Enabled = true;

                GuiCustomHeader.BackColor = ColorTranslator.FromHtml(historyColorsHeader["Custom"]);
                GuiCustomHeaderText.BackColor = ColorTranslator.FromHtml(historyColorsHeaderText["Custom"]);
                GuiCustomSearch.BackColor = ColorTranslator.FromHtml(historyColorsSearch["Custom"]);
                GuiCustomSearchText.BackColor = ColorTranslator.FromHtml(historyColorsSearchText["Custom"]);
                GuiCustomActive.BackColor = ColorTranslator.FromHtml(historyColorsActive["Custom"]);
                GuiCustomActiveText.BackColor = ColorTranslator.FromHtml(historyColorsActiveText["Custom"]);
                GuiCustomEntry.BackColor = ColorTranslator.FromHtml(historyColorsEntry["Custom"]);
                GuiCustomEntryText.BackColor = ColorTranslator.FromHtml(historyColorsEntryText["Custom"]);
                GuiCustomBorder.BackColor = ColorTranslator.FromHtml(historyColorsBorder["Custom"]);
            }
            else
            {
                GuiStyleGroup4.Enabled = false;
                GuiCustomHeader.Enabled = false;
                GuiCustomHeaderText.Enabled = false;
                GuiCustomSearch.Enabled = false;
                GuiCustomSearchText.Enabled = false;
                GuiCustomActive.Enabled = false;
                GuiCustomActiveText.Enabled = false;
                GuiCustomEntry.Enabled = false;
                GuiCustomEntryText.Enabled = false;
                GuiCustomBorder.Enabled = false;

                StyleLabelHeaderColorBackground.Enabled = false;
                StyleLabelHeaderColorText.Enabled = false;
                StyleLabelSearchColorBackground.Enabled = false;
                StyleLabelSearchColorText.Enabled = false;
                StyleLabelEntryColorBackground.Enabled = false;
                StyleLabelEntryColorText.Enabled = false;
                StyleLabelActiveColorBackground.Enabled = false;
                StyleLabelActiveColorText.Enabled = false;
                StyleLabelBorderColor.Enabled = false;

                GuiCustomHeader.BackColor = Color.WhiteSmoke;
                GuiCustomHeaderText.BackColor = Color.WhiteSmoke;
                GuiCustomSearch.BackColor = Color.WhiteSmoke;
                GuiCustomSearchText.BackColor = Color.WhiteSmoke;
                GuiCustomActive.BackColor = Color.WhiteSmoke;
                GuiCustomActiveText.BackColor = Color.WhiteSmoke;
                GuiCustomEntry.BackColor = Color.WhiteSmoke;
                GuiCustomEntryText.BackColor = Color.WhiteSmoke;
                GuiCustomBorder.BackColor = Color.WhiteSmoke;
            }
        }


        // ###########################################################################################
        // Change in the history color
        // ###########################################################################################

        private void GuiHistoryColorTheme_CheckedChanged(object sender, EventArgs e)
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
            GuiShowFontHeader.BackColor = ColorTranslator.FromHtml(historyColorsHeader[historyColorTheme]);
            GuiShowFontHeader.ForeColor = ColorTranslator.FromHtml(historyColorsHeaderText[historyColorTheme]);
            GuiShowFontSearch.BackColor = ColorTranslator.FromHtml(historyColorsSearch[historyColorTheme]);
            GuiShowFontSearch.ForeColor = ColorTranslator.FromHtml(historyColorsSearchText[historyColorTheme]);
            GuiShowFontActive.BackColor = ColorTranslator.FromHtml(historyColorsActive[historyColorTheme]);
            GuiShowFontActive.ForeColor = ColorTranslator.FromHtml(historyColorsActiveText[historyColorTheme]);
            GuiShowFontEntry.BackColor = ColorTranslator.FromHtml(historyColorsEntry[historyColorTheme]);
            GuiShowFontEntry.ForeColor = ColorTranslator.FromHtml(historyColorsEntryText[historyColorTheme]);
            GuiShowFont.BackColor = ColorTranslator.FromHtml(historyColorsActive[historyColorTheme]);
            GuiShowFont.ForeColor = ColorTranslator.FromHtml(historyColorsActiveText[historyColorTheme]);
        }


        // ###########################################################################################
        // Changes in "Close minimizes application to tray"
        // ###########################################################################################

        private void GuiCloseMinimize_CheckedChanged(object sender, EventArgs e)
        {
            string status = GuiCloseMinimize.Checked ? "1" : "0";
            SetRegistryKey(registryPath, "CloseMinimizes", status);
            if (GuiCloseMinimize.Checked)
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

        private void GuiRestoreOriginal_CheckedChanged(object sender, EventArgs e)
        {
            // History enabled
            string status = GuiRestoreOriginal.Checked ? "1" : "0";
            isRestoreOriginal = GuiRestoreOriginal.Checked;
            SetRegistryKey(registryPath, "RestoreOriginal", status);
        }


        // ###########################################################################################
        // Changes in "Do not copy images"
        // ###########################################################################################

        private void GuiCopyImages_CheckedChanged(object sender, EventArgs e)
        {
            string status = GuiCopyImages.Checked ? "1" : "0";
            isCopyImages = GuiCopyImages.Checked;
            SetRegistryKey(registryPath, "CopyImages", status);
        }


        // ###########################################################################################
        // Changes in "Enable history"
        // ###########################################################################################

        private void GuiSearch_CheckedChanged(object sender, EventArgs e)
        {
            string status = GuiSearch.Checked ? "1" : "0";
            SetRegistryKey(registryPath, "HistorySearch", status);
            RemoveAllHotkeys();
            SetFieldsBasedOnHistoryEnabled();
        }

        private void GuiInstantSelect_CheckedChanged(object sender, EventArgs e)
        {
            // History enabled
            string status = GuiInstantSelect.Checked ? "1" : "0";
            SetRegistryKey(registryPath, "HistoryInstantSelect", status);
            RemoveAllHotkeys();
            SetFieldsBasedOnHistoryEnabled();
        }

        private void SetFieldsBasedOnHistoryEnabled()
        {
            isEnabledHistory = GuiInstantSelect.Checked || GuiSearch.Checked;

            if (GuiSearch.Checked)
            {
                GuiHotkeySearch.Enabled = true;
                label3.Enabled = true;
            }
            else
            {
                GuiHotkeySearch.Enabled = false;
                label3.Enabled = false;
            }

            if (GuiInstantSelect.Checked)
            {
                GuiHotkeyOlder.Enabled = true;
                GuiHotkeyNewer.Enabled = true;
                label5.Enabled = true;
                label9.Enabled = true;
            }
            else
            {
                GuiHotkeyOlder.Enabled = false;
                GuiHotkeyNewer.Enabled = false;
                label5.Enabled = false;
                label9.Enabled = false;
            }

            if (GuiFavoritesEnabled.Checked)
            {
                GuiHotkeyToggleFavorite.Enabled = true;
                GuiHotkeyToggleView.Enabled = true;
            }
            else
            {
                GuiHotkeyToggleFavorite.Enabled = false;
                GuiHotkeyToggleView.Enabled = false;
            }

            if (isEnabledHistory)
            {
                GuiCopyImages.Enabled = true;
                GuiPasteOnSelection.Enabled = true;
                GuiAlwaysPasteOriginal.Enabled = true;
                GuiFavoritesEnabled.Enabled = true;
            }
            else
            {
                GuiCopyImages.Enabled = false;
                GuiPasteOnSelection.Enabled = false;
                GuiAlwaysPasteOriginal.Enabled = false;
                GuiFavoritesEnabled.Enabled = false;
            }

            // Set the clipboard again, as there could be changes how "GuiAlwaysPasteOriginal" behaves
            SetClipboard();

            // Enable/disable hotkeys
            SetHotkeys("Enable history change");

            // Update the tray icon
            UpdateNotifyIconText();
        }


        // ###########################################################################################
        // Changes in "Always paste original/formatted clipboard text"
        // ###########################################################################################

        private void GuiAlwaysPasteOriginal_CheckedChanged(object sender, EventArgs e)
        {
            string status = GuiAlwaysPasteOriginal.Checked ? "1" : "0";
            SetRegistryKey(registryPath, "AlwaysPasteOriginal", status);

            SetClipboard();
        }


        // ###########################################################################################
        // Changes in "Enable favorites"
        // ###########################################################################################

        private void GuiFavoritesEnabled_CheckedChanged(object sender, EventArgs e)
        {
            string status = GuiFavoritesEnabled.Checked ? "1" : "0";
            isEnabledFavorites = GuiFavoritesEnabled.Checked;
            SetRegistryKey(registryPath, "FavoritesEnable", status);
            if (isEnabledFavorites)
            {
                GuiHotkeyToggleFavorite.Enabled = true;
                GuiStorageChooseFavorites.Enabled = true;
            }
            else
            {
                GuiHotkeyToggleFavorite.Enabled = false;
                GuiStorageChooseFavorites.Enabled = false;
                showFavoriteList = false;

                // If we previously did save "Favorites only" then we need to change that to "All"
                if(GuiStorageChooseFavorites.Checked)
                {
                    GuiStorageChooseAll.Checked = true;
                    GuiStorageChooseFavorites.Enabled = false;
                }
            }

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


        // ###########################################################################################
        // Changes in "Paste on history selection"
        // ###########################################################################################

        private void GuiPasteOnSelection_CheckedChanged(object sender, EventArgs e)
        {
            string status = GuiPasteOnSelection.Checked ? "1" : "0";
            isEnabledPasteOnSelection = GuiPasteOnSelection.Checked;
            SetRegistryKey(registryPath, "PasteOnSelection", status);
        }


        // ###########################################################################################
        // Changes in "Trim whitespaces"
        // ###########################################################################################

        private void GuiTrimWhitespaces_CheckedChanged(object sender, EventArgs e)
        {
            string status = GuiTrimWhitespaces.Checked ? "1" : "0";
            isEnabledTrimWhitespacing = GuiTrimWhitespaces.Checked;
            SetRegistryKey(registryPath, "TrimWhitespaces", status);
        }


        // ###########################################################################################
        // Changes in "Start disabled"
        // ###########################################################################################

        private void GuiStartDisabled_CheckedChanged(object sender, EventArgs e)
        {
            // Start as disabled
            string status = GuiStartDisabled.Checked ? "1" : "0";
            isStartDisabled = GuiStartDisabled.Checked;
            SetRegistryKey(registryPath, "StartDisabled", status);
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

        private void TrayIconAbout_Click(object sender, EventArgs e)
        {
            Logging.Log("Clicked tray icon \"About\"");
            ShowSettingsForm();
            TabControl.SelectedIndex = 7; // About
        }


        // ###########################################################################################
        // When clicking the "Settings" in the tray icon menu
        // ###########################################################################################

        private void TrayIconSettings_Click(object sender, EventArgs e)
        {
            ShowSettingsForm();
            TabControl.SelectedIndex = 0; // General
            Logging.Log("Clicked tray icon \"Settings\"");
        }


        // ###########################################################################################
        // When clicking the "Exit" in the tray icon menu
        // ###########################################################################################

        private void TrayIconExit_Click(object sender, EventArgs e)
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

        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Give the double-click a chance to cancel this
                mouseClickTimer.Start();
            }
        }

        private void MouseClickTimer_Tick(object sender, EventArgs e)
        {
            Logging.Log("Tray icon single-clicked - toggling application enable/disable");
            mouseClickTimer.Stop();
            ToggleEnabled();
        }


        // ###########################################################################################
        // When double-clicking with the mouse on the tray icon
        // ###########################################################################################

        private void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Logging.Log("Tray icon double-clicked - opening \"Settings\"");

                // Cancel the single-click
                mouseClickTimer.Stop();

                ShowSettingsForm();
                TabControl.SelectedIndex = 0;
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

        private void GuiHotkeyBehaviourSystem_CheckedChanged(object sender, EventArgs e)
        {
            // Only react if this one gets checked - startup will trigger an event where this is false
            if (GuiHotkeyBehaviourSystem.Checked)
            {
                GuiHotkeyPaste.Enabled = false;
                SetRegistryKey(registryPath, "HotkeyBehaviour", "System");
                GuiRestoreOriginal.Enabled = true;
                SetNotifyIcon();
            }
        }


        // ###########################################################################################
        // Changes in "Action only on hotkey" hotkey behaviour
        // ###########################################################################################

        private void GuiHotkeyBehaviourPaste_CheckedChanged(object sender, EventArgs e)
        {
            GuiHotkeyPaste.Enabled = true;
            SetRegistryKey(registryPath, "HotkeyBehaviour", "Paste");
            SetHotkeys("Hotkey behaviour change");
            GuiRestoreOriginal.Enabled = false;
            SetNotifyIcon();
        }


        // ###########################################################################################
        // Clicking the tab help button
        // ###########################################################################################

        private void GuiHelp_Click(object sender, EventArgs e)
        {
            string selectedTab = TabControl.SelectedTab.AccessibilityObject.Name;

            // Show the specific help for a "Development" version
            string releaseTrain = "";
            releaseTrain += buildType == "Debug" ? "-dev" : "";

            System.Diagnostics.Process.Start(hovtextPage + "/documentation" + releaseTrain + "/#" + selectedTab);
            Logging.Log("Clicked the \"Help\" for \"" + selectedTab + "\"");
        }


        // ###########################################################################################
        // Action for the "Toggle application on/off" hotkey
        // ###########################################################################################

        private void HotkeyToggleApplication(object sender, NHotkey.HotkeyEventArgs e)
        {
            ToggleEnabled();
            e.Handled = true;
        }


        // ###########################################################################################
        // Action for the "Search" hotkey
        // ###########################################################################################

        private void HotkeySearch(object sender, NHotkey.HotkeyEventArgs e)
        {
            // Check if application is enabled
            if (isApplicationEnabled && entryCounter > 0)
            {

                // Hide the history list again, if it already is visible and I pressed the hotkey again
                if (history.Visible)
                {
                    history.ActionEscape();
                }
                else
                {

                    Logging.Log("Pressed the \"Search\" interface hotkey");

                    if (isFirstCallAfterHotkey)
                    {
                        // Hide the "Settings" form if it is visible (it will be restored after key-up)
                        isSettingsFormVisible = this.Visible;
                        if (isSettingsFormVisible)
                        {
                            Hide();
                        }
                        originatingApplicationName = GetActiveApplication();
                        showFavoriteListLast = showFavoriteList;
                        showFavoriteList = false;
                        history.SetupForm();
                    }
                    // Always change focus to HovText to ensure we can catch the key-up event
                    ChangeFocusToHovText();

                    // Only proceed if the entry counter is equal to or more than 0
                    if (entryCounter > 0)
                    {
                        isFirstCallAfterHotkey = false;
                        history.UpdateHistory("");
                    }
                }

            }
            e.Handled = true;
        }


        // ###########################################################################################
        // Action for the "Show older entry" hotkey
        // ###########################################################################################

        private void HotkeyGetOlderEntry(object sender, NHotkey.HotkeyEventArgs e)
        {
            Logging.Log("Pressed the \"Get older history entry\" hotkey");
            isHistoryHotkeyPressed = true;
            GoEntryLowerNumber();

            e.Handled = true;
        }


        // ###########################################################################################
        // Action for the "Show newer entry" hotkey
        // ###########################################################################################

        private void HotkeyGetNewerEntry(object sender, NHotkey.HotkeyEventArgs e)
        {
            Logging.Log("Pressed the \"Get newer history entry\" hotkey");
            isHistoryHotkeyPressed = true;
            GoEntryHigherNumber();

            e.Handled = true;
        }


        // ###########################################################################################
        // Action for the "Paste on hotkey" hotkey
        // ###########################################################################################

        private void HotkeyPasteOnHotkey(object sender, NHotkey.HotkeyEventArgs e)
        {
            Logging.Log("Pressed the \"Paste only on hotkey\" hotkey");

            // Only proceed if there are clipboard entries
            if (entriesText.Count > 0)
            {
                // Get active application and change focus to HovText
                originatingApplicationName = GetActiveApplication();
                ChangeFocusToHovText();

                // Show the invisible form, so we can catch the key-up event
                pasteOnHotkey.Show();
                pasteOnHotkey.Activate();
                pasteOnHotkey.TopMost = true;
            }

            e.Handled = true;
        }


        // ###########################################################################################
        // Rearrange the dynamic labels for the "interface" hotkeys
        // ###########################################################################################

        private void UpdateLocationDynamicLabels()
        {
            Graphics g = GuiSearch.CreateGraphics();
            SizeF size = g.MeasureString(GuiSearch.Text, GuiSearch.Font);
            label3.Text = "(" + GuiHotkeySearch.Text + ")";
            int centeredY = GuiSearch.Location.Y + (GuiSearch.Height - label3.Height) / 2;
            label3.Location = new Point(GuiSearch.Location.X + (int)size.Width + 10 + 10, centeredY);

            g = GuiInstantSelect.CreateGraphics();
            size = g.MeasureString(GuiInstantSelect.Text, GuiInstantSelect.Font);
            label5.Text = "(" + GuiHotkeyOlder.Text + ")";
            centeredY = GuiInstantSelect.Location.Y + (GuiInstantSelect.Height - label5.Height) / 2;
            label5.Location = new Point(GuiInstantSelect.Location.X + (int)size.Width + 10 + 10, centeredY);

            g = label3.CreateGraphics();
            size = g.MeasureString(label3.Text, label3.Font);
            label9.Text = "(" + GuiHotkeyNewer.Text + ")";
            centeredY = label5.Location.Y + (label5.Height - label9.Height) / 2;
            label9.Location = new Point(label5.Location.X + (int)size.Width + 12, centeredY);
        }


        // ###########################################################################################
        // Hotkey handling - how do I get this in to its own file or class!? Hmmm... for later
        // I feel this code is clumsy and inefficient so probably need rewrite!?
        // ###########################################################################################


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
            keyCode = hotkey == "hotkeyToggleFavorite" && keyCode == "Space" ? "Unsupported" : keyCode;

            string[] alphaKeys = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray().Select(c => c.ToString()).ToArray();
            string[] numericKeys = Enumerable.Range(0, 10).Select(n => $"D{n}").ToArray(); // Correct representation for numeric keys

            foreach (string key in alphaKeys.Concat(numericKeys))
            {
                keyCode = hotkey == "hotkeyToggleFavorite" && keyCode == key ? "Unsupported" : keyCode;
            }

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
        // Catch keyboard input on the hotkey fields
        // ###########################################################################################

        private void HotkeyEnable_KeyDown(object sender, KeyEventArgs e)
        {
            string hotkey = ConvertKeyboardInputToString(e);
            GuiHotkeyEnable.Text = hotkey;
            if (e.Alt)
            {
                // https://stackoverflow.com/a/3068797/2028935
                e.SuppressKeyPress = true;
            }
        }

        private void GuiHotkeySearch_KeyDown(object sender, KeyEventArgs e)
        {
            string hotkey = ConvertKeyboardInputToString(e);
            GuiHotkeySearch.Text = hotkey;

            if (e.Alt)
            {
                // https://stackoverflow.com/a/3068797/2028935
                e.SuppressKeyPress = true;
            }
        }

        private void HotkeyOlder_KeyDown(object sender, KeyEventArgs e)
        {
            string hotkey = ConvertKeyboardInputToString(e);
            GuiHotkeyOlder.Text = hotkey;
            if (e.Alt)
            {
                // https://stackoverflow.com/a/3068797/2028935
                e.SuppressKeyPress = true;
            }
        }

        private void HotkeyNewer_KeyDown(object sender, KeyEventArgs e)
        {
            string hotkey = ConvertKeyboardInputToString(e);
            GuiHotkeyNewer.Text = hotkey;
            if (e.Alt)
            {
                // https://stackoverflow.com/a/3068797/2028935
                e.SuppressKeyPress = true;
            }
        }

        private void HotkeyPaste_KeyDown(object sender, KeyEventArgs e)
        {
            string hotkey = ConvertKeyboardInputToString(e);
            GuiHotkeyPaste.Text = hotkey;
            if (e.Alt)
            {
                // https://stackoverflow.com/a/3068797/2028935
                e.SuppressKeyPress = true;
            }
        }

        private void HotkeyToggleFavorite_KeyDown(object sender, KeyEventArgs e)
        {
            string hotkey = ConvertKeyboardInputToString(e);
            GuiHotkeyToggleFavorite.Text = hotkey;
            if (e.Alt)
            {
                // https://stackoverflow.com/a/3068797/2028935
                e.SuppressKeyPress = true;
            }
        }

        private void HotkeyToggleView_KeyDown(object sender, KeyEventArgs e)
        {
            string hotkey = ConvertKeyboardInputToString(e);
            GuiHotkeyToggleView.Text = hotkey;
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

        private void GuiHotkeySearch_Enter(object sender, EventArgs e)
        {
            hotkey = "hotkeySearch";
            ModifyHotkey();
        }

        private void HotkeyOlder_Enter(object sender, EventArgs e)
        {
            hotkey = "hotkeyOlder";
            ModifyHotkey();
        }

        private void HotkeyNewer_Enter(object sender, EventArgs e)
        {
            hotkey = "hotkeyNewer";
            ModifyHotkey();
        }

        private void HotkeyPaste_Enter(object sender, EventArgs e)
        {
            hotkey = "hotkeyPaste";
            ModifyHotkey();
        }

        private void HotkeyToggleFavorite_Enter(object sender, EventArgs e)
        {
            hotkey = "hotkeyToggleFavorite";
            ModifyHotkey();
        }

        private void HotkeyToggleView_Enter(object sender, EventArgs e)
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
                    GuiHotkeyEnable.BackColor = SystemColors.Info;
                    break;
                case "hotkeySearch":
                    GuiHotkeySearch.BackColor = SystemColors.Info;
                    break;
                case "hotkeyOlder":
                    GuiHotkeyOlder.BackColor = SystemColors.Info;
                    break;
                case "hotkeyNewer":
                    GuiHotkeyNewer.BackColor = SystemColors.Info;
                    break;
                case "hotkeyPaste":
                    GuiHotkeyPaste.BackColor = SystemColors.Info;
                    break;
                case "hotkeyToggleFavorite":
                    GuiHotkeyToggleFavorite.BackColor = SystemColors.Info;
                    break;
                case "hotkeyToggleView":
                    GuiHotkeyToggleView.BackColor = SystemColors.Info;
                    break;
            }

            // Enable the two buttons, "Apply" and "Cancel"
            GuiApplyHotkey.Enabled = true;
            GuiCancelHotkey.Enabled = true;

            // Make sure to remove all the active application hotkeys
            RemoveAllHotkeys();
        }


        // ###########################################################################################
        // Remove all the hotkeys
        // ###########################################################################################

        public static void RemoveAllHotkeys()
        {
            HotkeyManager.Current.Remove("ToggleApplication");
            HotkeyManager.Current.Remove("Search");
            HotkeyManager.Current.Remove("GetOlderEntry");
            HotkeyManager.Current.Remove("GetNewerEntry");
            HotkeyManager.Current.Remove("PasteOnHotkey");
            Logging.Log("[HotkeyToggleApplication] removed");
            Logging.Log("[HotkeySearch] removed");
            Logging.Log("[HotkeyGetOlderEntry] removed");
            Logging.Log("[HotkeyGetNewerEntry] removed");
            Logging.Log("[HotkeyPasteOnHotkey] removed");
        }


        // ###########################################################################################
        // Apply the hotkeys
        // ###########################################################################################

        private void ApplyHotkeys_Click(object sender, EventArgs e)
        {
            SetHotkeys("Apply hotkeys button press");
            UpdateLocationDynamicLabels();
        }


        // ###########################################################################################
        // Set (or remove) the hotkeys and validate them
        // ###########################################################################################

        private void SetHotkeys(string from)
        {
            Logging.Log("Called \"SetHotkeys()\" from \"" + from + "\"");

            // Get all hotkey strings
            string hotkeyToggleApplication = GuiHotkeyEnable.Text;
            string hotkeySearch = GuiHotkeySearch.Text;
            string hotkeyGetOlderEntry = GuiHotkeyOlder.Text;
            string hotkeyGetNewerEntry = GuiHotkeyNewer.Text;
            string hotkeyPasteOnHotkey = GuiHotkeyPaste.Text;
            string hotkeyToggleFavorite = GuiHotkeyToggleFavorite.Text;
            string hotkeyToggleView = GuiHotkeyToggleView.Text;

            string conflictText = "";
            KeysConverter cvt;
            Keys key;

            // Convert the strings to hotkey objects

            if (GuiSearch.Checked)
            {
                // "Search"
                if (
                    hotkeySearch != "Unsupported"
                    && hotkeySearch != "Not set"
                    && hotkeySearch != "Hotkey conflicts"
                    )
                {
                    try
                    {
                        cvt = new KeysConverter();
                        key = (Keys)cvt.ConvertFrom(hotkeySearch);
                        HotkeyManager.Current.AddOrReplace("Search", key, HotkeySearch);
                        Logging.Log("[HotkeySearch] added as global hotkey and set to [" + hotkeySearch + "]");
                    }
                    catch (Exception ex)
                    {
                        Logging.Log("Exception #6 raised (Settings):");
                        Logging.Log("  Hotkey [HotkeySearch] conflicts");
                        Logging.Log("  " + ex.Message);
                        if (ex.Message.Contains("Hot key is already registered"))
                        {
                            hotkeyToggleApplication = "Hotkey conflicts";
                            conflictText += "Hotkey for \"Search\" conflicts with another application\r\n";
                        }
                    }
                }
            }

            // "Application toggle"
            if (
                hotkeyToggleApplication != "Unsupported"
                && hotkeyToggleApplication != "Not set"
                && hotkeyToggleApplication != "Hotkey conflicts"
                )
            {
                try
                {
                    cvt = new KeysConverter();
                    key = (Keys)cvt.ConvertFrom(hotkeyToggleApplication);
                    HotkeyManager.Current.AddOrReplace("ToggleApplication", key, HotkeyToggleApplication);
                    Logging.Log("[HotkeyToggleApplication] added as global hotkey and set to [" + hotkeyToggleApplication + "]");
                }
                catch (Exception ex)
                {
                    Logging.Log("Exception #6 raised (Settings):");
                    Logging.Log("  Hotkey [HotkeyToggleApplication] conflicts");
                    Logging.Log("  " + ex.Message);
                    if (ex.Message.Contains("Hot key is already registered"))
                    {
                        hotkeyToggleApplication = "Hotkey conflicts";
                        conflictText += "Hotkey for \"Toggle application on/off\" conflicts with another application\r\n";
                    }
                }
            }

            if (GuiInstantSelect.Checked)
            {
                // "Get older entry"
                if (
                    hotkeyGetOlderEntry != "Unsupported"
                    && hotkeyGetOlderEntry != "Not set"
                    && hotkeyGetOlderEntry != "Hotkey conflicts"
                    && GuiInstantSelect.Checked
                    )
                {
                    try
                    {
                        cvt = new KeysConverter();
                        key = (Keys)cvt.ConvertFrom(hotkeyGetOlderEntry);
                        HotkeyManager.Current.AddOrReplace("GetOlderEntry", key, HotkeyGetOlderEntry);
                        Logging.Log("[HotkeyGetOlderEntry] added as global hotkey and set to [" + hotkeyGetOlderEntry + "]");
                    }
                    catch (Exception ex)
                    {
                        Logging.Log("Exception #7 raised (Settings):");
                        Logging.Log("  Hotkey [HotkeyGetOlderEntry] conflicts");
                        Logging.Log("  " + ex.Message);
                        if (ex.Message.Contains("Hot key is already registered"))
                        {
                            hotkeyGetOlderEntry = "Hotkey conflicts";
                            conflictText += "Hotkey for \"Show older entry\" conflicts with another application\r\n";
                        }
                    }
                }
                else
                {
                    // Remove the hotkey, if it is not available
                    HotkeyManager.Current.Remove("GetOlderEntry");
                }

                // "Get newer entry"
                if (
                    hotkeyGetNewerEntry != "Unsupported"
                    && hotkeyGetNewerEntry != "Not set"
                    && hotkeyGetNewerEntry != "Hotkey conflicts"
                    && GuiInstantSelect.Checked
                    )
                {
                    try
                    {
                        cvt = new KeysConverter();
                        key = (Keys)cvt.ConvertFrom(hotkeyGetNewerEntry);
                        HotkeyManager.Current.AddOrReplace("GetNewerEntry", key, HotkeyGetNewerEntry);
                        Logging.Log("[HotkeyGetNewerEntry] added as global hotkey and set to [" + hotkeyGetNewerEntry + "]");
                    }
                    catch (Exception ex)
                    {
                        Logging.Log("Exception #8 raised (Settings):");
                        Logging.Log("  Hotkey [HotkeyGetNewerEntry] conflicts");
                        Logging.Log("  " + ex.Message);
                        if (ex.Message.Contains("Hot key is already registered"))
                        {
                            hotkeyGetNewerEntry = "Hotkey conflicts";
                            conflictText += "Hotkey for \"Show newer entry\" conflicts with another application\r\n";
                        }
                    }
                }
                else
                {
                    // Remove the hotkey, if it is not available
                    HotkeyManager.Current.Remove("GetNewerEntry");
                }
            }

            // "Paste only on hotkey"
            if (
                hotkeyPasteOnHotkey != "Unsupported"
                && hotkeyPasteOnHotkey != "Not set"
                && hotkeyPasteOnHotkey != "Hotkey conflicts"
                && GuiHotkeyBehaviourPaste.Checked
                )
            {
                try
                {
                    cvt = new KeysConverter();
                    key = (Keys)cvt.ConvertFrom(hotkeyPasteOnHotkey);

                    // Only (re)enable it, if the "Action only on hotkey" behaviour has been chosen
                    if (GuiHotkeyBehaviourPaste.Checked)
                    {
                        HotkeyManager.Current.AddOrReplace("PasteOnHotkey", key, HotkeyPasteOnHotkey);
                        Logging.Log("[HotkeyPasteOnHotkey] added as global hotkey and set to [" + hotkeyPasteOnHotkey + "]");
                    }
                }
                catch (Exception ex)
                {
                    Logging.Log("Exception #9 raised (Settings):");
                    Logging.Log("  Hotkey [HotkeyPasteOnHotkey] conflicts");
                    Logging.Log("  " + ex.Message);
                    if (ex.Message.Contains("Hot key is already registered"))
                    {
                        hotkeyPasteOnHotkey = "Hotkey conflicts";
                        conflictText += "Hotkey for \"Paste on hotkey\" conflicts with another application\r\n";
                    }
                }
            }
            else
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
                TabControl.SelectedIndex = 1;
            }

            // Save the hotkeys to registry, if no erros
            if (
                hotkeyToggleApplication != "Unsupported" && hotkeyToggleApplication != "Hotkey conflicts" &&
                hotkeySearch != "Unsupported" && hotkeySearch != "Hotkey conflicts" &&
                hotkeyGetOlderEntry != "Unsupported" && hotkeyGetOlderEntry != "Hotkey conflicts" &&
                hotkeyGetNewerEntry != "Unsupported" && hotkeyGetNewerEntry != "Hotkey conflicts" &&
                hotkeyPasteOnHotkey != "Unsupported" && hotkeyPasteOnHotkey != "Hotkey conflicts" &&
                hotkeyToggleFavorite != "Unsupported" && hotkeyToggleFavorite != "Hotkey conflicts" &&
                hotkeyToggleView != "Unsupported" && hotkeyToggleView != "Hotkey conflicts"
                )
            {
                SetRegistryKey(registryPath, "HotkeyToggleApplication", hotkeyToggleApplication);
                SetRegistryKey(registryPath, "HotkeySearch", hotkeySearch);
                SetRegistryKey(registryPath, "HotkeyGetOlderEntry", hotkeyGetOlderEntry);
                SetRegistryKey(registryPath, "HotkeyGetNewerEntry", hotkeyGetNewerEntry);
                SetRegistryKey(registryPath, "HotkeyPasteOnHotkey", hotkeyPasteOnHotkey);
                SetRegistryKey(registryPath, "HotkeyToggleFavorite", hotkeyToggleFavorite);
                SetRegistryKey(registryPath, "HotkeyToggleView", hotkeyToggleView);
            }

            bool hasError = false;

            // Update the hotkey fields to reflect if they are good or bad

            // "Application toggle"
            if (hotkeyToggleApplication == "Unsupported" || hotkeyToggleApplication == "Hotkey conflicts")
            {
                hasError = true;
                GuiHotkeyEnable.Text = hotkeyToggleApplication;
                GuiHotkeyEnable.BackColor = Color.DarkSalmon;
            }
            else
            {
                GuiHotkeyEnable.BackColor = SystemColors.Window;
            }

            // "Search"
            if (hotkeySearch == "Unsupported" || hotkeySearch == "Hotkey conflicts")
            {
                hasError = true;
                GuiHotkeySearch.Text = hotkeySearch;
                GuiHotkeySearch.BackColor = Color.DarkSalmon;
            }
            else
            {
                GuiHotkeySearch.BackColor = SystemColors.Window;
            }

            // "Get older entry"
            if (hotkeyGetOlderEntry == "Unsupported" || hotkeyGetOlderEntry == "Hotkey conflicts")
            {
                hasError = true;
                GuiHotkeyOlder.Text = hotkeyGetOlderEntry;
                GuiHotkeyOlder.BackColor = Color.DarkSalmon;
            }
            else
            {
                GuiHotkeyOlder.BackColor = SystemColors.Window;
            }

            // "Get newer entry"
            if (hotkeyGetNewerEntry == "Unsupported" || hotkeyGetNewerEntry == "Hotkey conflicts")
            {
                hasError = true;
                GuiHotkeyNewer.Text = hotkeyGetNewerEntry;
                GuiHotkeyNewer.BackColor = Color.DarkSalmon;
            }
            else
            {
                GuiHotkeyNewer.BackColor = SystemColors.Window;
            }

            // "Paste only on hotkey"
            if (hotkeyPasteOnHotkey == "Unsupported" || hotkeyPasteOnHotkey == "Hotkey conflicts")
            {
                hasError = true;
                GuiHotkeyPaste.Text = hotkeyPasteOnHotkey;
                GuiHotkeyPaste.BackColor = Color.DarkSalmon;
            }
            else
            {
                GuiHotkeyPaste.BackColor = SystemColors.Window;
            }

            // "Toggle favorite entry"
            if (hotkeyToggleFavorite == "Unsupported" || hotkeyToggleFavorite == "Hotkey conflicts")
            {
                hasError = true;
                GuiHotkeyToggleFavorite.Text = hotkeyToggleFavorite;
                GuiHotkeyToggleFavorite.BackColor = Color.DarkSalmon;
            }
            else
            {
                GuiHotkeyToggleFavorite.BackColor = SystemColors.Window;
            }

            // "Toggle list view"
            if (hotkeyToggleView == "Unsupported" || hotkeyToggleView == "Hotkey conflicts")
            {
                hasError = true;
                GuiHotkeyToggleView.Text = hotkeyToggleView;
                GuiHotkeyToggleView.BackColor = Color.DarkSalmon;
            }
            else
            {
                GuiHotkeyToggleView.BackColor = SystemColors.Window;
            }

            // Accept the changes and disable the two buttons again
            if (!hasError)
            {
                GuiApplyHotkey.Enabled = false;
                GuiCancelHotkey.Enabled = false;
            }
        }


        // ###########################################################################################
        // Cancel the hotkeys and restore original content
        // ###########################################################################################

        private void CancelHotkey_Click(object sender, EventArgs e)
        {
            string hotkeyToggleApplication = GetRegistryKey(registryPath, "HotkeyToggleApplication");
            string hotkeySearch = GetRegistryKey(registryPath, "HotkeySearch");
            string hotkeyGetOlderEntry = GetRegistryKey(registryPath, "HotkeyGetOlderEntry");
            string hotkeyGetNewerEntry = GetRegistryKey(registryPath, "HotkeyGetNewerEntry");
            string hotkeyPasteOnHotkey = GetRegistryKey(registryPath, "HotkeyPasteOnHotkey");
            string hotkeyToggleFavorite = GetRegistryKey(registryPath, "HotkeyToggleFavorite");
            string hotkeyToggleView = GetRegistryKey(registryPath, "HotkeyToggleView");
            GuiHotkeyEnable.Text = hotkeyToggleApplication;
            GuiHotkeySearch.Text = hotkeySearch;
            GuiHotkeyOlder.Text = hotkeyGetOlderEntry;
            GuiHotkeyNewer.Text = hotkeyGetNewerEntry;
            GuiHotkeyPaste.Text = hotkeyPasteOnHotkey;
            GuiHotkeyToggleFavorite.Text = hotkeyToggleFavorite;
            GuiHotkeyToggleView.Text = hotkeyToggleView;
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
                    "HovText continues running in the background to perform its duties. You can see the icon in the tray area.",
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

        private void PictureBox1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.paypal.com/donate?hosted_button_id=U23UUA8YWABGU");
            Logging.Log("Clicked the \"Donate\" picture in \"About\"");
        }


        // ###########################################################################################
        // Get customer color inputs
        // ###########################################################################################

        private void GuiCustomHeader_Enter(object sender, EventArgs e)
        {
            colorDialogHeader.Color = GuiCustomHeader.BackColor;
            colorDialogHeader.FullOpen = true;
            if (colorDialogHeader.ShowDialog() == DialogResult.OK)
            {
                string color = String.Format("#{0:X2}{1:X2}{2:X2}", colorDialogHeader.Color.R, colorDialogHeader.Color.G, colorDialogHeader.Color.B);
                SetRegistryKey(registryPath, "HistoryColorCustomHeader", color);
                historyColorsHeader["Custom"] = color;
                GuiCustomHeader.BackColor = colorDialogHeader.Color;
                SetHistoryColors();
            }
            ActiveControl = null; // do not focus this form - so we can reclick the textbox again right after
        }

        private void GuiCustomHeaderText_Enter(object sender, EventArgs e)
        {
            colorDialogHeaderText.Color = GuiCustomHeaderText.BackColor;
            colorDialogHeaderText.FullOpen = true;
            if (colorDialogHeaderText.ShowDialog() == DialogResult.OK)
            {
                string color = String.Format("#{0:X2}{1:X2}{2:X2}", colorDialogHeaderText.Color.R, colorDialogHeaderText.Color.G, colorDialogHeaderText.Color.B);
                SetRegistryKey(registryPath, "HistoryColorCustomHeaderText", color);
                historyColorsHeaderText["Custom"] = color;
                GuiCustomHeaderText.BackColor = colorDialogHeaderText.Color;
                SetHistoryColors();
            }
            ActiveControl = null; // do not focus this form - so we can reclick the textbox again right after
        }

        private void GuiCustomSearch_Enter(object sender, EventArgs e)
        {
            colorDialogSearch.Color = GuiCustomSearch.BackColor;
            colorDialogSearch.FullOpen = true;
            if (colorDialogSearch.ShowDialog() == DialogResult.OK)
            {
                string color = String.Format("#{0:X2}{1:X2}{2:X2}", colorDialogSearch.Color.R, colorDialogSearch.Color.G, colorDialogSearch.Color.B);
                SetRegistryKey(registryPath, "HistoryColorCustomSearch", color);
                historyColorsSearch["Custom"] = color;
                GuiCustomSearch.BackColor = colorDialogSearch.Color;
                SetHistoryColors();
            }
            ActiveControl = null; // do not focus this form - so we can reclick the textbox again right after
        }

        private void GuiCustomSearchText_Enter(object sender, EventArgs e)
        {
            colorDialogSearchText.Color = GuiCustomSearchText.BackColor;
            colorDialogSearchText.FullOpen = true;
            if (colorDialogSearchText.ShowDialog() == DialogResult.OK)
            {
                string color = String.Format("#{0:X2}{1:X2}{2:X2}", colorDialogSearchText.Color.R, colorDialogSearchText.Color.G, colorDialogSearchText.Color.B);
                SetRegistryKey(registryPath, "HistoryColorCustomSearchText", color);
                historyColorsSearchText["Custom"] = color;
                GuiCustomSearchText.BackColor = colorDialogSearchText.Color;
                SetHistoryColors();
            }
            ActiveControl = null; // do not focus this form - so we can reclick the textbox again right after
        }

        private void GuiCustomActive_Enter(object sender, EventArgs e)
        {
            colorDialogActive.Color = GuiShowFontActive.BackColor;
            colorDialogActive.FullOpen = true;
            if (colorDialogActive.ShowDialog() == DialogResult.OK)
            {
                string color = String.Format("#{0:X2}{1:X2}{2:X2}", colorDialogActive.Color.R, colorDialogActive.Color.G, colorDialogActive.Color.B);
                SetRegistryKey(registryPath, "HistoryColorCustomActive", color);
                historyColorsActive["Custom"] = color;
                GuiCustomActive.BackColor = colorDialogActive.Color;
                SetHistoryColors();
            }
            ActiveControl = null; // do not focus this form - so we can reclick the textbox again right after
        }

        private void GuiCustomActiveText_Enter(object sender, EventArgs e)
        {
            colorDialogActiveText.Color = GuiShowFontActive.BackColor;
            colorDialogActiveText.FullOpen = true;
            if (colorDialogActiveText.ShowDialog() == DialogResult.OK)
            {
                string color = String.Format("#{0:X2}{1:X2}{2:X2}", colorDialogActiveText.Color.R, colorDialogActiveText.Color.G, colorDialogActiveText.Color.B);
                SetRegistryKey(registryPath, "HistoryColorCustomActiveText", color);
                historyColorsActiveText["Custom"] = color;
                GuiCustomActiveText.BackColor = colorDialogActiveText.Color;
                SetHistoryColors();
            }
            ActiveControl = null; // do not focus this form - so we can reclick the textbox again right after
        }

        private void GuiCustomEntry_Enter(object sender, EventArgs e)
        {
            colorDialogEntry.Color = GuiShowFontEntry.BackColor;
            colorDialogEntry.FullOpen = true;
            if (colorDialogEntry.ShowDialog() == DialogResult.OK)
            {
                string color = String.Format("#{0:X2}{1:X2}{2:X2}", colorDialogEntry.Color.R, colorDialogEntry.Color.G, colorDialogEntry.Color.B);
                SetRegistryKey(registryPath, "HistoryColorCustomEntry", color);
                historyColorsEntry["Custom"] = color;
                GuiCustomEntry.BackColor = colorDialogEntry.Color;
                SetHistoryColors();
            }
            ActiveControl = null; // do not focus this form - so we can reclick the textbox again right after
        }

        private void GuiCustomEntryText_Enter(object sender, EventArgs e)
        {
            colorDialogEntryText.Color = GuiShowFontEntry.BackColor;
            colorDialogEntryText.FullOpen = true;
            if (colorDialogEntryText.ShowDialog() == DialogResult.OK)
            {
                string color = String.Format("#{0:X2}{1:X2}{2:X2}", colorDialogEntryText.Color.R, colorDialogEntryText.Color.G, colorDialogEntryText.Color.B);
                SetRegistryKey(registryPath, "HistoryColorCustomEntryText", color);
                historyColorsEntryText["Custom"] = color;
                GuiCustomEntryText.BackColor = colorDialogEntryText.Color;
                SetHistoryColors();
            }
            ActiveControl = null; // do not focus this form - so we can reclick the textbox again right after
        }

        private void GuiCustomBorder_Enter(object sender, EventArgs e)
        {
            colorDialogBorder.Color = GuiCustomBorder.BackColor;
            colorDialogBorder.FullOpen = true;
            if (colorDialogBorder.ShowDialog() == DialogResult.OK)
            {
                string color = String.Format("#{0:X2}{1:X2}{2:X2}", colorDialogBorder.Color.R, colorDialogBorder.Color.G, colorDialogBorder.Color.B);
                SetRegistryKey(registryPath, "HistoryColorCustomBorder", color);
                historyColorsBorder["Custom"] = color;
                GuiCustomBorder.BackColor = colorDialogBorder.Color;
                GuiShowFontActive.Refresh(); // update/redraw the border
                GuiShowFont.Refresh(); // update/redraw the border
            }
            ActiveControl = null; // do not focus this form - so we can reclick the textbox again right after
        }


        // ###########################################################################################
        // Take over redraw for a specific UI element, "history border"
        // ###########################################################################################

        private void GuiShowFontBottom_Paint(object sender, PaintEventArgs e)
        {
            if (historyBorderThickness > 0)
            {
                // Set padding
                if (historyBorderThickness >= 2)
                {
                    GuiShowFont.Padding = new Padding(historyBorderThickness - 2);
                    GuiShowFontActive.Padding = new Padding(historyBorderThickness - 2);
                    GuiShowFontEntry.Padding = new Padding(historyBorderThickness - 2);
                }
                else
                {
                    GuiShowFont.Padding = new Padding(historyBorderThickness - 1);
                    GuiShowFontActive.Padding = new Padding(historyBorderThickness - 1);
                    GuiShowFontEntry.Padding = new Padding(historyBorderThickness - 1);
                }

                // Redraw border with a solid color
                ControlPaint.DrawBorder(e.Graphics, e.ClipRectangle,
                                ColorTranslator.FromHtml(Settings.historyColorsBorder[historyColorTheme]), historyBorderThickness, ButtonBorderStyle.Solid,
                                ColorTranslator.FromHtml(Settings.historyColorsBorder[historyColorTheme]), historyBorderThickness, ButtonBorderStyle.Solid,
                                ColorTranslator.FromHtml(Settings.historyColorsBorder[historyColorTheme]), historyBorderThickness, ButtonBorderStyle.Solid,
                                ColorTranslator.FromHtml(Settings.historyColorsBorder[historyColorTheme]), historyBorderThickness, ButtonBorderStyle.Solid);
            }
            else
            {
                ControlPaint.DrawBorder(e.Graphics, e.ClipRectangle, this.BackColor, ButtonBorderStyle.None);
            }
        }


        // ###########################################################################################
        // Troubleshooting, enable or disable
        // ###########################################################################################

        private void GuiTroubleshootEnabled_CheckedChanged(object sender, EventArgs e)
        {

            if (GuiTroubleshootEnabled.Checked)
            {
                // Get new status now, if logging is enabled or disabled
                isTroubleshootEnabled = GuiTroubleshootEnabled.Checked;

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

                // Wait with new status, so we can do the end-logginf first
                isTroubleshootEnabled = GuiTroubleshootEnabled.Checked;
            }

            // Save it to the registry
            string status = GuiTroubleshootEnabled.Checked ? "1" : "0";
            SetRegistryKey(registryPath, "TroubleshootEnable", status);

            // If there is a logfile present then enable the UI fields for it
            if (File.Exists(pathAndLog))
            {
                GuiTroubleshootDeleteFile.Enabled = true;
            }
        }


        // ###########################################################################################
        // Troubleshooting, open explorer location for the executable and highlight the file
        // ###########################################################################################

        private void GuiExecuteableOpenLocation_Click(object sender, EventArgs e)
        {
            string appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            OpenExecuteableLocation(appPath);
            Logging.Log("Clicked the \"Open executeable location\"");
        }


        // ###########################################################################################
        // Troubleshooting, delete the logfile
        // ###########################################################################################

        private void GuiTroubleshootDeleteFile_Click(object sender, EventArgs e)
        {
            if (File.Exists(pathAndLog))
            {
                File.Delete(@pathAndLog);
                GuiTroubleshootDeleteFile.Enabled = false;
            }
            else
            {
                MessageBox.Show(pathAndLog + " does not exists!",
                    "WARNING",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                GuiTroubleshootDeleteFile.Enabled = false;
            }
        }


        // ###########################################################################################
        // Troubleshooting, refresh the buttons for the logfile, if viewing the "Advanced" tab
        // ###########################################################################################

        private async void TabControl_Selected(object sender, TabControlEventArgs e)
        {
            // "Advanced" tab
            if (TabControl.SelectedTab.AccessibilityObject.Name == "Advanced")
            {
                if (File.Exists(pathAndLog))
                {
                    GuiTroubleshootDeleteFile.Enabled = true;
                }
                else
                {
                    GuiTroubleshootDeleteFile.Enabled = false;
                }
            }

            // "Feedback" tab
            if (TabControl.SelectedTab.AccessibilityObject.Name == "Feedback")
            {
                if (File.Exists(pathAndLog))
                {
                    FileInfo fileInfo = new FileInfo(pathAndLog);
                    if (fileInfo.Length > 0)
                    {
                        GuiAttachFile.Enabled = true;
                        long fileSizeInBytes = fileInfo.Length;
                        double fileSizeInKilobytes = fileSizeInBytes / 1024.0;
                        double fileSizeInMegabytes = fileSizeInBytes / (1024.0 * 1024.0);
                        double fileSize;
                        fileSize = fileSizeInBytes > 1000 ? fileSizeInKilobytes : fileSizeInBytes;
                        fileSize = fileSizeInKilobytes > 1000 ? fileSizeInMegabytes : fileSize;
                        fileSize = fileSize > 10 ? Math.Round(fileSize, 0) : fileSize;
                        string fileDescription = fileSizeInBytes > 1000 ? "KBytes" : "bytes";
                        fileDescription = fileSizeInKilobytes > 1000 ? "MBytes" : fileDescription;
                        bool hasDecimalPart = fileSize != Math.Truncate(fileSize);
                        fileSize = hasDecimalPart ? Math.Round(fileSize, 1) : fileSize;
                        if (fileSizeInBytes > 5 * 1_024_000)
                        {
                            GuiAttachFile.Text = "Attach troubleshooting logfile (" + fileSize.ToString() + " " + fileDescription + " - must not exceed 5MB)";
                            GuiAttachFile.Enabled = false;
                            GuiAttachFile.Checked = false;
                        }
                        else
                        {
                            GuiAttachFile.Text = "Attach troubleshooting logfile (" + fileSize.ToString() + " " + fileDescription + ")";
                            GuiAttachFile.Enabled = true;
                        }
                    }
                    else
                    {
                        GuiAttachFile.Enabled = false;
                        GuiAttachFile.Text = "Attach troubleshooting logfile (file is empty)";
                    }
                }
                else
                {
                    GuiAttachFile.Enabled = false;
                    GuiAttachFile.Text = "Attach troubleshooting logfile (file does not exists)";
                }
            }
        }


        // ###########################################################################################
        // Troubleshooting, remove all registry keys and delete the logfile - this is resetting HovText to "factory default"
        // ###########################################################################################

        private void GuiCleanUpExit_Click(object sender, EventArgs e)
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

        private void GuiSendFeedback_Click(object sender, EventArgs e)
        {
            string email = GuiEmailAddr.Text;
            string feedback = GuiFeedbackText.Text;

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
                    webClient.Headers.Add("user-agent", ("HovText " + AboutLabelVersion.Text).Trim());

                    // Build the data to send
                    string textFilePath = pathAndLog;
                    string textFileContent = File.ReadAllText(textFilePath);
                    var data = new NameValueCollection
                        {
                            { "version", AboutLabelVersion.Text },
                            { "email", email }
                        };
                    if (GuiAttachFile.Checked)
                    {
                        data["attachment"] = textFileContent;
                    }
                    data["feedback"] = feedback;

                    // Send it to the server
                    var response = webClient.UploadValues(hovtextPage + "/contact/sendmail/", "POST", data);
                    string resultFromServer = Encoding.UTF8.GetString(response);
                    if (resultFromServer == "Success")
                    {
                        GuiEmailAddr.Text = "";
                        GuiFeedbackText.Text = "";
                        GuiAttachFile.Checked = false;
                        GuiSendFeedback.Enabled = false;
                        if (email.Length > 0)
                        {
                            string txt = "Feedback sent - please allow for some time, if any response is required";
                            Logging.Log(txt);
                            Logging.Log("  Email used = [" + email + "]");
                            MessageBox.Show(txt,
                                "OK",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                        }
                        else
                        {
                            string txt = "Feedback sent - no response will be given as you did not specify an email address";
                            Logging.Log(txt);
                            MessageBox.Show(txt,
                                "OK",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }
                    }
                }
                catch (WebException ex)
                {
                    // Catch the exception though this is not so critical that we need to disturb the developer
                    Logging.Log("Exception raised (Settings):");
                    Logging.Log("  Cannot connect with server to submit feedback:");
                    Logging.Log("  " + ex.Message);
                    MessageBox.Show("HovText cannot connect to the server, where it should submit the feedback. Please connect directly with the developer at \"dennis@hovtext.com\" and state this is a problem, thanks.\r\n\r\nThe exact error is:\r\n\r\n" + ex.Message,
                        "ERROR",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }

            }
            else
            {
                string txt = "Invalid email address [" + email + "]";
                Logging.Log("EXCEPTION #12 raised:");
                Logging.Log("  " + txt);
                MessageBox.Show(txt,
                    "ERROR",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }


        // ###########################################################################################
        // Check if the "Send feedback" button should be enabled or disabled
        // ###########################################################################################

        private void GuiFeedbackText_TextChanged(object sender, EventArgs e)
        {
            if (GuiAttachFile.Checked || GuiFeedbackText.Text.Length > 0)
            {
                GuiSendFeedback.Enabled = true;
            }
            else
            {
                GuiSendFeedback.Enabled = false;
            }
        }

        private void GuiAttachFile_CheckedChanged(object sender, EventArgs e)
        {
            if (GuiAttachFile.Checked || GuiFeedbackText.Text.Length > 0)
            {
                GuiSendFeedback.Enabled = true;
            }
            else
            {
                GuiSendFeedback.Enabled = false;
            }
        }


        // ###########################################################################################
        // Check if the email address typed in feedback is valid - not a very good check though!
        // ###########################################################################################

        private static bool IsValidEmail(string email)
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


        // ###########################################################################################
        // Download or install the development version
        // ###########################################################################################

        private void GuiDevelopmentDownload_Click(object sender, EventArgs e)
        {
            // Open file location for the executeable
            string appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            OpenExecuteableLocation(appPath);

            // Download executeable
            Process.Start(hovtextPage + "/autoupdate/development/HovText.exe");

            Logging.Log("Clicked the \"Download\" development version");
        }

        private void Button1_Click_1(object sender, EventArgs e)
        {
            GuiDevelopmentAutoInstall.Enabled = false;
            Logging.Log("Auto-install new [DEVELOPMENT] version");
            DownloadInstall("Development");
        }


        // ###########################################################################################
        // Detect the main/primary display.
        // Return will be a 0-indexed screen/display
        // ###########################################################################################

        private static int GetPrimaryDisplay()
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

        private static string GetUniqueDisplayLayout()
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

        private static bool IsDisplayValid(int display)
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

        private void PopulateDisplaySetup()
        {
            // Remove all elements
            GuiLayoutGroup3.Controls.Clear();

            // Build new radio buttons
            int numDisplays = Screen.AllScreens.Length; // total number of displays
            if (numDisplays > 1)
            {
                Logging.Log("Detected [" + numDisplays + "] displays");
            }
            else
            {
                Logging.Log("Detected [" + numDisplays + "] display");
            }

            // Walk through all displays
            int dividerSpace = 32;
            for (int i = 0; i < numDisplays; i++)
            {
                // Check if this display number is the main/primary one
                bool isDisplayMain = Screen.AllScreens[i].Primary;

                // Build a new radio UI element
                RadioButton display = new RadioButton
                {
                    Name = "uiScreen" + i,
                    Tag = i,
                    Text = isDisplayMain ? "Display " + (i + 1) + " (Main)" : "Display " + (i + 1),
                    Location = new Point(29, dividerSpace),
                    AutoSize = true,
                    Enabled = numDisplays > 1
                };
                display.CheckedChanged += new EventHandler(GuiDisplayGroup_Changed);
                GuiLayoutGroup3.Controls.Add(display);
                dividerSpace += 31;
            }
        }


        // ###########################################################################################
        // Catch event when changing the display
        // Main contributor: FNI
        // ###########################################################################################

        private void GuiDisplayGroup_Changed(object sender, EventArgs e)
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
            GuiLayoutGroup3.Controls["uiScreen" + activeDisplay].Select();

            // Update the registry, as we do not know if it has been changed or not (too lazy to make variable for it)
            if (regVal == null)
            {
                SetRegistryKey(registryPathDisplays, displaysId, activeDisplay.ToString());
            }
        }


        // ###########################################################################################
        // Detect events for changing border thickness
        // ###########################################################################################

        private void GuiBorderThickness_Scroll(object sender, EventArgs e)
        {
            LabelBorderThickness.Text = GuiBorderThickness.Value.ToString();
            historyBorderThickness = GuiBorderThickness.Value;
            SetRegistryKey(registryPath, "HistoryBorderThickness", historyBorderThickness.ToString());
            GuiShowFontActive.Refresh(); // update/redraw the border
            GuiShowFont.Refresh(); // update/redraw the border
        }


        // ###########################################################################################
        // Detect events for changing icon set
        // ###########################################################################################

        private void GuiIconsRound_CheckedChanged(object sender, EventArgs e)
        {
            iconSet = "Round";
            SetRegistryKey(registryPath, "IconSet", iconSet);
            SetNotifyIcon();
        }

        private void GuiIconsSquare_CheckedChanged(object sender, EventArgs e)
        {
            iconSet = "SquareOld";
            SetRegistryKey(registryPath, "IconSet", iconSet);
            SetNotifyIcon();
        }

        private void GuiIconsSquareNew_CheckedChanged(object sender, EventArgs e)
        {
            iconSet = "SquareNew";
            SetRegistryKey(registryPath, "IconSet", iconSet);
            SetNotifyIcon();
        }


        // ###########################################################################################
        // Download and install either the STABLE or the DEVELOPMENT version
        // ###########################################################################################

        public static void DownloadInstall(string versionType)
        {
            /*
            // --------------------------------
            // EXPERIMENTAL AUTO-UPDATE/INSTALL
            // --------------------------------
            // This approach may no work perfectly on all systems, as it does require Powershell +
            // the commands in the batch file. I am fairly confident this "should" work on
            // standard Windows 7 and newer, but will not expect it to work on Windows XP - but 
            // not sure how many of these are still running out there?
            //
            // Also the approach is dependent on system settings - e.g. some IT departments
            // could have blocked executing batch files or alike, so there are all kind of
            // potential issues with this - so better still keep the old method open ;-)
            //
            // The solution here "could" also be considered dangerous by some malware/antivirus 
            // scanners, as I am dynamically generating a batch file, unblocking it so Windows Defender 
            // will not distrub and then I will execute the updated executeable file from the 
            // internet - so some red flags may be triggered here?
            //            
            */

            string urlToExe = versionType == "Development" ? "/autoupdate/development/HovText.exe" : "/download/" + versionType + "/HovText.exe";
            string appUrl = Settings.hovtextPage + urlToExe;

            pathAndTempExe = Path.Combine(Path.GetTempPath(), tempExe);
            pathAndTempCmd = Path.Combine(Path.GetTempPath(), tempCmd);
            pathAndTempLog = Path.Combine(Path.GetTempPath(), tempLog);

            try
            {
                // Download the updated file
                Logging.Log("Downloading new executable file [" + pathAndTempExe + "]");
                WebClient webClient = new WebClient();
                webClient.DownloadFile(appUrl, pathAndTempExe);

                string stepsForStable =
    @"rem --   * Go to " + hovtextPage + @"/download and download newest HovText       --
rem --   * In the ""Advanced"" tab click the ""Open executeable location""          --
rem --   * Close / exit the running HovText application                         --
rem --   * Replace the executeable file                                         --
rem --   * Run the new file - check the ""About"" tab, that you are using the     --
rem --     correct new version                                                  --";

                string stepsForDevelopment =
    @"rem --   * In the ""Advanced"" tab then click the ""Download"".                     --
rem --   * Then click the ""Open executeable location""                           --
rem --   * Close / exit the running HovText application                         --
rem --   * Replace the executeable file                                         --
rem --   * Run the new file - check the ""About"" tab, that you are using the     --
rem --     correct new version                                                  --";

                // Create batchfile content
                string stableOrDevelopment = versionType == "Development" ? stepsForDevelopment : stepsForStable;
                string batchContents =
@"@echo off

rem ------------------------------------------------------------------------------
rem -- If you see this, then probably there is a problem with the auto-install. --
rem -- The problem could be related to local settings, policies or alike, but   --
rem -- it this means you must manually do the update, sorry :-)                 --
rem --                                                                          --
rem -- Follow these steps:                                                      --
" + stableOrDevelopment + @"
rem ------------------------------------------------------------------------------

echo > """ + pathAndTempLog + @"""

rem Wait until HovText process finishes
:wait
  timeout /T 1 >> """ + pathAndTempLog + @"""
  tasklist /NH /fi ""IMAGENAME eq " + exeOnly + @""" | find /I """ + exeOnly + @""" >> """ + pathAndTempLog + @""" && goto :wait

rem Move temporary (new) file to same location as existing
timeout /T 2 >> """ + pathAndTempLog + @"""
echo Moving: """ + pathAndTempExe + @""" to """ + pathAndExe + @""" >> """ + pathAndTempLog + @"""
move /y """ + pathAndTempExe + @""" """ + pathAndExe + @""" >> """ + pathAndTempLog + @"""
IF %ERRORLEVEL% NEQ 0 (
  echo ""Move failed! You need to do the update manually, sorry :-/"" >> """ + pathAndTempLog + @"""
  pause 
  exit /b 1
)
echo ""Move successful"" >> """ + pathAndTempLog + @"""

rem Run the new file
start """" """ + pathAndExe + @"""

rem Delete this batch file
del ""%~f0"" >> """ + pathAndTempLog + @"""
";

                // Create the batchfile
                Logging.Log("Creating batch file [" + pathAndTempCmd + "]");
                File.WriteAllText(pathAndTempCmd, batchContents);

                // Unblock batch file and newly downloaded executable
                Logging.Log("Unblocking batch file [" + pathAndTempCmd + "]");
                UnblockFile(pathAndTempCmd);
                Logging.Log("Unblocking new executable [" + pathAndTempExe + "]");
                UnblockFile(pathAndTempExe);

                // Run/execute the batch file, which will copy the new version and launch the new version.
                // Will not run until this HovText instance has been shutdown.
                ProcessStartInfo psi = new ProcessStartInfo(pathAndTempCmd)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                Logging.Log("Launching batch file [" + pathAndTempCmd + "]");
                Logging.Log("Creating batch logfile [" + pathAndTempLog + "]");
                Process.Start(psi);

                // IF the below exit does not work, then enforce an exit after 5 seconds
                settings.terminateTimer.Enabled = true;

                // Exit HovText so batch file can process
                Logging.Log("Exiting application gracefully");
                Logging.Log("HovText will be relaunched with new version after exit");
                Application.Exit();
            }
            catch (WebException ex)
            {
                // Catch the exception though this is not so critical that we need to disturb the developer
                Logging.Log("Exception raised (Settings):");
                Logging.Log("  Cannot connect with server to auto-install new version");
                Logging.Log("  " + ex.Message);

                MessageBox.Show("HovText cannot connect to the server, where it should get the new version. Please retry later ...\r\n\r\nThe exact error is:\r\n\r\n" + ex.Message,
                        "ERROR",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
            }
        }


        // ###########################################################################################
        // Unblock a file so it can execute without Microsoft Defender complains
        // ###########################################################################################

        private static void UnblockFile(string fileAndPath)
        {
            // Prepare for unblocking the file via Powershell
            string command = $"get-childitem {fileAndPath} | Unblock-File -Verbose";
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = "powershell.exe",
                Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{command}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            process.StartInfo = startInfo;

            // Get information from output (verbose should output something) or error
            StringBuilder errorData = new StringBuilder();
            process.ErrorDataReceived += (errorSender, errorEventArgs) =>
            {
                if (!string.IsNullOrEmpty(errorEventArgs.Data))
                {
                    errorData.AppendLine(errorEventArgs.Data);
                }
            };

            // Do the unblocking
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            process.Dispose();

            // Log stuff
            string errorResult = errorData.ToString();
            if (errorResult.Length > 0)
            {
                Logging.Log("  Error: [" + errorResult + "]");
            }
        }


        // ###########################################################################################
        // Open the file explorer location for the executeable
        // ###########################################################################################

        public static void OpenExecuteableLocation(string fileAndPath)
        {
            // https://stackoverflow.com/a/696144/2028935
            string argument = "/select, \"" + fileAndPath + "\"";
            Process.Start("explorer.exe", argument);
        }


        // ###########################################################################################
        // Set the notify icon, which is dependent on hotkey behaviour and icon style
        // ###########################################################################################

        private void SetNotifyIcon()
        {
            string iconSet = GetRegistryKey(registryPath, "IconSet"); // Round, SquareNew, SquareOld
            string hotkeyBehaviour = GetRegistryKey(registryPath, "HotkeyBehaviour"); // System, Paste

            if (buildType == "Debug")
            {
                this.Text = "HovText DEVELOPMENT version";
            }

            switch (hotkeyBehaviour)
            {
                case "Paste":
                    if (iconSet == "SquareOld")
                    {
                        if (isApplicationEnabled)
                        {
                            notifyIcon.Icon = Resources.Square_Old_Hotkey_16x16;
                            Icon = Resources.Square_Old_Hotkey_16x16;
                        }
                        else
                        {
                            notifyIcon.Icon = Resources.Square_Old_Inactive_16x16;
                            Icon = Resources.Square_Old_Inactive_16x16;
                        }
                    }
                    else if (iconSet == "SquareNew")
                    {
                        if (isApplicationEnabled)
                        {
                            notifyIcon.Icon = Resources.Square_New_Hotkey_Edited_48x48;
                            Icon = Resources.Square_New_Hotkey_Edited_48x48;
                        }
                        else
                        {
                            notifyIcon.Icon = Resources.Square_New_Inactive_Edited_48x48;
                            Icon = Resources.Square_New_Inactive_Edited_48x48;
                        }
                    }
                    else
                    {
                        if (isApplicationEnabled)
                        {
                            notifyIcon.Icon = Resources.Round_Hotkey_48x48;
                            Icon = Resources.Round_Hotkey_48x48;
                        }
                        else
                        {
                            notifyIcon.Icon = Resources.Round_Inactive_48x48;
                            Icon = Resources.Round_Inactive_48x48;
                        }
                    }
                    break;

                default:
                    if (iconSet == "SquareOld")
                    {
                        if (isApplicationEnabled)
                        {
                            notifyIcon.Icon = Resources.Square_Old_Active_16x16;
                            Icon = Resources.Square_Old_Active_16x16;
                        }
                        else
                        {
                            notifyIcon.Icon = Resources.Square_Old_Inactive_16x16;
                            Icon = Resources.Square_Old_Inactive_16x16;
                        }
                    }
                    else if (iconSet == "SquareNew")
                    {
                        if (isApplicationEnabled)
                        {
                            notifyIcon.Icon = Resources.Square_New_Active_Edited_48x48;
                            Icon = Resources.Square_New_Active_Edited_48x48;
                        }
                        else
                        {
                            notifyIcon.Icon = Resources.Square_New_Inactive_Edited_48x48;
                            Icon = Resources.Square_New_Inactive_Edited_48x48;
                        }
                    }
                    else
                    {
                        if (isApplicationEnabled)
                        {
                            notifyIcon.Icon = Resources.Round_Active_48x48;
                            Icon = Resources.Round_Active_48x48;
                        }
                        else
                        {
                            notifyIcon.Icon = Resources.Round_Inactive_48x48;
                            Icon = Resources.Round_Inactive_48x48;
                        }
                    }
                    break;
            }
        }


        // ###########################################################################################
        // Refresh and get the development version information
        // ###########################################################################################

        private void GuiDevelopmentRefresh_Click(object sender, EventArgs e)
        {
            Logging.Log("Clicked \"Refresh development version\" in \"Advanced\"");
            FetchInfoForDevelopment();
        }

        private void FetchInfoForDevelopment()
        {
            AdvancedLabelDevelopmentVersion.Text = "Please wait ...";

            // Check for a new development version
            try
            {
                WebClient webClient = new WebClient();
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
                webClient.Headers.Add("user-agent", ("HovText " + AboutLabelVersion.Text).Trim());
                string checkedVersion = webClient.DownloadString(hovtextPage + "/autoupdate/development/");
                if (checkedVersion.Substring(0, 7) == "Version")
                {
                    if (checkedVersion != "Version: No development version available")
                    {
                        checkedVersion = checkedVersion.Substring(9);
                        AdvancedLabelDevelopmentVersion.Text = checkedVersion;
                        GuiDevelopmentDownload.Enabled = true;
                        GuiDevelopmentAutoInstall.Enabled = true;
                        Logging.Log("  Development version available = [" + checkedVersion + "]");
                    }
                    else
                    {
                        GuiDevelopmentDownload.Enabled = false;
                        GuiDevelopmentAutoInstall.Enabled = false;
                        AdvancedLabelInfoDevelopment.Enabled = false;
                        AdvancedLabelDevelopmentVersion.Text = "No development version available";
                        Logging.Log("  Development version available = [No development version available]");
                    }
                }
                else
                {

                    GuiDevelopmentDownload.Enabled = false;
                    GuiDevelopmentAutoInstall.Enabled = false;
                    AdvancedLabelInfoDevelopment.Enabled = false;
                    AdvancedLabelDevelopmentVersion.Text = "ERROR";
                    Logging.Log("  Development version available = [ERROR]");
                }
            }
            catch (WebException ex)
            {
                // Catch the exception though this is not so critical that we need to disturb the developer
                Logging.Log("Exception raised (Settings):");
                Logging.Log("  Cannot connect with server to get information about newest available [DEVELOPMENT] version:");
                Logging.Log("  " + ex.Message);
                AdvancedLabelDevelopmentVersion.Text = "Cannot connect with server - retry later";
            }
        }


        // ###########################################################################################
        // Check if the version has been updated from a DEVELOPMENT version to a STABLE version
        // ###########################################################################################

        private void CheckIfUpdatedFromDevelopmentToStable()
        {
            // Get the filesize og the logfile
            if (File.Exists(pathAndLog))
            {
                FileInfo fileInfo = new FileInfo(pathAndLog);
                long fileSize = fileInfo.Length;

                // React if the file is larger than 10MB
                if (fileSize > (10 * 1_024_000))
                {
                    Logging.Log("Shown popup that the troubleshooting logfile is bigger than 10MB");
                    tooBigLogfile.Show();
                    tooBigLogfile.Activate();
                }
            }
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            CheckIfUpdatedFromDevelopmentToStable();
        }


        // ###########################################################################################
        // Disable troubleshoot logging
        // ###########################################################################################

        public void UpdateTroubleshootDisabled()
        {
            GuiTroubleshootEnabled.Checked = false;
        }


        // ###########################################################################################
        // Update amount of entries to save to file
        // ###########################################################################################

        private void GuiStorageEntries_ValueChanged(object sender, EventArgs e)
        {
            GuiStorageEntriesText.Text = GuiStorageEntries.Value.ToString();
            SetRegistryKey(registryPath, "StorageSaveEntries", GuiStorageEntriesText.Text);
        }


        // ###########################################################################################
        // Update if we should save the clipboard on exit or not
        // ###########################################################################################

        private void GuiStorageSaveClipboard_CheckedChanged(object sender, EventArgs e)
        {
            string status = GuiStorageSaveClipboard.Checked ? "1" : "0";
            SetRegistryKey(registryPath, "StorageSaveOnExit", status);
            GuiStorageChooseAll_EnableDisableSave();
        }


        // ###########################################################################################
        // Update if we should load the clipboard on application launch or not
        // ###########################################################################################

        private void GuiStorageLoadClipboard_CheckedChanged(object sender, EventArgs e)
        {
            string status = GuiStorageLoadClipboard.Checked ? "1" : "0";
            SetRegistryKey(registryPath, "StorageLoadOnLaunch", status);
        }


        // ###########################################################################################
        // Update which clipboard type we should save
        // ###########################################################################################

        private void GuiStorageChooseFavorites_CheckedChanged(object sender, EventArgs e)
        {
            string status = GuiStorageChooseAll.Checked ? "All" : "Favorites";
            SetRegistryKey(registryPath, "StorageSaveType", status);
        }


        // ###########################################################################################
        // Update which clipboard type we should save
        // ###########################################################################################

        private void GuiStorageChooseAll_EnableDisableSave()
        {
            if (GuiStorageSaveClipboard.Checked)
            {
                GuiStorageChooseAll.Enabled = true;
                GuiStorageEntries.Enabled = true;
                if (GuiStorageSaveClipboard.Checked)
                {
                    if (isEnabledFavorites)
                    {
                        GuiStorageChooseFavorites.Enabled = true;
                    }
                    else
                    {
                        GuiStorageChooseFavorites.Enabled = false;
                    }
                }
            }
            else
            {
                GuiStorageChooseAll.Enabled = false;
                GuiStorageChooseFavorites.Enabled = false;
                GuiStorageEntries.Enabled = false;
            }
        }


        // ###########################################################################################
        // Clear history
        // ###########################################################################################

        private void GuiClearHistory_Click_2(object sender, EventArgs e)
        {
            int count = entriesOriginal.Count;

            Logging.Log($"Cleaning all [" + count + "] history elements");
            ClearHistory();
            UpdateNotifyIconText();

            // Trigger the garbage collector to see the impact immediately
            GC.Collect();
            GC.WaitForPendingFinalizers();

            LogMemoryConsumed();
        }


        // ###########################################################################################
        // Log the amount of memory used
        // -----------------------------
        // Quote ChatGPT:
        // Process.WorkingSet64: Gives a more comprehensive view, including both managed
        // and unmanaged memory, but it represents the memory used by the entire process,
        // which may include overhead from the .NET runtime and other libraries.
        // ###########################################################################################

        private string GetMemoryUsage()
        {
            Process currentProcess = Process.GetCurrentProcess();
            double memoryInMB = currentProcess.WorkingSet64 / 1024d / 1024d;

            // Format the output to one decimal place with a comma as a thousand separator
            string formattedMemoryUsage = memoryInMB.ToString("N1", CultureInfo.InvariantCulture);

            return formattedMemoryUsage;
        }

        private void LogMemoryConsumed()
        {
            string tmp = GetMemoryUsage();

            Logging.Log($"Memory usage: {tmp} MB");
        }

        private void UpdateAdvancedStatus()
        {
            string tmp = GetMemoryUsage();
            GuiMemoryUsed.Text = "Memory usage: " + tmp + " MB";
            GuiHistoryEntriesCount.Text = "Clipboard entries: " + entriesOriginal.Count.ToString();
        }

        private void advancedTimer_Tick(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                UpdateAdvancedStatus();
            }
        }


        // ###########################################################################################
    }


}
