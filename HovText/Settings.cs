using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net;
using Microsoft.Win32;
using HovText.Properties;
using System.Text;
using NHotkey.WindowsForms; // https://github.com/thomaslevesque/NHotkey

// NuGet: "Costura.Fody" to merge the DLLs in to the EXE
// Incredible cool and simple compared to the other complex stuff I have seen! :-)
// https://stackoverflow.com/a/40786196/2028935

namespace HovText
{

    public partial class Settings : Form
    {
        // ###########################################################################################
        // Define "Settings" class variables - real spaghetti :-)
        // ###########################################################################################

        // Hotkey and related (e.g. switching application focus when using hotkey)
        private string hotkeyBehaviour = "System"; // default
        private static string hotkeyToggleApplication = "Control + Oem5"; // default hotkey "Toggle application on/off"
        private static string hotkeyGetOlderEntry = "Alt + H"; // default hotkey for "Show older entry"
        private static string hotkeyGetNewerEntry = "Shift + Alt + H"; // default hotkey for "Show newer entry"
        private static string hotkeyPasteOnHotkey = "Alt + O"; // default hotkey for "Paste on hotkey"
        private static IntPtr originatingHandle = IntPtr.Zero;
        private static bool isFirstCallAfterHotkey = true;

        // History
        public static string historyFontFamily = "Sergoe UI"; // default
        public static float historyFontSize = 11; // default
        public static int historyListElements = 1; // default
        public static string historySize = "Medium"; // default
        public static int historyListWidthSmall = 350;
        public static int historyListWidthMedium = 500;
        public static int historyListWidthLarge = 650;
        public static int[] historyBoxHeightSmall = new int[] { 157, 87, 65, 48, 28, 28, 28, 28, 28, 28 };
        public static int[] historyBoxHeightMedium = new int[] { 250, 152, 83, 65, 48, 48, 48, 48, 48, 48 };
        public static int[] historyBoxHeightLarge = new int[] { 320, 170, 120, 100, 84, 84, 84, 84, 84, 84 };
        private static string historyColorTopBlue = "#dae1e7";
        private static string historyColorBottomBlue = "#f5faff";
        private static string historyColorTopBrown = "#dac1a0";
        private static string historyColorBottomBrown = "#eee3d5";
        private static string historyColorTopGreen = "#c1dac1";
        private static string historyColorBottomGreen = "#eaf2ea";
        private static string historyColorTopWhite = "#e7e7e7";
        private static string historyColorBottomWhite = "#ffffff";
        private static string historyColorTopYellow = "#eee8aa";
        private static string historyColorBottomYellow = "#ffffe1";
        public static string historyColorTop = "#eee8aa"; // default
        public static string historyColorBottom = "#ffffe1"; // default
        public static string historyLocation = "Right Bottom"; // default

        // Registry
        public string registryPath = "SOFTWARE\\HovText";
        private string registryCheckUpdates = "1"; // default, 1 = check for updates
        private string registryCloseMinimizes = "1"; // default, 1 = minimize to tray
        private string registryRestoreOriginal = "1"; // default, 1 = restore original
        private string registryCopyImages = "1"; // default, 1 = copy images to history
        private string registryEnableHistory = "1"; // default, 1 = enable history
        private string registryPasteOnSelection = "0"; // default, 0 = do not paste selected entry when selected
        private string registryTrimWhitespaces = "1"; // default, 1 = trim whitespaces
        private string registryHistoryColor = "Yellow"; // default
        private string registryHistorySize = "Medium"; // default

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
        Image clipboardImage = null;
        string clipboardImageHashLast = "";
        IDataObject clipboardObject;
        public static SortedDictionary<int, string> entriesApplication = new SortedDictionary<int, string>();
        public static SortedDictionary<int, string> entriesText = new SortedDictionary<int, string>();
        public static SortedDictionary<int, Image> entriesImage = new SortedDictionary<int, Image>();
        SortedList<int, Dictionary<string, object>> entriesOriginal = new SortedList<int, Dictionary<string, object>>();
        const int WM_CLIPBOARDUPDATE = 0x031D;
        string whoUpdatedClipboardName = "";

        // Misc
        public static bool isSettingsFormVisible = false;
        public static string hovtextPage = "https://hovtext.com/";
        public static string appDate = "";
        internal static Settings settings;
        HistoryList historyList = new HistoryList();
        Update update = new Update();
        HotkeyConflict hotkeyConflict = new HotkeyConflict();


    // ###########################################################################################
    // Main
    // ###########################################################################################

