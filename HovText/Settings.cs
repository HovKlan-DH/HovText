using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net;
using Microsoft.Win32;
using HovText.Properties;
using WK.Libraries.HotkeyListenerNS; // https://github.com/Willy-Kimura/HotkeyListener
//using NHotkey.WindowsForms; // https://github.com/thomaslevesque/NHotkey

namespace HovText
{
    public partial class Settings : Form
    {
        internal static Settings settings;

        History history = new History();
        Update update = new Update();

        // Define class variables - real spaghetti
        public static string hovtextPage = "https://hovtext.com/";
        public static bool isFirstCall = true; // is this the first time after ALT hotkey has been pressed
        SortedDictionary<int, string> entriesApplication = new SortedDictionary<int, string>();
        SortedDictionary<int, string> entriesText = new SortedDictionary<int, string>();
        SortedDictionary<int, Image> entriesImage = new SortedDictionary<int, Image>();
        int entryIndex = -1;
        int entryCounter = -1;
        bool isClipboardText = false;
        bool isClipboardImage = false;
        string clipboardText = "";
        string clipboardTextLast = "";
        Image clipboardImage = null;
        string clipboardImageHashLast = "";
        IDataObject clipboardObject = null;
        public static IntPtr originatingHandle = IntPtr.Zero;
        public static float fontSize = 11; // "static" makes it available in the history form
        public static string fontFamily = "Sergoe UI";
        public static int historyWidth = 500; // medium
        public static int historyHeight = 250; // medium
        public static double historySize = 1.1; // small=0.7, medium=1.1, large=1.5
        public static string themeColor = "Yellow";
        public static string colorYellowTop = "#eee8aa";
        public static string colorYellowBottom = "#ffffe1";
        public static string colorBlueTop = "#dae1e7";
        public static string colorBlueBottom = "#f5faff";
        public static string colorGreenTop = "#c1dac1";
        public static string colorGreenBottom = "#eaf2ea";
        public static string colorBrownTop = "#dac1a0";
        public static string colorBrownBottom = "#eee3d5";
        public static string colorWhiteTop = "#e7e7e7";
        public static string colorWhiteBottom = "#ffffff";
        public static bool isEnabledHistory = true;
        public static bool isEnabledPasteOnSelection = true;
        public static bool isEnabledTrimWhitespacing = true;
        public static bool isCloseMinimizes = true;
        public static bool isClosedFromNotifyIcon = false;
        public static bool isAltPressedInThisApp = false;
        public string registryPath = "SOFTWARE\\HovText";
        public static string appDate = "";

        // HotkeyListener - hopefully I can get this working with CTRL+Oem5
        public static string hotkeyCtrlOem5 = "Control + D1"; // default hotkey for enable/disable application
        public static string hotkeyAltH = "Alt + H"; // default hotkey for getting older history entries
        public static string hotkeyShiftAltH = "Shift, Alt + H"; // default hotkey for getting newer history entries
        Hotkey hkCtrlOem5 = new Hotkey(hotkeyCtrlOem5);
        Hotkey hkAltH = new Hotkey(hotkeyAltH);
        Hotkey hkShiftAltH = new Hotkey(hotkeyShiftAltH);
        HotkeyListener hkl = new HotkeyListener();
        HotkeySelector hks = new HotkeySelector();


        // ###########################################################################################
        // Main

        public Settings()
        {

            settings = this; // refering to the current form - used in the history form

            /*
                        // NHotKey
                        HotkeyManager.Current.AddOrReplace("AltH", Keys.Alt | Keys.H, AltH);
                        HotkeyManager.Current.AddOrReplace("ShiftAltH", Keys.Alt | Keys.Shift | Keys.H, ShiftAltH);
                        HotkeyManager.Current.AddOrReplace("CtrlOem5", Keys.Control | Keys.Oem5, CtrlOem5);
            //            HotkeyManager.Current.AddOrReplace("AltO", Keys.Alt | Keys.O, AltO);
            */

            InitializeComponent();

            // HotkeyListener
//            hkl.Add(hkCtrlOem5);
            hkl.Add(hkAltH);
            hkl.Add(hkShiftAltH);
            hkl.HotkeyPressed += Hkl_HotkeyPressed;
            hkl.HotkeyUpdated += HotkeyListener_HotkeyUpdated;
            hkl.SuspendOn(settings);



            /*
                        // Test
                        Hotkey hotkey1 = new Hotkey(Keys.Alt, Keys.L);
                        hkl.Add(hotkey1);
            //            hkl.Update(ref hotkey1, new Hotkey("Alt + M"));
                        hkl.RemoveAll();
                        hkl.Update(ref hotkey1, new Hotkey("Alt + N"));

            */

            // Set KeyPreview object to true to allow the form to process 
            // the key before the control with focus processes it.
            //            this.KeyPreview = true;

            linkLabel1.BackColor = System.Drawing.Color.Transparent;

            // Initialize registry and get its values for the various checkboxes
            InitializeRegistry();
            GetStartupSettings();

            Program.AddClipboardFormatListener(this.Handle);

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

            // Update UI
            UpdateUiForm1();
        }





