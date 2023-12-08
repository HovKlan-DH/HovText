/*
##################################################################################################
SETTINGS
--------

This is the main form for the HovText application.

##################################################################################################
*/

using Guna.UI2.WinForms;
using HovText.Properties;
using Microsoft.Win32;
using NHotkey.WindowsForms; // https://github.com/thomaslevesque/NHotkey
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.IO.Compression;
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
        private const string registryHotkeySearch = "Alt + S"; // hotkey "Search"
        private const string registryHotkeyPasteOnHotkey = "Alt + O"; // hotkey "Paste on hotkey"
        private const string registryHotkeyToggleFavorite = "Oem5"; // hotkey "Toggle favorite entry"
        private const string registryHotkeyBehaviour = "System"; // use system clipboard
        private const string registryStorageSaveOnExit = "1"; // 0 = do not save clipboards at exit, 1 = save clipboards on exit
        private const string registryStorageLoadOnLaunch = "1"; // 0 = do not load clipboards, 1 = load clipboards on launch
        private const string registryStorageSaveType = "Text"; // "Text", "Favorites" or "All"
        private const string registryStorageSaveEntries = "100"; // number of clipboard entries to save
        private const string registryCloseMinimizes = "1"; // 0 = terminates, 1 = minimize to tray
        private const string registryStartDisabled = "0"; // 0 = start active, 1 = start disabled
        private const string registryRestoreOriginal = "1"; // 1 = restore original
        private const string registryCopyImages = "1"; // 1 = copy images to history
        private const string registryHistorySearch = "1"; // 1 = enable history and Search
        private const string registryHistoryInstantSelect = "0"; // 1 = enable history and Instant-select
        private const string registryEnableFavorites = "1"; // 0 = do not enable favorites
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
        private static SortedDictionary<int, string> entriesApplicationLoad = new SortedDictionary<int, string>();
        public static SortedDictionary<int, Image> entriesApplicationIcon = new SortedDictionary<int, Image>();
        private static SortedDictionary<int, Image> entriesApplicationIconLoad = new SortedDictionary<int, Image>();
        public static SortedDictionary<int, string> entriesText = new SortedDictionary<int, string>();
        public static SortedDictionary<int, bool> entriesShow = new SortedDictionary<int, bool>();
        public static SortedDictionary<int, bool> entriesIsFavorite = new SortedDictionary<int, bool>();
        private static SortedDictionary<int, bool> entriesIsFavoriteLoad = new SortedDictionary<int, bool>();
        public static SortedDictionary<int, bool> entriesIsUrl = new SortedDictionary<int, bool>();
        public static SortedDictionary<int, bool> entriesIsEmail = new SortedDictionary<int, bool>();
        public static SortedDictionary<int, bool> entriesIsImage = new SortedDictionary<int, bool>();
        public static SortedDictionary<int, bool> entriesIsTransparent = new SortedDictionary<int, bool>();
        private static SortedDictionary<int, bool> entriesIsTransparentLoad = new SortedDictionary<int, bool>();
        public static SortedDictionary<int, Image> entriesImage = new SortedDictionary<int, Image>();
        public static SortedDictionary<int, Image> entriesImageTrans = new SortedDictionary<int, Image>();
        public static SortedList<int, Dictionary<string, object>> entriesOriginal = new SortedList<int, Dictionary<string, object>>();
        private static SortedList<int, Dictionary<string, object>> entriesOriginalLoad = new SortedList<int, Dictionary<string, object>>();
        private static Dictionary<string, object> clipboardData;
        private static Image appIcon; // converted from extracted icon
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
        private static string hovTextGithub = "https://github.com/HovKlan-DH/HovText";
        private static string hovTextDonators = "https://hovtext.com/donators";
        internal static Settings settings;
        public static bool isTroubleshootEnabled;
        public static bool hasTroubleshootLogged;
        private static bool cleanupApp = false;
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
        readonly string saveContentFileExist = "HovText-save-content-in-logfile.txt"; // should ONLY be used by Dennis/developer for debugging!!!
        static readonly string tempExe = "HovText-new.exe";
        static readonly string tempCmd = "HovText-batch-update.cmd";
        static readonly string tempLog = "HovText-batch-update-log.txt";
        static string exeFileNameWithPath;
        static string exeFileNameWithoutExtension;
        private bool isApplicationEnabled = true;
        public static byte[] encryptionKey;
        public static byte[] encryptionInitializationVector;
        private int maxClipboardEntriesToSave = 500;
        private bool isClosing = false;
        private bool shownMemoryWarning = false; // "true" if the memory tray notification warning has been shown
        private int memoryInMB = 0;
        private int old_memoryInMB = -1;
        int borderWidth = 1;


        // ###########################################################################################
        // Main - Settings
        // ###########################################################################################

        public Settings()
        {
            // Setup form and all elements
            InitializeComponent();
            Padding = new Padding(borderWidth);

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

            // Get the name for this HovText executable (it may not be "HovText.exe")
            exeFileNameWithPath = Process.GetCurrentProcess().MainModule.FileName;
            exeFileNameWithoutExtension = Path.GetFileNameWithoutExtension(exeFileNameWithPath);

            // Start logging, if relevant
            Logging.StartLogging();
            hasTroubleshootLogged = isTroubleshootEnabled;

            // Instantiate the "TooBigLogfile" form
            this.Load += Settings_Load;
            tooBigLogfile = new TooBigLogfile(this);

            // As the UI elements now have been initialized then we can setup the version
            UiFormLabelApplicationVersion.Text = appVer;

            // Mark very clearly with a color, that this is not a normal version (red equals danger) :-)
            if (buildType == "Debug")
            {
                UiGeneralLabelDisclaimer.Visible = true;
                UiAboutLabelRelease.Text = "Development version (64-bit)";
                BackColor = Color.IndianRed;
                UiFormPanel.BackColor = Color.IndianRed;
                UiFormLabelApplicationName.BackColor = Color.IndianRed;
                UiFormLabelApplicationVersion.BackColor = Color.IndianRed;
                UiFormPictureBoxIcon.BackColor = Color.IndianRed;
                UiFormLabelLoadingPanel.BackColor = Color.IndianRed;
                UiFormLabelLoadingText.ForeColor = Color.Black;
            }
            else
            {
                UiAboutLabelRelease.Text = "Stable release (64-bit)";
                UiFormLabelLoadingText.ForeColor = Color.LightGray;
            }

            // Refering to the current form - used in the history form
            settings = this;

            // Catch repaint event for this specific element (to draw the border)
            UiColorsLabelActive.Paint += new System.Windows.Forms.PaintEventHandler(this.GuiShowFontBottom_Paint);
            UiStyleLabelFont.Paint += new System.Windows.Forms.PaintEventHandler(this.GuiShowFontBottom_Paint);

            // Catch display change events (e.g. add/remove displays or change of main display)
            SystemEvents.DisplaySettingsChanged += new EventHandler(DisplayChangesEvent);

            // Initialize registry and get its values for the various checkboxes
            ConvertLegacyRegistry();
            InitializeRegistry();
            GetStartupSettings();

            // Do not at all show the form, if it should start minimized
            if (Program.StartMinimized)
            {
                this.WindowState = FormWindowState.Minimized;
//                this.ShowInTaskbar = false;
            }

            // Should we start in "disabled" mode?
            if (isStartDisabled)
            {
                ToggleEnabled();
            }

            // Set the notify icon
            SetNotifyIcon();

            NativeMethods.AddClipboardFormatListener(this.Handle);
            Logging.Log("Added HovText to clipboard chain");
            
            UiColorsLabelActive.Text = "Active entry\r\nLine 2\r\nLine 3\r\nLine 4\r\nLine 5\r\nLine 6\r\nLine 7";
            UiColorsLabelEntry.Text = "Entry\r\nLine 2\r\nLine 3\r\nLine 4\r\nLine 5\r\nLine 6\r\nLine 7";

            // Set the initial text on the tray icon
            UpdateNotifyIconText();

            // Update the location of the dynamic labels
            UiGeneralLabelEnableClipboardShortcut.Text = "(" + UiHotkeysButtonSearch.Text + ")";

            // Should we load the clipboard data file?
            if (UiGeneralToggleEnableClipboard.Checked && UiStorageToggleLoadClipboards.Checked)
            {
                if (File.Exists(pathAndData))
                {
                    isClipboardLoadingFromFile = true; // calling many of the same functions, but somewhere we need to do special stuff when this is loaded from start

                    // Show the "Loading" panel
                    UpdateDataFileProcessingUi("Please wait while processing data file");

                    LoadEntriesFromFile();
                }
//            } else
//            {
//                UiAdvancedButtonClearClipboards.Enabled = true;
            }

            // Catch "MouseDown" events for moving the application window
            UiFormPanel.MouseDown += new MouseEventHandler(TopBannerPanel_MouseDown);
            UiFormLabelApplicationName.MouseDown += new MouseEventHandler(TopBannerPanel_MouseDown);
            UiFormLabelApplicationVersion.MouseDown += new MouseEventHandler(TopBannerPanel_MouseDown);
            UiFormLabelLoadingPanel.MouseDown += new MouseEventHandler(TopBannerPanel_MouseDown);
            UiFormLabelLoadingText.MouseDown += new MouseEventHandler(TopBannerPanel_MouseDown);

            // Update meory status and clipboard entries in "Advanced" tab
            GetMemoryConsumption();
            LogMemoryConsumed();
            UpdateStorageInfo();
            UpdateAdvancedStatus();

            // Start "GetMemoryConsumption" timer
            TimerGetMemoryConsumption.Start();
        }


        // ###########################################################################################
        // Save history to a data file
        // ###########################################################################################

        public void SaveEntriesToFile()
        {
            // Start a stopwatch to measure the time it takes to save the data
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Only save if the "Save" toggle is enabled
            if (UiGeneralToggleEnableClipboard.Checked)
            {

                // Temporary dictionaries/lists
                List<int> keysToRemove = new List<int>();

                // Should we only save "Text" entries
                if (UiStorageRadioSaveOnlyText.Checked)
                {
                    // Walk through the original entries and check if they are favorites
                    foreach (var entry in entriesOriginal)
                    {
                        int key = entry.Key;

                        // Check if the key exists in entriesIsFavorite and if its value is true
                        if (entriesIsImage.TryGetValue(key, out bool isImage) && !isImage)
                        {
                            Logging.Log("Entry index [" + key + "] is text and saved");
                        }
                        else
                        {
                            keysToRemove.Add(key); // Add key to the removal list
                        }
                    }
                }

                // Should we only save "Favorites"
                if (UiStorageRadioSaveOnlyFavorites.Checked)
                {
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
                }

                // Remove the entries
                if (UiStorageRadioSaveOnlyText.Checked || UiStorageRadioSaveOnlyFavorites.Checked)
                {
                    foreach (var key in keysToRemove)
                    {
                        entriesOriginal.Remove(key);
                        entriesIsFavorite.Remove(key);
                        entriesIsTransparent.Remove(key);
                        entriesApplication.Remove(key);
                        entriesApplicationIcon.Remove(key);
                    }
                }

                // Number of entries to keep
                int keepCount = UiStorageTrackBarEntriesToSave.Value;

                // Remove entries if there are more than the number of entries to keep
                if (entriesOriginal.Count > keepCount)
                {
                    int removeCount = entriesOriginal.Count - keepCount; // number of entries to remove

                    // Get the keys of the entries to remove
                    keysToRemove = entriesOriginal.Keys.Take(removeCount).ToList();

                    // Remove the entries not needed
                    foreach (var key in keysToRemove)
                    {
                        entriesOriginal.Remove(key);
                        entriesIsFavorite.Remove(key);
                        entriesIsTransparent.Remove(key);
                        entriesApplication.Remove(key);
                        entriesApplicationIcon.Remove(key);
                    }
                }

                // Update the Tuple type declaration to include the new dictionary
                var dataToSerialize = new Tuple<
                    SortedList<int, Dictionary<string, object>>,
                    SortedDictionary<int, bool>,
                    SortedDictionary<int, string>,
                    SortedDictionary<int, Image>,
                    SortedDictionary<int, bool>
                >(entriesOriginal, entriesIsFavorite, entriesApplication, entriesApplicationIcon, entriesIsTransparent);

                // Serialization process
                var binaryFormatter = new BinaryFormatter();
                using (var memoryStream = new MemoryStream())
                {
                    binaryFormatter.Serialize(memoryStream, dataToSerialize);
                    var serializedData = memoryStream.ToArray();

                    // Compress the serialized data
                    using (var compressedStream = new MemoryStream())
                    {
                        using (var compressionStream = new GZipStream(compressedStream, CompressionMode.Compress))
                        {
                            compressionStream.Write(serializedData, 0, serializedData.Length);
                        }

                        var compressedData = compressedStream.ToArray();

                        // Encrypt the compressed data
                        var encryptedData = Encryption.EncryptStringToBytes_Aes(compressedData, encryptionKey, encryptionInitializationVector);

                        // Write the encrypted data to a file
                        using (var fileStream = new FileStream(pathAndData, FileMode.Create))
                        {
                            fileStream.Write(encryptedData, 0, encryptedData.Length);
                            Logging.Log("Clipboards, favorites, and image transparency settings saved successfully.");
                        }
                    }
                }
            }

            // Stop the stopwatch and log the time it took to save the data
            stopwatch.Stop();
            string elapsedTimeFormatted = string.Format("Timing for saving clipboards to file: {0:hh\\:mm\\:ss\\.fff}", stopwatch.Elapsed);
            Logging.Log(elapsedTimeFormatted);
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
                                // Decompress the decrypted data
                                byte[] decompressedData;
                                using (var compressedStream = new MemoryStream(decryptedData))
                                using (var decompressionStream = new GZipStream(compressedStream, CompressionMode.Decompress))
                                using (var resultStream = new MemoryStream())
                                {
                                    decompressionStream.CopyTo(resultStream);
                                    decompressedData = resultStream.ToArray();
                                }

                                // Deserialize the decompressed data
                                using (var memoryStream = new MemoryStream(decompressedData))
                                {
                                    var binaryFormatter = new BinaryFormatter();
                                    var loadedData = (Tuple<
                                        SortedList<int, Dictionary<string, object>>,
                                        SortedDictionary<int, bool>,
                                        SortedDictionary<int, string>,
                                        SortedDictionary<int, Image>,
                                        SortedDictionary<int, bool>
                                    >)binaryFormatter.Deserialize(memoryStream);

                                    // Extract the dictionaries from the tuple into temporary dictionaries
                                    entriesOriginalLoad = loadedData.Item1;
                                    entriesIsFavoriteLoad = loadedData.Item2;
                                    entriesApplicationLoad = loadedData.Item3;
                                    entriesApplicationIconLoad = loadedData.Item4;
                                    entriesIsTransparentLoad = loadedData.Item5;
                                }
                            }
                            catch (SerializationException ex)
                            {
                                Logging.Log($"Error during deserialization: {ex.Message}");
                                entriesOriginalLoad = null;
                                entriesIsFavoriteLoad = null;
                                entriesIsTransparentLoad = null;
                                entriesApplicationLoad = null;
                                entriesApplicationIconLoad = null;
                            }

                            // Process entries if decryption was successful
                            if (entriesOriginalLoad != null)
                            {
                                Logging.Log("---");
                                foreach (var entry in entriesOriginalLoad)
                                {
                                    try
                                    {
                                        // Safely update GuiLoadingText on the UI thread
                                        UiFormLabelLoadingText.Invoke((MethodInvoker)(() =>
                                        {
                                            UiFormLabelLoadingText.Text = $"Please wait while processing data file - loading entry [{entry.Key}] ...";
                                        }));


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
                                Logging.Log($"Loaded [{entriesOriginalLoad.Count}] entries from data file");

                                entriesOriginalLoad = null;
                                entriesIsFavoriteLoad = null;
                                entriesIsTransparentLoad = null;
                                entriesApplicationLoad = null;
                                entriesApplicationIconLoad = null;
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
//            if (UiFormLabelLoadingPanel.InvokeRequired)
//            {
                UiFormLabelLoadingPanel.Invoke(new Action(() =>
                {
                    UiFormLabelLoadingPanel.Visible = false;
                    isClipboardLoadingFromFile = false;
//                    UiAdvancedButtonClearClipboards.Enabled = true; // enable the "Clear clipboards" button
                    UiFormTabControl.Enabled = true;
                    UiFormTabControl.TabButtonSelectedState.FillColor = Color.FromArgb(56, 97, 55);
                    UiFormTabControl.TabButtonSelectedState.ForeColor = Color.FromArgb(227, 227, 227);
                }));
            /*
                        }
                        else
                        {
                            UiFormLabelLoadingPanel.Visible = false;
                            isClipboardLoadingFromFile = false;
                            UiAdvancedButtonClearClipboards.Enabled = true; // enable the "Clear clipboards" button
                            UiFormTabControl.Enabled = true;
                            UiFormTabControl.TabButtonSelectedState.FillColor = Color.FromArgb(56, 97, 55);
                            UiFormTabControl.TabButtonSelectedState.ForeColor = Color.FromArgb(227, 227, 227);
                        }
            */
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

                    // Only react if we have either a text or an image in the clipboard
                    if (Clipboard.ContainsText() || Clipboard.ContainsImage())
                    {                   

                        // Get the application name which updated the clipboard
                        IntPtr whoUpdatedClipboardHwnd = NativeMethods.GetClipboardOwner();
                        uint threadId = NativeMethods.GetWindowThreadProcessId(whoUpdatedClipboardHwnd, out uint thisProcessId);
                        whoUpdatedClipboardName = Process.GetProcessById((int)thisProcessId).ProcessName;

                        // Do not process clipboard, if this is coming from HovText itself
                        if (whoUpdatedClipboardName != exeFileNameWithoutExtension)
                        {
                            Logging.Log("Clipboard [UPDATE] event from application [" + whoUpdatedClipboardName + "]");

                            // Find the process icon - if possible - applications running with higher privs cannot be queried
                            appIcon = null;
                            try
                            {
                                Process process = null;
                                process = Process.GetProcessById((int)thisProcessId);
                                if (process != null && !process.HasExited)
                                {
                                    string processFilePath = process.MainModule.FileName;
                                    using (Icon appIconTmp = Icon.ExtractAssociatedIcon(processFilePath))
                                    {
                                        appIcon = appIconTmp.ToBitmap();
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Logging.Log("Error getting application icon: " + ex.Message);
                            }

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
                        //if (!GuiAlwaysPasteOriginal.Checked)
                        if (!UiGeneralToggleAlwaysPasteOriginal.Checked)
                        {
                            if (clipboardData == null) // this is when we are loading from file
                            {
                                SetClipboard();
                            }
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

                        isClipboardImageTransparent = false;

                        // Check if picture is transparent
                        if (clipboardData == null)
                        {
                            Image imgCopy = null;
                            imgCopy = GetImageFromClipboard();
                            if (imgCopy != null)
                            {
                                isClipboardImageTransparent = IsImageTransparent(imgCopy);
                            }
                        } else
                        {
                            isClipboardImageTransparent = entriesIsTransparentLoad[isClipboardLoadingFromFileKey];
                        }

                        // Add the image to the entries array and update the clipboard
                        AddEntry();
                        GetEntryCounter();
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
//                Clipboard.Clear();
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

                // Get hash value of picture in clipboard
                ImageConverter converter = new ImageConverter();
                byte[] byteArray = (byte[])converter.ConvertTo(clipboardImage, typeof(byte[]));
                string clipboardImageHash = Convert.ToBase64String(byteArray);

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
            // ---
            clipboardData = null;
            entriesApplication.Clear();
            entriesApplicationIcon.Clear();
            entriesImage.Clear();
            entriesImageTrans.Clear();
            entriesIsEmail.Clear();
            entriesIsFavorite.Clear();
            entriesIsImage.Clear();
            entriesIsTransparent.Clear();
            entriesIsUrl.Clear();
            entriesOriginal.Clear();
            entriesShow.Clear();
            entriesText.Clear();
        }


    // ###########################################################################################
    // Add the content from clipboard to the data arrays
    // ###########################################################################################

    private void AddEntry()
        {
            // Clear the dictionaries if we do not catch the history
            if (!isEnabledHistory)
            {
                ClearHistory();
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

                if (isClipboardImageTransparent && clipboardImage != null)
                {
                    Bitmap bmp = new Bitmap(clipboardImage);
                    bmp.MakeTransparent(bmp.GetPixel(0, 0));
                    entriesImageTrans.Add(entryIndex, (Image)bmp);
                    entriesIsTransparent.Add(entryIndex, true);
                } else
                {
                    entriesImageTrans.Add(entryIndex, null);
                    entriesIsTransparent.Add(entryIndex, false);
                }

                entriesShow.Add(entryIndex, true);
                if (isClipboardLoadingFromFile)
                {
                    bool hest = entriesIsFavoriteLoad[isClipboardLoadingFromFileKey];
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
                        Logging.Log("  Discarding format [" + format + "]");
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
                    string appName = entriesApplicationLoad[isClipboardLoadingFromFileKey];
                    Image icon = entriesApplicationIconLoad[isClipboardLoadingFromFileKey];
                    entriesApplication.Add(entryIndex, appName);
                    entriesApplicationIcon.Add(entryIndex, icon);
                }
                else
                {
                    entriesApplication.Add(entryIndex, whoUpdatedClipboardName);
                    entriesApplicationIcon.Add(entryIndex, appIcon);
                }

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
                        if ((UiHotkeysRadioPasteOnHotkey.Checked && !pasteOnHotkeySetCleartext) || (UiGeneralToggleAlwaysPasteOriginal.Enabled && UiGeneralToggleAlwaysPasteOriginal.Checked))
                        {
                            RestoreOriginal(entryIndex);
                        }
                        else
                        {
                            if (UiGeneralToggleTrimWhitespaces.Checked)
                            {
                                entryText = entryText.Trim();
                            }
//                            Clipboard.Clear();
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
            SuspendLayout();

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
//                ChangeFocusToHovText();

                // Only proceed if the entry counter is equal to or more than 0
                if (entryCounter > 0)
                {
                    isFirstCallAfterHotkey = false;
                    history.UpdateHistory("down");
                }
            }
            ResumeLayout();
        }


        // ###########################################################################################
        // Get the next newer entry from history
        // ###########################################################################################

        public void GoEntryHigherNumber()
        {
            SuspendLayout();

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
//                ChangeFocusToHovText();

                // Only proceed if the entry counter is less than the total amount of entries
                if (entryCounter <= entriesText.Count)
                {
                    isFirstCallAfterHotkey = false;
                    history.UpdateHistory("up");
                }
            }
            ResumeLayout();
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
                    if (UiHotkeysRadioPasteOnHotkey.Checked)
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
            entriesImageTrans.Add(insertKey, entriesImageTrans[entryIndex]);
            entriesApplication.Add(insertKey, entriesApplication[entryIndex]);
            entriesApplicationIcon.Add(insertKey, entriesApplicationIcon[entryIndex]);
            entriesOriginal.Add(insertKey, entriesOriginal[entryIndex]);
            entriesShow.Add(insertKey, entriesShow[entryIndex]);
            entriesIsFavorite.Add(insertKey, entriesIsFavorite[entryIndex]);
            entriesIsUrl.Add(insertKey, entriesIsUrl[entryIndex]);
            entriesIsEmail.Add(insertKey, entriesIsEmail[entryIndex]);
            entriesIsTransparent.Add(insertKey, entriesIsTransparent[entryIndex]);
            entriesIsImage.Add(insertKey, entriesIsImage[entryIndex]);

            // Remove the chosen entry, so it does not show duplicates
            entriesText.Remove(entryIndex);
            entriesImage.Remove(entryIndex);
            entriesImageTrans.Remove(entryIndex);
            entriesApplication.Remove(entryIndex);
            entriesApplicationIcon.Remove(entryIndex);
            entriesOriginal.Remove(entryIndex);
            entriesShow.Remove(entryIndex);
            entriesIsFavorite.Remove(entryIndex);
            entriesIsUrl.Remove(entryIndex);
            entriesIsEmail.Remove(entryIndex);
            entriesIsTransparent.Remove(entryIndex);
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
                        UiHotkeysRadioPasteOnHotkey.Checked = true;
                        UiHotkeysButtonPasteHotkey.Enabled = true;
                        break;
                    default:
                        UiHotkeysRadioUseStandardWindowsMethod.Checked = true;
                        UiHotkeysButtonPasteHotkey.Enabled = false;
                        break;
                }

                if(UiGeneralToggleEnableFavorites.Checked && UiGeneralToggleEnableClipboard.Checked)
                {
                    UiHotkeysLabelSearch.Enabled = true;
                    UiHotkeysLabelToggleFavorite.Enabled = true;
                }

                if (UiHotkeysRadioPasteOnHotkey.Checked)
                {
                    UiHotkeysLabelPasteHotkey.Enabled = true;
                }
                    
                UiGeneralToggleEnableClipboard.Enabled = true;
                UiGeneralLabelEnableClipboard.Enabled = true;
                UiGeneralLabelEnableClipboardShortcut.Enabled = true;
                UiHotkeysButtonToggleApplication.Enabled = true;

                SetFieldsBasedOnHistoryEnabled();

                UiGeneralToggleEnableFavorites.Enabled = true;
                UiGeneralLabelEnableFavorites.Enabled = true;
                UiGeneralToggleRestoreOriginal.Enabled = true;
                UiGeneralLabelRestoreOriginal.Enabled = true;
                UiHotkeysRadioUseStandardWindowsMethod.Enabled = true;
                UiHotkeysLabelUseStandardWindowsMethod.Enabled = true;
                UiHotkeysRadioPasteOnHotkey.Enabled = true;
                UiHotkeysLabelPasteOnHotkey.Enabled = true;
                UiGeneralToggleTrimWhitespaces.Enabled = true;
                UiGeneralLabelTrimWhitespaces.Enabled = true;
                UiGeneralToggleIncludeImages.Enabled = true;
                UiGeneralLabelIncludeImages.Enabled = true;
                UiGeneralTogglePasteToApplication.Enabled = true;
                UiGeneralLabelPasteToApplication.Enabled = true;
                UiGeneralToggleAlwaysPasteOriginal.Enabled = true;
                UiGeneralLabelAlwaysPasteOriginal.Enabled = true;
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

                UiHotkeysLabelSearch.Enabled = false;
                UiHotkeysLabelToggleFavorite.Enabled = false;
                UiHotkeysLabelPasteHotkey.Enabled = false;

                // Disable other checkboxes
                UiGeneralToggleEnableClipboard.Enabled = false;
                UiGeneralLabelEnableClipboard.Enabled = false;
                UiGeneralLabelEnableClipboardShortcut.Enabled = false;
                UiHotkeysButtonSearch.Enabled = false;
                UiHotkeysLabelUseStandardWindowsMethod.Enabled = false;
                UiGeneralToggleEnableFavorites.Enabled = false;
                UiGeneralLabelEnableFavorites.Enabled = false;
                UiGeneralToggleRestoreOriginal.Enabled = false;
                UiGeneralLabelRestoreOriginal.Enabled = false;
                UiGeneralToggleIncludeImages.Enabled = false;
                UiGeneralLabelIncludeImages.Enabled = false;
                UiGeneralToggleTrimWhitespaces.Enabled = false;
                UiGeneralLabelTrimWhitespaces.Enabled = false;
                UiGeneralTogglePasteToApplication.Enabled = false;
                UiGeneralLabelPasteToApplication.Enabled = false;
                UiGeneralToggleAlwaysPasteOriginal.Enabled = false;
                UiGeneralLabelAlwaysPasteOriginal.Enabled = false;
                UiHotkeysButtonToggleApplication.Enabled = false;
                UiHotkeysButtonPasteHotkey.Enabled = false;
                UiHotkeysRadioUseStandardWindowsMethod.Enabled = false;
                UiHotkeysRadioPasteOnHotkey.Enabled = false;
                UiHotkeysLabelPasteOnHotkey.Enabled = false;
                UiHotkeysButtonToggleFavorite.Enabled = false;
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
                ShowTrayNotifications("Running In Background");
                Hide();
            }
        }


        // ###########################################################################################
        // Launch browser and go to HovText web page when link is clicked
        // ###########################################################################################

        private void AboutBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Logging.Log("Clicked the web page link in \"About\"");
            Process.Start(e.LinkText);
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
        // Update the closing UI for the "save file"
        // ###########################################################################################

        private void UpdateDataFileProcessingUi(string text)
        {                    
            // Update UI from the UI thread
            UiFormTabControl.Enabled = false;
            UiFormTabControl.TabButtonSelectedState.FillColor = Color.LightGray;
            UiFormTabControl.TabButtonSelectedState.ForeColor = Color.DarkGray;

            // Show the "Loading" panel
            UiFormLabelLoadingText.Text = text;
            UiFormLabelLoadingPanel.Dock = DockStyle.Fill;
            UiFormLabelLoadingPanel.Visible = true;
            UiFormLabelLoadingPanel.BringToFront();

            // Calculate and set the label's location to center it within panel1
            UiFormLabelLoadingText.AutoSize = false;
            UiFormLabelLoadingText.Width = UiFormLabelLoadingPanel.ClientSize.Width;
            UiFormLabelLoadingText.Height = 30; // Set this to an appropriate value for your label
            UiFormLabelLoadingText.TextAlign = ContentAlignment.MiddleCenter;
            int x = (UiFormLabelLoadingPanel.ClientSize.Width - UiFormLabelLoadingText.Width) / 2;
            int y = (UiFormLabelLoadingPanel.ClientSize.Height - UiFormLabelLoadingText.Height) / 2;
            UiFormLabelLoadingText.Location = new Point(x, y);
        }


        // ###########################################################################################
        // Unregister from the clipboard chain, and remove hotkeys when application is closing down
        // ###########################################################################################

        private async void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            // In case windows is trying to shut down, don't hold up the process
            if (e.CloseReason == CloseReason.WindowsShutDown)
            {

                Logging.Log("Exit HovText");
                NativeMethods.RemoveClipboardFormatListener(this.Handle);
                Logging.Log("Removed HovText from clipboard chain");

                RemoveAllHotkeys();

                // Should we save the clipboard to data file?
                if (UiStorageToggleSaveClipboards.Checked)
                {
                    SaveEntriesToFile();
                }

                Logging.EndLogging();

                // Delete the troubleshooting logfile as the very last thing
                if (cleanupApp)
                {
                    DeleteFiles();
                }

                return;
            }

            // Prevent the application from closing, if we are currently loading the clipboard data file
            if (isClipboardLoadingFromFile)
            {
                e.Cancel = true;
                MessageBox.Show("HovText is currently processing a file - please wait until this finishes, before you close the application",
                    "INFO",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            // Normal exit
            if (!UiGeneralToggleCloseMinimizes.Checked || isClosedFromNotifyIcon)
            {
                if (isClosing) return;
                e.Cancel = true; // Prevent form from closing immediately

                UpdateDataFileProcessingUi("Please wait while processing data file - saving relevant clipboards ...");

                Logging.Log("Exit HovText");
                NativeMethods.RemoveClipboardFormatListener(this.Handle);
                Logging.Log("Removed HovText from clipboard chain");

                RemoveAllHotkeys();

                await Task.Run(() =>
                {
                    // Should we save the clipboard to data file?
                    if (UiStorageToggleSaveClipboards.Checked)
                    {
                        SaveEntriesToFile();
                    }
                });

                Logging.EndLogging();

                // Delete the troubleshooting logfile as the very last thing
                if (cleanupApp)
                {
                    DeleteFiles();
                }

                isClosing = true; // indicate that closing is in progress
                this.BeginInvoke(new Action(() => this.Close())); // close the form on the UI thread
                
                return;
            }

            // Called when closing from "Clean up"
            if (cleanupApp)
            {
                NativeMethods.RemoveClipboardFormatListener(this.Handle);
                RemoveAllHotkeys();

                DeleteFiles();

                return;
            }

            // Do not close as the X should minimize
            if (WindowState == FormWindowState.Normal)
            {
                ShowTrayNotifications("Running In Background");
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
            TimerUpdateVersion.Enabled = false;

            //            Logging.Log("Version check timer expired");
            Logging.Log("Versions in scope:");
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
                        if (!firstTimeLaunch)
                        {
                            update.Show();
                            update.Activate();
                            Logging.Log("  Notified on new [STABLE] version available");
                        } else
                        {
                            Logging.Log("  Did not notify on new [STABLE] version available, as this is the first launch");
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

            // ------------------------------------------------------------------------------------
            // Changes for the version 2024-January-20
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

            key = key.Length != 32 ? null : key;
            iv = iv.Length != 16 ? null : iv;
            if (key == null || iv == null)
            {
                Encryption.GenerateKeyAndIV(out key, out iv);
                strKey = Convert.ToBase64String(key);
                strIv = Convert.ToBase64String(iv);
                SetRegistryKey(registryPath, "EncryptionKey", strKey);
                SetRegistryKey(registryPath, "EncryptionInitializationVector", strIv);
                Logging.Log("    \"EncryptionKey\" and \"EncryptionInitializationVector\" have been reset as it was invalid in registry");
            }

            // Hotkeys
            // -------
            Logging.Log("  Hotkeys:");

            RegistryKeyCheckOrCreate(registryPath, "HotkeyBehaviour", registryHotkeyBehaviour);
            RegistryKeyCheckOrCreate(registryPath, "HotkeyToggleApplication", registryHotkeyToggleApplication);
            RegistryKeyCheckOrCreate(registryPath, "HotkeySearch", registryHotkeySearch);
            RegistryKeyCheckOrCreate(registryPath, "HotkeyToggleFavorite", registryHotkeyToggleFavorite);
            RegistryKeyCheckOrCreate(registryPath, "HotkeyPasteOnHotkey", registryHotkeyPasteOnHotkey);

            regVal = GetRegistryKey(registryPath, "HotkeyBehaviour");
            Logging.Log("    \"HotkeyBehaviour\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "HotkeyToggleApplication");
            Logging.Log("    \"HotkeyToggleApplication\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "HotkeySearch");
            Logging.Log("    \"HotkeySearch\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "HotkeyToggleFavorite");
            Logging.Log("    \"HotkeyToggleFavorite\" = [" + regVal + "]");
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
            RegistryKeyCheckOrCreate(registryPath, "WelcomeShown", "0");
            RegistryKeyCheckOrCreate(registryPath, "CheckedVersion", appVer);

            regVal = GetRegistryKey(registryPath, "NotificationShown");
            Logging.Log("    \"NotificationShown\" = [" + regVal + "]");
            regVal = GetRegistryKey(registryPath, "WelcomeShown");
            Logging.Log("    \"WelcomeShown\" = [" + regVal + "]");
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

        public static void RegistryKeyCheckOrCreate(string regPath, string regKey, string regValue)
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
                if(key != "EncryptionKey" && key != "EncryptionInitializationVector")
                {
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
            string hotkeyPasteOnHotkey = GetRegistryKey(registryPath, "HotkeyPasteOnHotkey");
            string hotkeyToggleFavorite = GetRegistryKey(registryPath, "HotkeyToggleFavorite");
            hotkeyToggleApplication = hotkeyToggleApplication.Length == 0 ? "Not set" : hotkeyToggleApplication;
            hotkeySearch = hotkeySearch.Length == 0 ? "Not set" : hotkeySearch;
            hotkeyPasteOnHotkey = hotkeyPasteOnHotkey.Length == 0 ? "Not set" : hotkeyPasteOnHotkey;
            hotkeyToggleFavorite = hotkeyToggleFavorite.Length == 0 ? "Not set" : hotkeyToggleFavorite;
            UiHotkeysButtonToggleApplication.Text = hotkeyToggleApplication;
            UiHotkeysButtonSearch.Text = hotkeySearch;
            UiHotkeysButtonPasteHotkey.Text = hotkeyPasteOnHotkey;
            UiHotkeysButtonToggleFavorite.Text = hotkeyToggleFavorite;
            SetHotkeys("Startup of application");

            // Hotkey behaviour
            string hotkeyBehaviour = GetRegistryKey(registryPath, "HotkeyBehaviour");
            switch (hotkeyBehaviour)
            {
                case "Paste":
                    UiHotkeysButtonPasteHotkey.Enabled = true;
                    UiHotkeysLabelPasteHotkey.Enabled = true;
                    UiHotkeysRadioPasteOnHotkey.Checked = true;
                    break;
                default:
                    UiHotkeysButtonPasteHotkey.Enabled = false;
                    UiHotkeysLabelPasteHotkey.Enabled = false;
                    UiHotkeysRadioUseStandardWindowsMethod.Checked = true;
                    break;
            }


            // ------------------------------------------
            // "General" tab
            // ------------------------------------------

            // Start with Windows
            string getKey = GetRegistryKey(registryPathRun, "HovText");
            if (getKey == null)
            {
                UiGeneralToggleStartWithWindows.Checked = false;
            }
            else
            {
                UiGeneralToggleStartWithWindows.Checked = true;

                // Overwrite "Run" if it does not contain "HovText.exe" or "--start-minimized"
                string runEntry = GetRegistryKey(registryPathRun, "HovText");
                string thisEntry = "\"" + System.Windows.Forms.Application.ExecutablePath + "\" --start-minimized";
                if (runEntry != thisEntry)
                {
                    SetRegistryKey(registryPathRun, "HovText", "\"" + System.Windows.Forms.Application.ExecutablePath + "\" --start-minimized");
                }
            }

            // Update timer
            TimerUpdateVersion.Enabled = true;
            UiAdvancedPicture1BoxDevRefresh.Visible = true;
            UiAdvancedPicture2BoxDevRefresh.Visible = false;
            UiAdvancedLabelDevVersion.Enabled = true;
            UiAdvancedLabelDisclaimer.Enabled = true;
            UiAdvancedLabelDevVersion.Text = "Please wait ...";
//            Logging.Log("Version check timer started");

            // Startup disabled
            int startDisabled = int.Parse((string)GetRegistryKey(registryPath, "StartDisabled"));
            UiGeneralToggleStartDisabled.Checked = startDisabled == 1;
            isStartDisabled = UiGeneralToggleStartDisabled.Checked;

            // Restore original when disabling application
            int restoreOriginal = int.Parse((string)GetRegistryKey(registryPath, "RestoreOriginal"));
            UiGeneralToggleRestoreOriginal.Checked = restoreOriginal == 1;
            isRestoreOriginal = UiGeneralToggleRestoreOriginal.Checked;

            // Do not copy images
            int copyImages = int.Parse((string)GetRegistryKey(registryPath, "CopyImages"));
            UiGeneralToggleIncludeImages.Checked = copyImages == 1;
            isCopyImages = UiGeneralToggleIncludeImages.Checked;

            // Close minimizes HovText
            int closeMinimizes = int.Parse((string)GetRegistryKey(registryPath, "CloseMinimizes"));
            UiGeneralToggleCloseMinimizes.Checked = closeMinimizes == 1;
            SetCloseMinimize();

            // Enable history
            int historySearch = int.Parse((string)GetRegistryKey(registryPath, "HistorySearch"));
            int historyInstantSelect = int.Parse((string)GetRegistryKey(registryPath, "HistoryInstantSelect"));
            isEnabledHistory = historySearch == 1 || historyInstantSelect == 1;
            UiGeneralToggleEnableClipboard.Checked = historySearch == 1;

            // Enable favorites
            int favoritesEnabled = int.Parse((string)GetRegistryKey(registryPath, "FavoritesEnable"));
            UiGeneralToggleEnableFavorites.Checked = favoritesEnabled == 1;
            isEnabledFavorites = UiGeneralToggleEnableFavorites.Checked;
            if (isEnabledFavorites && isEnabledHistory)
            {
                UiHotkeysButtonToggleFavorite.Enabled = true;
                UiHotkeysLabelToggleFavorite.Enabled = true;
                UiStorageRadioSaveOnlyFavorites.Enabled = true;
            }
            else
            {
                UiHotkeysButtonToggleFavorite.Enabled = false;
                UiHotkeysLabelToggleFavorite.Enabled = false;
                UiStorageRadioSaveOnlyFavorites.Enabled = false;
            }

            // Paste on history selection
            int pasteOnSelection = int.Parse((string)GetRegistryKey(registryPath, "PasteOnSelection"));
            UiGeneralTogglePasteToApplication.Checked = pasteOnSelection == 1;
            isEnabledPasteOnSelection = UiGeneralTogglePasteToApplication.Checked;

            // Paste on history selection
            int alwaysPasteOriginal = int.Parse((string)GetRegistryKey(registryPath, "AlwaysPasteOriginal"));
            UiGeneralToggleAlwaysPasteOriginal.Checked = alwaysPasteOriginal == 1;

            // Trim whitespaces
            int trimWhitespaces = int.Parse((string)GetRegistryKey(registryPath, "TrimWhitespaces"));
            UiGeneralToggleTrimWhitespaces.Checked = trimWhitespaces == 1;
            isEnabledTrimWhitespacing = UiGeneralToggleTrimWhitespaces.Checked;


            // ------------------------------------------
            // "Storage" tab
            // ------------------------------------------

            // Save clipboards on exit
            int saveOnExit = int.Parse((string)GetRegistryKey(registryPath, "StorageSaveOnExit"));
            UiStorageToggleSaveClipboards.Checked = saveOnExit == 1;

            // Load clipboards on launch
            int loadOnLaunch = int.Parse((string)GetRegistryKey(registryPath, "StorageLoadOnLaunch"));
            UiStorageToggleLoadClipboards.Checked = loadOnLaunch == 1;

            // Save type (all or favorites only)
            string saveType = GetRegistryKey(registryPath, "StorageSaveType");
            switch (saveType)
            {
                case "All":
                    UiStorageRadioSaveAll.Checked = true;
                    break;
                case "Favorites":
                    if (isEnabledFavorites)
                    {
                        UiStorageRadioSaveOnlyFavorites.Checked = true;
                    }
                    else
                    {
                        UiStorageRadioSaveOnlyText.Checked = true;
                        UiStorageRadioSaveOnlyFavorites.Enabled = false;
                    }
                    break;
                default: // Text
                    UiStorageRadioSaveOnlyText.Checked = true;
                    break;
            }

            // Save number of entries
            int entries = Int32.Parse(GetRegistryKey(registryPath, "StorageSaveEntries"));
            UiStorageTrackBarEntriesToSave.Value = entries;
            UiStorageTrackBarEntriesToSaveText.Text = entries.ToString();

            UiStorageTrackBarEntriesToSave.Maximum = maxClipboardEntriesToSave;


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
            UiLayoutTrackBarVisibleElements.Value = historyListElements;
            UiLayoutLabelVisibleElementsText.Text = historyListElements.ToString();

            // History area size
            historySizeWidth = Int32.Parse(GetRegistryKey(registryPath, "HistorySizeWidth"));
            historySizeHeight = Int32.Parse(GetRegistryKey(registryPath, "HistorySizeHeight"));
            historyMargin = Int32.Parse(GetRegistryKey(registryPath, "HistoryMargin"));
            UiLayoutTrackBarWidth.Value = historySizeWidth;
            UiLayoutTrackBarHeight.Value = historySizeHeight;
            UiLayoutTrackBarMargin.Value = historyMargin;
            UiLayoutLabelWidthText.Text = historySizeWidth.ToString() + "%";
            UiLayoutLabelHeightText.Text = historySizeHeight.ToString() + "%";
            UiLayoutLabelMarginText.Text = historyMargin.ToString() + "px";
            CheckIfDisableHistoryMargin();

            // History border thickness
            historyBorderThickness = Int32.Parse(GetRegistryKey(registryPath, "HistoryBorderThickness"));
            UiStyleTrackBarBorder.Value = historyBorderThickness;
            UiStyleLabelBorderText.Text = historyBorderThickness.ToString()+ "px";

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
                    UiColorsRadioBlue.Checked = true;
                    break;
                case "Brown":
                    UiColorsRadioBrown.Checked = true;
                    break;
                case "Green":
                    UiColorsRadioGreen.Checked = true;
                    break;
                case "Contrast":
                    UiColorsRadioContrast.Checked = true;
                    break;
                case "Custom":
                    UiColorsRadioCustom.Checked = true;
                    EnableDisableCustomColor();
                    break;
                default: // Yellow
                    UiColorsRadioYellow.Checked = true;
                    break;
            }
            EnableDisableCustomColor();
            SetHistoryColors();

            // History location
            historyLocation = GetRegistryKey(registryPath, "HistoryLocation");
            switch (historyLocation)
            {
                case "Left Top":
                    UiLayoutRadioLeftTop.Checked = true;
                    break;
                case "Left Bottom":
                    UiLayoutRadioLeftBottom.Checked = true;
                    break;
                case "Center":
                    UiLayoutRadioCenter.Checked = true;
                    break;
                case "Right Top":
                    UiLayoutRadioRightTop.Checked = true;
                    break;
                default: // Right Bottom
                    UiLayoutRadioRightBottom.Checked = true;
                    break;
            }

            // History font
            historyFontFamily = GetRegistryKey(registryPath, "HistoryFontFamily");
            historyFontSize = float.Parse((string)GetRegistryKey(registryPath, "HistoryFontSize"));
            UiColorsLabelHeader.Font = new Font(historyFontFamily, historyFontSize);
            UiColorsLabelSearch.Font = new Font(historyFontFamily, historyFontSize);
            UiStyleLabelFont.Text = historyFontFamily + "\r\n" + historyFontSize;
            UiStyleLabelFont.Font = new Font(historyFontFamily, historyFontSize);
            UiColorsLabelActive.Font = new Font(historyFontFamily, historyFontSize);
            UiColorsLabelEntry.Font = new Font(historyFontFamily, historyFontSize);
            SetHistoryColors();

            // Icon set
            iconSet = GetRegistryKey(registryPath, "IconSet");
            if (iconSet == "Round")
            {
                UiStyleRadioIconRoundModern.Checked = true;
            }
            else if (iconSet == "SquareOld")
            {
                UiStyleRadioIconSquareOriginal.Checked = true;
            }
            else if (iconSet == "SquareNew")
            {
                UiStyleRadioIconSquareModern.Checked = true;
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
            var control = UiLayoutGroupBoxShowClipboardListOn.Controls["uiScreen" + activeDisplay] as Guna2CustomRadioButton;
            if (control != null)
            {
                control.Checked = true;
            }


            // ------------------------------------------
            // "Advanced" tab
            // ------------------------------------------

            // Troubleshooting
            int troubleshootEnable = int.Parse((string)GetRegistryKey(registryPath, "TroubleshootEnable"));
            UiAdvancedToggleEnableLog.Checked = troubleshootEnable == 1;
        }


        // ###########################################################################################
        // Show or hide the control boxes (Close/Minimize)
        // ###########################################################################################

        private void SetCloseMinimize()
        {
            if (UiGeneralToggleCloseMinimizes.Checked)
            {
                UiFormControlBoxMinimize.Visible = false;
                UiFormControlBoxClose.HoverState.FillColor = Color.DarkSeaGreen;
            }
            else
            {
                UiFormControlBoxMinimize.Visible = true;
                UiFormControlBoxClose.HoverState.FillColor = Color.Firebrick;
            }
        }


        // ###########################################################################################
        // Enable or disable that the application starts up with Windows
        // https://www.fluxbytes.com/csharp/start-application-at-windows-startup/
        // ###########################################################################################

        private void GuiStartWithWindows_CheckedChanged(object sender, EventArgs e)
        {
            if(UiGeneralToggleStartWithWindows.Checked)
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
                Font = UiStyleLabelFont.Font, // initialize the font dialogue with the font from "uiShowFont"
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
                UiColorsLabelHeader.Font = new Font(historyFontFamily, historyFontSize);
                UiColorsLabelSearch.Font = new Font(historyFontFamily, historyFontSize);
                UiStyleLabelFont.Text = historyFontFamily + "\r\n" + historyFontSize;
                UiStyleLabelFont.Font = new Font(historyFontFamily, historyFontSize);
                UiColorsLabelActive.Font = new Font(historyFontFamily, historyFontSize);
                UiColorsLabelEntry.Font = new Font(historyFontFamily, historyFontSize);
                SetRegistryKey(registryPath, "HistoryFontFamily", historyFontFamily);
                SetRegistryKey(registryPath, "HistoryFontSize", historyFontSize.ToString());
            }
        }


        // ###########################################################################################
        // Changes in the number of visible history elements
        // ###########################################################################################

        private void HistoryElements_Scroll(object sender, EventArgs e)
        {
            UiLayoutLabelVisibleElementsText.Text = UiLayoutTrackBarVisibleElements.Value.ToString();
            historyListElements = UiLayoutTrackBarVisibleElements.Value;
            SetRegistryKey(registryPath, "HistoryEntries", historyListElements.ToString());
        }


        // ###########################################################################################
        // Disable history margin, if width or height is more tyhan 90%
        // ###########################################################################################

        private void CheckIfDisableHistoryMargin()
        {
            if (UiLayoutTrackBarWidth.Value > 90 || UiLayoutTrackBarHeight.Value > 90)
            {
                UiLayoutLabelMargin.Enabled = false;
                UiLayoutTrackBarMargin.Enabled = false;
                UiLayoutLabelMarginText.Enabled = false;
                isHistoryMarginEnabled = false;
            }
            else
            {
                UiLayoutLabelMargin.Enabled = true;
                UiLayoutTrackBarMargin.Enabled = true;
                UiLayoutLabelMarginText.Enabled = true;
                isHistoryMarginEnabled = true;
            }
        }


        // ###########################################################################################
        // Changes in the history (area) size
        // ###########################################################################################

        private void HistorySizeWidth_ValueChanged(object sender, EventArgs e)
        {
            UiLayoutLabelWidthText.Text = UiLayoutTrackBarWidth.Value.ToString() + "%";
            historySizeWidth = UiLayoutTrackBarWidth.Value;
            SetRegistryKey(registryPath, "HistorySizeWidth", historySizeWidth.ToString());

            // Disable margin, if above 90%
            CheckIfDisableHistoryMargin();
        }

        private void HistorySizeHeight_ValueChanged(object sender, EventArgs e)
        {
            UiLayoutLabelHeightText.Text = UiLayoutTrackBarHeight.Value.ToString() + "%";
            historySizeHeight = UiLayoutTrackBarHeight.Value;
            SetRegistryKey(registryPath, "HistorySizeHeight", historySizeHeight.ToString());

            // Disable margin, if above 90%
            CheckIfDisableHistoryMargin();
        }

        private void GuiHistoryMargin_ValueChanged(object sender, EventArgs e)
        {
            UiLayoutLabelMarginText.Text = UiLayoutTrackBarMargin.Value.ToString() + "px";
            historyMargin = UiLayoutTrackBarMargin.Value;
            SetRegistryKey(registryPath, "HistoryMargin", historyMargin.ToString());
        }


        // ###########################################################################################
        // Change in the history location
        // ###########################################################################################

        private void GuiHistoryLocation_CheckedChanged(object sender, EventArgs e)
        {
            if (UiLayoutRadioLeftTop.Checked)
            {
                historyLocation = "Left Top";
                SetRegistryKey(registryPath, "HistoryLocation", historyLocation);
            } else if (UiLayoutRadioLeftBottom.Checked)
            {
                historyLocation = "Left Bottom";
                SetRegistryKey(registryPath, "HistoryLocation", historyLocation);
            } else if (UiLayoutRadioCenter.Checked)
            {
                historyLocation = "Center";
                SetRegistryKey(registryPath, "HistoryLocation", historyLocation);
            } else if (UiLayoutRadioRightTop.Checked)
            {
                historyLocation = "Right Top";
                SetRegistryKey(registryPath, "HistoryLocation", historyLocation);
            } else if (UiLayoutRadioRightBottom.Checked)
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
            if (UiColorsRadioCustom.Checked)
            {
                UiColorsGroupBoxCustom.Enabled = true;
                UiColorsButtonHeaderBackground.Enabled = true;
                UiColorsButtonHeaderText.Enabled = true;
                UiColorsButtonSearchBackground.Enabled = true;
                UiColorsButtonSearchText.Enabled = true;
                UiColorsButtonEntryBackground.Enabled = true;
                UiColorsButtonEntryText.Enabled = true;
                UiColorsButtonActiveBackground.Enabled = true;
                UiColorsButtonActiveText.Enabled = true;
                UiColorsButtonBorder.Enabled = true;

                UiColorsLabelHeaderBackground.Enabled = true;
                UiColorsLabelHeaderText.Enabled = true;
                UiColorsLabelSearchBackground.Enabled = true;
                UiColorsLabelSearchText.Enabled = true;
                UiColorsLabelEntryBackground.Enabled = true;
                UiColorsLabelEntryText.Enabled = true;
                UiColorsLabelActiveBackground.Enabled = true;
                UiColorsLabelActiveText.Enabled = true;
                UiColorsLabelBorder.Enabled = true;

                UiColorsButtonHeaderBackground.FillColor = ColorTranslator.FromHtml(historyColorsHeader["Custom"]);
                UiColorsButtonHeaderText.FillColor = ColorTranslator.FromHtml(historyColorsHeaderText["Custom"]);
                UiColorsButtonSearchBackground.FillColor = ColorTranslator.FromHtml(historyColorsSearch["Custom"]);
                UiColorsButtonSearchText.FillColor = ColorTranslator.FromHtml(historyColorsSearchText["Custom"]);
                UiColorsButtonActiveBackground.FillColor = ColorTranslator.FromHtml(historyColorsActive["Custom"]);
                UiColorsButtonActiveText.FillColor = ColorTranslator.FromHtml(historyColorsActiveText["Custom"]);
                UiColorsButtonEntryBackground.FillColor = ColorTranslator.FromHtml(historyColorsEntry["Custom"]);
                UiColorsButtonEntryText.FillColor = ColorTranslator.FromHtml(historyColorsEntryText["Custom"]);
                UiColorsButtonBorder.FillColor = ColorTranslator.FromHtml(historyColorsBorder["Custom"]);
            }
            else
            {
                UiColorsGroupBoxCustom.Enabled = false;
                UiColorsButtonHeaderBackground.Enabled = false;
                UiColorsButtonHeaderText.Enabled = false;
                UiColorsButtonSearchBackground.Enabled = false;
                UiColorsButtonSearchText.Enabled = false;
                UiColorsButtonActiveBackground.Enabled = false;
                UiColorsButtonActiveText.Enabled = false;
                UiColorsButtonEntryBackground.Enabled = false;
                UiColorsButtonEntryText.Enabled = false;
                UiColorsButtonBorder.Enabled = false;

                UiColorsLabelHeaderBackground.Enabled = false;
                UiColorsLabelHeaderText.Enabled = false;
                UiColorsLabelSearchBackground.Enabled = false;
                UiColorsLabelSearchText.Enabled = false;
                UiColorsLabelEntryBackground.Enabled = false;
                UiColorsLabelEntryText.Enabled = false;
                UiColorsLabelActiveBackground.Enabled = false;
                UiColorsLabelActiveText.Enabled = false;
                UiColorsLabelBorder.Enabled = false;

                UiColorsButtonHeaderBackground.FillColor = Color.WhiteSmoke;
                UiColorsButtonHeaderText.FillColor = Color.WhiteSmoke;
                UiColorsButtonSearchBackground.FillColor = Color.WhiteSmoke;
                UiColorsButtonSearchText.FillColor = Color.WhiteSmoke;
                UiColorsButtonActiveBackground.FillColor = Color.WhiteSmoke;
                UiColorsButtonActiveText.FillColor = Color.WhiteSmoke;
                UiColorsButtonEntryBackground.FillColor = Color.WhiteSmoke;
                UiColorsButtonEntryText.FillColor = Color.WhiteSmoke;
                UiColorsButtonBorder.FillColor = Color.WhiteSmoke;
            }
        }


        // ###########################################################################################
        // Change in the history color
        // ###########################################################################################

        private void GuiHistoryColorTheme_CheckedChanged(object sender, EventArgs e)
        {
            // Get the text name for the clicked radio item
            historyColorTheme = (sender as Guna2CustomRadioButton).Text;

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
            UiColorsLabelHeader.BackColor = ColorTranslator.FromHtml(historyColorsHeader[historyColorTheme]);
            UiColorsLabelHeader.ForeColor = ColorTranslator.FromHtml(historyColorsHeaderText[historyColorTheme]);
            UiColorsLabelSearch.BackColor = ColorTranslator.FromHtml(historyColorsSearch[historyColorTheme]);
            UiColorsLabelSearch.ForeColor = ColorTranslator.FromHtml(historyColorsSearchText[historyColorTheme]);
            UiColorsLabelActive.BackColor = ColorTranslator.FromHtml(historyColorsActive[historyColorTheme]);
            UiColorsLabelActive.ForeColor = ColorTranslator.FromHtml(historyColorsActiveText[historyColorTheme]);
            UiColorsLabelEntry.BackColor = ColorTranslator.FromHtml(historyColorsEntry[historyColorTheme]);
            UiColorsLabelEntry.ForeColor = ColorTranslator.FromHtml(historyColorsEntryText[historyColorTheme]);
            UiStyleLabelFont.BackColor = ColorTranslator.FromHtml(historyColorsActive[historyColorTheme]);
            UiStyleLabelFont.ForeColor = ColorTranslator.FromHtml(historyColorsActiveText[historyColorTheme]);
            
        }


        // ###########################################################################################
        // Changes in "Close minimizes application to tray"
        // ###########################################################################################

        private void GuiCloseMinimize_CheckedChanged(object sender, EventArgs e)
        {
            string status = UiGeneralToggleCloseMinimizes.Checked ? "1" : "0";
            SetRegistryKey(registryPath, "CloseMinimizes", status);
            SetCloseMinimize();
        }


        // ###########################################################################################
        // Changes in "Restore original formatting when disabling application"
        // ###########################################################################################

        private void GuiRestoreOriginal_CheckedChanged(object sender, EventArgs e)
        {
            // History enabled
            string status = UiGeneralToggleRestoreOriginal.Checked ? "1" : "0";
            isRestoreOriginal = UiGeneralToggleRestoreOriginal.Checked;
            SetRegistryKey(registryPath, "RestoreOriginal", status);
        }


        // ###########################################################################################
        // Changes in "Do not copy images"
        // ###########################################################################################

        private void GuiCopyImages_CheckedChanged(object sender, EventArgs e)
        {
            string status = UiGeneralToggleIncludeImages.Checked ? "1" : "0";
            isCopyImages = UiGeneralToggleIncludeImages.Checked;
            SetRegistryKey(registryPath, "CopyImages", status);
        }


        // ###########################################################################################
        // Changes in "Enable history"
        // ###########################################################################################

        private void GuiSearch_CheckedChanged(object sender, EventArgs e)
        {
            string status = UiGeneralToggleEnableClipboard.Checked ? "1" : "0";
            SetRegistryKey(registryPath, "HistorySearch", status);
            RemoveAllHotkeys();
            SetFieldsBasedOnHistoryEnabled();
        }

        private void SetFieldsBasedOnHistoryEnabled()
        {
            isEnabledHistory = UiGeneralToggleEnableClipboard.Checked;

            if (UiGeneralToggleEnableClipboard.Checked)
            {
                UiHotkeysButtonSearch.Enabled = true;
                UiGeneralLabelEnableClipboardShortcut.Enabled = true;
                UiHotkeysLabelSearch.Enabled = true;
            }
            else
            {
                UiHotkeysButtonSearch.Enabled = false;
                UiGeneralLabelEnableClipboardShortcut.Enabled = false;
            }

            if (UiGeneralToggleEnableFavorites.Checked)
            {
                UiHotkeysButtonToggleFavorite.Enabled = true;
                UiHotkeysLabelToggleFavorite.Enabled = true;
            }
            else
            {
                UiHotkeysButtonToggleFavorite.Enabled = false;
            }

            if (isEnabledHistory)
            {
                UiGeneralToggleIncludeImages.Enabled = true;
                UiGeneralLabelIncludeImages.Enabled = true;
                UiGeneralTogglePasteToApplication.Enabled = true;
                UiGeneralLabelPasteToApplication.Enabled = true;
                UiGeneralToggleAlwaysPasteOriginal.Enabled = true;
                UiGeneralLabelAlwaysPasteOriginal.Enabled = true;
                UiGeneralToggleEnableFavorites.Enabled = true;
                UiGeneralLabelEnableFavorites.Enabled = true;
                UiStorageGroupBoxStorage.Enabled = true;
            }
            else
            {
                UiGeneralToggleIncludeImages.Enabled = false;
                UiGeneralLabelIncludeImages.Enabled = false;
                UiGeneralTogglePasteToApplication.Enabled = false;
                UiGeneralLabelPasteToApplication.Enabled = false;
                UiGeneralToggleAlwaysPasteOriginal.Enabled = false;
                UiGeneralLabelAlwaysPasteOriginal.Enabled = false;
                UiGeneralToggleEnableFavorites.Enabled = false;
                UiGeneralLabelEnableFavorites.Enabled = false;
                UiHotkeysButtonToggleFavorite.Enabled = false;
                UiHotkeysLabelToggleFavorite.Enabled = false;
                UiHotkeysLabelSearch.Enabled = false;
                UiStorageGroupBoxStorage.Enabled = false;

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
            string status = UiGeneralToggleAlwaysPasteOriginal.Checked ? "1" : "0";
            SetRegistryKey(registryPath, "AlwaysPasteOriginal", status);

            SetClipboard();
        }


        // ###########################################################################################
        // Changes in "Enable favorites"
        // ###########################################################################################

        private void GuiFavoritesEnabled_CheckedChanged(object sender, EventArgs e)
        {
            string status = UiGeneralToggleEnableFavorites.Checked ? "1" : "0";
            isEnabledFavorites = UiGeneralToggleEnableFavorites.Checked;
            SetRegistryKey(registryPath, "FavoritesEnable", status);
            if (isEnabledFavorites)
            {
                if(isEnabledHistory)
                {

                    UiHotkeysButtonToggleFavorite.Enabled = true;
                    UiHotkeysLabelToggleFavorite.Enabled = true;
                    UiStorageRadioSaveOnlyFavorites.Enabled = true;
                }
            }
            else
            {
                UiHotkeysButtonToggleFavorite.Enabled = false;
                UiHotkeysLabelToggleFavorite.Enabled = false;
                UiStorageRadioSaveOnlyFavorites.Enabled = false;
                showFavoriteList = false;

                // If we previously did save "Favorites only" then we need to change that to "All"
                if (UiStorageRadioSaveOnlyFavorites.Checked)
                {
                    UiStorageRadioSaveAll.Checked = true;
                    UiStorageRadioSaveOnlyFavorites.Enabled = false;
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
            string status = UiGeneralTogglePasteToApplication.Checked ? "1" : "0";
            isEnabledPasteOnSelection = UiGeneralTogglePasteToApplication.Checked;
            SetRegistryKey(registryPath, "PasteOnSelection", status);
        }


        // ###########################################################################################
        // Changes in "Trim whitespaces"
        // ###########################################################################################

        private void GuiTrimWhitespaces_CheckedChanged(object sender, EventArgs e)
        {
            string status = UiGeneralToggleTrimWhitespaces.Checked ? "1" : "0";
            isEnabledTrimWhitespacing = UiGeneralToggleTrimWhitespaces.Checked;
            SetRegistryKey(registryPath, "TrimWhitespaces", status);
        }


        // ###########################################################################################
        // Changes in "Start disabled"
        // ###########################################################################################

        private void GuiStartDisabled_CheckedChanged(object sender, EventArgs e)
        {
            // Start as disabled
            string status = UiGeneralToggleStartDisabled.Checked ? "1" : "0";
            isStartDisabled = UiGeneralToggleStartDisabled.Checked;
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
            UiFormTabControl.SelectedIndex = 9; // About
        }


        // ###########################################################################################
        // When clicking the "Settings" in the tray icon menu
        // ###########################################################################################

        private void TrayIconSettings_Click(object sender, EventArgs e)
        {
            ShowSettingsForm();
            UiFormTabControl.SelectedIndex = 0; // General
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
                TimerMouseClick.Start();
            }
        }

        private void MouseClickTimer_Tick(object sender, EventArgs e)
        {
            Logging.Log("Tray icon single-clicked - toggling application enable/disable");
            TimerMouseClick.Stop();
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
                TimerMouseClick.Stop();

                ShowSettingsForm();
                UiFormTabControl.SelectedIndex = 0;
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
            if (UiHotkeysRadioUseStandardWindowsMethod.Checked)
            {
                UiHotkeysButtonPasteHotkey.Enabled = false;
                SetRegistryKey(registryPath, "HotkeyBehaviour", "System");
                UiGeneralToggleRestoreOriginal.Enabled = true;
                UiGeneralLabelRestoreOriginal.Enabled = true;
                UiHotkeysLabelPasteHotkey.Enabled = false;
                SetNotifyIcon();
            }
        }


        // ###########################################################################################
        // Changes in "Action only on hotkey" hotkey behaviour
        // ###########################################################################################

        private void GuiHotkeyBehaviourPaste_CheckedChanged(object sender, EventArgs e)
        {
            UiHotkeysButtonPasteHotkey.Enabled = true;
            SetRegistryKey(registryPath, "HotkeyBehaviour", "Paste");
            SetHotkeys("Hotkey behaviour change");
            UiGeneralToggleRestoreOriginal.Enabled = false;
            UiGeneralLabelRestoreOriginal.Enabled = false;
            UiHotkeysLabelPasteHotkey.Enabled = true;
            SetNotifyIcon();
        }


        // ###########################################################################################
        // Clicking the "Documentation" button
        // ###########################################################################################

        private void GuiHelp_Click(object sender, EventArgs e)
        {
            // Show the specific help for a "Development" version
            string releaseTrain = "";
            releaseTrain += buildType == "Debug" ? "-dev" : "";

            Process.Start(hovtextPage + "/documentation" + releaseTrain);
            Logging.Log("Clicked the \"Documentation\" button under \"Documentation\"");
        }


        // ###########################################################################################
        // Clicking the "Changelog" button
        // ###########################################################################################

        private void guna2Button25_Click(object sender, EventArgs e)
        {
            Process.Start(hovtextPage + "/download");
            Logging.Log("Clicked the \"Changelog\" button under \"Documentation\"");
        }


        // ###########################################################################################
        // Clicking the "GitHub" button
        // ###########################################################################################

        private void guna2Button26_Click(object sender, EventArgs e)
        {
            Process.Start(hovTextGithub);
            Logging.Log("Clicked the \"GitHub\" button under \"Documentation\"");
        }


        // ###########################################################################################
        // Clicking the "Donators" button
        // ###########################################################################################

        private void guna2Button27_Click(object sender, EventArgs e)
        {
            Process.Start(hovTextDonators);
            Logging.Log("Clicked the \"Donators\" button under \"Documentation\"");
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
            if (isApplicationEnabled && entryCounter > 0 && !isClipboardLoadingFromFile)
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
            UiHotkeysButtonToggleApplication.Text = hotkey;
            if (e.Alt)
            {
                // https://stackoverflow.com/a/3068797/2028935
                e.SuppressKeyPress = true;
            }
        }

        private void GuiHotkeySearch_KeyDown(object sender, KeyEventArgs e)
        {
            string hotkey = ConvertKeyboardInputToString(e);
            UiHotkeysButtonSearch.Text = hotkey;

            if (e.Alt)
            {
                // https://stackoverflow.com/a/3068797/2028935
                e.SuppressKeyPress = true;
            }
        }

        private void HotkeyPaste_KeyDown(object sender, KeyEventArgs e)
        {
            string hotkey = ConvertKeyboardInputToString(e);
            UiHotkeysButtonPasteHotkey.Text = hotkey;
            if (e.Alt)
            {
                // https://stackoverflow.com/a/3068797/2028935
                e.SuppressKeyPress = true;
            }
        }

        private void HotkeyToggleFavorite_KeyDown(object sender, KeyEventArgs e)
        {
            string hotkey = ConvertKeyboardInputToString(e);
            UiHotkeysButtonToggleFavorite.Text = hotkey;
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


        // ###########################################################################################
        // Color the hotkey field and enable the "Apply" and "Cancel" buttons
        // ###########################################################################################

        private void ModifyHotkey()
        {
            switch (hotkey)
            {
                case "hotkeyEnable":
                    UiHotkeysButtonToggleApplication.FillColor = SystemColors.Info;
                    break;
                case "hotkeySearch":
                    UiHotkeysButtonSearch.FillColor = SystemColors.Info;
                    break;
                case "hotkeyPaste":
                    UiHotkeysButtonPasteHotkey.FillColor = SystemColors.Info;
                    break;
                case "hotkeyToggleFavorite":
                    UiHotkeysButtonToggleFavorite.FillColor = SystemColors.Info;
                    break;
            }

            // Enable the two buttons, "Apply" and "Cancel"
            UiHotkeysButtonApply.Enabled = true;
            UiHotkeysButtonCancel.Enabled = true;

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
            HotkeyManager.Current.Remove("PasteOnHotkey");
            Logging.Log("[HotkeyToggleApplication] removed");
            Logging.Log("[HotkeySearch] removed");
            Logging.Log("[HotkeyPasteOnHotkey] removed");
        }


        // ###########################################################################################
        // Apply the hotkeys
        // ###########################################################################################

        private void ApplyHotkeys_Click(object sender, EventArgs e)
        {
            SetHotkeys("Apply hotkeys button press");
            UiGeneralLabelEnableClipboardShortcut.Text = "(" + UiHotkeysButtonSearch.Text + ")";
        }


        // ###########################################################################################
        // Set (or remove) the hotkeys and validate them
        // ###########################################################################################

        private void SetHotkeys(string from)
        {
            Logging.Log("Called \"SetHotkeys()\" from \"" + from + "\"");

            // Get all hotkey strings
            string hotkeyToggleApplication = UiHotkeysButtonToggleApplication.Text;
            string hotkeySearch = UiHotkeysButtonSearch.Text;
            string hotkeyPasteOnHotkey = UiHotkeysButtonPasteHotkey.Text;
            string hotkeyToggleFavorite = UiHotkeysButtonToggleFavorite.Text;

            string conflictText = "";
            KeysConverter cvt;
            Keys key;

            // Convert the strings to hotkey objects

            //if (GuiSearch.Checked)
            if (UiGeneralToggleEnableClipboard.Checked)
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

            // "Paste only on hotkey"
            if (
                hotkeyPasteOnHotkey != "Unsupported"
                && hotkeyPasteOnHotkey != "Not set"
                && hotkeyPasteOnHotkey != "Hotkey conflicts"
                && UiHotkeysRadioPasteOnHotkey.Checked
                )
            {
                try
                {
                    cvt = new KeysConverter();
                    key = (Keys)cvt.ConvertFrom(hotkeyPasteOnHotkey);

                    // Only (re)enable it, if the "Action only on hotkey" behaviour has been chosen
                    if (UiHotkeysRadioPasteOnHotkey.Checked)
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
                UiFormTabControl.SelectedIndex = 1;
            }

            // Save the hotkeys to registry, if no erros
            if (
                hotkeyToggleApplication != "Unsupported" && hotkeyToggleApplication != "Hotkey conflicts" &&
                hotkeySearch != "Unsupported" && hotkeySearch != "Hotkey conflicts" &&
                hotkeyPasteOnHotkey != "Unsupported" && hotkeyPasteOnHotkey != "Hotkey conflicts" &&
                hotkeyToggleFavorite != "Unsupported" && hotkeyToggleFavorite != "Hotkey conflicts"
                )
            {
                SetRegistryKey(registryPath, "HotkeyToggleApplication", hotkeyToggleApplication);
                SetRegistryKey(registryPath, "HotkeySearch", hotkeySearch);
                SetRegistryKey(registryPath, "HotkeyPasteOnHotkey", hotkeyPasteOnHotkey);
                SetRegistryKey(registryPath, "HotkeyToggleFavorite", hotkeyToggleFavorite);
            }

            bool hasError = false;

            // Update the hotkey fields to reflect if they are good or bad

            // "Application toggle"
            if (hotkeyToggleApplication == "Unsupported" || hotkeyToggleApplication == "Hotkey conflicts")
            {
                hasError = true;
                UiHotkeysButtonToggleApplication.Text = hotkeyToggleApplication;
                UiHotkeysButtonToggleApplication.FillColor = Color.DarkSalmon;
            }
            else
            {
                UiHotkeysButtonToggleApplication.FillColor = Color.FromArgb(220,227,220);
            }

            // "Search"
            if (hotkeySearch == "Unsupported" || hotkeySearch == "Hotkey conflicts")
            {
                hasError = true;
                UiHotkeysButtonSearch.Text = hotkeySearch;
                UiHotkeysButtonSearch.FillColor = Color.DarkSalmon;
            }
            else
            {
                UiHotkeysButtonSearch.FillColor = Color.FromArgb(220, 227, 220);
            }

            // "Paste only on hotkey"
            if (hotkeyPasteOnHotkey == "Unsupported" || hotkeyPasteOnHotkey == "Hotkey conflicts")
            {
                hasError = true;
                UiHotkeysButtonPasteHotkey.Text = hotkeyPasteOnHotkey;
                UiHotkeysButtonPasteHotkey.FillColor = Color.DarkSalmon;
            }
            else
            {
                UiHotkeysButtonPasteHotkey.FillColor = Color.FromArgb(220, 227, 220);
            }

            // "Toggle favorite entry"
            if (hotkeyToggleFavorite == "Unsupported" || hotkeyToggleFavorite == "Hotkey conflicts")
            {
                hasError = true;
                UiHotkeysButtonToggleFavorite.Text = hotkeyToggleFavorite;
                UiHotkeysButtonToggleFavorite.FillColor = Color.DarkSalmon;
            }
            else
            {
                UiHotkeysButtonToggleFavorite.FillColor = Color.FromArgb(220, 227, 220);
            }

            // Accept the changes and disable the two buttons again
            if (!hasError)
            {
                UiHotkeysButtonApply.Enabled = false;
                UiHotkeysButtonCancel.Enabled = false;
            }
        }


        // ###########################################################################################
        // Cancel the hotkeys and restore original content
        // ###########################################################################################

        private void CancelHotkey_Click(object sender, EventArgs e)
        {
            string hotkeyToggleApplication = GetRegistryKey(registryPath, "HotkeyToggleApplication");
            string hotkeySearch = GetRegistryKey(registryPath, "HotkeySearch");
            string hotkeyPasteOnHotkey = GetRegistryKey(registryPath, "HotkeyPasteOnHotkey");
            string hotkeyToggleFavorite = GetRegistryKey(registryPath, "HotkeyToggleFavorite");
            UiHotkeysButtonToggleApplication.Text = hotkeyToggleApplication;
            UiHotkeysButtonSearch.Text = hotkeySearch;
            UiHotkeysButtonPasteHotkey.Text = hotkeyPasteOnHotkey;
            UiHotkeysButtonToggleFavorite.Text = hotkeyToggleFavorite;
            SetHotkeys("Cancel hotkeys button press");
            Logging.Log("Cancelling hotkeys association and reverting to previous values");
        }


        // ###########################################################################################
        // Show a notification if the user closes the application to tray for the very first time
        // ###########################################################################################

        private void ShowTrayNotifications(string action)
        {

            // Show a tray notification if this is the first time the user close the form (without exiting the application)
            if(action == "Running In Background")
            {
                int notificationShown = int.Parse((string)GetRegistryKey(registryPath, "NotificationShown"));
                if (notificationShown == 0)
                {
                    IconNotify.Visible = true;
                    IconNotify.ShowBalloonTip(
                        10000,
                        "HovText is still running",
                        "HovText continues running in the background to perform its duties. You can see the icon in the tray area.",
                        ToolTipIcon.Info
                        );

                    // Mark that we now have shown this
                    SetRegistryKey(registryPath, "NotificationShown", "1");
                }
            }

            // Show a tray notification if the application is consuming too much memory
            if(action == "Memory Warning")
            {
                IconNotify.Visible = true;
                IconNotify.ShowBalloonTip(
                    25000,
                    "HovText is consuming more than 500 MB memory",
                    "Please reduce the amount of images. View \"HovText Settings\" for more details.",
                    ToolTipIcon.Warning
                    );
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
                    IconNotify.Text = "HovText (" + entries + " entry)";
                }
                else
                {
                    IconNotify.Text = "HovText (" + entries + " entries)";
                }
            }
            else
            {
                IconNotify.Text = "HovText";
            }
        }


        // ###########################################################################################
        // Donate
        // ###########################################################################################

        private void PictureBox1_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.paypal.com/donate?hosted_button_id=U23UUA8YWABGU");
            Logging.Log("Clicked the \"Donate\" picture in \"About\"");
        }


        // ###########################################################################################
        // Get custom color inputs
        // ###########################################################################################

        private void GuiCustomHeader_Enter(object sender, EventArgs e)
        {
            ColorDialogHeader.Color = UiColorsButtonHeaderBackground.FillColor;
            ColorDialogHeader.FullOpen = true;
            if (ColorDialogHeader.ShowDialog() == DialogResult.OK)
            {
                string color = String.Format("#{0:X2}{1:X2}{2:X2}", ColorDialogHeader.Color.R, ColorDialogHeader.Color.G, ColorDialogHeader.Color.B);
                SetRegistryKey(registryPath, "HistoryColorCustomHeader", color);
                historyColorsHeader["Custom"] = color;
                UiColorsButtonHeaderBackground.FillColor = ColorDialogHeader.Color;
                SetHistoryColors();
            }
            ActiveControl = null; // do not focus this form - so we can reclick the textbox again right after
        }

        private void GuiCustomHeaderText_Enter(object sender, EventArgs e)
        {
            ColorDialogHeaderText.Color = UiColorsButtonHeaderText.FillColor;
            ColorDialogHeaderText.FullOpen = true;
            if (ColorDialogHeaderText.ShowDialog() == DialogResult.OK)
            {
                string color = String.Format("#{0:X2}{1:X2}{2:X2}", ColorDialogHeaderText.Color.R, ColorDialogHeaderText.Color.G, ColorDialogHeaderText.Color.B);
                SetRegistryKey(registryPath, "HistoryColorCustomHeaderText", color);
                historyColorsHeaderText["Custom"] = color;
                UiColorsButtonHeaderText.FillColor = ColorDialogHeaderText.Color;
                SetHistoryColors();
            }
            ActiveControl = null; // do not focus this form - so we can reclick the textbox again right after
        }

        private void GuiCustomSearch_Enter(object sender, EventArgs e)
        {
            ColorDialogSearch.Color = UiColorsButtonSearchBackground.FillColor;
            ColorDialogSearch.FullOpen = true;
            if (ColorDialogSearch.ShowDialog() == DialogResult.OK)
            {
                string color = String.Format("#{0:X2}{1:X2}{2:X2}", ColorDialogSearch.Color.R, ColorDialogSearch.Color.G, ColorDialogSearch.Color.B);
                SetRegistryKey(registryPath, "HistoryColorCustomSearch", color);
                historyColorsSearch["Custom"] = color;
                UiColorsButtonSearchBackground.FillColor = ColorDialogSearch.Color;
                SetHistoryColors();
            }
            ActiveControl = null; // do not focus this form - so we can reclick the textbox again right after
        }

        private void GuiCustomSearchText_Enter(object sender, EventArgs e)
        {
            ColorDialogSearchText.Color = UiColorsButtonSearchText.FillColor;
            ColorDialogSearchText.FullOpen = true;
            if (ColorDialogSearchText.ShowDialog() == DialogResult.OK)
            {
                string color = String.Format("#{0:X2}{1:X2}{2:X2}", ColorDialogSearchText.Color.R, ColorDialogSearchText.Color.G, ColorDialogSearchText.Color.B);
                SetRegistryKey(registryPath, "HistoryColorCustomSearchText", color);
                historyColorsSearchText["Custom"] = color;
                UiColorsButtonSearchText.FillColor = ColorDialogSearchText.Color;
                SetHistoryColors();
            }
            ActiveControl = null; // do not focus this form - so we can reclick the textbox again right after
        }

        private void GuiCustomActive_Enter(object sender, EventArgs e)
        {
            ColorDialogActive.Color = UiColorsLabelActive.BackColor;
            ColorDialogActive.FullOpen = true;
            if (ColorDialogActive.ShowDialog() == DialogResult.OK)
            {
                string color = String.Format("#{0:X2}{1:X2}{2:X2}", ColorDialogActive.Color.R, ColorDialogActive.Color.G, ColorDialogActive.Color.B);
                SetRegistryKey(registryPath, "HistoryColorCustomActive", color);
                historyColorsActive["Custom"] = color;
                UiColorsButtonActiveBackground.FillColor = ColorDialogActive.Color;
                SetHistoryColors();
            }
            ActiveControl = null; // do not focus this form - so we can reclick the textbox again right after
        }

        private void GuiCustomActiveText_Enter(object sender, EventArgs e)
        {
            ColorDialogActiveText.Color = UiColorsLabelActive.BackColor;
            ColorDialogActiveText.FullOpen = true;
            if (ColorDialogActiveText.ShowDialog() == DialogResult.OK)
            {
                string color = String.Format("#{0:X2}{1:X2}{2:X2}", ColorDialogActiveText.Color.R, ColorDialogActiveText.Color.G, ColorDialogActiveText.Color.B);
                SetRegistryKey(registryPath, "HistoryColorCustomActiveText", color);
                historyColorsActiveText["Custom"] = color;
                UiColorsButtonActiveText.FillColor = ColorDialogActiveText.Color;
                SetHistoryColors();
            }
            ActiveControl = null; // do not focus this form - so we can reclick the textbox again right after
        }

        private void GuiCustomEntry_Enter(object sender, EventArgs e)
        {
            ColorDialogEntry.Color = UiColorsLabelEntry.BackColor;
            ColorDialogEntry.FullOpen = true;
            if (ColorDialogEntry.ShowDialog() == DialogResult.OK)
            {
                string color = String.Format("#{0:X2}{1:X2}{2:X2}", ColorDialogEntry.Color.R, ColorDialogEntry.Color.G, ColorDialogEntry.Color.B);
                SetRegistryKey(registryPath, "HistoryColorCustomEntry", color);
                historyColorsEntry["Custom"] = color;
                UiColorsButtonEntryBackground.FillColor = ColorDialogEntry.Color;
                SetHistoryColors();
            }
            ActiveControl = null; // do not focus this form - so we can reclick the textbox again right after
        }

        private void GuiCustomEntryText_Enter(object sender, EventArgs e)
        {
            ColorDialogEntryText.Color = UiColorsLabelEntry.BackColor;
            ColorDialogEntryText.FullOpen = true;
            if (ColorDialogEntryText.ShowDialog() == DialogResult.OK)
            {
                string color = String.Format("#{0:X2}{1:X2}{2:X2}", ColorDialogEntryText.Color.R, ColorDialogEntryText.Color.G, ColorDialogEntryText.Color.B);
                SetRegistryKey(registryPath, "HistoryColorCustomEntryText", color);
                historyColorsEntryText["Custom"] = color;
                UiColorsButtonEntryText.FillColor = ColorDialogEntryText.Color;
                SetHistoryColors();
            }
            ActiveControl = null; // do not focus this form - so we can reclick the textbox again right after
        }

        private void GuiCustomBorder_Enter(object sender, EventArgs e)
        {
            ColorDialogBorder.Color = UiColorsButtonBorder.FillColor;
            ColorDialogBorder.FullOpen = true;
            if (ColorDialogBorder.ShowDialog() == DialogResult.OK)
            {
                string color = String.Format("#{0:X2}{1:X2}{2:X2}", ColorDialogBorder.Color.R, ColorDialogBorder.Color.G, ColorDialogBorder.Color.B);
                SetRegistryKey(registryPath, "HistoryColorCustomBorder", color);
                historyColorsBorder["Custom"] = color;
                UiColorsButtonBorder.FillColor = ColorDialogBorder.Color;
                UiColorsLabelActive.Refresh(); // update/redraw the border
                UiStyleLabelFont.Refresh(); // update/redraw the border
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
                    UiStyleLabelFont.Padding = new Padding(historyBorderThickness - 2);
                    UiColorsLabelActive.Padding = new Padding(historyBorderThickness - 2);
                    UiColorsLabelEntry.Padding = new Padding(historyBorderThickness - 2);
                }
                else
                {
                    UiStyleLabelFont.Padding = new Padding(historyBorderThickness - 1);
                    UiColorsLabelActive.Padding = new Padding(historyBorderThickness - 1);
                    UiColorsLabelEntry.Padding = new Padding(historyBorderThickness - 1);
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
            if (UiAdvancedToggleEnableLog.Checked)
            {
                // Get new status now, if logging is enabled or disabled
                isTroubleshootEnabled = UiAdvancedToggleEnableLog.Checked;

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
                isTroubleshootEnabled = UiAdvancedToggleEnableLog.Checked;
            }

            // Save it to the registry
            string status = UiAdvancedToggleEnableLog.Checked ? "1" : "0";
            SetRegistryKey(registryPath, "TroubleshootEnable", status);

            // If there is a logfile present then enable the UI fields for it
            if (File.Exists(pathAndLog))
            {
                UiAdvancedButtonDeleteLog.Enabled = true;
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
                UiAdvancedButtonDeleteLog.Enabled = false;
            }
            else
            {
                MessageBox.Show(pathAndLog + " does not exists!",
                    "WARNING",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                UiAdvancedButtonDeleteLog.Enabled = false;
            }
        }


        // ###########################################################################################
        // Troubleshooting, refresh the buttons for the logfile, if viewing the "Advanced" tab
        // ###########################################################################################

        private async void TabControl_Selected(object sender, TabControlEventArgs e)
        {
            // "Advanced" tab
            if (UiFormTabControl.SelectedTab.AccessibilityObject.Name == "Advanced")
            {
                if (File.Exists(pathAndLog))
                {
                    UiAdvancedButtonDeleteLog.Enabled = true;
                }
                else
                {
                    UiAdvancedButtonDeleteLog.Enabled = false;
                }
            }

            // "Feedback" tab
            if (UiFormTabControl.SelectedTab.AccessibilityObject.Name == "Feedback")
            {
                if (File.Exists(pathAndLog))
                {
                    FileInfo fileInfo = new FileInfo(pathAndLog);
                    if (fileInfo.Length > 0)
                    {
                        UiFeedbackToggleAttachLog.Enabled = true;
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
                            UiFeedbackToggleAttachLog.Text = "Attach troubleshooting logfile (" + fileSize.ToString() + " " + fileDescription + " - must not exceed 5MB)";
                            UiFeedbackToggleAttachLog.Enabled = false;
                            UiFeedbackToggleAttachLog.Checked = false;
                        }
                        else
                        {
                            UiFeedbackToggleAttachLog.Text = "Attach troubleshooting logfile (" + fileSize.ToString() + " " + fileDescription + ")";
                            UiFeedbackToggleAttachLog.Enabled = true;
                        }
                    }
                    else
                    {
                        UiFeedbackToggleAttachLog.Enabled = false;
                        UiFeedbackToggleAttachLog.Text = "Attach troubleshooting logfile (file is empty)";
                    }
                }
                else
                {
                    UiFeedbackToggleAttachLog.Enabled = false;
                    UiFeedbackToggleAttachLog.Text = "Attach troubleshooting logfile (file does not exists)";
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
            cleanupApp = true;

            Close();
        }


        // ###########################################################################################
        // Send feedback to the developer
        // ###########################################################################################

        private void GuiSendFeedback_Click(object sender, EventArgs e)
        {
            string email = UiFeedbackTextBoxEmail.Text;
            string feedback = UiFeedbackTextBoxFeedback.Text;

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
                    webClient.Headers.Add("user-agent", ("HovText " + UiFormLabelApplicationVersion.Text).Trim());

                    // Build the data to send
                    string textFilePath = pathAndLog;
                    string textFileContent = File.ReadAllText(textFilePath);
                    var data = new NameValueCollection
                        {
                            { "version", UiFormLabelApplicationVersion.Text },
                            { "email", email }
                        };
                    if (UiFeedbackToggleAttachLog.Checked)
                    {
                        data["attachment"] = textFileContent;
                    }
                    data["feedback"] = feedback;

                    // Send it to the server
                    var response = webClient.UploadValues(hovtextPage + "/contact/sendmail/", "POST", data);
                    string resultFromServer = Encoding.UTF8.GetString(response);
                    if (resultFromServer == "Success")
                    {
                        UiFeedbackTextBoxEmail.Text = "";
                        UiFeedbackTextBoxFeedback.Text = "";
                        UiFeedbackToggleAttachLog.Checked = false;
                        UiFeedbackButtonSubmit.Enabled = false;
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
            if (UiFeedbackToggleAttachLog.Checked || UiFeedbackTextBoxFeedback.Text.Length > 0)
            {
                UiFeedbackButtonSubmit.Enabled = true;
            }
            else
            {
                UiFeedbackButtonSubmit.Enabled = false;
            }
        }

        private void GuiAttachFile_CheckedChanged(object sender, EventArgs e)
        {
            if (UiFeedbackToggleAttachLog.Checked || UiFeedbackTextBoxFeedback.Text.Length > 0)
            {
                UiFeedbackButtonSubmit.Enabled = true;
            }
            else
            {
                UiFeedbackButtonSubmit.Enabled = false;
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
            UiAdvancedButtonAutoInstall.Enabled = false;
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
            UiLayoutGroupBoxShowClipboardListOn.Controls.Clear();

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
            int offsetSpaceHorizontalRadio = 18;
            int offsetSpaceHorizontalText = 27;
            int offsetSpaceVertical = 22;
            int dividerSpace = 32;
            for (int i = 0; i < numDisplays; i++)
            {
                // Check if this display number is the main/primary one
                bool isDisplayMain = Screen.AllScreens[i].Primary;

                // Build a new radio UI element
                Guna2CustomRadioButton display = new Guna2CustomRadioButton
                {
                    Name = "uiScreen" + i,
                    Tag = i,
                    Location = new Point(offsetSpaceHorizontalRadio, offsetSpaceVertical + dividerSpace),
                    Enabled = numDisplays > 1,
                    CheckedState = { FillColor = Color.FromArgb(56, 97, 55) },
                };

                // Build the corresponding label to it
                Label label = new Label
                {
                    Name = "uiScreenLabel" + i,
                    Tag = i,
                    Text = isDisplayMain ? "Display " + (i + 1) + " (Main)" : "Display " + (i + 1),
                    AutoSize = true,
                    Location = new Point(offsetSpaceHorizontalRadio + offsetSpaceHorizontalText, offsetSpaceVertical + dividerSpace),
                    Enabled = numDisplays > 1,
                    Cursor = Cursors.Hand,
                };
                label.Font = new Font(label.Font.FontFamily, 10);
                label.Click += DisplayLabel_Click;

                display.CheckedChanged += new EventHandler(GuiDisplayGroup_Changed);
                UiLayoutGroupBoxShowClipboardListOn.Controls.Add(display);
                UiLayoutGroupBoxShowClipboardListOn.Controls.Add(label);
                dividerSpace += 31;
            }
        }

        private void DisplayLabel_Click(object sender, EventArgs e)
        {
            if (sender is Label clickedLabel && clickedLabel.Tag is int labelTag)
            {
                string radioButtonName = "uiScreen" + labelTag;
                foreach (Control ctrl in UiLayoutGroupBoxShowClipboardListOn.Controls) 
                {
                    if (ctrl is Guna2CustomRadioButton radioButton && radioButton.Name == radioButtonName)
                    {
                        radioButton.Checked = true;
                        break; // break the loop, once the correct button is found
                    }
                }
            }
        }


        // ###########################################################################################
        // Catch event when changing the display
        // Main contributor: FNI
        // ###########################################################################################

        private void GuiDisplayGroup_Changed(object sender, EventArgs e)
        {
            Guna2CustomRadioButton displaySelectedTag = (Guna2CustomRadioButton)sender;
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
            UiLayoutGroupBoxShowClipboardListOn.Controls["uiScreen" + activeDisplay].Select();

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
            UiStyleLabelBorderText.Text = UiStyleTrackBarBorder.Value.ToString() + "px";
            historyBorderThickness = UiStyleTrackBarBorder.Value;
            SetRegistryKey(registryPath, "HistoryBorderThickness", historyBorderThickness.ToString());
            UiColorsLabelActive.Refresh(); // update/redraw the border
            UiStyleLabelFont.Refresh(); // update/redraw the border
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
                settings.TimerTerminate.Enabled = true;

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
                            IconNotify.Icon = Resources.Square_Old_Hotkey_48x48;
                            base.Icon = Resources.Square_Old_Hotkey_48x48;
                            UiFormPictureBoxIcon.Image = Bitmap.FromHicon(Resources.Square_Old_Hotkey_48x48.Handle);
                        }
                        else
                        {
                            IconNotify.Icon = Resources.Square_Old_Inactive_48x48;
                            base.Icon = Resources.Square_Old_Inactive_48x48;
                            UiFormPictureBoxIcon.Image = Bitmap.FromHicon(Resources.Square_Old_Inactive_48x48.Handle);
                        }
                    }
                    else if (iconSet == "SquareNew")
                    {
                        if (isApplicationEnabled)
                        {
                            IconNotify.Icon = Resources.Square_New_Hotkey_Edited_48x48;
                            base.Icon = Resources.Square_New_Hotkey_Edited_48x48;
                            UiFormPictureBoxIcon.Image = Bitmap.FromHicon(Resources.Square_New_Hotkey_Edited_48x48.Handle);
                        }
                        else
                        {
                            IconNotify.Icon = Resources.Square_New_Inactive_Edited_48x48;
                            base.Icon = Resources.Square_New_Inactive_Edited_48x48;
                            UiFormPictureBoxIcon.Image = Bitmap.FromHicon(Resources.Square_New_Inactive_Edited_48x48.Handle);
                        }
                    }
                    else
                    {
                        if (isApplicationEnabled)
                        {
                            IconNotify.Icon = Resources.Round_Hotkey_48x48;
                            base.Icon = Resources.Round_Hotkey_48x48;
                            UiFormPictureBoxIcon.Image = Bitmap.FromHicon(Resources.Round_Hotkey_48x48.Handle);
                        }
                        else
                        {
                            IconNotify.Icon = Resources.Round_Inactive_48x48;
                            base.Icon = Resources.Round_Inactive_48x48;
                            UiFormPictureBoxIcon.Image = Bitmap.FromHicon(Resources.Round_Inactive_48x48.Handle);
                        }
                    }
                    break;

                default:
                    if (iconSet == "SquareOld")
                    {
                        if (isApplicationEnabled)
                        {
                            IconNotify.Icon = Resources.Square_Old_Active_48x48;
                            base.Icon = Resources.Square_Old_Active_48x48;
                            UiFormPictureBoxIcon.Image = Bitmap.FromHicon(Resources.Square_Old_Active_48x48.Handle);
                        }
                        else
                        {
                            IconNotify.Icon = Resources.Square_Old_Inactive_48x48;
                            base.Icon = Resources.Square_Old_Inactive_48x48;
                            UiFormPictureBoxIcon.Image = Bitmap.FromHicon(Resources.Square_Old_Inactive_48x48.Handle);
                        }
                    }
                    else if (iconSet == "SquareNew")
                    {
                        if (isApplicationEnabled)
                        {
                            IconNotify.Icon = Resources.Square_New_Active_Edited_48x48;
                            base.Icon = Resources.Square_New_Active_Edited_48x48;
                            UiFormPictureBoxIcon.Image = Bitmap.FromHicon(Resources.Square_New_Active_Edited_48x48.Handle);
                        }
                        else
                        {
                            IconNotify.Icon = Resources.Square_New_Inactive_Edited_48x48;
                            base.Icon = Resources.Square_New_Inactive_Edited_48x48;
                            UiFormPictureBoxIcon.Image = Bitmap.FromHicon(Resources.Square_New_Inactive_Edited_48x48.Handle);
                        }
                    }
                    else
                    {
                        if (isApplicationEnabled)
                        {
                            IconNotify.Icon = Resources.Round_Active_48x48;
                            base.Icon = Resources.Round_Active_48x48;
                            UiFormPictureBoxIcon.Image = Bitmap.FromHicon(Resources.Round_Active_48x48.Handle);
                        }
                        else
                        {
                            IconNotify.Icon = Resources.Round_Inactive_48x48;
                            base.Icon = Resources.Round_Inactive_48x48;
                            UiFormPictureBoxIcon.Image = Bitmap.FromHicon(Resources.Round_Inactive_48x48.Handle);
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
            UiAdvancedLabelDevVersion.Text = "Please wait ...";

            UiAdvancedPicture1BoxDevRefresh.Visible = true;
            UiAdvancedPicture2BoxDevRefresh.Visible = false;

            // Check for a new development version
            try
            {
                WebClient webClient = new WebClient();
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
                webClient.Headers.Add("user-agent", ("HovText " + UiFormLabelApplicationVersion.Text).Trim());
                string checkedVersion = webClient.DownloadString(hovtextPage + "/autoupdate/development/");
                if (checkedVersion.Substring(0, 7) == "Version")
                {
                    if (checkedVersion != "Version: No development version available")
                    {
                        checkedVersion = checkedVersion.Substring(9);
                        UiAdvancedLabelDevVersion.Text = checkedVersion;
                        UiAdvancedButtonManualDownload.Enabled = true;
                        UiAdvancedButtonAutoInstall.Enabled = true;
                        Logging.Log("  Development version available = [" + checkedVersion + "]");
                    }
                    else
                    {
                        UiAdvancedButtonManualDownload.Enabled = false;
                        UiAdvancedButtonAutoInstall.Enabled = false;
                        UiAdvancedLabelDisclaimer.Enabled = false;
                        UiAdvancedLabelDevVersion.Text = "No development version available";
                        Logging.Log("  Development version available = [No development version available]");
                    }
                }
                else
                {

                    UiAdvancedButtonManualDownload.Enabled = false;
                    UiAdvancedButtonAutoInstall.Enabled = false;
                    UiAdvancedLabelDisclaimer.Enabled = false;
                    UiAdvancedLabelDevVersion.Text = "ERROR";
                    Logging.Log("  Development version available = [ERROR]");
                }
                UiAdvancedPicture1BoxDevRefresh.Visible = false;
                UiAdvancedPicture2BoxDevRefresh.Visible = true;

            }
            catch (WebException ex)
            {
                // Catch the exception though this is not so critical that we need to disturb the developer
                Logging.Log("Exception raised (Settings):");
                Logging.Log("  Cannot connect with server to get information about newest available [DEVELOPMENT] version:");
                Logging.Log("  " + ex.Message);
                UiAdvancedLabelDevVersion.Text = "Cannot connect with server - retry later";
            }
        }


        // ###########################################################################################
        // Check if the logfile is too large
        // ###########################################################################################

        private void CheckIfLogfileIsTooLarge()
        {
            // Only check the logfile size, if we have active logging
            if(UiAdvancedToggleEnableLog.Checked)
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
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            CheckIfLogfileIsTooLarge();
        }


        // ###########################################################################################
        // Disable troubleshoot logging
        // ###########################################################################################

        public void UpdateTroubleshootDisabled()
        {
            UiAdvancedToggleEnableLog.Checked = false;
        }


        // ###########################################################################################
        // Update amount of entries to save to file
        // ###########################################################################################

        private void GuiStorageEntries_ValueChanged(object sender, EventArgs e)
        {
            UiStorageTrackBarEntriesToSaveText.Text = UiStorageTrackBarEntriesToSave.Value.ToString();
            SetRegistryKey(registryPath, "StorageSaveEntries", UiStorageTrackBarEntriesToSaveText.Text);
        }


        // ###########################################################################################
        // Update if we should load the clipboard on application launch or not
        // ###########################################################################################

        private void GuiStorageLoadClipboard_CheckedChanged(object sender, EventArgs e)
        {
            string status = UiStorageToggleLoadClipboards.Checked ? "1" : "0";
            SetRegistryKey(registryPath, "StorageLoadOnLaunch", status);
        }


        // ###########################################################################################
        // Update which type of clipboards we should save
        // ###########################################################################################

        private void GuiStorageChoose_CheckedChanged(object sender, EventArgs e)
        {
            string set;

            if (UiStorageRadioSaveAll.Checked)
            {
                set = "All";
            } else if (UiStorageRadioSaveOnlyFavorites.Checked) { 
                set = "Favorites";
            } else {
                set = "Text";
            }

            SetRegistryKey(registryPath, "StorageSaveType", set);
        }


        // ###########################################################################################
        // Update which clipboard type we should save
        // ###########################################################################################

        private void GuiStorageSaveClipboard_CheckedChanged(object sender, EventArgs e)
        {
            if (UiStorageToggleSaveClipboards.Checked)
            {
                UiStorageRadioSaveOnlyText.Enabled = true;
                UiStorageRadioSaveAll.Enabled = true;
                UiStorageTrackBarEntriesToSave.Enabled = true;
                if (UiStorageToggleSaveClipboards.Checked)
                {
                    if (isEnabledFavorites)
                    {
                        UiStorageRadioSaveOnlyFavorites.Enabled = true;
                    }
                    else
                    {
                        UiStorageRadioSaveOnlyFavorites.Enabled = false;
                    }
                }
                SetRegistryKey(registryPath, "StorageSaveOnExit", "1");
            }
            else
            {
                UiStorageRadioSaveOnlyText.Enabled = false;
                UiStorageRadioSaveAll.Enabled = false;
                UiStorageRadioSaveOnlyFavorites.Enabled = false;
                UiStorageTrackBarEntriesToSave.Enabled = false;
                SetRegistryKey(registryPath, "StorageSaveOnExit", "0");
            }
        }


        // ###########################################################################################
        // Handle the "Advanced Status" - e.g. memory usage and total amount of entries
        // ###########################################################################################

        // ###########################################################################################
        // Log the amount of memory used
        // -----------------------------
        // Quote ChatGPT:
        // Process.WorkingSet64: Gives a more comprehensive view, including both managed
        // and unmanaged memory, but it represents the memory used by the entire process,
        // which may include overhead from the .NET runtime and other libraries.
        // ###########################################################################################


        private void LogMemoryConsumed()
        {
            if(memoryInMB != old_memoryInMB)
            {
                Logging.Log("Memory consumption here-and-now: [" + memoryInMB + "] MB");
            }
            old_memoryInMB = memoryInMB;
        }

        public void UpdateStorageInfo()
        {
            // In "Storage" tab and the the "Info" GroupBox
            if (memoryInMB > 500)
            {
                if (!shownMemoryWarning)
                {
                    UiStorageGroupBoxInfo.CustomBorderColor = Color.IndianRed;
                    UiStorageGroupBoxInfo.ForeColor = Color.White;
                    UiStorageGroupBoxInfo.Text = "Info - Warning";
                    guna2HtmlLabel2.ForeColor = Color.FromArgb(64, 64, 64);
                    guna2HtmlLabel2.Text = "The high memory consumption indicates there <i>might</i> be a risk that the clipboard data cannot be saved in due time at computer shutdown or reboot! This is because it can take several seconds to save this much data to disk and Windows will enforce application termination when doing a Windows shutdown or reboot, if it takes too long.<br /><br />Please <b>reduce the amount of image clipboards</b>.";
                    ShowTrayNotifications("Memory Warning");
                    shownMemoryWarning = true;
                }
            }
            else
            {
                UiStorageGroupBoxInfo.CustomBorderColor = Color.FromArgb(220, 227, 220);
                UiStorageGroupBoxInfo.ForeColor = Color.FromArgb(64, 64, 64);
                UiStorageGroupBoxInfo.Text = "Info";
                guna2HtmlLabel2.Text = "Storage is per default configured to save only text entries, as you need to know that storing many image clipboards can cripple the responsiveness of the UI and consume a huge amount of memory! You are encouraged to test it, but do set a reasonable (low) amount of entries to save, if you include images. This of course fully depends on your computer configuration and resources.";
            }
        }

        private void UpdateAdvancedStatus()
        {
            
            string formattedMemoryUsage = "Memory usage: " + memoryInMB + " MB";
            UiAdvancedLabelMemUsed.Text = formattedMemoryUsage;

            if (!isClipboardLoadingFromFile)
            {    
                UiAdvancedLabelClipboardEntries.Text = "Clipboard entries: " + entriesOriginal.Count.ToString();
                int countImages = entriesIsImage.Count(entry => entry.Value == true);
                int countTrue = entriesIsFavorite.Count(entry => entry.Value == true);
                int countAll = entriesIsFavorite.Count();
                int countText = countAll - countImages;
                if (countText == 1)
                {
                    UiStorageLabelSaveOnlyTextEntries.Text = "(" + countText + " entry)";
                }
                else
                {
                    UiStorageLabelSaveOnlyTextEntries.Text = "(" + countText + " entries)";
                }

                if (countTrue == 1)
                {
                    UiStorageLabelSaveOnlyFavoritesEntries.Text = "("+ countTrue +" entry)";
                } else
                {
                    UiStorageLabelSaveOnlyFavoritesEntries.Text = "(" + countTrue + " entries)";
                }
                if (countAll == 1)
                {
                    UiStorageLabelSaveAllEntries.Text = "(" + countAll + " entry)";
                }
                else
                {
                    UiStorageLabelSaveAllEntries.Text = "(" + countAll + " entries)";
                }                  
            }
        }     


        // ###########################################################################################
        // Clear clipboard history
        // ###########################################################################################

        private void GuiClearHistory_Click(object sender, EventArgs e)
        {
            int count = entriesOriginal.Count;
            Logging.Log($"Cleaning all [" + count + "] clipboard entries from memory");

            if (File.Exists(pathAndData))
            {
                File.Delete(@pathAndData);
                Logging.Log($"Deleted the clipboard data file ["+ pathAndData +"]");
            }

            ClearHistory();
            UpdateNotifyIconText();

            // Trigger the garbage collector to see the impact immediately
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }


        // ###########################################################################################
        // Handling the "MouseDown" even on the top-banner elements
        // ###########################################################################################

        private void TopBannerPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, 0xA1, 0x2, 0);
            }
        }


        // ###########################################################################################
        // Clicking the labels in the "General" tab
        // ###########################################################################################

        private void UiGeneralLabelEnableClipboard_Click(object sender, EventArgs e)
        {
            UiGeneralToggleEnableClipboard.Checked = !UiGeneralToggleEnableClipboard.Checked;
        }

        private void UiGeneralLabelEnableFavorites_Click(object sender, EventArgs e)
        {
            UiGeneralToggleEnableFavorites.Checked = !UiGeneralToggleEnableFavorites.Checked;
        }

        private void UiGeneralLabelIncludeImages_Click(object sender, EventArgs e)
        {
            UiGeneralToggleIncludeImages.Checked = !UiGeneralToggleIncludeImages.Checked;
        }

        private void UiGeneralLabelPasteToApplication_Click(object sender, EventArgs e)
        {
            UiGeneralTogglePasteToApplication.Checked = !UiGeneralTogglePasteToApplication.Checked;
        }

        private void UiGeneralLabelAlwaysPasteOriginal_Click(object sender, EventArgs e)
        {
            UiGeneralToggleAlwaysPasteOriginal.Checked = !UiGeneralToggleAlwaysPasteOriginal.Checked;
        }

        private void UiGeneralLabelStartWithWindows_Click(object sender, EventArgs e)
        {
            UiGeneralToggleStartWithWindows.Checked = !UiGeneralToggleStartWithWindows.Checked;
        }

        private void UiGeneralLabelStartDisabled_Click(object sender, EventArgs e)
        {
            UiGeneralToggleStartDisabled.Checked = !UiGeneralToggleStartDisabled.Checked;
        }

        private void UiGeneralLabelRestoreOriginal_Click(object sender, EventArgs e)
        {
            UiGeneralToggleRestoreOriginal.Checked = !UiGeneralToggleRestoreOriginal.Checked;
        }

        private void UiGeneralLabelTrimWhitespaces_Click(object sender, EventArgs e)
        {
            UiGeneralToggleTrimWhitespaces.Checked = !UiGeneralToggleTrimWhitespaces.Checked;
        }

        private void UiGeneralLabelCloseMinimizes_Click(object sender, EventArgs e)
        {
            UiGeneralToggleCloseMinimizes.Checked = !UiGeneralToggleCloseMinimizes.Checked;
        }


        // ###########################################################################################
        // Clicking the labels in the "Save clipboards at exit" and "Load clipboards at startup"
        // ###########################################################################################

        private void UiStorageLabelSaveClipboards_Click(object sender, EventArgs e)
        {
            UiStorageToggleSaveClipboards.Checked = !UiStorageToggleSaveClipboards.Checked;
        }

        private void UiStorageLabelLoadClipboards_Click(object sender, EventArgs e)
        {
            UiStorageToggleLoadClipboards.Checked = !UiStorageToggleLoadClipboards.Checked;
        }


        // ###########################################################################################
        // Clicking the labels in the "Which clipboards to save" group
        // ###########################################################################################

        private void UiStorageLabelSaveOnlyText_Click(object sender, EventArgs e)
        {
            UiStorageRadioSaveOnlyText.Checked = true;
        }

        private void UiStorageLabelSaveOnlyFavorites_Click(object sender, EventArgs e)
        {
            UiStorageRadioSaveOnlyFavorites.Checked = true;
        }

        private void UiStorageLabelSaveAll_Click(object sender, EventArgs e)
        {
            UiStorageRadioSaveAll.Checked = true;
        }


        // ###########################################################################################
        // Clicking the labels in the "Paste behaviour" group
        // ###########################################################################################

        private void UiHotkeysLabelUseStandardWindowsMethod_Click(object sender, EventArgs e)
        {
            UiHotkeysRadioUseStandardWindowsMethod.Checked = true;
        }

        private void UiHotkeysLabelPasteOnHotkey_Click(object sender, EventArgs e)
        {
            UiHotkeysRadioPasteOnHotkey.Checked = true;

        }


        // ###########################################################################################
        // Repaint the form border
        // ###########################################################################################

        protected override void OnPaint(PaintEventArgs e)
        {
            int cornerRadius = 10;

            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Create a path for the rounded rectangle
            GraphicsPath path = new GraphicsPath();
            path.AddArc(0, 0, cornerRadius, cornerRadius, 180, 90);
            path.AddArc(this.Width - cornerRadius - borderWidth, 0, cornerRadius, cornerRadius, 270, 90);
            path.AddArc(this.Width - cornerRadius - borderWidth, this.Height - cornerRadius - borderWidth, cornerRadius, cornerRadius, 0, 90);
            path.AddArc(0, this.Height - cornerRadius - borderWidth, cornerRadius, cornerRadius, 90, 90);
            path.CloseAllFigures();

            // Set the form's region
            this.Region = new Region(path);

            // Draw the border
            using (Pen pen = new Pen(this.BackColor, borderWidth))
            {
                pen.Alignment = PenAlignment.Inset; // This is needed to draw inside the bounds of the form
                g.DrawPath(pen, path);
            }
        }


        // ###########################################################################################
        // Clicking the labels in the "Clipboard location" group
        // ###########################################################################################

        private void ClipboardLocation_Click(object sender, EventArgs e)
        {
            string text = "";
            if (sender is Control control)
            {
                text = control.Text;
            }
            switch (text)
            {
                case "Left Top":
                    UiLayoutRadioLeftTop.Checked = true;
                    break;
                case "Left Bottom":
                    UiLayoutRadioLeftBottom.Checked = true;
                    break;
                case "Center":
                    UiLayoutRadioCenter.Checked = true;
                    break;
                case "Right Top":
                    UiLayoutRadioRightTop.Checked = true;
                    break;
                case "Right Bottom":
                    UiLayoutRadioRightBottom.Checked = true;
                    break;
            }
        }


        // ###########################################################################################
        // Clicking the labels in the "Icon set to use" group
        // ###########################################################################################

        private void IconSet_Click(object sender, EventArgs e)
        {
            string text = "";
            if (sender is Control control)
            {
                text = control.Text;
            }
            switch (text)
            {
                case "Round, Modern":
                    UiStyleRadioIconRoundModern.Checked = true;
                    break;
                case "Square, Modern":
                    UiStyleRadioIconSquareModern.Checked = true;
                    break;
                case "Square, Original":
                    UiStyleRadioIconSquareOriginal.Checked = true;
                    break;
            }
        }


        // ###########################################################################################
        // Clicking the labels in the "Color Theme" group
        // ###########################################################################################

        private void ColorTheme_Click(object sender, EventArgs e)
        {
            string text = "";
            if (sender is Control control)
            {
                text = control.Text;
            }
            switch (text)
            {
                case "Blue":
                    UiColorsRadioBlue.Checked = true;
                    break;
                case "Brown":
                    UiColorsRadioBrown.Checked = true;
                    break;
                case "Green":
                    UiColorsRadioGreen.Checked = true;
                    break;
                case "Yellow":
                    UiColorsRadioYellow.Checked = true;
                    break;
                case "Contrast":
                    UiColorsRadioContrast.Checked = true;
                    break;
                case "Custom":
                    UiColorsRadioCustom.Checked = true;
                    break;
            }
        }


        // ###########################################################################################
        // Clicking the "Enable troubleshooting"
        // ###########################################################################################

        private void UiAdvancedLabelEnableLog_Click(object sender, EventArgs e)
        {
            UiAdvancedToggleEnableLog.Checked = !UiAdvancedToggleEnableLog.Checked;
        }


        // ###########################################################################################
        // Clicking the "Attach troubleshooting logfile"
        // ###########################################################################################

        private void UiFeedbackLabelAttachLog_Click(object sender, EventArgs e)
        {
            UiFeedbackToggleAttachLog.Checked = !UiFeedbackToggleAttachLog.Checked;
        }


        // ###########################################################################################
        // Clicking the large form application icon
        // ###########################################################################################

        private void UiFormPictureBoxIcon_Click(object sender, EventArgs e)
        {
            ToggleEnabled();
        }


        // ###########################################################################################
        // Memory consumption timer
        // ###########################################################################################

        private void TimerGetMemoryConsumption_Tick(object sender, EventArgs e)
        {
            GetMemoryConsumption();
            LogMemoryConsumed();
            UpdateStorageInfo();
            UpdateAdvancedStatus();
        }

        private void GetMemoryConsumption()
        {
            // Get the current memory usage
            Process currentProcess = Process.GetCurrentProcess();
            double tmp_memoryInMB = currentProcess.WorkingSet64 / 1024d / 1024d;
            memoryInMB = (int) Math.Round(tmp_memoryInMB);
        }


        // ###########################################################################################

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();


        // ###########################################################################################
    }
}