    public Settings()
        {
            // Refering to the current form - used in the history form
            settings = this;

            InitializeComponent();

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
            day = day.TrimStart(new Char[] {'0'}); // remove leading zero
            day = day.TrimEnd(new Char[] { '.' }); // remove last dot
            string date = year + "-" + month + "-" + day;
            appDate = date;
            appVer.Text = "Version " + appDate;

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
            sb.Append(@" This application is open source and you can use it on any computer you want without any cost. You are of course encouraged to donate some $$$, if you use it and want to motivate for continues support :-) It is \ul not\ulnone  allowed to sell or distribute it in any commercial regard. \line ");
            sb.Append(@" \line ");
            sb.Append(@" Visit the HovText home page where you also can contact the developers: \line ");
            sb.Append(@" https://hovtext.com/ \line ");
            sb.Append(@" \line ");
            sb.Append(@" Kind regards from Jesper and Dennis ");
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
                    uint thisProcessId = 0;
                    IntPtr whoUpdatedClipboardHwnd = Program.GetClipboardOwner();
                    Program.GetWindowThreadProcessId(whoUpdatedClipboardHwnd, out thisProcessId);
                    whoUpdatedClipboardName = Process.GetProcessById((int)thisProcessId).ProcessName;
                    Console.WriteLine("Clipboard UPDATE event from [" + whoUpdatedClipboardName + "]");

                    // I am not sure why some(?) applications are returned as "Idle" when coming from clipboard - in this case the get the active application and use that name instead
                    // This could potentially be a problem, if a process is correctly called "Idle" but not sure if this is realistic?
                    if (whoUpdatedClipboardName.ToLower() == "idle")
                    {
                        string activeProcessName = GetActiveApplication();
                        whoUpdatedClipboardName = activeProcessName;
                        Console.WriteLine("Finding process name the secondary way, [" + activeProcessName + "]");
                    }

                    // Check if application is enabled
                    if (uiAppEnabled.Checked)
                    {
                        ProcessClipboard();
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
                    if (format.ToUpper() != "TEXT")
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

                    if (clipboardText != "")
                    {
                        // Set the last clipboard text to be identical to this one
                        clipboardTextLast = clipboardText;

                        // Add text to the entries array and update the clipboard
                        AddEntry();
                        GetEntryCounter();
                        SetClipboard(false, "WM_CLIPBOARDUPDATE:TEXT");
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

                        // Add the image to the entries array and update the clipboard
                        AddEntry();
                        GetEntryCounter();
                        SetClipboard(false, "WM_CLIPBOARDUPDATE:IMAGE");
                    }
                }
            }
            else
            // We have come in to here, if clipboard has changed but it does not contains a text or an image
            // It could be empty or e.g. a file copy

            if (Clipboard.GetText() == "")
            {
                // If active application is EXCEL then restore last entry, as EXCEL clears clipboard when 
                if (entriesText.Count() > 0 && whoUpdatedClipboardName.ToUpper() == "EXCEL")
                {
                    // Restore the last entry to the clipboard
                    SetClipboard(false, "WM_CLIPBOARDUPDATE:EMPTY-TEXT-2");
                    clipboardTextLast = Clipboard.GetText();
                }
            }
        }


        // ###########################################################################################
        // Check if clipboard content is already in the data arrays - if so then move the entry to top
        // ###########################################################################################