        // ###########################################################################################
        // HotkeyListener events

        private void Hkl_HotkeyPressed(object sender, HotkeyEventArgs e)
        {
            Console.WriteLine("Hotkey "+ e.Hotkey + " pressed");
            if (e.Hotkey == hkCtrlOem5)
            {
                CtrlOem5();
            }
            if (e.Hotkey == hkAltH)
            {
                AltH();
            }
            if (e.Hotkey == hkShiftAltH)
            {
                ShiftAltH();
            }

        }


        private void HotkeyListener_HotkeyUpdated(object sender, HotkeyListener.HotkeyUpdatedEventArgs e)
        {
            Console.WriteLine("Updated hotkey " + e.UpdatedHotkey);
            //            if (e.UpdatedHotkey == hotkey1)
            //            {
            // Do something...
            //            }
        }


        private void AltH()
        {
            GoEntryLowerNumber();
            isAltPressedInThisApp = true;
            Console.WriteLine("ALT + H");
        }


        private void ShiftAltH()
        {
            GoEntryHigherNumber();
            isAltPressedInThisApp = true;
            Console.WriteLine("SHIFT + ALT + H");
        }

        private void CtrlOem5()
        {
            ToggleEnabled();
            Console.WriteLine("CTRL + ½");
        }


        private void button1_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Apply hotkey updates");
//            hkl.RemoveAll(); 
            
            hkl.Update(ref hkCtrlOem5, new Hotkey(hotkeyEnable.Text));
            hkl.Update(ref hkAltH, new Hotkey(hotkeyOlder.Text));
            hkl.Update(ref hkShiftAltH, new Hotkey(hotkeyNewer.Text));
            SetRegistryKey(registryPath, "Hotkey1", hotkeyEnable.Text);
            SetRegistryKey(registryPath, "Hotkey2", hotkeyOlder.Text);
            SetRegistryKey(registryPath, "Hotkey3", hotkeyNewer.Text);
        }


        // ###########################################################################################
        // NHotKey events

        /*
                private void AltH(object sender, NHotkey.HotkeyEventArgs e)
                {
                    GoEntryLowerNumber();
                    isAltPressedInThisApp = true;
                    Console.WriteLine("ALT + H");
                    e.Handled = true;
                }


                private void ShiftAltH(object sender, NHotkey.HotkeyEventArgs e)
                {
                    GoEntryHigherNumber();
                    isAltPressedInThisApp = true;
                    Console.WriteLine("SHIFT + ALT + H");
                    e.Handled = true;
                }

                private void AltO(object sender, NHotkey.HotkeyEventArgs e)
                {
                    Settings.isAltPressedInThisApp = false;
                    ReleaseAltKey();
                    SendKeys.Send("^v");
                    Console.WriteLine("ALT + O");
                    e.Handled = true;
                }

                private void CtrlOem5(object sender, NHotkey.HotkeyEventArgs e)
                {
                    ToggleEnabled();
                    Console.WriteLine("CTRL + ½");
                    e.Handled = true;
                }
        */


        // ###########################################################################################
        // Called when clipboard is updated
        // Clipboard Monitor, https://stackoverflow.com/questions/38148400/clipboard-monitor

        const int WM_CLIPBOARDUPDATE = 0x031D;

