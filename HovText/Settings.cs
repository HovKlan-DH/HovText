/*
##################################################################################################
SETTINGS (FORM)
---------------

This is the main form for the HovText application.

Upload the application (binary) to these places:
  https://www.microsoft.com/en-us/wdsi/filesubmission
  https://virustotal.com

NuGet: "Costura.Fody" to merge the DLLs in to the EXE - to get 
only one EXE file for this application. Incredible cool and 
simple compared to the other complex stuff I have seen! :-)
https://stackoverflow.com/a/40786196/2028935

##################################################################################################
*/

using Guna.UI2.WinForms; // https://gunaui.com/products/ui-winforms
using HovText.Properties;
using static HovText.Program;
using IWshRuntimeLibrary;
using Microsoft.Win32;
using NHotkey.WindowsForms; // https://github.com/thomaslevesque/NHotkey
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HovText
{
    public sealed partial class Settings : Form
    {

        // ###########################################################################################
        // Class variables
        // ###########################################################################################

        // History, default values
        public static string historyFontFamily = "Segoe UI";
        public static float historyFontSize = 11;
        public static int historyListElements = 6; // 1-30
        public static int historySizeWidth = 30; // percentage (10-100%)
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
        private const string registryStorageSaveEntries = "50"; // number of clipboard entries to save
        private const string registryCloseMinimizes = "1"; // 0 = terminates, 1 = minimize to tray
        private const string registryStartDisabled = "0"; // 0 = start active, 1 = start disabled
        private const string registryRestoreOriginal = "1"; // 1 = restore original
        private const string registryCopyImages = "1"; // 1 = copy images to history
        private const string registryHistorySearch = "1"; // 1 = enable history and Search
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
        public static bool isEnabledAlwaysPasteOriginal;
        public static bool isEnabledPasteOnHotkey;
        public static bool isEnabledTrimWhitespacing;
        public static bool isRestoreOriginal;
        public static bool isStartDisabled;
        public static bool isCopyImages;
        public static bool isClosedFromNotifyIcon;
        public static bool isHistoryMarginEnabled;
        public static bool isTroubleshootEnabled;
        public static bool isApplicationEnabled = true;

        // Clipboard
        public static int entryIndex = -1;
        public static int entryCounter = -1;
        public static SortedDictionary<int, string> entriesApplication = new SortedDictionary<int, string>();
        public static SortedDictionary<int, Image> entriesApplicationIcon = new SortedDictionary<int, Image>();
        public static SortedDictionary<int, string> entriesText = new SortedDictionary<int, string>();
        public static SortedDictionary<int, string> entriesTextTrimmed = new SortedDictionary<int, string>();
        public static SortedDictionary<int, bool> entriesShow = new SortedDictionary<int, bool>();
        public static SortedDictionary<int, bool> entriesIsFavorite = new SortedDictionary<int, bool>();
        public static SortedDictionary<int, bool> entriesIsUrl = new SortedDictionary<int, bool>();
        public static SortedDictionary<int, bool> entriesIsEmail = new SortedDictionary<int, bool>();
        public static SortedDictionary<int, bool> entriesIsImage = new SortedDictionary<int, bool>();
        public static SortedDictionary<int, bool> entriesIsTransparent = new SortedDictionary<int, bool>();
        public static SortedDictionary<int, Image> entriesImage = new SortedDictionary<int, Image>();
        public static SortedDictionary<int, Image> entriesImageTrans = new SortedDictionary<int, Image>();
        public static SortedDictionary<int, string> entriesChecksum = new SortedDictionary<int, string>();
        public static SortedList<int, Dictionary<string, object>> entriesOriginal = new SortedList<int, Dictionary<string, object>>();
        public static SortedDictionary<int, int> entriesOrder = new SortedDictionary<int, int>();
        const int WM_CLIPBOARDUPDATE = 0x031D;
        public static bool pasteOnHotkeySetCleartext; // used when "Paste behaviour" is set to "Paste only on hotkey" - when this one here is "true" then it should populate the clipboard with the cleartext-only (no formatting)
        public static List<int> clipboardSaveQueue = new List<int>(); // will contain index-number of entries to save
        private bool isClipboardSaveQueueBeingProcessed = false; // "true" when we are actively working on the "save queue"
        public static int clipboardEntriesToSave = 50;
        public static int maxClipboardEntriesToSave = 500;

        // Create instances of other classes
        readonly History history = new History();
        readonly Update update = new Update();
        readonly TooBigLogfile tooBigLogfile;
        readonly PasteOnHotkey pasteOnHotkey = new PasteOnHotkey();
        readonly HotkeyConflict hotkeyConflict = new HotkeyConflict();
        HandleClipboard clipboardHandler;

        // URLs
        public static string hovtextPage = "https://hovtext.com";
        public static string hovtextPageDownload = "https://hovtext.com/download";
        private static string hovTextGithub = "https://github.com/HovKlan-DH/HovText";
        private static string hovTextDonators = "https://hovtext.com/donators";

        // Paths and filenames
        public static string baseDirectory;
        public static string pathAndExe;
        public static string pathAndData;
        public static string pathAndDataLoad;
        public static string pathAndDataFavorite;
        public static string pathAndDataFavoriteLoad;
        public static string pathAndDataIndex;
        public static string pathAndDataIndexLoad;
        public static string pathAndLog;
        public static string pathAndSpecial;
        public static readonly string dataName = "HovText-data-";
        public static readonly string dataFavoriteName = "HovText-favorite-";
        public static readonly string dataIndexName = "HovText-index-";
        readonly string troubleshootLog = "HovText-troubleshooting.txt";
        readonly string saveContentFileExist = "HovText-save-content-in-logfile.txt"; // should ONLY be used by Dennis/developer for debugging!!!
        public static string updateExe = "HovText Auto-Install.exe";
        private static string exeOnly;
        static string exeFileNameWithPath;
        public static string exeFileNameWithoutExtension;

        // Misc
        public static string appVer = "";
//        public static bool isFirstCallAfterHotkey = true;
        public static bool isSettingsFormVisible;
        internal static Settings settings;
        public static bool hasTroubleshootLogged;
        private static bool cleanupApp = false;
        private static string originatingApplicationName = "";
        public static int activeDisplay; // selected display to show the history (default will be the main display)
        private static string hotkey; // needed for validating the keys as it is not set in the event
        public static string osVersion;
        private static bool firstTimeLaunch;
        public static string buildType = ""; // Debug, Release
        public static byte[] encryptionKey;
        public static byte[] encryptionInitializationVector;
        bool isStartingUp = false;
        private bool isClosing = false;
        private static int memoryInMB = 0;
        private int old_memoryInMB = -1;
        int borderWidth = 1;
        public static bool hasWriteAccess;
        private bool floppyToggle = false;
        static string checkedVersion = "";
        bool hasCheckedForUpdate = false;
        public static bool isProcessingClipboardQueue = false;
        bool saveFilesActive = false;
        bool activatedHotkeys = false; // the hotkeys should not be enabled until we have loaded everything into the clipboard list
        private DateTime lastClipboardEvent = DateTime.MinValue;


        // ###########################################################################################
        // Form initialization
        // ###########################################################################################

        public Settings()
        {
            isStartingUp = true;

            // Setup form and all elements
            InitializeComponent();
            Padding = new Padding(borderWidth);

            clipboardHandler = new HandleClipboard(this);


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

            // Get todays date and time (for filename)
            string dt = GetCurrentDateTimeFormatted();

            // Get paths and files
            baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            exeOnly = System.AppDomain.CurrentDomain.FriendlyName;
            pathAndExe = Path.Combine(baseDirectory, exeOnly);
            pathAndData = Path.Combine(baseDirectory, dataName + dt + ".bin");
            pathAndDataFavorite = Path.Combine(baseDirectory, dataFavoriteName + dt + ".bin");
            pathAndDataIndex = Path.Combine(baseDirectory, dataIndexName + dt + ".bin");
            pathAndLog = Path.Combine(baseDirectory, troubleshootLog);
            pathAndSpecial = Path.Combine(baseDirectory, saveContentFileExist); // should ONLY be used by Dennis!

            // Get the name for this HovText executable (it may not be "HovText.exe")
            exeFileNameWithPath = Process.GetCurrentProcess().MainModule.FileName;
            exeFileNameWithoutExtension = Path.GetFileNameWithoutExtension(exeFileNameWithPath);

            // Get if we have write access
            hasWriteAccess = HandleFiles.HasWriteAccess();

            // Start logging, if relevant
            Logging.StartLogging();
            hasTroubleshootLogged = isTroubleshootEnabled;

            // Instantiate the "TooBigLogfile" form - it will be shown later, if needed
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
            }

            // Refering to the current form - used in the history form
            settings = this;

            // Catch repaint event for this specific element (to draw the border)
            UiColorsLabelActive.Paint += new PaintEventHandler(GuiShowFontBottom_Paint);
            UiStyleLabelFont.Paint += new PaintEventHandler(GuiShowFontBottom_Paint);

            // Catch display change events (e.g. add/remove displays or change of main display)
            SystemEvents.DisplaySettingsChanged += new EventHandler(DisplayChangesEvent);

            // Initialize registry and get its values for the various checkboxes
            ConvertLegacyRegistry();
            InitializeRegistry();
            GetStartupSettings();

            // Do not at all show the form, if it should start minimized
            if (StartMinimized)
            {
                WindowState = FormWindowState.Minimized;
                ShowInTaskbar = false;
            }

            // Should we start in "disabled" mode?
            if (isStartDisabled)
            {
                ToggleEnabled();
            }

            // Set the notify icon
            SetNotifyIcon();

            UiColorsLabelActive.Text = "Active entry\r\nLine 2\r\nLine 3\r\nLine 4\r\nLine 5\r\nLine 6\r\nLine 7";
            UiColorsLabelEntry.Text = "Entry\r\nLine 2\r\nLine 3\r\nLine 4\r\nLine 5\r\nLine 6\r\nLine 7";

            // Set the initial text on the tray icon
            UpdateNotifyIconText();

            // Update the location of the dynamic labels
            UiGeneralLabelEnableClipboardShortcut.Text = "(" + UiHotkeysButtonSearch.Text + ")";

            // Catch "MouseDown" events for moving the application window
            UiFormPanel.MouseDown += new MouseEventHandler(TopBannerPanel_MouseDown);
            UiFormLabelApplicationName.MouseDown += new MouseEventHandler(TopBannerPanel_MouseDown);
            UiFormLabelApplicationVersion.MouseDown += new MouseEventHandler(TopBannerPanel_MouseDown);
            pictureBox1.MouseDown += new MouseEventHandler(TopBannerPanel_MouseDown);
            pictureBox2.MouseDown += new MouseEventHandler(TopBannerPanel_MouseDown);

            // Update meory status and clipboard entries in "Advanced" tab
            GetMemoryConsumption();
            LogMemoryConsumed();
            UpdateMemoryConsumed();
            UpdateClipboardEntriesCounters();

            // Start "GetMemoryConsumption" timer
            TimerGetMemoryConsumption.Start();

            isStartingUp = false;

            // Start the loading process of data files AFTER the UI form has been fully intialized and shown
            Shown += new EventHandler(Settings_Shown);

            // Create/overwrite shortcut in "Start Menu"
            CreateShortcut("HovText", @pathAndExe, "HovText Clipboard Manager");
        }


        // ###########################################################################################
        // Catch window/form messages from Windows
        // ###########################################################################################

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                // Catch the clipboard chain [UPDATE] event
                case WM_CLIPBOARDUPDATE:

                    Logging.Log("Received clipboard [UPDATE] event");

                    // Only capture the clipboard, if HovText is enabled
                    if (isApplicationEnabled)
                    {

                        // Do not process clipboard, if the update is faster than ???ms
                        int minMsBetween = 100;
                        DateTime now = DateTime.Now;
                        if ((now - lastClipboardEvent).TotalMilliseconds >= minMsBetween)
                        {
                            // Parallize the task to get the clipboard data - this does not block the UI then
                            Task.Run(() =>
                            {
                                clipboardHandler.GetClipboardData();
                            });
                            lastClipboardEvent = now;
                        } else
                        {
                            Logging.Log($"Warning: Last clipboard UPDATE event was less than [{minMsBetween}ms] ago - ignoring this event");
                        }
                    }
                    else
                    {
                        Logging.Log("Discarding clipboard UPDATE event as HovText is not enabled");
                    }

                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }        


        // ###########################################################################################
        // After the form has been shown then load the data (if any)
        // ###########################################################################################   

        private void Settings_Shown(object sender, EventArgs e)
        {
            // Should we load the clipboard data file?
            if (
                UiGeneralToggleEnableClipboard.Checked && 
                UiStorageToggleLoadClipboards.Checked && 
                hasWriteAccess)
            {
                // Get the full path and filenames for the different files
                
                // HovText-data-20231216155102.bin
                string fileMask = $"{dataName}*.bin";
                pathAndDataLoad = HandleFiles.GetNewestFile(baseDirectory, fileMask);

                // HovText-favorite-20231216155102.bin
                fileMask = $"{dataFavoriteName}*.bin";
                pathAndDataFavoriteLoad = HandleFiles.GetNewestFile(baseDirectory, fileMask);

                // HovText-index-20231216155102.bin
                fileMask = $"{dataIndexName}*.bin";
                pathAndDataIndexLoad = HandleFiles.GetNewestFile(baseDirectory, fileMask);

                // Continue, if we have the "data" and "index" file (they are the most important ones)
                if (System.IO.File.Exists(pathAndDataLoad) && System.IO.File.Exists(pathAndDataIndexLoad))
                {
                    TimerToggleSaveIcon.Enabled = true; // show the loader-icons
                    DisableTabControl();

                    // Parallize the loading, to not block the UI
                    Task.Run(() =>
                    {
                        // Load the two minor files
                        HandleFiles.LoadIndexesFromFile();
                        HandleFiles.LoadFavoritesFromFile();

                        // Time how long it takes to load the "data" file
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();
                        HandleFiles.LoadDataFile();
                        stopwatch.Stop();
                        Logging.Log($"Execution time for \"LoadDataFile()\": {stopwatch.Elapsed.TotalSeconds} seconds");

                        // We can come in the situation that we have not loaded any data
                        if(HandleClipboard.clipboardQueue.Count == 0)
                        {
                            // (Re)set all booleans handling loading routines
                            ResetLoad();
                        }
                    });
                }
                else
                {
                    // (Re)set all booleans handling loading routines
                    ResetLoad();
                }
            } else
            {
                // (Re)set all booleans handling loading routines
                ResetLoad();
            }
        }


        // ###########################################################################################
        // Set all "loading booleans" to "true", which means we have finished everything with load.
        // This is because there is either no files or somehow no queue is present.
        // ###########################################################################################   

        private void ResetLoad ()
        {
            HandleFiles.onLoadAllEntriesInClipboardQueue = true;
            HandleFiles.onLoadAllEntriesProcessedInClipboardQueue = true;
            HandleFiles.onLoadAllEntriesSavedFromQueue = true;
        }


        // ###########################################################################################
        // Things that needs to be done AFTER all clipboards have been loaded and processed,
        // but not yet (re)saved
        // ###########################################################################################   

        private void ToDoWhenAppHasFinishedStartupButBeforeSaved()
        {
            // Enable the tab-control
            UiFormTabControl.Enabled = true;
            UiFormTabControl.TabButtonSelectedState.FillColor = Color.FromArgb(56, 97, 55);
            UiFormTabControl.TabButtonSelectedState.ForeColor = Color.FromArgb(227, 227, 227);

            AddClipboardToChain();

            // Start timer that will update counters
            TimerUpdateCounters.Enabled = true;

            // Check and show popup, if the troubleshoot logfile is larger than 10MB
            CheckIfLogfileIsTooLarge();
        }


        // ###########################################################################################
        // Things that needs to be done AFTER all clipboards have been loaded and processed,
        // AND it all have been (re)saved again
        // ###########################################################################################   

        private void ToDoWhenAppHasFinishedStartupAndSaved()
        {
            // Other stuff to do once everything has been finished
            UiAdvancedLabelDevVersion.Enabled = true;
            UiAdvancedButtonClearClipboards.Enabled = true;
            UiAdvancedButtonCleanup.Enabled = true;

            // Start deleting old/obsolete files
            TimerDeleteOldFiles.Enabled = true;
        }


        // ###########################################################################################
        // TIMERS
        // ------
        // Various timers as defined in "Settings Designer"
        // ###########################################################################################   


        // ###########################################################################################
        // Process the clipboard queue
        // ###########################################################################################

        private void TimerProcessClipboardQueue_Tick(object sender, EventArgs e)
        {
            // Process the clipboard queue, if we are not currently working in it AND if it has entries to process
            if (
                !isProcessingClipboardQueue &&
                HandleClipboard.clipboardQueue.Count > 0 &&
                HandleFiles.onLoadAllEntriesInClipboardQueue)
            {
                isProcessingClipboardQueue = true;
                TimerToggleSaveIcon.Enabled = true; // show the loader-icons

                // Parallize the task, to not block the UI
                Task.Run(() =>
                {
                    // Will process all entries in queue
                    clipboardHandler.ReleaseClipboardQueue();

                    // Save the "index" and "favorite" files afterwards
                    HandleFiles.saveIndexAndFavoriteFiles = true;

                    // Update the counter/entries in the tray icon
                    UpdateNotifyIconText();
                });
            }

            // Once all entries have been processed from queue then ...
            if (!activatedHotkeys && HandleFiles.onLoadAllEntriesProcessedInClipboardQueue)
            {
                activatedHotkeys = true;
                SetHotkeys("Startup of application");
                TimerProcessSaveQueue.Enabled = true;
                ToDoWhenAppHasFinishedStartupButBeforeSaved();
            }
        }


        // ###########################################################################################
        // Process the save-queue - clipboards that needs to be saved to the "data" file.
        // The save-queue will be populated with a list of index-numbers of clipboard entries to save.
        // ###########################################################################################   

        private void TimerProcessSaveQueue_Tick(object sender, EventArgs e)
        {
            if(UiStorageToggleSaveClipboards.Checked)
            {            
                // Only process the queue, if it is currently NOT being worked on AND if it has content
                if (clipboardSaveQueue.Count > 0 && !isClipboardSaveQueueBeingProcessed)
                {
                    // We can start saving data, once all entries have been processed (the boolean
                    // is set to "true" of no file is loaded)
                    if (HandleFiles.onLoadAllEntriesProcessedInClipboardQueue)
                    {
                        isClipboardSaveQueueBeingProcessed = true;
                        TimerToggleSaveIcon.Enabled = true; // show the "saving" icons

                        // Parallize it so we do not block UI
                        Task.Run(() =>
                        {
                            // Take a copy of "clipboardSaveQueue"
                            List<int> clipboardSaveQueueTmp = new List<int>(clipboardSaveQueue); 

                            // Walk through all elements of the queue
                            foreach (var index in clipboardSaveQueueTmp)
                            {
                                if (entriesOriginal.ContainsKey(index))
                                {
                                    HandleFiles.SaveClipboardEntryToFile(index);
                                    clipboardSaveQueue.Remove(index);
                                }
                                else
                                {
                                    Logging.Log($"Error: Could not find index [{index}] for saving!? Removing it from list");
                                    clipboardSaveQueue.Remove(index);
                                }
                            }

                            HandleFiles.saveIndexAndFavoriteFiles = true;
                            HandleFiles.onLoadAllEntriesSavedFromQueue = true; // if we have loaded content from files, then mark everything as saved now
                            isClipboardSaveQueueBeingProcessed = false;
                        });
                    }
                }
            }
        }


        // ###########################################################################################
        // When this timer gets enabled, then it will show one-of-two floppydisk images.
        // This will show disk activity.
        // ###########################################################################################   

        private void TimerToggleSaveIcon_Tick(object sender, EventArgs e)
        {
            if (floppyToggle)
            {
                pictureBox1.Visible = false;
                pictureBox2.Visible = true;
            }
            else
            {
                pictureBox1.Visible = true;
                pictureBox2.Visible = false;
            }

            floppyToggle = !floppyToggle;

            // Hide the loader-icons, if there is no queues left to process
            if (HandleClipboard.clipboardQueue.Count == 0 && (clipboardSaveQueue.Count == 0 || !UiStorageToggleSaveClipboards.Checked))
            {
                TimerToggleSaveIcon.Enabled = false;
                pictureBox1.Visible = false;
                pictureBox2.Visible = false;
            }
        }


        // ###########################################################################################
        // When a boolean gets "true" then it will save both the "data" and the "favorite" file.
        // ###########################################################################################   

        private void SaveIndexAndFavoriteFiles_Tick(object sender, EventArgs e)
        {
            if(UiStorageToggleSaveClipboards.Checked)
            {
                if (HandleFiles.saveIndexAndFavoriteFiles && !saveFilesActive)
                {
                    saveFilesActive = true; 
                    TimerToggleSaveIcon.Enabled = true;
                    HandleFiles.SaveIndexAndFavoritesToFiles();
                    HandleFiles.saveIndexAndFavoriteFiles = false;
                    saveFilesActive = false;
                }
            }
        }


        // ###########################################################################################
        // Check for a new update
        // ###########################################################################################

        private void TimerCheckForUpdate_Tick(object sender, EventArgs e)
        {
            // Check for update, if it has not been done before
            if (HandleFiles.onLoadAllEntriesSavedFromQueue)
            {
                // Disable the timer - no more need for it
                TimerCheckForUpdate.Enabled = false;

                if (!hasCheckedForUpdate)
                {
                    hasCheckedForUpdate = true;
                    CheckForUpdate();
                }

                ToDoWhenAppHasFinishedStartupAndSaved();
            }
        }


        // ###########################################################################################
        // Old file monitoring timer
        // ###########################################################################################

        private void TimerDeleteOldFiles_Tick(object sender, EventArgs e)
        {
            bool wasAnyFilesDeleted = HandleFiles.DeleteOldFiles();

            if (!wasAnyFilesDeleted)
            {
                TimerDeleteOldFiles.Enabled = false;
                Logging.Log($"Disabled timer for monitoring old file deletions");
            }
        }


        // ###########################################################################################
        // Update various counters
        // ###########################################################################################

        private void TimerUpdateCounters_Tick(object sender, EventArgs e)
        {
            if(Visible)
            {
                UpdateClipboardEntriesCounters();
            }
            
        }


        // ###########################################################################################
        // For the 3 encrypted files, we need a timestamp for the filename, so they are identical
        // ###########################################################################################   

        private string GetCurrentDateTimeFormatted()
        {
            DateTime now = DateTime.Now;
            string formattedDateTime = now.ToString("yyyyMMddHHmmss");
            return formattedDateTime;
        }

        
        // ###########################################################################################
        // Add HovText process to the clipboard chain
        // ###########################################################################################   

        private void AddClipboardToChain()
        {
            NativeMethods.AddClipboardFormatListener(Handle);
            Logging.Log("Added HovText to clipboard chain");
        }


        // ###########################################################################################
        // Clear the clipboard list - as-if we just launched the application
        // ###########################################################################################

        public static void ClearHistory()
        {
            entryCounter = -1;
//            entryIndex = -1;
            // ---
            entriesApplication.Clear();
            entriesApplicationIcon.Clear();
            entriesImage.Clear();
            entriesImageTrans.Clear();
            entriesChecksum.Clear();
            entriesIsEmail.Clear();
            entriesIsFavorite.Clear();
            entriesIsImage.Clear();
            entriesIsTransparent.Clear();
            entriesIsUrl.Clear();
            entriesOriginal.Clear();
            entriesShow.Clear();
            entriesText.Clear();
            entriesTextTrimmed.Clear();
            entriesOrder.Clear();
        }


        // ###########################################################################################
        // Called when a clipboard entry has been selected in the clipboard list
        // ###########################################################################################

        public void SelectHistoryEntry()
        {
            // Check if application is enabled
            if (isApplicationEnabled && entryCounter > 0)
            {
                // Only proceed if we should actually restore something (we could come here from an empty list)
                int entriesInList = History.entriesInList;
                if (entriesInList > 0)
                {
//                    pasteOnHotkeySetCleartext = true;

                    MoveEntryToTop(entryIndex);

                    // Set the clipboard with the new data
                    //HandleClipboard.SetClipboard(entryIndex);

                    // Restore the original clipboard, if we are within the "Paste on hotkey only" mode
                    if (UiHotkeysRadioPasteOnHotkey.Checked)
                    {
                        PasteOnHotkey.StartTimerToRestoreOriginal();
                    }
                }

                // Set focus back to the originating application
                ChangeFocusToOriginatingApplication();

                // Reset some stuff
//                isFirstCallAfterHotkey = true;
                entryIndex = entriesTextTrimmed.Keys.Last();
                GetEntryCounter();

                // Save the new order of the entries
                HandleFiles.saveIndexAndFavoriteFiles = true;
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
                /*
                if (isFirstCallAfterHotkey)
                {
                    // Hide the "Settings" form if it is visible (it will be restored after key-up)
                    isSettingsFormVisible = Visible;
                    if (isSettingsFormVisible)
                    {
                        Hide();
                    }
                    originatingApplicationName = HandleClipboard.GetActiveApplicationName();
                    history.SetupForm();
                }
                */

                // Always change focus to HovText to ensure we can catch the key-up event
                //ChangeFocusToHovText();

                // Only proceed if the entry counter is equal to or more than 0
                if (entryCounter > 0)
                {
//                    isFirstCallAfterHotkey = false;
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

                /*
                if (isFirstCallAfterHotkey)
                {
                    // Hide the "Settings" form if it is visible (it will be restored after key-up)
                    isSettingsFormVisible = Visible;
                    if (isSettingsFormVisible)
                    {
                        Hide();
                    }
                    originatingApplicationName = HandleClipboard.GetActiveApplicationName();
                    history.SetupForm();
                }
                */

                // Always change focus to HovText to ensure we can catch the key-up event
                //ChangeFocusToHovText();

                // Only proceed if the entry counter is less than the total amount of entries
                if (entryCounter <= entriesTextTrimmed.Count)
                {
//                    isFirstCallAfterHotkey = false;
                    history.UpdateHistory("up");
                }
            }
            ResumeLayout();
        }       


        // ###########################################################################################
        // Convert the entry index to which logical number it is
        // ###########################################################################################

        public static void GetEntryCounter()
        {
            entryCounter = entriesOrder.Count;
            entryIndex = entriesOrder.Keys.LastOrDefault();
        }


        // ###########################################################################################
        // Move the active entry to the top of the data arrays (newest entry)
        // ###########################################################################################

        public static void MoveEntryToTop(int index)
        {
            int insertIndex = Interlocked.Increment(ref HandleClipboard.threadSafeIndex) - 1; // thread-safe incremental of a sequence number

            // Copy the chosen entry to the top of the array lists (so it becomes the newest entry)
            entriesText.Add(insertIndex, entriesText[index]);
            entriesTextTrimmed.Add(insertIndex, entriesTextTrimmed[index]);
            entriesImage.Add(insertIndex, entriesImage[index]);
            entriesImageTrans.Add(insertIndex, entriesImageTrans[index]);
            entriesChecksum.Add(insertIndex, entriesChecksum[index]);
            entriesApplication.Add(insertIndex, entriesApplication[index]);
            entriesApplicationIcon.Add(insertIndex, entriesApplicationIcon[index]);
            entriesOriginal.Add(insertIndex, entriesOriginal[index]);
            entriesIsFavorite.Add(insertIndex, entriesIsFavorite[index]);
            entriesIsUrl.Add(insertIndex, entriesIsUrl[index]);
            entriesIsEmail.Add(insertIndex, entriesIsEmail[index]);
            entriesIsTransparent.Add(insertIndex, entriesIsTransparent[index]);
            entriesIsImage.Add(insertIndex, entriesIsImage[index]);
            entriesOrder.Add(insertIndex, entriesOrder[index]);

            // Set the clipboard (depending if we come from a threaded or non-threaded call)
            if (settings.InvokeRequired)
            {
                settings.Invoke(new Action(() => HandleClipboard.SetClipboard(index)));
            } else
            {
                HandleClipboard.SetClipboard(index);
            }

            // Remove the chosen entry, so it does not show duplicates
            RemoveEntryFromLists(index);

            // Set the index to be the last one
            entryIndex = entriesTextTrimmed.Keys.Last();
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
                AddClipboardToChain();

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

                if (UiGeneralToggleEnableFavorites.Checked && UiGeneralToggleEnableClipboard.Checked)
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
                //AddClipboardToChain();
                bool keepApplicationToggleActive = true;
                RemoveAllHotkeys(keepApplicationToggleActive);

                // Restore the original clipboard format
                if (isRestoreOriginal && entriesOriginal.Count > 0)
                {
                    HandleClipboard.RestoreOriginal(HandleClipboard.threadSafeIndex - 1);
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

                history.ActionEscape();
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
                ShowTrayNotifications();
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
        // Update the closing UI for the "save file"
        // ###########################################################################################

        private void DisableTabControl()
        {
            // Update UI from the UI thread
            UiFormTabControl.Enabled = false;
            UiFormTabControl.TabButtonSelectedState.FillColor = Color.LightGray;
            UiFormTabControl.TabButtonSelectedState.ForeColor = Color.DarkGray;
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
                NativeMethods.RemoveClipboardFormatListener(Handle);
                Logging.Log("Removed HovText from clipboard chain");

                RemoveAllHotkeys();

                Logging.EndLogging();

                // Delete the troubleshooting logfile as the very last thing
                if (cleanupApp)
                {
                    HandleFiles.DeleteFilesOnCleanup();
                }

                return;
            }

            // Prevent the application from closing, if we are currently processing clipboards
            if (HandleClipboard.clipboardQueue.Count > 0 || (clipboardSaveQueue.Count > 0 && UiStorageToggleSaveClipboards.Checked))
            {
                e.Cancel = true;
                MessageBox.Show("HovText is currently processing one or more clipboards - please wait until this finishes, before you close the application",
                    "HovText INFO",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            // Normal exit
            if (!UiGeneralToggleCloseMinimizes.Checked || isClosedFromNotifyIcon)
            {
                if (isClosing) return;
                e.Cancel = true; // Prevent form from closing immediately

                DisableTabControl();

                Logging.Log("Exit HovText");
                NativeMethods.RemoveClipboardFormatListener(Handle);
                Logging.Log("Removed HovText from clipboard chain");

                RemoveAllHotkeys();

                Logging.EndLogging();

                // Delete the troubleshooting logfile as the very last thing
                if (cleanupApp)
                {
                    HandleFiles.DeleteFilesOnCleanup();
                }

                isClosing = true; // indicate that closing is in progress
                BeginInvoke(new Action(() => Close())); // close the form on the UI thread

                return;
            }

            // Called when closing from "Clean up"
            if (cleanupApp)
            {
                NativeMethods.RemoveClipboardFormatListener(Handle);
                RemoveAllHotkeys();

                HandleFiles.DeleteFilesOnCleanup();

                return;
            }

            // Do not close as the X should minimize
            if (WindowState == FormWindowState.Normal)
            {
                ShowTrayNotifications();
                Hide();
            }

            e.Cancel = true;
        }


        // ###########################################################################################
        // When pressing one of the history hotkeys then change focus to this application to prevent the keypresses go in to the active application
        // ###########################################################################################

        private void ChangeFocusToHovText()
        {
            NativeMethods.SetForegroundWindow(Handle);
            Logging.Log("Set focus to HovText");
        }


        // ###########################################################################################
        // When an entry has been submitted to the clipboard then pass back focus to the originating application
        // ###########################################################################################

        public static void ChangeFocusToOriginatingApplication()
        {
            NativeMethods.SetForegroundWindow(HandleClipboard.originatingHandle);
            Logging.Log("Set focus to originating application [" + originatingApplicationName + "]");
        }


        // ###########################################################################################
        // Check for HovText updates online.
        // Stable versions will be notified via popup.
        // Development versions will be shown in "Advanced" tab only.
        // ###########################################################################################

        private void CheckForUpdate()
        {
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
                    { "osVersion", osVersion }
//                    { "cpuArchitecture", cpuArchitecture }
                };

                // Send the POST data to the server
                byte[] responseBytes = webClient.UploadValues(hovtextPage + "/autoupdate/", postData);

                // Convert the response bytes to a string
                checkedVersion = Encoding.UTF8.GetString(responseBytes);

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
                        }
                        else
                        {
                            Logging.Log("  Did not notify on new [STABLE] version available, as this is the first launch");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Catch the exception though this is not so critical that we need to disturb the developer
                Logging.Log("Error: Cannot connect with server to get information about newest available [STABLE] version");
                Logging.LogException(ex);
            }

            // Show the development version (if any)
            FetchInfoForDevelopment();
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

            // Delete "HistoryInstantSelect" as this is no longer available
            regVal = GetRegistryKey(registryPath, "HistoryInstantSelect");
            if (regVal != null || regVal?.Length == 0)
            {
                DeleteRegistryKey(registryPath, "HistoryInstantSelect");
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
                Logging.Log("An error occurred - needs more investigation");
                Logging.LogException(ex);
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
                if (key != "EncryptionKey" && key != "EncryptionInitializationVector")
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

            // Hotkey behaviour
            string hotkeyBehaviour = GetRegistryKey(registryPath, "HotkeyBehaviour");
            switch (hotkeyBehaviour)
            {
                case "Paste":
                    UiHotkeysButtonPasteHotkey.Enabled = true;
                    UiHotkeysLabelPasteHotkey.Enabled = true;
                    UiHotkeysRadioPasteOnHotkey.Checked = true;
                    isEnabledPasteOnHotkey = true;
                    break;
                default:
                    UiHotkeysButtonPasteHotkey.Enabled = false;
                    UiHotkeysLabelPasteHotkey.Enabled = false;
                    UiHotkeysRadioUseStandardWindowsMethod.Checked = true;
                    isEnabledPasteOnHotkey = false;
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
                if (!UiGeneralToggleStartWithWindows.Checked)
                {
                    UiGeneralToggleStartWithWindows.Checked = true;
                }

                // Overwrite "Run" if it does not contain "HovText.exe" or "--start-minimized"
                string runEntry = GetRegistryKey(registryPathRun, "HovText");
                string thisEntry = "\"" + System.Windows.Forms.Application.ExecutablePath + "\" --start-minimized";
                if (runEntry != thisEntry)
                {
                    SetRegistryKey(registryPathRun, "HovText", "\"" + System.Windows.Forms.Application.ExecutablePath + "\" --start-minimized");
                }
            }

            // Update info
            UiAdvancedPicture1BoxDevRefresh.Visible = true;
            UiAdvancedPicture2BoxDevRefresh.Visible = false;
            //UiAdvancedLabelDevVersion.Enabled = true;
            UiAdvancedLabelDisclaimer.Enabled = true;
            UiAdvancedLabelDevVersion.Text = "Please wait ...";

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
            isEnabledHistory = historySearch == 1;
            if(!isEnabledHistory)
            {
                UiGeneralToggleEnableClipboard.Checked = false; // it is enabled by default
            }

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
            isEnabledAlwaysPasteOriginal = UiGeneralToggleAlwaysPasteOriginal.Checked;

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
            bool shouldNotifyNoWriteAccess = false;
            if (!hasWriteAccess && UiStorageToggleSaveClipboards.Checked)
            {
                UiStorageToggleSaveClipboards.Checked = false;
                shouldNotifyNoWriteAccess = true;
            }

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
                case "Text+Favorite":
                    UiStorageRadioSaveBothTextAndFavorites.Checked = true;
                    break;
                case "Favorite":
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
            clipboardEntriesToSave = entries;

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
            UiStyleLabelBorderText.Text = historyBorderThickness.ToString() + "px";

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
            if (!hasWriteAccess && UiAdvancedToggleEnableLog.Checked)
            {
                UiAdvancedToggleEnableLog.Checked = false;
                shouldNotifyNoWriteAccess = true;
            }


            // ------------------------------------------
            // Notify on "any saving is disabled as no write access"
            // ------------------------------------------
            if (shouldNotifyNoWriteAccess)
            {
                HandleFiles.InformOnMissingWriteAccess();
            }

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
            if (UiGeneralToggleStartWithWindows.Checked)
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
            }
            else if (UiLayoutRadioLeftBottom.Checked)
            {
                historyLocation = "Left Bottom";
                SetRegistryKey(registryPath, "HistoryLocation", historyLocation);
            }
            else if (UiLayoutRadioCenter.Checked)
            {
                historyLocation = "Center";
                SetRegistryKey(registryPath, "HistoryLocation", historyLocation);
            }
            else if (UiLayoutRadioRightTop.Checked)
            {
                historyLocation = "Right Top";
                SetRegistryKey(registryPath, "HistoryLocation", historyLocation);
            }
            else if (UiLayoutRadioRightBottom.Checked)
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

            if(!isCopyImages && HandleFiles.onLoadAllEntriesProcessedInClipboardQueue)
            {
                // Make a temporary copy of the "entriesOrder" and work  with this instance
                SortedDictionary<int, int> entriesOrderTmp = new SortedDictionary<int, int>(entriesOrder);

                foreach (var entry in entriesOrderTmp)
                {
                    bool isImage = entriesIsImage[entry.Key];
                    if (isImage)
                    {
                        // Delete the image from the lists
                        RemoveEntryFromLists(entry.Value);
                    }
                }
                HandleFiles.saveIndexAndFavoriteFiles = true;
            }
        }


        // ###########################################################################################
        // Remove an entry from the clipboard list
        // ###########################################################################################

        public static void RemoveEntryFromLists (int index)
        {
            entriesText.Remove(index);
            entriesTextTrimmed.Remove(index);
            entriesImage.Remove(index);
            entriesImageTrans.Remove(index);
            entriesChecksum.Remove(index);
            entriesApplication.Remove(index);
            entriesApplicationIcon.Remove(index);
            entriesOriginal.Remove(index);
            entriesShow.Remove(index);
            entriesIsFavorite.Remove(index);
            entriesIsUrl.Remove(index);
            entriesIsEmail.Remove(index);
            entriesIsTransparent.Remove(index);
            entriesIsImage.Remove(index);
            entriesOrder.Remove(index);
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
                UiStorageTrackBarEntriesToSave.Enabled = true;
            }
            else
            {
                UiHotkeysButtonSearch.Enabled = false;
                UiGeneralLabelEnableClipboardShortcut.Enabled = false;
                UiHotkeysLabelSearch.Enabled = false;
                UiStorageTrackBarEntriesToSave.Enabled = false;
            }

            if (UiGeneralToggleEnableFavorites.Checked)
            {
                UiHotkeysButtonToggleFavorite.Enabled = true;
                UiHotkeysLabelToggleFavorite.Enabled = true;
            }
            else
            {
                UiHotkeysButtonToggleFavorite.Enabled = false;
                UiHotkeysLabelToggleFavorite.Enabled = false;
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
            HandleClipboard.SetClipboard(HandleClipboard.threadSafeIndex - 1);

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
            isEnabledAlwaysPasteOriginal = UiGeneralToggleAlwaysPasteOriginal.Checked ? true : false;

            HandleClipboard.SetClipboard(HandleClipboard.threadSafeIndex - 1);
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
                if (isEnabledHistory)
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
                    bool doesKeyExist = entriesTextTrimmed.ContainsKey(i);
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

            // Set the clipboard again, as there could be changes how "GuiAlwaysPasteOriginal" behaves
            HandleClipboard.SetClipboard(HandleClipboard.threadSafeIndex - 1);
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
                isEnabledPasteOnHotkey = false;
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
            isEnabledPasteOnHotkey = true;
//            HandleClipboard.SetClipboard(HandleClipboard.threadSafeIndex - 1);
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
            GetEntryCounter();

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

                    // Default show all clipboards - no filtering
                    entriesShow.Clear();
                    {
                        foreach (var key in entriesOrder.Keys)
                        {
                            entriesShow[key] = true;
                        }
                    }

//                    if (isFirstCallAfterHotkey)
//                    {
                        // Hide the "Settings" form if it is visible (it will be restored after key-up)
                        isSettingsFormVisible = Visible;
                        if (isSettingsFormVisible)
                        {
                            Hide();
                        }
                        originatingApplicationName = HandleClipboard.GetActiveApplicationName();
                        history.SetupForm();
//                    }

                    // Always change focus to HovText to ensure we can catch the key-up event
                    ChangeFocusToHovText();

                    // Only proceed if the entry counter is equal to or more than 0
                    if (entryCounter > 0)
                    {
//                        isFirstCallAfterHotkey = false;
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
            if (entriesTextTrimmed.Count > 0)
            {
                // Get active application and change focus to HovText
                originatingApplicationName = HandleClipboard.GetActiveApplicationName();
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

        public static void RemoveAllHotkeys(bool keepApplicationToggleActive = false)
        {
            if(!keepApplicationToggleActive)
            {
                HotkeyManager.Current.Remove("ToggleApplication");
                Logging.Log("[HotkeyToggleApplication] removed");
            }
            
            HotkeyManager.Current.Remove("Search");
            HotkeyManager.Current.Remove("PasteOnHotkey");
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
                        Logging.LogException(ex);
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
                    Logging.LogException(ex);
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
                    Logging.LogException(ex);
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
                UiHotkeysButtonToggleApplication.FillColor = Color.FromArgb(220, 227, 220);
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

        private void ShowTrayNotifications()
        {
            // Show a tray notification if this is the first time the user close the form (without exiting the application)
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


        // ###########################################################################################
        // Update the entries on the tray icon (when hovering the mouse over it)
        // ###########################################################################################

        private void UpdateNotifyIconText()
        {
            // Update the counter if the history is enabled
            if (isEnabledHistory)
            {
                int entries = entriesTextTrimmed.Count;
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
                ControlPaint.DrawBorder(
                    e.Graphics, e.ClipRectangle,
                    ColorTranslator.FromHtml(Settings.historyColorsBorder[historyColorTheme]), historyBorderThickness, ButtonBorderStyle.Solid,
                    ColorTranslator.FromHtml(Settings.historyColorsBorder[historyColorTheme]), historyBorderThickness, ButtonBorderStyle.Solid,
                    ColorTranslator.FromHtml(Settings.historyColorsBorder[historyColorTheme]), historyBorderThickness, ButtonBorderStyle.Solid,
                    ColorTranslator.FromHtml(Settings.historyColorsBorder[historyColorTheme]), historyBorderThickness, ButtonBorderStyle.Solid);
            }
            else
            {
                ControlPaint.DrawBorder(
                    e.Graphics, 
                    e.ClipRectangle, 
                    BackColor, ButtonBorderStyle.None);
            }
        }


        // ###########################################################################################
        // Troubleshooting, enable or disable
        // ###########################################################################################

        private void GuiTroubleshootEnabled_CheckedChanged(object sender, EventArgs e)
        {

            // Check if we have write access - if not, then revert
            if (!HandleFiles.HasWriteAccess() && UiAdvancedToggleEnableLog.Checked)
            {
                UiAdvancedToggleEnableLog.Checked = false;
                HandleFiles.InformOnMissingWriteAccess();
                return;
            }

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
            if (System.IO.File.Exists(pathAndLog))
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
            if (System.IO.File.Exists(pathAndLog))
            {
                try
                {
                    System.IO.File.Delete(@pathAndLog);
                }
                catch (Exception ex)
                {
                    Logging.LogException(ex);
                }
                UiAdvancedButtonDeleteLog.Enabled = false;
            }
            else
            {
                MessageBox.Show(pathAndLog + " does not exists!",
                    "HovText WARNING",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                UiAdvancedButtonDeleteLog.Enabled = false;
            }
        }


        // ###########################################################################################
        // Troubleshooting, refresh the buttons for the logfile, if viewing the "Advanced" tab
        // ###########################################################################################

        private void TabControl_Selected(object sender, TabControlEventArgs e)
        {
            // "Advanced" tab
            if (UiFormTabControl.SelectedTab.AccessibilityObject.Name == "Advanced")
            {
                if (System.IO.File.Exists(pathAndLog))
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
                if (System.IO.File.Exists(pathAndLog))
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
                            UiFeedbackToggleAttachLog.Checked = false;
                            UiFeedbackToggleAttachLog.Enabled = false;
                            UiFeedbackLabelAttachLog.Enabled = false;
                            UiFeedbackLabelAttachLog.Text = "Attach troubleshooting logfile (" + fileSize.ToString() + " " + fileDescription + " - must not exceed 5MB)";
                        }
                        else
                        {
                            UiFeedbackToggleAttachLog.Enabled = true;
                            UiFeedbackLabelAttachLog.Enabled = true;
                            UiFeedbackLabelAttachLog.Text = "Attach troubleshooting logfile (" + fileSize.ToString() + " " + fileDescription + ")";
                        }
                    }
                    else
                    {
                        UiFeedbackToggleAttachLog.Enabled = false;
                        UiFeedbackLabelAttachLog.Enabled = false;
                        UiFeedbackLabelAttachLog.Text = "Attach troubleshooting logfile (file is empty)";
                    }
                }
                else
                {
                    UiFeedbackToggleAttachLog.Enabled = false;
                    UiFeedbackLabelAttachLog.Enabled = false;
                    UiFeedbackLabelAttachLog.Text = "Attach troubleshooting logfile (file does not exists)";
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
            if (CheckIfQueuesAreEmpty())
            {

                isTroubleshootEnabled = false;

                // Remove HovText from starting up at Windows boot
                DeleteRegistryKey(registryPathRun, "HovText");

                // Delete the "HovText" folder in registry
                RegistryKey parentClass = Registry.CurrentUser;
                RegistryKey parentSoftware = parentClass.OpenSubKey("Software", true);
                parentSoftware.DeleteSubKeyTree("HovText");

                // Delete the "Start Menu" shortcut
                string startMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
                string appStartMenuPath = Path.Combine(startMenuPath, "Programs", "HovText");
                try
                {
                    if (Directory.Exists(appStartMenuPath))
                    {
                        Directory.Delete(appStartMenuPath, true);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error deleting \"Start Menu\" shortcut");
                    Logging.LogException(ex);
                }

                // Exit HovText
                cleanupApp = true;

                Close();

            }
            else
            {
                Logging.Log($"Could not proceed as one or more clipboards are being processed");
            }
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
                    string textFileContent = System.IO.File.ReadAllText(textFilePath);
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
                                "HovText OK",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }
                        else
                        {
                            string txt = "Feedback sent - no response will be given as you did not specify an email address";
                            Logging.Log(txt);
                            MessageBox.Show(txt,
                                "HovText OK",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }
                    }
                }
                catch (WebException ex)
                {
                    // Catch the exception though this is not so critical that we need to disturb the developer
                    Logging.Log("Error: Cannot connect with server to submit feedback");
                    Logging.LogException(ex);
                    MessageBox.Show("HovText cannot connect to the server, where it should submit the feedback. Please connect directly with the developer at \"dennis@hovtext.com\" and state this is a problem, thanks.\r\n\r\nThe exact error is:\r\n\r\n" + ex.Message,
                        "HovText ERROR",
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
                    "HovText ERROR",
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
                Text = "HovText DEVELOPMENT version";
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
                        Logging.Log("  Development version available = [" + checkedVersion + "]");
                        UiAdvancedButtonAutoInstall.Enabled = true;
                    }
                    else
                    {
                        UiAdvancedButtonManualDownload.Enabled = false;
                        UiAdvancedLabelDisclaimer.Enabled = false;
                        UiAdvancedLabelDevVersion.Text = "No development version available";
                        Logging.Log("  Development version available = [No development version available]");
                    }
                }
                else
                {

                    UiAdvancedButtonManualDownload.Enabled = false;
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
                Logging.Log("Error: Cannot connect with server to get information about newest available [DEVELOPMENT] version:");
                Logging.LogException(ex);
                UiAdvancedLabelDevVersion.Text = "Cannot connect with server - retry later";
            }
        }


        // ###########################################################################################
        // Check if the logfile is too large
        // ###########################################################################################

        private void CheckIfLogfileIsTooLarge()
        {
            // Only check the logfile size, if we have active logging
            if (UiAdvancedToggleEnableLog.Checked)
            {
                // Get the filesize og the logfile
                if (System.IO.File.Exists(pathAndLog))
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
            clipboardEntriesToSave = UiStorageTrackBarEntriesToSave.Value;
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

            if (!isStartingUp)
            {

                SortedDictionary<int, int> entriesOrderTmp = new SortedDictionary<int, int>();

                if (UiStorageRadioSaveAll.Checked)
                {
                    set = "All";
                }
                else if (UiStorageRadioSaveBothTextAndFavorites.Checked)
                {
                    set = "Text+Favorite";
                }
                else if (UiStorageRadioSaveOnlyFavorites.Checked)
                {
                    set = "Favorite";
                }
                else
                {
                    set = "Text";
                }

                HandleFiles.saveIndexAndFavoriteFiles = true;


                SetRegistryKey(registryPath, "StorageSaveType", set);
            }
        }


        // ###########################################################################################
        // Update which clipboard type we should save
        // ###########################################################################################

        private void GuiStorageSaveClipboard_CheckedChanged(object sender, EventArgs e)
        {

            // Check if we have write access - if not, then revert
            if (!HandleFiles.HasWriteAccess() && UiStorageToggleSaveClipboards.Checked)
            {
                UiStorageToggleSaveClipboards.Checked = false;
                HandleFiles.InformOnMissingWriteAccess();
                return;
            }

            if (UiStorageToggleSaveClipboards.Checked)
            {
                UiStorageRadioSaveOnlyText.Enabled = true;
                UiStorageLabelSaveOnlyText.Enabled = true;
                UiStorageRadioSaveAll.Enabled = true;
                UiStorageLabelSaveAll.Enabled = true;
                if (UiStorageToggleSaveClipboards.Checked)
                {
                    if (isEnabledFavorites)
                    {
                        UiStorageRadioSaveOnlyFavorites.Enabled = true;
                        UiStorageLabelSaveOnlyFavorites.Enabled = true;
                    }
                    else
                    {
                        UiStorageRadioSaveOnlyFavorites.Enabled = false;
                        UiStorageLabelSaveOnlyFavorites.Enabled = false;
                    }
                }
                SetRegistryKey(registryPath, "StorageSaveOnExit", "1");
            }
            else
            {
                UiStorageRadioSaveOnlyText.Enabled = false;
                UiStorageLabelSaveOnlyText.Enabled = false;
                UiStorageRadioSaveAll.Enabled = false;
                UiStorageLabelSaveAll.Enabled = false;
                UiStorageRadioSaveOnlyFavorites.Enabled = false;
                UiStorageLabelSaveOnlyFavorites.Enabled = false;
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
            // Only show memory, if it has changed more than 5%
            if (
                memoryInMB != old_memoryInMB &&
                (
                memoryInMB <= (int)(old_memoryInMB * 0.95) ||
                memoryInMB >= (int)(old_memoryInMB * 1.05)
                )
            )
            {
                Logging.Log($"Memory consumption: [{memoryInMB}] MB");
                old_memoryInMB = memoryInMB;
            }
        }


        public void UpdateMemoryConsumed()
        {
            string formattedMemoryUsage = "Memory usage: " + memoryInMB + " MB";
            UiAdvancedLabelMemUsed.Text = formattedMemoryUsage;
        }


        // ###########################################################################################
        // Update the amount of clipboard entries in the "Storage" tab
        // ###########################################################################################

        public void UpdateClipboardEntriesCounters()
        {
            UiAdvancedLabelClipboardEntries.Text = "Clipboard entries: " + entriesOriginal.Count.ToString();
            int countImages = entriesIsImage.Count(entry => entry.Value == true);
            int countFavorites = entriesIsFavorite.Count(entry => entry.Value == true);
            int countAll = entriesOrder.Count();
            int countText = countAll - countImages;

            // Find unique keys to find counter for "countTextAndFavorites"
            var nonImageKeys = entriesOrder.Keys.Except(entriesIsImage.Where(entry => entry.Value == true).Select(entry => entry.Key));
            var favoriteKeys = entriesIsFavorite.Where(entry => entry.Value == true).Select(entry => entry.Key);
            var textAndFavoritesKeys = nonImageKeys.Union(favoriteKeys);
            int countTextAndFavorites = textAndFavoritesKeys.Count();

            if (countText == 1)
            {
                UiStorageLabelSaveOnlyTextEntries.Text = "(" + countText + " entry)";
            }
            else
            {
                UiStorageLabelSaveOnlyTextEntries.Text = "(" + countText + " entries)";
            }

            if (countFavorites == 1)
            {
                UiStorageLabelSaveOnlyFavoritesEntries.Text = "(" + countFavorites + " entry)";
            }
            else
            {
                UiStorageLabelSaveOnlyFavoritesEntries.Text = "(" + countFavorites + " entries)";
            }

            if (countAll == 1)
            {
                UiStorageLabelSaveAllEntries.Text = "(" + countAll + " entry)";
            }
            else
            {
                UiStorageLabelSaveAllEntries.Text = "(" + countAll + " entries)";
            }

            if (countTextAndFavorites == 1)
            {
                UiStorageLabelSaveTextAndFavoritesEntries.Text = "(" + countTextAndFavorites + " entry)";
            }
            else
            {
                UiStorageLabelSaveTextAndFavoritesEntries.Text = "(" + countTextAndFavorites + " entries)";
            }
        }


        // ###########################################################################################
        // Clear clipboard history
        // ###########################################################################################

        private void GuiClearHistory_Click(object sender, EventArgs e)
        {
            int count = entriesOriginal.Count;
            Logging.Log($"Cleaning all [" + count + "] clipboard entries from memory");

            if(CheckIfQueuesAreEmpty())
            {

                if (System.IO.File.Exists(pathAndData))
                {
                    try
                    {
                        System.IO.File.Delete(@pathAndData);
                    }
                    catch (Exception ex)
                    {
                        Logging.LogException(ex);
                    }
                    Logging.Log($"Deleted the clipboard data file [" + pathAndData + "]");
                }

                ClearHistory();
                UpdateNotifyIconText();

                // Trigger the garbage collector to see the impact immediately
                GC.Collect();
                GC.WaitForPendingFinalizers();

            } else
            {
                Logging.Log($"Could not proceed as one or more clipboards are being processed");
            }
        }


        // ###########################################################################################
        // Check if we can do this action now - not possible if any clipboard is being processed
        // ###########################################################################################

        private bool CheckIfQueuesAreEmpty ()
        {
            if(HandleClipboard.clipboardQueue.Count > 0 || clipboardSaveQueue.Count > 0)
            {
                MessageBox.Show("You cannot do this now, as it is processing one or more clipboards - please wait until this finishes",
                    "HovText ERROR",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            } else
            {
                return true;
            }
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

        private void UiStorageLabelSaveTextAndFavorites_Click(object sender, EventArgs e)
        {
            UiStorageRadioSaveBothTextAndFavorites.Checked = true;
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
            path.AddArc(Width - cornerRadius - borderWidth, 0, cornerRadius, cornerRadius, 270, 90);
            path.AddArc(Width - cornerRadius - borderWidth, Height - cornerRadius - borderWidth, cornerRadius, cornerRadius, 0, 90);
            path.AddArc(0, Height - cornerRadius - borderWidth, cornerRadius, cornerRadius, 90, 90);
            path.CloseAllFigures();

            // Set the form's region
            Region = new Region(path);

            // Draw the border
            using (Pen pen = new Pen(BackColor, borderWidth))
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
            UpdateMemoryConsumed();
        }

        private void GetMemoryConsumption()
        {
            // Get the current memory usage
            Process currentProcess = Process.GetCurrentProcess();
            double tmp_memoryInMB = currentProcess.WorkingSet64 / 1024d / 1024d;
            memoryInMB = (int)Math.Round(tmp_memoryInMB);
        }


        // ###########################################################################################
        // New auto-install method
        // ###########################################################################################

        private void UiAdvancedButtonAutoInstall_Click(object sender, EventArgs e)
        {
            Logging.Log("Auto-install new [DEVELOPMENT] version");
            AutoInstall("Development");
        }

        public static void AutoInstall(string argument)
        {
            // Set up the start info for the update process, including the arguments
            string tempExe = Path.Combine(baseDirectory, updateExe);
            ProcessStartInfo startInfo = new ProcessStartInfo(tempExe)
            {
                Arguments = argument,
                UseShellExecute = false
            };

            // Get the embedded binary resource
            string resourcePath = "HovText.Resources." + updateExe;

            // Extract the embedded update to a local file
            using (Stream input = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath))
            {
                if (input == null)
                {
                    Logging.Log("Error: Resource not found: " + resourcePath);
                    return;
                }

                using (Stream output = System.IO.File.Create(tempExe))
                {
                    input.CopyTo(output);
                }
            }

            // Start the update process with the arguments
            Logging.Log($"Auto-install called with: {tempExe} {argument}");
            Process.Start(startInfo);

            // Terminate the main application
            Environment.Exit(0);
        }


        // ###########################################################################################
        // Create link in "Start Menu".
        // It will create the shortcut at every launch, to make sure it is
        // launching the latest version launched by the user.
        // ###########################################################################################

        public void CreateShortcut(string appName, string appPath, string appDescription)
        {
            string startMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
            string appStartMenuPath = Path.Combine(startMenuPath, "Programs", appName);

            if (!Directory.Exists(appStartMenuPath))
            {
                try
                {
                    Directory.CreateDirectory(appStartMenuPath);
                    Logging.Log($"Created \"Start Menu\" shortcut directory [{appStartMenuPath}]");
                }
                catch (Exception ex)
                {
                    Logging.Log($"Error - could not create \"Start Menu\" shortcut directory [{appStartMenuPath}]");
                    Logging.LogException(ex);
                    return;
                }
            }

            string shortcutLocation = Path.Combine(appStartMenuPath, appName + ".lnk");

            // Check if the shortcut already exists
            if (System.IO.File.Exists(shortcutLocation))
            {
                Logging.Log($"HovText \"Start Menu\" shortcut link [{shortcutLocation}] already exists - overwriting it");
            } else {
                Logging.Log($"HovText \"Start Menu\" shortcut link [{shortcutLocation}] created");
            }

            /*
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

            shortcut.Description = appDescription;
            shortcut.TargetPath = appPath;
            shortcut.Save();
            */

            WshShell shell = null;
            IWshShortcut shortcut = null;
            try
            {
                shell = new WshShell();
                shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

                shortcut.Description = appDescription;
                shortcut.TargetPath = appPath;
                shortcut.Save();
            }
            catch (Exception ex)
            {
                Logging.LogException(ex);
            }
            finally
            {
                // Release the COM objects
                if (shortcut != null)
                {
                    Marshal.ReleaseComObject(shortcut);
                }
                if (shell != null)
                {
                    Marshal.ReleaseComObject(shell);
                }
            }
        }


        // ###########################################################################################

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();


        // ###########################################################################################
    }
}