        private bool IsClipboardContentAlreadyInDataArrays()
        {
            if (entriesText.Count() > 0)
            {
                for (int i = 0; i < entriesText.Count(); i++)
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
            if (isEnabledHistory)
            {
                bool isAlreadyInDataArray = IsClipboardContentAlreadyInDataArrays();

                if (!isAlreadyInDataArray)
                {
                    // If this is the first time then set the index to 0
                    entryIndex = entryIndex >= 0 ? entriesText.Keys.Last() + 1 : 0;

                    // Add the text and image to the entries array
                    entriesText.Add(entryIndex, clipboardText);
                    entriesImage.Add(entryIndex, clipboardImage);

                    // Walk through all (relevant) clipboard object formats and store them
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
                            format.Contains("Bitmap")
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
        }


        // ###########################################################################################
        // Place data in the clipboard based on the entry index
        // ###########################################################################################

        private void SetClipboard(bool original, string from)
        {
            string entryText = clipboardText;
            Image entryImage = clipboardImage;
            bool isEntryText = isClipboardText;
            bool isEntryImage = isClipboardImage;

            if (isEnabledHistory)
            {
                if (entriesText.Count() > 0)
                {
                    entryText = entriesText[entryIndex];
                    entryImage = entriesImage[entryIndex];
                    isEntryText = entryText == "" ? false : true;
                    isEntryImage = entryImage == null ? false : true;
                }
            }

            // Put text to the clipboard
            if (isEntryText)
            {
                try
                {
                    Clipboard.SetText(entryText, TextDataFormat.Text);
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
                    Clipboard.SetImage(entryImage);
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
                    historyList.UpdateHistoryList("down");
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
                    historyList.UpdateHistoryList("up");
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

                // Set the clipboard with the new data
                SetClipboard(false, "SelectHistoryEntry");

                // Set focus back to the originating application
                ChangeFocusToOriginatingApplication();

                // Reset some stuff
                isFirstCallAfterHotkey = true;
                entryIndex = entriesText.Keys.Last();
                GetEntryCounter();

                // Hide history UI
                historyList.Hide();
            }
        }


        // ###########################################################################################
        // Convert the entry index to which logical number it is
        // ###########################################################################################

        private void GetEntryCounter()
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
            entriesApplication.Add(insertKey, entriesApplication[entryIndex]);
            entriesOriginal.Add(insertKey, entriesOriginal[entryIndex]);

            // Remove the chosen entry, so it does not show duplicates
            entriesText.Remove(entryIndex);
            entriesImage.Remove(entryIndex);
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

                // Change the icons to be green (active)
                Icon = Resources.Active;
                notifyIcon.Icon = Resources.Active;

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

                // Enable other checkboxes
                uiHistoryEnabled.Enabled = true;
                uiRestoreOriginal.Enabled = true;
                uiCopyImages.Enabled = true;
                uiTrimWhitespaces.Enabled = true;
                uiPasteOnSelection.Enabled = true;
                uiHotkeyOlder.Enabled = true;
                uiHotkeyNewer.Enabled = true;
            }
            else
            {
                // Remove this application from the clipboard chain
                Program.RemoveClipboardFormatListener(this.Handle);

                // Restore the original clipboard format
                if (isRestoreOriginal && entriesOriginal.Count() > 0)
                {
                    try
                    {
                        //                    Clipboard.Clear();

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
                        Clipboard.SetDataObject(data, true);
                        // ---
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("EXCEPTION 5 - please report to developer: [" + ex.ToString() + "]");
                    }
                }

                // Change the icons to be red (inactive)
                Icon = Resources.Inactive;
                notifyIcon.Icon = Resources.Inactive;

                // Disable other checkboxes
                uiHistoryEnabled.Enabled = false;
                uiRestoreOriginal.Enabled = false;
                uiCopyImages.Enabled = false;
                uiTrimWhitespaces.Enabled = false;
                uiPasteOnSelection.Enabled = false;
                uiHotkeyOlder.Enabled = false;
                uiHotkeyNewer.Enabled = false;
                uiHotkeyPaste.Enabled = false;
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

        private string GetActiveApplication()
        {
            // Get the active application
            originatingHandle = Program.GetForegroundWindow();

            // Get the process ID and find the name for that ID
            uint processId = 0;
            Program.GetWindowThreadProcessId(originatingHandle, out processId);
            string appProcessName = Process.GetProcessById((int)processId).ProcessName;
            Console.WriteLine("Active application is ["+ appProcessName +"]");
            return appProcessName;
        }


        // ###########################################################################################
        // When pressing one of the history hotkeys then change focus to this application to prevent the keypresses go in to the active application
        // ###########################################################################################

        private void ChangeFocusToThisApplication()
        {
            Program.SetForegroundWindow(this.Handle);
            Console.WriteLine("Set focus to HovText");
        }


        // ###########################################################################################
        // When an entry has been submitted to the clipboard then pass back focus to the originating application
        // ###########################################################################################

        private void ChangeFocusToOriginatingApplication()
        {
            Program.SetForegroundWindow(originatingHandle);
            Console.WriteLine("Set focus to originating application");
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
                client.Headers.Add("user-agent", "HovText "+ appDate);
                string checkedVersion = client.DownloadString(hovtextPage+"autoupdate/");
                if (checkedVersion.Substring(0,7) == "Version")
                {
                    checkedVersion = checkedVersion.Substring(9);
                    update.uiAppVerYours.Text = appDate;
                    update.uiAppVerOnline.Text = checkedVersion;
                    string lastCheckedVersion = GetRegistryKey(registryPath, "CheckedVersion");
                    if (lastCheckedVersion != checkedVersion && checkedVersion != appDate)
                    {
                        update.Show();
                    }
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine("No communication to HovText update checker page");
            }
        }


        // ###########################################################################################
        // Initialize registry - check if all registry keys have been created
        // ###########################################################################################

        private void InitializeRegistry()
        {
            // Check if the "HovText" path exists in SOFTWARE - if not then create it
            using (RegistryKey registryPathExists = Registry.CurrentUser.OpenSubKey(registryPath))
            {
                if (registryPathExists == null)
                {
                    RegistryKey registryApp = Registry.CurrentUser.CreateSubKey(registryPath);
                }
            }

            // Check if all registry keys are set - if not the set default values
            using (RegistryKey registryPathExists = Registry.CurrentUser.OpenSubKey(registryPath, true))
            {
                // NotificationShown
                if (registryPathExists.GetValue("NotificationShown") == null)
                {
                    registryPathExists.SetValue("NotificationShown", "0");
                }
                // CheckedVersion
                if (registryPathExists.GetValue("CheckedVersion") == null)
                {
                    registryPathExists.SetValue("CheckedVersion", appDate);
                }
                // CheckUpdates
                if (registryPathExists.GetValue("CheckUpdates") == null)
                {
                    registryPathExists.SetValue("CheckUpdates", registryCheckUpdates);
                }
                // CloseMinimizes
                if (registryPathExists.GetValue("CloseMinimizes") == null)
                {
                    registryPathExists.SetValue("CloseMinimizes", registryCloseMinimizes);
                }
                // RestoreOriginal
                if (registryPathExists.GetValue("RestoreOriginal") == null)
                {
                    registryPathExists.SetValue("RestoreOriginal", registryRestoreOriginal);
                }
                // CopyImages
                if (registryPathExists.GetValue("CopyImages") == null)
                {
                    registryPathExists.SetValue("CopyImages", registryCopyImages);
                }
                // EnablePasteOnSelection
                if (registryPathExists.GetValue("EnablePasteOnSelection") == null)
                {
                    registryPathExists.SetValue("EnablePasteOnSelection", registryPasteOnSelection);
                }
                // EnableTrimWhitespaces
                if (registryPathExists.GetValue("EnableTrimWhitespaces") == null)
                {
                    registryPathExists.SetValue("EnableTrimWhitespaces", registryTrimWhitespaces);
                }
                // HistoryEntries
                if (registryPathExists.GetValue("HistoryEntries") == null)
                {
                    registryPathExists.SetValue("HistoryEntries", historyListElements.ToString());
                }
                // HistoryColor
                if (registryPathExists.GetValue("HistoryColor") == null)
                {
                    // Breaking change in version 2020-X-Y where this was renamed - can be removed after a couple of versions
                    if (registryPathExists.GetValue("ThemeColor") != null)
                    {
                        string tmpRegistryHistoryColor = GetRegistryKey(registryPath, "ThemeColor");
                        if (tmpRegistryHistoryColor.Length > 0)
                        {
                            registryPathExists.SetValue("HistoryColor", tmpRegistryHistoryColor);
                            DeleteRegistryKey(registryPath, "ThemeColor");
                        }
                    }
                    else
                    {
                        registryPathExists.SetValue("HistoryColor", registryHistoryColor);
                    }
                }
                // HistoryEnabled
                if (registryPathExists.GetValue("HistoryEnable") == null)
                {
                    // Breaking change in version 2020-X-Y where this was renamed - can be removed after a couple of versions
                    if (registryPathExists.GetValue("EnableHistory") != null)
                    {
                        string tmpRegistry = GetRegistryKey(registryPath, "EnableHistory");
                        if (tmpRegistry.Length > 0)
                        {
                            registryPathExists.SetValue("HistoryEnable", tmpRegistry);
                            DeleteRegistryKey(registryPath, "EnableHistory");
                        }
                    }
                    else
                    {
                        registryPathExists.SetValue("HistoryEnable", registryEnableHistory);
                    }
                }
                // HistoryFontFamily
                if (registryPathExists.GetValue("HistoryFontFamily") == null)
                {
                    // Breaking change in version 2020-X-Y where this was renamed - can be removed after a couple of versions
                    if (registryPathExists.GetValue("FontFamily") != null)
                    {
                        string tmpRegistry = GetRegistryKey(registryPath, "FontFamily");
                        if (tmpRegistry.Length > 0)
                        {
                            registryPathExists.SetValue("HistoryFontFamily", tmpRegistry);
                            DeleteRegistryKey(registryPath, "FontFamily");
                        }
                    }
                    else
                    {
                        registryPathExists.SetValue("HistoryFontFamily", historyFontFamily);
                    }
                }
                // HistoryFontSize
                if (registryPathExists.GetValue("HistoryFontSize") == null)
                {
                    // Breaking change in version 2020-X-Y where this was renamed - can be removed after a couple of versions
                    if (registryPathExists.GetValue("FontSize") != null)
                    {
                        string tmpRegistry = GetRegistryKey(registryPath, "FontSize");
                        if (tmpRegistry.Length > 0)
                        {
                            registryPathExists.SetValue("HistoryFontSize", tmpRegistry);
                            DeleteRegistryKey(registryPath, "FontSize");
                        }
                    }
                    else
                    {
                        registryPathExists.SetValue("HistoryFontSize", historyFontSize);
                    }
                }
                // HistoryLocation
                if (registryPathExists.GetValue("HistoryLocation") == null)
                {
                    registryPathExists.SetValue("HistoryLocation", historyLocation);
                }
                // HistorySize
                if (registryPathExists.GetValue("HistorySize") == null)
                {
                    // Breaking change in version 2020-X-Y where this was renamed - can be removed after a couple of versions
                    if (registryPathExists.GetValue("AreaSize") != null)
                    {
                        string tmpRegistryHistorySize = GetRegistryKey(registryPath, "AreaSize");
                        if (tmpRegistryHistorySize.Length > 0)
                        {
                            registryPathExists.SetValue("HistorySize", tmpRegistryHistorySize);
                            DeleteRegistryKey(registryPath, "AreaSize");
                        }
                    }
                    else
                    {
                        registryPathExists.SetValue("HistorySize", registryHistorySize);
                    }
                }
                // Hotkey1
                if (registryPathExists.GetValue("Hotkey1") == null)
                {
                    registryPathExists.SetValue("Hotkey1", hotkeyToggleApplication);
                }
                // Hotkey2
                if (registryPathExists.GetValue("Hotkey2") == null)
                {
                    registryPathExists.SetValue("Hotkey2", hotkeyGetOlderEntry);
                }
                // Hotkey3
                if (registryPathExists.GetValue("Hotkey3") == null)
                {
                    registryPathExists.SetValue("Hotkey3", hotkeyGetNewerEntry);
                }
                // Hotkey4
                if (registryPathExists.GetValue("Hotkey4") == null)
                {
                    registryPathExists.SetValue("Hotkey4", hotkeyPasteOnHotkey);
                }
                // HotkeyBehaviour
                if (registryPathExists.GetValue("HotkeyBehaviour") == null)
                {
                    registryPathExists.SetValue("HotkeyBehaviour", hotkeyBehaviour);
                }
            }
        }


        // ###########################################################################################
        // Get regsitry key value
        // ###########################################################################################

        private string GetRegistryKey(string path, string key)
        {
            using (RegistryKey getKey = Registry.CurrentUser.OpenSubKey(path))
            {
                return (string)(getKey.GetValue(key));
            }
        }


        // ###########################################################################################
        // Set registry key value
        // ###########################################################################################

        public void SetRegistryKey(string path, string key, string value)
        {
            using (RegistryKey setKey = Registry.CurrentUser.OpenSubKey(path, true))
            {
                setKey.SetValue(key, value);
            }
        }


        // ###########################################################################################
        // Delete registry key
        // ###########################################################################################

        private void DeleteRegistryKey(string path, string key)
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
            int pasteOnSelection = int.Parse((string)GetRegistryKey(registryPath, "EnablePasteOnSelection"));
            uiPasteOnSelection.Checked = pasteOnSelection == 1 ? true : false;
            isEnabledPasteOnSelection = uiPasteOnSelection.Checked;

            // Trim whitespaces
            int trimWhitespaces = int.Parse((string)GetRegistryKey(registryPath, "EnableTrimWhitespaces"));
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
            SetHotkeys("Startup");

            // ------------------------------------------
            // "Apperance" tab
            // ------------------------------------------

            // History entries
            historyListElements = Int32.Parse(GetRegistryKey(registryPath, "HistoryEntries"));
            switch (historyListElements)
            {
                case 2:
                    uiElement2.Checked = true;
                    break;
                case 3:
                    uiElement3.Checked = true;
                    break;
                case 4:
                    uiElement4.Checked = true;
                    break;
                case 5:
                    uiElement5.Checked = true;
                    break;
                case 6:
                    uiElement6.Checked = true;
                    break;
                case 7:
                    uiElement7.Checked = true;
                    break;
                case 8:
                    uiElement8.Checked = true;
                    break;
                case 9:
                    uiElement9.Checked = true;
                    break;
                case 10:
                    uiElement10.Checked = true;
                    break;
                default:
                    uiElement1.Checked = true;
                    break;
            }
            HideHistoryBoxes();

            // History area
            string historySizeRegistry = GetRegistryKey(registryPath, "HistorySize");
            switch (historySizeRegistry)
            {
                case "Small":
                    uiAreaSmall.Checked = true;
                    historySize = "Small";
                    break;
                case "Large":
                    uiAreaLarge.Checked = true;
                    historySize = "Large";
                    break;
                default: // Medium
                    uiAreaMedium.Checked = true;
                    historySize = "Medium";
                    break;
            }

            // History color theme
            registryHistoryColor = GetRegistryKey(registryPath, "HistoryColor");
            SetHistoryColors(registryHistoryColor);
            switch (registryHistoryColor)
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
                case "White":
                    uiHistoryColorWhite.Checked = true;
                    break;
                default: // Yellow
                    uiHistoryColorYellow.Checked = true;
                    break;
            }

            // History location
            historyLocation = GetRegistryKey(registryPath, "HistoryLocation");
            switch (historyLocation)
            {
                case "Left Top":
                    uiHistoryLocationLeftTop.Checked = true;
                    break;
                case "Left Bottom":
                    uiHistoryLocationLeftBottom.Checked = true;
                    break;
                case "Center":
                    uiHistoryLocationCenter.Checked = true;
                    break;
                case "Right Top":
                    uiHistoryLocationRightTop.Checked = true;
                    break;
                default: // Right Bottom
                    uiHistoryLocationRightBottom.Checked = true;
                    break;
            }

            // History font
            historyFontFamily = GetRegistryKey(registryPath, "HistoryFontFamily");
            historyFontSize = float.Parse((string)GetRegistryKey(registryPath, "HistoryFontSize"));
            uiShowFontBottom.Font = new Font(historyFontFamily, historyFontSize);
            uiShowFontBottom.Text = historyFontFamily + ", " + historyFontSize;
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
                uiShowFontBottom.Text = historyFontFamily + ", " + historyFontSize;
                uiShowFontBottom.Font = new Font(historyFontFamily, historyFontSize);
                SetRegistryKey(registryPath, "HistoryFontFamily", historyFontFamily);
                SetRegistryKey(registryPath, "HistoryFontSize", historyFontSize.ToString());
            }
        }


        // ###########################################################################################
        // Changes in the history elements
        // ###########################################################################################
                
        private void uiElement1_CheckedChanged(object sender, EventArgs e)
        {
            historyListElements = 1;
            SetRegistryKey(registryPath, "HistoryEntries", historyListElements.ToString());
            HideHistoryBoxes();
        }

        private void uiElement2_CheckedChanged(object sender, EventArgs e)
        {
            historyListElements = 2;
            SetRegistryKey(registryPath, "HistoryEntries", historyListElements.ToString());
            HideHistoryBoxes();
        }

        private void uiElement3_CheckedChanged(object sender, EventArgs e)
        {
            historyListElements = 3;
            SetRegistryKey(registryPath, "HistoryEntries", historyListElements.ToString());
            HideHistoryBoxes();
        }

        private void uiElement4_CheckedChanged(object sender, EventArgs e)
        {
            historyListElements = 4;
            SetRegistryKey(registryPath, "HistoryEntries", historyListElements.ToString());
            HideHistoryBoxes();
        }

        private void uiElement5_CheckedChanged(object sender, EventArgs e)
        {
            historyListElements = 5;
            SetRegistryKey(registryPath, "HistoryEntries", historyListElements.ToString());
            HideHistoryBoxes();
        }

        private void uiElement6_CheckedChanged(object sender, EventArgs e)
        {
            historyListElements = 6;
            SetRegistryKey(registryPath, "HistoryEntries", historyListElements.ToString());
            HideHistoryBoxes();
        }

        private void uiElement7_CheckedChanged(object sender, EventArgs e)
        {
            historyListElements = 7;
            SetRegistryKey(registryPath, "HistoryEntries", historyListElements.ToString());
            HideHistoryBoxes();
        }

        private void uiElement8_CheckedChanged(object sender, EventArgs e)
        {
            historyListElements = 8;
            SetRegistryKey(registryPath, "HistoryEntries", historyListElements.ToString());
            HideHistoryBoxes();
        }

        private void uiElement9_CheckedChanged(object sender, EventArgs e)
        {
            historyListElements = 9;
            SetRegistryKey(registryPath, "HistoryEntries", historyListElements.ToString());
            HideHistoryBoxes();
        }

        private void uiElement10_CheckedChanged(object sender, EventArgs e)
        {
            historyListElements = 10;
            SetRegistryKey(registryPath, "HistoryEntries", historyListElements.ToString());
            HideHistoryBoxes();
        }
        
        private void HideHistoryBoxes()
        {
            historyList.HideBox(1);
            historyList.HideBox(2);
            historyList.HideBox(3);
            historyList.HideBox(4);
            historyList.HideBox(5);
            historyList.HideBox(6);
            historyList.HideBox(7);
            historyList.HideBox(8);
            historyList.HideBox(9);
            historyList.HideBox(10);
        }


        // ###########################################################################################
        // Changes in the history (area) size
        // ###########################################################################################

        private void uiAreaSmall_CheckedChanged(object sender, EventArgs e)
        {
            historySize = "Small";
            SetRegistryKey(registryPath, "HistorySize", "Small");
        }

        private void uiAreaMedium_CheckedChanged(object sender, EventArgs e)
        {
            historySize = "Medium";
            SetRegistryKey(registryPath, "HistorySize", "Medium");
        }

        private void uiAreaLarge_CheckedChanged(object sender, EventArgs e)
        {
            historySize = "Large";
            SetRegistryKey(registryPath, "HistorySize", "Large");
        }


        // ###########################################################################################
        // Change in the history location
        // ###########################################################################################

        private void historyLocationLeftTop_CheckedChanged(object sender, EventArgs e)
        {
            historyLocation = "Left Top";
            SetRegistryKey(registryPath, "HistoryLocation", historyLocation);
        }

        private void historyLocationLeftBottom_CheckedChanged(object sender, EventArgs e)
        {
            historyLocation = "Left Bottom";
            SetRegistryKey(registryPath, "HistoryLocation", historyLocation);
        }

        private void historyLocationCenter_CheckedChanged(object sender, EventArgs e)
        {
            historyLocation = "Center";
            SetRegistryKey(registryPath, "HistoryLocation", historyLocation);
        }

        private void historyLocationRightTop_CheckedChanged(object sender, EventArgs e)
        {
            historyLocation = "Right Top";
            SetRegistryKey(registryPath, "HistoryLocation", historyLocation);
        }

        private void historyLocationRightBottom_CheckedChanged(object sender, EventArgs e)
        {
            historyLocation = "Right Bottom";
            SetRegistryKey(registryPath, "HistoryLocation", historyLocation);
        }


        // ###########################################################################################
        // Change in the history color
        // ###########################################################################################

        private void uiHistoryColorBlue_CheckedChanged(object sender, EventArgs e)
        {
            registryHistoryColor = "Blue";
            SetRegistryKey(registryPath, "HistoryColor", registryHistoryColor);
            SetHistoryColors(registryHistoryColor);
        }

        private void uiHistoryColorBrown_CheckedChanged(object sender, EventArgs e)
        {
            registryHistoryColor = "Brown";
            SetRegistryKey(registryPath, "HistoryColor", registryHistoryColor);
            SetHistoryColors(registryHistoryColor);
        }

        private void uiHistoryColorGreen_CheckedChanged(object sender, EventArgs e)
        {
            registryHistoryColor = "Green";
            SetRegistryKey(registryPath, "HistoryColor", registryHistoryColor);
            SetHistoryColors(registryHistoryColor);
        }

        private void uiHistoryColorWhite_CheckedChanged(object sender, EventArgs e)
        {
            registryHistoryColor = "White";
            SetRegistryKey(registryPath, "HistoryColor", registryHistoryColor);
            SetHistoryColors(registryHistoryColor);
        }

        private void uiHistoryColorYellow_CheckedChanged(object sender, EventArgs e)
        {
            registryHistoryColor = "Yellow";
            SetRegistryKey(registryPath, "HistoryColor", registryHistoryColor);
            SetHistoryColors(registryHistoryColor);
        }


        // ###########################################################################################
        // Set the history colors (and selected value in UI)
        // ###########################################################################################

        private void SetHistoryColors(string historyColor)
        {
            switch (historyColor)
            {
                case "Blue":
                    historyColorTop = historyColorTopBlue;
                    historyColorBottom = historyColorBottomBlue;
                    uiShowFontTop.BackColor = ColorTranslator.FromHtml(historyColorTopBlue); 
                    uiShowFontBottom.BackColor = ColorTranslator.FromHtml(historyColorBottomBlue);
                    break;
                case "Brown":
                    historyColorTop = historyColorTopBrown;
                    historyColorBottom = historyColorBottomBrown;
                    uiShowFontTop.BackColor = ColorTranslator.FromHtml(historyColorTopBrown); 
                    uiShowFontBottom.BackColor = ColorTranslator.FromHtml(historyColorBottomBrown);
                    break;
                case "Green":
                    historyColorTop = historyColorTopGreen;
                    historyColorBottom = historyColorBottomGreen;
                    uiShowFontTop.BackColor = ColorTranslator.FromHtml(historyColorTopGreen); 
                    uiShowFontBottom.BackColor = ColorTranslator.FromHtml(historyColorBottomGreen);
                    break;
                case "White":
                    historyColorTop = historyColorTopWhite;
                    historyColorBottom = historyColorBottomWhite;
                    uiShowFontTop.BackColor = ColorTranslator.FromHtml(historyColorTopWhite);
                    uiShowFontBottom.BackColor = ColorTranslator.FromHtml(historyColorBottomWhite);
                    break;
                default:
                    historyColorTop = historyColorTopYellow;
                    historyColorBottom = historyColorBottomYellow;
                    uiShowFontTop.BackColor = ColorTranslator.FromHtml(historyColorTopYellow);
                    uiShowFontBottom.BackColor = ColorTranslator.FromHtml(historyColorBottomYellow);
                    break;
            }
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
            SetRegistryKey(registryPath, "EnablePasteOnSelection", status);
        }


        // ###########################################################################################
        // Changes in "Trim whitespaces
        // ###########################################################################################

        private void uiTrimWhitespaces_CheckedChanged(object sender, EventArgs e)
        {
            string status = uiTrimWhitespaces.Checked ? "1" : "0";
            isEnabledTrimWhitespacing = uiTrimWhitespaces.Checked;
            SetRegistryKey(registryPath, "EnableTrimWhitespaces", status);
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
                Console.WriteLine("Start mouse double-click timer");
            }
        }