        protected override void WndProc(ref Message m)
        {

            // CLIPBOARD CHAIN
            switch (m.Msg)
            {
                case WM_CLIPBOARDUPDATE:

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

        private void ProcessClipboard()
        {
            // Check which text formats are available
            isClipboardText = Clipboard.ContainsText();
            isClipboardImage = Clipboard.ContainsImage();

            // Get clipboard content
            clipboardText = isClipboardText ? Clipboard.GetText() : "";
            clipboardImage = isClipboardImage ? Clipboard.GetImage() : null;
            clipboardObject = Clipboard.GetDataObject();
            int clipboardFormatsCount = clipboardObject.GetFormats(false).Count(); // count number of formats in clipboard
            bool isClipboardUnicodeOnly = false;
            if (clipboardFormatsCount > 0)
            {
                string clipboardFormat0 = clipboardObject.GetFormats(false)[0];
                isClipboardUnicodeOnly = clipboardFormatsCount == 1 && clipboardFormat0 == "UnicodeText" ? true : false; // if this is true then I will bet(?) that this is text-only and if so then no more processing required
            }

            // Is clipboard text - also triggers when copying whitespaces only
            if (isClipboardText)
            {
                // Check if number of data formats equals 1 and this is unicode or if clipboard is different than last time
                if (!isClipboardUnicodeOnly || clipboardText != clipboardTextLast)
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
                        SetClipboard(false, "WM_CLIPBOARDUPDATE:TEXT");
                        UpdateUiForm1();
                    }
                    else
                    {
                        if (clipboardFormatsCount > 0)
                        {
                            Clipboard.Clear();
                        }
                    }
                }
            }
            else
            if (isClipboardImage) // Is clipboard an image
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
                    SetClipboard(false, "WM_CLIPBOARDUPDATE:IMAGE");
                    UpdateUiForm1();
                }
            }
            else
            // We have come in to here, if clipboard has changed but it does not contains a text or an image
            // It could be empty or e.g. a file cop

            if (Clipboard.GetText() == "")
            {
                // If active application is EXCEL then restore last entry, as EXCEL clears clipboard when 
                string activeAppName = GetActiveApplication();
                if (entriesText.Count() > 0 && activeAppName == "EXCEL")
                {
                    // Restore the last entry to the clipboard
                    SetClipboard(false, "WM_CLIPBOARDUPDATE:EMPTY-TEXT-2");
                    clipboardTextLast = Clipboard.GetText();
                }
            }
        }


        // ###########################################################################################
        // Check if clipboard content is already in the data arrays - if so move the entry to top

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

                    // Walk through all clipboard object formats and store them
                    /*
                                              var formats = clipboardObject.GetFormats(false);
                                                var dictionary = new SortedDictionary<string, object>();
                                                foreach (var format in formats)
                                                {
                                                    if (
                    //                                    !format.Contains("EnhancedMetafile")
                                                        format.Contains("Object Descriptor") ||
                                                        format.Contains("Embed Source") ||
                                                        format.Contains("Text") ||
                                                        format.Contains("HTML") ||
                                                        format.Contains("Csv") ||
                                                        format.Contains("Link") ||
                                                        format.Contains("Hyperlink") ||
                                                        format.Contains("Bitmap")
                                                        )
                                                    {
                                                        dictionary.Add(format, clipboardObject.GetData(format));
                                                    }
                                                }
                                                entriesObject.Add(entryIndex, dictionary);
                    */
                    //                entriesObject.Add(entryIndex, Clipboard.GetDataObject());

                    // Add the process name for the active application to the entries array
                    string activeAppName = GetActiveApplication();
                    entriesApplication.Add(entryIndex, activeAppName);
                }
            }
        }


        // ###########################################################################################
        // Place data in the clipboard based on the entry index

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
                    Clipboard.Clear();
                    Clipboard.SetText(entryText);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("EXCEPTION raised from [" + from + "] [" + ex.ToString() + "]");
                }
            }
            else
            if (isEntryImage) // Put an image to the clipboard
            {
                try
                {
                    Clipboard.Clear();
                    Clipboard.SetImage(entryImage);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("EXCEPTION raised from [" + from + "] [" + ex.ToString() + "]");
                }
            }
            else
            {
                MessageBox.Show("WTF!?");
            }

            timer1_Tick();

        }


        // ###########################################################################################
        // Update "Form 1" UI - this is where the application settings are

        private void UpdateUiForm1()
        {
            GetEntryCounter();
            uiEntries.Text = entriesText.Count.ToString();
        }


        // ###########################################################################################
        // Update "Form 2" UI - this is where the clipboatd data is shown

        private void UpdateUiHistory()
        {

            string entryText = entriesText[entryIndex];
            Image entryImage = entriesImage[entryIndex];
            string entryApplication = entriesApplication[entryIndex];

            bool isEntryText = entryText == "" ? false : true;
            bool isEntryImage = entryImage == null ? false : true;

            // Show text in form 2
            if (isEntryText)
            {
                history.ShowText(entryText, entryCounter + " of " + entriesText.Count + " from \"" + entryApplication + "\"", entryApplication);
            }
            else
            if (isEntryImage) // Show the image in form 2
            {
                history.ShowImage(entryImage, entryCounter + " of " + entriesText.Count + " from \"" + entryApplication + "\"", entryApplication);
            }
            else
            {
                MessageBox.Show("WTF 2");

            }
        }


        // ###########################################################################################
        // Called when ALT + H is pressed

        public void GoEntryLowerNumber()
        {

            // Check if application is enabled
            if (uiAppEnabled.Checked && entryCounter > 0)
            {

                GetActiveApplication();
                ChangeFocusToThisApplication();

                // Only proceed if the entry counter is equal to or more than 0
                if (entryCounter > 0)
                {

                    // Check if this is the first call (we want to show the newest entry at the first keypress)
                    if (!isFirstCall && entryCounter > 1)
                    {
                        entryIndex = GetEntryLowerNumber();
                    }
                    else
                    {
                        if (!isFirstCall)
                        {
                            history.Flash(100);
                        }
                    }
                    isFirstCall = false;

                    // Update the UI
                    UpdateUiForm1();
                    UpdateUiHistory();
                }
            }
        }


        // ###########################################################################################
        // Called when ALT + SHIFT + H is pressed

        public void GoEntryHigherNumber()
        {

            // Check if application is enabled
            if (uiAppEnabled.Checked && entryCounter > 0)
            {

                GetActiveApplication();
                ChangeFocusToThisApplication();

                // Only proceed if the entry counter is less than the total amount of entries
                if (entryCounter <= entriesText.Count)
                {

                    // Check if this is the first call (we want to show the newest entry at the first keypress)
                    if (!isFirstCall && entryCounter < entriesText.Count)
                    {
                        entryIndex = GetEntryHigherNumber();
                    }
                    else
                    {
                        if (!isFirstCall)
                        {
                            history.Flash(100);
                        }
                    }
                    isFirstCall = false;

                    // Update the UI
                    UpdateUiForm1();
                    UpdateUiHistory();
                }
            }
        }


        // ###########################################################################################
        // Called when ALT is released

        public void ReleaseAltKey()
        {

            // Check if application is enabled
            if (uiAppEnabled.Checked && entryCounter > 0)
            {

                MoveEntryToTop();

                // Set the clipboard with the new data
                SetClipboard(false, "ReleaseAltKey");

                // Set focus back to the originating application
                ChangeFocusToOriginatingApplication();

                //                SendKeys.Send("^v");

                // Reset some stuff
                isFirstCall = true;
                entryIndex = entriesText.Keys.Last();

                // Update UI
                UpdateUiForm1();
                history.Hide();

            }
        }


        // ###########################################################################################
        // Go back in time and get an older entry index

        private int GetEntryLowerNumber()
        {
            var element = entriesText.ElementAt(entryCounter - 2);
            return element.Key;
        }


        // ###########################################################################################
        // Go forward in time and get a newer entry index

        private int GetEntryHigherNumber()
        {
            var element = entriesText.ElementAt(entryCounter);
            return element.Key;
        }


        // ###########################################################################################
        // Convert the entry index to which logical number it is

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

        private void MoveEntryToTop()
        {

            clipboardTextLast = entriesText[entryIndex];

            int lastKey = entriesText.Keys.Last();
            int insertKey = lastKey + 1;

            // Copy the chosen entry to the top of the array lists (so it becomes the newest entry)
            entriesText.Add(insertKey, entriesText[entryIndex]);
            entriesImage.Add(insertKey, entriesImage[entryIndex]);
            entriesApplication.Add(insertKey, entriesApplication[entryIndex]);
            //            entriesObject.Add(insertKey, entriesObject[entryIndex]);

            // Remove the chosen entry, so it does not show duplicates
            entriesText.Remove(entryIndex);
            entriesImage.Remove(entryIndex);
            entriesApplication.Remove(entryIndex);
            //            entriesObject.Remove(entryIndex);

            entryIndex = entriesText.Keys.Last();

        }


        // ###########################################################################################
        // Called when the "Enable application" checkbox is clicked with mouse or CTRL + ½ is pressed

        public void ToggleEnabled()
        {
            // Toggle the checkbox - this will also fire an "appEnabled_CheckedChanged" event
            uiAppEnabled.Checked = !uiAppEnabled.Checked;
        }


        // ###########################################################################################
        // Event that is triggede when application is toggled, either enabled or disabled

        private void uiAppEnabled_CheckedChanged_1(object sender, EventArgs e)
        {

            // Check if application is enabled
            if (uiAppEnabled.Checked)
            {

                ProcessClipboard();

                // Change the icons to be green (active)
                Icon = Resources.Active;
                notifyIcon.Icon = Resources.Active;

                string hotkeyBehaviour = GetRegistryKey(registryPath, "HotkeyBehaviour");
                switch (hotkeyBehaviour)
                {
                    case "Paste":
                        uiHotkeyPaste.Checked = true;
                        hotkeyPaste.Enabled = true;
                        break;
                    default:
                        uiHotkeySystem.Checked = true;
                        hotkeyPaste.Enabled = false;
                        break;
                }

                // Enable thw two checkboxes
                uiHistoryEnabled.Enabled = true;
                uiTrimWhitespaces.Enabled = true;
                uiPasteOnSelection.Enabled = true;
                hotkeyOlder.Enabled = true;
                hotkeyNewer.Enabled = true;
            }
            else
            {

                //                SetClipboard(true, "appEnabled_CheckedChanged_1():Checked-key["+entryIndex+"]");
                //                Clipboard.SetDataObject(clipboardObject, true);

                // Change the icons to be red (inactive)
                Icon = Resources.Inactive;
                notifyIcon.Icon = Resources.Inactive;

                // Disable thw two checkboxes
                uiHistoryEnabled.Enabled = false;
                uiTrimWhitespaces.Enabled = false;
                uiPasteOnSelection.Enabled = false;
                hotkeyOlder.Enabled = false;
                hotkeyNewer.Enabled = false;
                hotkeyPaste.Enabled = false;
            }
        }


        // ###########################################################################################
        // Called when application is minimized - it will minimize "Form 1"

        private void MainWindow_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
            }
        }


        // ###########################################################################################
        // Called when tray icon is clicked - to show "Form 1"

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            Show(); // will show this form (Form 1)
            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
            }
        }


        // ###########################################################################################
        // Go to web page when clicked with mouse

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(hovtextPage);
        }


        // ###########################################################################################
        // Unregister from the clipboard chain when application is closing down

        private void MainWindow_FormClosing_1(object sender, FormClosingEventArgs e)
        {
            // In case windows is trying to shut down, don't hold the process up
            if (e.CloseReason == CloseReason.WindowsShutDown)
            {
                Program.RemoveClipboardFormatListener(this.Handle);
                //                hkl.RemoveAll();
                return;
            }

            if (!isCloseMinimizes || isClosedFromNotifyIcon)
            {
                Program.RemoveClipboardFormatListener(this.Handle);
                //                hkl.RemoveAll();
                return;
            }

            // Do not close as the X should minimize
            if (WindowState == FormWindowState.Normal)
            {
                Hide();
            }
            e.Cancel = true;
        }


        // ###########################################################################################
        // Get process info for active application (the one where the hotkey is pressed)

        private string GetActiveApplication()
        {
            if (isFirstCall)
            {
                //                IntPtr handle = Program.GetForegroundWindow();
                originatingHandle = Program.GetForegroundWindow();
            }

            // Get the process ID and find the name for that ID
            uint processId = 0;
            Program.GetWindowThreadProcessId(originatingHandle, out processId);
            string appProcessName = Process.GetProcessById((int)processId).ProcessName;

            /*
                        if(appProcessName!="devenv" && appProcessName!="HovText")
                        {
                            originatingHandle = handle;
                            Console.WriteLine("Setting original handle");
                        }
            */

            return appProcessName;
        }


        // ###########################################################################################
        // When activating the entry selector then change focus to this application to prevent the keypresses go in to the real active application

        private void ChangeFocusToThisApplication()
        {
            // Make sure FORM2 is
            //            if (isFirstCall)
            //            {
            Program.SetForegroundWindow(this.Handle);
            //                form2.TopMost = true;
            //                form2.Show();
            Console.WriteLine("Set focus to HovText");
            //            }
        }

        private void Settings_KeyUp(object sender, KeyEventArgs e)
        {
            Console.WriteLine("FORM 1 key up");
        }

        // ###########################################################################################
        // When an entry has been submitted to the clipboard then pass back focus to the real originating application

        private void ChangeFocusToOriginatingApplication()
        {
            Program.SetForegroundWindow(originatingHandle);
            Console.WriteLine("Set focus to originating application");
        }


        // ###########################################################################################
        // Timer interval - not required at all but kept for now


        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            timerUpdate.Enabled = false;
            try
            {
                WebClient client = new WebClient();
                client.Headers.Add("user-agent", "HovText "+ appDate);
                string checkedVersion = client.DownloadString(hovtextPage+"autoupdate/");
//                string checkedVersion = client.DownloadString(hovtextPage + "update/?v=" + appDate);
                if (checkedVersion.Substring(0,7) == "Version")
                {
                    checkedVersion = checkedVersion.Substring(9);
                    update.uiAppVerF3.Text = appDate;
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
            }
        }


        private void timer1_Tick()
        {

            // Check if application is enabled
            if (uiAppEnabled.Checked)
            {

                string hest = "";
                for (int count = entriesText.Count - 1; count >= 0; count--)
                {
                    var element = entriesText.ElementAt(count);
                    var Key = element.Key;
                    var Value = element.Value;
                    if (Value == "")
                    {
                        //                    hest = hest + "key=[" + Key + "], Picture" + Environment.NewLine;
                        hest = hest + "[" + Key + "]Picture" + Environment.NewLine;
                    }
                    else
                    {
                        //                    hest = hest + "key=[" + Key + "], Value=[" + Value + "]" + Environment.NewLine;
                        if (Value.Length > 55)
                        {
                            hest = hest + "[" + Key + "]" + Value.Substring(0, 55) + "..." + Environment.NewLine;
                        }
                        else
                        {
                            hest = hest + "[" + Key + "]" + Value + Environment.NewLine;
                        }

                    }
                    hest = hest + "────────────────────────" + Environment.NewLine;
                }
                textBox1.Text = hest;

                // Update the ticks counter (only for show - not used for anything)
                //                ticks += 1;
                //                strTick.Text = ticks.ToString() + " ticks";
            }
        }


        // ###########################################################################################
        // Set the various checkboxes when the application starts up

        private void GetStartupSettings()
        {

            // Start with Windows
            string getKey = GetRegistryKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", "HovText");
            if (getKey == null)
            {
                uiStartWithWindows.Checked = false;
            }
            else
            {
                uiStartWithWindows.Checked = true;
            }

            // Check updates
            string checkUpdates = GetRegistryKey(registryPath, "CheckUpdates");
            uiCheckUpdates.Checked = checkUpdates == "1" ? true : false;
            if (uiCheckUpdates.Checked)
            {
                timerUpdate.Enabled = true;
            }

            // Hotkey behaviour
            string hotkeyBehaviour = GetRegistryKey(registryPath, "HotkeyBehaviour");
            switch (hotkeyBehaviour)
            {
                case "Paste":
                    uiHotkeyPaste.Checked = true;
                    hotkeyPaste.Enabled = true;
                    break;
                default:
                    uiHotkeySystem.Checked = true;
                    hotkeyPaste.Enabled = false;
                    break;
            }

            // Close button minimizes
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

            // Paste on selection
            int pasteOnSelection = int.Parse((string)GetRegistryKey(registryPath, "EnablePasteOnSelection"));
            uiPasteOnSelection.Checked = pasteOnSelection == 1 ? true : false;
            isEnabledPasteOnSelection = uiPasteOnSelection.Checked;

            // History enabled
            int historyEnabled = int.Parse((string)GetRegistryKey(registryPath, "EnableHistory"));
            uiHistoryEnabled.Checked = historyEnabled == 1 ? true : false;
            isEnabledHistory = uiHistoryEnabled.Checked;
            if (isEnabledHistory)
            {
                hotkeyOlder.Enabled = true;
                hotkeyNewer.Enabled = true;
                uiPasteOnSelection.Enabled = true;
            }
            else
            {
                hotkeyOlder.Enabled = false;
                hotkeyNewer.Enabled = false;
                uiPasteOnSelection.Enabled = false;
            }

            // Trim whitespaces
            int trimWhitespaces = int.Parse((string)GetRegistryKey(registryPath, "EnableTrimWhitespaces"));
            uiTrimWhitespaces.Checked = trimWhitespaces == 1 ? true : false;
            isEnabledTrimWhitespacing = uiTrimWhitespaces.Checked;

            // Notification area
            string areaSize = GetRegistryKey(registryPath, "AreaSize");
            switch (areaSize)
            {
                case "Small":
                    uiAreaSmall.Checked = true;
                    break;
                case "Medium":
                    uiAreaMedium.Checked = true;
                    break;
                case "Large":
                    uiAreaLarge.Checked = true;
                    break;
                default:
                    uiAreaMedium.Checked = true;
                    break;
            }

            // Notification font
            string fontFamily = GetRegistryKey(registryPath, "FontFamily");
            float fontSize = float.Parse((string)GetRegistryKey(registryPath, "FontSize"));
            uiShowFont.Font = new Font(fontFamily, fontSize);
            uiShowFont.Text = fontFamily + ", " + fontSize;

            // Notification theme color
            string themeColorRegistry = GetRegistryKey(registryPath, "ThemeColor");
            themeColor = themeColorRegistry;
            SetThemeColors();

            // Set the history form size
            historySize = uiAreaSmall.Checked ? 0.7 : historySize;
            historySize = uiAreaMedium.Checked ? 1.1 : historySize;
            historySize = uiAreaLarge.Checked ? 1.5 : historySize;
            history.SetPosition();

            // Get the hotkeys from registry
            string hotkey1 = GetRegistryKey(registryPath, "Hotkey1");
            string hotkey2 = GetRegistryKey(registryPath, "Hotkey2");
            string hotkey3 = GetRegistryKey(registryPath, "Hotkey3");
            hkl.Update(ref hkCtrlOem5, new Hotkey(hotkey1));
            hkl.Update(ref hkAltH, new Hotkey(hotkey2));
            hkl.Update(ref hkShiftAltH, new Hotkey(hotkey3));
            hks.Enable(hotkeyEnable, hkCtrlOem5);
            hks.Enable(hotkeyOlder, hkAltH);
            hks.Enable(hotkeyNewer, hkShiftAltH);

        }


        // ###########################################################################################
        // Enable or disable that the application starts up with Windows
        // https://www.fluxbytes.com/csharp/start-application-at-windows-startup/

        private void uiStartWithWindows_CheckedChanged(object sender, EventArgs e)
        {
            if (uiStartWithWindows.Checked)
            {
                StartAppWithWindows();
            }
            else
            {
                RemoveAppFromStartup();
            }
        }

        private void StartAppWithWindows()
        {
            SetRegistryKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", "HovText", "\"" + Application.ExecutablePath + "\" --minimized");
        }

        private void RemoveAppFromStartup()
        {
            DeleteRegistryKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", "HovText");
        }


        // ###########################################################################################
        // Check if all registry keys have been created

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

            // Check if all keys are set - if not the set default values
            using (RegistryKey registryPathExists = Registry.CurrentUser.OpenSubKey(registryPath, true))
            {
                if (registryPathExists.GetValue("CheckUpdates") == null)
                {
                    registryPathExists.SetValue("CheckUpdates", "1");
                }
                if (registryPathExists.GetValue("HotkeyBehaviour") == null)
                {
                    registryPathExists.SetValue("HotkeyBehaviour", "System");
                }
                if (registryPathExists.GetValue("AreaSize") == null)
                {
                    registryPathExists.SetValue("AreaSize", "Medium");
                }
                if (registryPathExists.GetValue("FontFamily") == null)
                {
                    registryPathExists.SetValue("FontFamily", fontFamily);
                }
                if (registryPathExists.GetValue("FontSize") == null)
                {
                    registryPathExists.SetValue("FontSize", fontSize);
                }
                if (registryPathExists.GetValue("ThemeColor") == null)
                {
                    registryPathExists.SetValue("ThemeColor", "Yellow");
                }
                if (registryPathExists.GetValue("CloseMinimizes") == null)
                {
                    registryPathExists.SetValue("CloseMinimizes", "1");
                }
                if (registryPathExists.GetValue("EnableHistory") == null)
                {
                    registryPathExists.SetValue("EnableHistory", "1");
                }
                if (registryPathExists.GetValue("EnablePasteOnSelection") == null)
                {
                    registryPathExists.SetValue("EnablePasteOnSelection", "0");
                }
                if (registryPathExists.GetValue("EnableTrimWhitespaces") == null)
                {
                    registryPathExists.SetValue("EnableTrimWhitespaces", "1");
                }
                if (registryPathExists.GetValue("Hotkey1") == null)
                {
                    registryPathExists.SetValue("Hotkey1", hotkeyCtrlOem5);
                }
                if (registryPathExists.GetValue("Hotkey2") == null)
                {
                    registryPathExists.SetValue("Hotkey2", hotkeyAltH);
                }
                if (registryPathExists.GetValue("Hotkey3") == null)
                {
                    registryPathExists.SetValue("Hotkey3", hotkeyShiftAltH);
                }



            }
        }

        private string GetRegistryKey(string path, string key)
        {
            using (RegistryKey getKey = Registry.CurrentUser.OpenSubKey(path))
            {
                return (string)(getKey.GetValue(key));
            }
        }

        public void SetRegistryKey(string path, string key, string value)
        {
            using (RegistryKey setKey = Registry.CurrentUser.OpenSubKey(path, true))
            {
                setKey.SetValue(key, value);
            }
        }

        private void DeleteRegistryKey(string path, string key)
        {
            using (RegistryKey deleteKey = Registry.CurrentUser.OpenSubKey(path, true))
            {
                deleteKey.DeleteValue(key, false);
            }
        }


        // ###########################################################################################

        private void uiChangeFont_Click(object sender, EventArgs e)
        {
            FontDialog fontDlg = new FontDialog();
            fontDlg.Font = uiShowFont.Font; // initialize the font dialouge with the font from "uiShowFont"
            fontDlg.AllowVerticalFonts = false;
            fontDlg.FontMustExist = true;
            fontDlg.ShowColor = false;
            fontDlg.ShowApply = false;
            fontDlg.ShowEffects = false;
            fontDlg.ShowHelp = false;
            if (fontDlg.ShowDialog() != DialogResult.Cancel)
            {
                fontSize = fontDlg.Font.Size;
                fontFamily = fontDlg.Font.Name;
                uiShowFont.Text = fontFamily + ", " + fontSize;
                uiShowFont.Font = new Font(fontFamily, fontSize);
                SetRegistryKey(registryPath, "FontFamily", fontFamily);
                SetRegistryKey(registryPath, "FontSize", fontSize.ToString());
            }
        }


        // ###########################################################################################
        // Changes in the size for the notification area

        private void uiAreaSmall_CheckedChanged(object sender, EventArgs e)
        {
            historySize = 0.7;
            SetRegistryKey(registryPath, "AreaSize", "Small");
        }

        private void uiAreaMedium_CheckedChanged(object sender, EventArgs e)
        {
            historySize = 1.1;
            SetRegistryKey(registryPath, "AreaSize", "Medium");
        }

        private void uiAreaLarge_CheckedChanged(object sender, EventArgs e)
        {
            historySize = 1.5;
            SetRegistryKey(registryPath, "AreaSize", "Large");
        }


        // ###########################################################################################
        // Change in the theme color for the notification area

        private void uiThemeColorSelector_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            themeColor = (string)(uiThemeColorSelector.SelectedItem);
            SetRegistryKey(registryPath, "ThemeColor", (string)(uiThemeColorSelector.SelectedItem));
            SetThemeColors();
        }

        public void SetThemeColors()
        {
            switch (themeColor)
            {
                case "Blue":
                    uiShowFont.BackColor = ColorTranslator.FromHtml(colorBlueBottom);
                    uiThemeColorSelector.SelectedItem = "Blue";
                    break;
                case "Brown":
                    uiShowFont.BackColor = ColorTranslator.FromHtml(colorBrownBottom);
                    uiThemeColorSelector.SelectedItem = "Brown";
                    break;
                case "Green":
                    uiShowFont.BackColor = ColorTranslator.FromHtml(colorGreenBottom);
                    uiThemeColorSelector.SelectedItem = "Green";
                    break;
                case "Yellow":
                    uiShowFont.BackColor = ColorTranslator.FromHtml(colorYellowBottom);
                    uiThemeColorSelector.SelectedItem = "Yellow";
                    break;
                case "White":
                    uiShowFont.BackColor = ColorTranslator.FromHtml(colorWhiteBottom);
                    uiThemeColorSelector.SelectedItem = "White";
                    break;
                default:
                    uiShowFont.BackColor = ColorTranslator.FromHtml(colorYellowBottom);
                    uiThemeColorSelector.SelectedItem = "Yellow";
                    break;
            }

        }


        // ###########################################################################################
        // Application behaviours

        private void uiHistoryEnabled_CheckedChanged(object sender, EventArgs e)
        {
            // History enabled
            string status = uiHistoryEnabled.Checked ? "1" : "0";
            isEnabledHistory = uiHistoryEnabled.Checked;
            SetRegistryKey(registryPath, "EnableHistory", status);
            if (isEnabledHistory)
            {
                hotkeyOlder.Enabled = true;
                hotkeyNewer.Enabled = true;
                uiPasteOnSelection.Enabled = true;
            }
            else
            {
                hotkeyOlder.Enabled = false;
                hotkeyNewer.Enabled = false;
                uiPasteOnSelection.Enabled = false;
            }
        }

        private void uiPasteOnSelection_CheckedChanged(object sender, EventArgs e)
        {
            string status = uiPasteOnSelection.Checked ? "1" : "0";
            isEnabledPasteOnSelection = uiPasteOnSelection.Checked;
            SetRegistryKey(registryPath, "EnablePasteOnSelection", status);
        }

        private void uiTrimWhitespaces_CheckedChanged(object sender, EventArgs e)
        {
            string status = uiTrimWhitespaces.Checked ? "1" : "0";
            isEnabledTrimWhitespacing = uiTrimWhitespaces.Checked;
            SetRegistryKey(registryPath, "EnableTrimWhitespaces", status);
        }


        // ###########################################################################################
        // Start minimized, if required
        // https://stackoverflow.com/a/8486441/2028935

        private void MainWindow_Shown(object sender, EventArgs e)
        {
            if (Program.arg0.ToLower() == "--minimized")
            {
                //to minimize window
                WindowState = FormWindowState.Minimized;

                //to hide from taskbar
                Hide();
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            isClosedFromNotifyIcon = true;
            Close();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FocusApplication();
            tabControl1.SelectedIndex = 0; // General
        }


        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ToggleEnabled();
            }
        }


        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            FocusApplication();
            tabControl1.SelectedIndex = 4; // About
        }

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

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            FocusApplication();
            tabControl1.SelectedIndex = 0;
        }


        // ###########################################################################################
        // https://stackoverflow.com/a/11941579/2028935

        private void FocusApplication()
        {
            //            WindowState = FormWindowState.Minimized;
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void uiHotkeySystem_CheckedChanged(object sender, EventArgs e)
        {
            hotkeyPaste.Enabled = false;
            SetRegistryKey(registryPath, "HotkeyBehaviour", "System");
        }

        private void uiHotkeyPaste_CheckedChanged(object sender, EventArgs e)
        {
            hotkeyPaste.Enabled = true;
            SetRegistryKey(registryPath, "HotkeyBehaviour", "Paste");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string selectedTab = tabControl1.SelectedTab.AccessibilityObject.Name;
            System.Diagnostics.Process.Start(hovtextPage+"documentation/#" + selectedTab);
        }

        private void uiCheckUpdates_CheckedChanged(object sender, EventArgs e)
        {
            string status = uiCheckUpdates.Checked ? "1" : "0";
            SetRegistryKey(registryPath, "CheckUpdates", status);
        }


        // ###########################################################################################

    }
}