        private void mouseClickTimer_Tick(object sender, EventArgs e)
        {
            Console.WriteLine("Mouse single-click detected");
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

                Console.WriteLine("Mouse double-click detected");
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
            WindowState = FormWindowState.Normal;
        }


        // ###########################################################################################
        // Changes in "One entry only" look-and-feel
        // ###########################################################################################

        private void uiApperanceOne_CheckedChanged(object sender, EventArgs e)
        {
            SetRegistryKey(registryPath, "HistoryLook", "One entry only");
        }


        // ###########################################################################################
        // Changes in "List of entries" look-and-feel
        // ###########################################################################################

        private void uiApperanceList_CheckedChanged(object sender, EventArgs e)
        {
            SetRegistryKey(registryPath, "HistoryLook", "List of entries");
        }



        // ###########################################################################################
        // Changes in "Always action" hotkey behaviour
        // ###########################################################################################

        private void uiHotkeyBehaviourSystem_CheckedChanged(object sender, EventArgs e)
        {
            uiHotkeyPaste.Enabled = false;
            SetRegistryKey(registryPath, "HotkeyBehaviour", "System");
        }


        // ###########################################################################################
        // Changes in "Action only on hotkey" hotkey behaviour
        // ###########################################################################################

        private void uiHotkeyBehaviourPaste_CheckedChanged(object sender, EventArgs e)
        {
            uiHotkeyPaste.Enabled = true;
            SetRegistryKey(registryPath, "HotkeyBehaviour", "Paste");
        }


        // ###########################################################################################
        // Clicking the tab help button
        // ###########################################################################################

        private void uiHelp_Click(object sender, EventArgs e)
        {
            string selectedTab = tabControl.SelectedTab.AccessibilityObject.Name;
            System.Diagnostics.Process.Start(hovtextPage+"documentation/#" + selectedTab);
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
            Console.WriteLine("Pressed the [Enable/Disable application] hotkey");
            ToggleEnabled();
            e.Handled = true;
        }


        // ###########################################################################################
        // Action for the "Show older entry" hotkey
        // ###########################################################################################

        private void HotkeyGetOlderEntry(object sender, NHotkey.HotkeyEventArgs e)
        {
            Console.WriteLine("Pressed the [Get older history entry] hotkey");
            GoEntryLowerNumber();
            isHistoryHotkeyPressed = true;
            e.Handled = true;
        }


        // ###########################################################################################
        // Action for the "Show newer entry" hotkey
        // ###########################################################################################

        private void HotkeyGetNewerEntry(object sender, NHotkey.HotkeyEventArgs e)
        {
            Console.WriteLine("Pressed the [Get newer history entry] hotkey");
            GoEntryHigherNumber();
            isHistoryHotkeyPressed = true;
            e.Handled = true;
        }


        // ###########################################################################################
        // Action for the "Paste on hotkey" hotkey
        // ###########################################################################################

        private void HotkeyPasteOnHotkey(object sender, NHotkey.HotkeyEventArgs e)
        {
            Console.WriteLine("Pressed the [Paste only on hotkey] hotkey");
            SelectHistoryEntry();
            SendKeys.Send("^v");
            e.Handled = true;
        }


        // ###########################################################################################
        // On "key down" in one of the hotkey fields then convert that in to a string
        // ###########################################################################################

        private string ConvertKeyboardInputToString(KeyEventArgs e)
        {
            Console.WriteLine("Set hotkey: [" + e.KeyData + "] [" + e.KeyCode + "] [" + e.Modifiers + "] [" + e.SuppressKeyPress + "]");

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
            hotkey = hotkey + keyCode;

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

        private void RemoveAllHotkeys()
        {
            HotkeyManager.Current.Remove("ToggleApplication");
            HotkeyManager.Current.Remove("GetOlderEntry");
            HotkeyManager.Current.Remove("GetNewerEntry");
//            HotkeyManager.Current.Remove("PasteOnHotkey");
        }


        // ###########################################################################################
        // Apply the hotkeys
        // ###########################################################################################

        private void ApplyHotkeys_Click(object sender, EventArgs e)
        {
            SetHotkeys("Apply");
        }


        // ###########################################################################################
        // Set the hotkeys and validate them
        // ###########################################################################################

        private void SetHotkeys(string from)
        {
            // Get all hotkey strings
            string hotkey1 = uiHotkeyEnable.Text;
            string hotkey2 = uiHotkeyOlder.Text;
            string hotkey3 = uiHotkeyNewer.Text;
            string hotkey4 = uiHotkeyPaste.Text;

            string conflictText = "";

            // Convert the strings to hotkey objects

            KeysConverter cvt;
            Keys key;

            // Activate the hotkeys

            // Hotkey 1
            if (hotkey1 != "Unsupported" && hotkey1 != "Not set" && hotkey1 != "Hotkey conflicts")
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
                        conflictText = conflictText + "Hotkey for \"Toggle application on/off\" conflicts with another application\r\n";
                    }
                }
            }

            // Hotkey 2
            if (hotkey2 != "Unsupported" && hotkey2 != "Not set" && hotkey2 != "Hotkey conflicts")
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
                        conflictText = conflictText + "Hotkey for \"Show older entry\" conflicts with another application\r\n";
                    }
                }
            }

            // Hotkey 3
            if (hotkey3 != "Unsupported" && hotkey3 != "Not set" && hotkey3 != "Hotkey conflicts")
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
                        conflictText = conflictText + "Hotkey for \"Show newer entry\" conflicts with another application\r\n";
                    }
                }
            }

/*
            // Hotkey 4
            if (hotkey4 != "Unsupported" && hotkey4 != "Not set" && hotkey4 != "Hotkey conflicts")
            {
                cvt = new KeysConverter();
                key = (Keys)cvt.ConvertFrom(hotkey4);
                try
                {
                    HotkeyManager.Current.AddOrReplace("PasteOnHotkey", key, HotkeyPasteOnHotkey);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Hot key is already registered"))
                    {
                        hotkey4 = "Hotkey conflicts";
                        conflictText = conflictText + "Hotkey for \"Paste on hotkey\" conflicts with another application\r\n";
                    }
                }
            }
*/

            // If this is called from startup then show an error, if there is a conflict
            if (conflictText.Length > 0 && from == "Startup")
            {
                hotkeyConflict.label2.Text = conflictText;
                hotkeyConflict.Show();
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

            // Hotkey 1
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

            // Hotkey 2
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

            // Hotkey 3
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

            // Hotkey 4
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
            SetHotkeys("Cancel");
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
                int entries = entriesText.Count();
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
    }
}
