namespace HovText
{
    partial class Settings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Settings));
            this.uiAppEnabled = new System.Windows.Forms.CheckBox();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.notifyIconMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tabAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.tabSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.exit = new System.Windows.Forms.ToolStripMenuItem();
            this.uiHistoryEnabled = new System.Windows.Forms.CheckBox();
            this.uiStartWithWindows = new System.Windows.Forms.CheckBox();
            this.fontDialog = new System.Windows.Forms.FontDialog();
            this.uiChangeFont = new System.Windows.Forms.Button();
            this.uiShowFont = new System.Windows.Forms.TextBox();
            this.uiAreaSmall = new System.Windows.Forms.RadioButton();
            this.uiAreaMedium = new System.Windows.Forms.RadioButton();
            this.uiAreaLarge = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.uiCheckUpdates = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.uiPasteOnSelection = new System.Windows.Forms.CheckBox();
            this.uiCloseMinimize = new System.Windows.Forms.CheckBox();
            this.uiTrimWhitespaces = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.uiHistoryColorSelector = new System.Windows.Forms.ComboBox();
            this.appVer = new System.Windows.Forms.Label();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.uiHotkeyBehaviourSystem = new System.Windows.Forms.RadioButton();
            this.uiHotkeyBehaviourPaste = new System.Windows.Forms.RadioButton();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.uiHotkeyPaste = new System.Windows.Forms.TextBox();
            this.uiHotkeyNewer = new System.Windows.Forms.TextBox();
            this.uiHotkeyOlder = new System.Windows.Forms.TextBox();
            this.cancelHotkey = new System.Windows.Forms.Button();
            this.uiHotkeyEnable = new System.Windows.Forms.TextBox();
            this.applyHotkey = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.uiHistoryLocation = new System.Windows.Forms.ComboBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.aboutBox = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.trayIcon = new System.Windows.Forms.ToolTip(this.components);
            this.uiHelp = new System.Windows.Forms.Button();
            this.updateTimer = new System.Windows.Forms.Timer(this.components);
            this.mouseClickTimer = new System.Windows.Forms.Timer(this.components);
            this.notifyIconMenuStrip.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // uiAppEnabled
            // 
            this.uiAppEnabled.AutoSize = true;
            this.uiAppEnabled.Checked = true;
            this.uiAppEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.uiAppEnabled.Location = new System.Drawing.Point(29, 31);
            this.uiAppEnabled.Name = "uiAppEnabled";
            this.uiAppEnabled.Size = new System.Drawing.Size(190, 29);
            this.uiAppEnabled.TabIndex = 21;
            this.uiAppEnabled.Text = "Enable application";
            this.uiAppEnabled.UseVisualStyleBackColor = true;
            this.uiAppEnabled.CheckedChanged += new System.EventHandler(this.uiAppEnabled_CheckedChanged_1);
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.notifyIconMenuStrip;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "HovText";
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseClick);
            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseDoubleClick);
            // 
            // notifyIconMenuStrip
            // 
            this.notifyIconMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.notifyIconMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tabAbout,
            this.tabSettings,
            this.exit});
            this.notifyIconMenuStrip.Name = "notofyIconMenuStrip";
            this.notifyIconMenuStrip.Size = new System.Drawing.Size(178, 76);
            // 
            // tabAbout
            // 
            this.tabAbout.Name = "tabAbout";
            this.tabAbout.Size = new System.Drawing.Size(177, 24);
            this.tabAbout.Text = "About HovText";
            this.tabAbout.Click += new System.EventHandler(this.trayIconAbout_Click);
            // 
            // tabSettings
            // 
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Size = new System.Drawing.Size(177, 24);
            this.tabSettings.Text = "Settings";
            this.tabSettings.Click += new System.EventHandler(this.trayIconSettings_Click);
            // 
            // exit
            // 
            this.exit.Name = "exit";
            this.exit.Size = new System.Drawing.Size(177, 24);
            this.exit.Text = "Exit";
            this.exit.Click += new System.EventHandler(this.trayIconExit_Click);
            // 
            // uiHistoryEnabled
            // 
            this.uiHistoryEnabled.AutoSize = true;
            this.uiHistoryEnabled.Checked = true;
            this.uiHistoryEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.uiHistoryEnabled.Location = new System.Drawing.Point(29, 95);
            this.uiHistoryEnabled.Name = "uiHistoryEnabled";
            this.uiHistoryEnabled.Size = new System.Drawing.Size(153, 29);
            this.uiHistoryEnabled.TabIndex = 23;
            this.uiHistoryEnabled.Text = "Enable history";
            this.uiHistoryEnabled.UseVisualStyleBackColor = true;
            this.uiHistoryEnabled.CheckedChanged += new System.EventHandler(this.uiHistoryEnabled_CheckedChanged);
            // 
            // uiStartWithWindows
            // 
            this.uiStartWithWindows.AutoSize = true;
            this.uiStartWithWindows.Checked = true;
            this.uiStartWithWindows.CheckState = System.Windows.Forms.CheckState.Checked;
            this.uiStartWithWindows.Location = new System.Drawing.Point(29, 31);
            this.uiStartWithWindows.Name = "uiStartWithWindows";
            this.uiStartWithWindows.Size = new System.Drawing.Size(196, 29);
            this.uiStartWithWindows.TabIndex = 11;
            this.uiStartWithWindows.Text = "Start with Windows";
            this.uiStartWithWindows.UseVisualStyleBackColor = true;
            this.uiStartWithWindows.CheckedChanged += new System.EventHandler(this.uiStartWithWindows_CheckedChanged);
            // 
            // uiChangeFont
            // 
            this.uiChangeFont.Location = new System.Drawing.Point(16, 36);
            this.uiChangeFont.Name = "uiChangeFont";
            this.uiChangeFont.Size = new System.Drawing.Size(129, 34);
            this.uiChangeFont.TabIndex = 81;
            this.uiChangeFont.Text = "Change font";
            this.uiChangeFont.UseVisualStyleBackColor = true;
            this.uiChangeFont.Click += new System.EventHandler(this.uiChangeFont_Click);
            // 
            // uiShowFont
            // 
            this.uiShowFont.Location = new System.Drawing.Point(16, 76);
            this.uiShowFont.Multiline = true;
            this.uiShowFont.Name = "uiShowFont";
            this.uiShowFont.Size = new System.Drawing.Size(243, 72);
            this.uiShowFont.TabIndex = 82;
            this.uiShowFont.TabStop = false;
            // 
            // uiAreaSmall
            // 
            this.uiAreaSmall.AutoSize = true;
            this.uiAreaSmall.Checked = true;
            this.uiAreaSmall.Location = new System.Drawing.Point(29, 36);
            this.uiAreaSmall.Name = "uiAreaSmall";
            this.uiAreaSmall.Size = new System.Drawing.Size(79, 29);
            this.uiAreaSmall.TabIndex = 61;
            this.uiAreaSmall.TabStop = true;
            this.uiAreaSmall.Text = "Small";
            this.uiAreaSmall.UseVisualStyleBackColor = true;
            this.uiAreaSmall.CheckedChanged += new System.EventHandler(this.uiAreaSmall_CheckedChanged);
            // 
            // uiAreaMedium
            // 
            this.uiAreaMedium.AutoSize = true;
            this.uiAreaMedium.Location = new System.Drawing.Point(29, 68);
            this.uiAreaMedium.Name = "uiAreaMedium";
            this.uiAreaMedium.Size = new System.Drawing.Size(103, 29);
            this.uiAreaMedium.TabIndex = 62;
            this.uiAreaMedium.Text = "Medium";
            this.uiAreaMedium.UseVisualStyleBackColor = true;
            this.uiAreaMedium.CheckedChanged += new System.EventHandler(this.uiAreaMedium_CheckedChanged);
            // 
            // uiAreaLarge
            // 
            this.uiAreaLarge.AutoSize = true;
            this.uiAreaLarge.Location = new System.Drawing.Point(29, 100);
            this.uiAreaLarge.Name = "uiAreaLarge";
            this.uiAreaLarge.Size = new System.Drawing.Size(80, 29);
            this.uiAreaLarge.TabIndex = 63;
            this.uiAreaLarge.Text = "Large";
            this.uiAreaLarge.UseVisualStyleBackColor = true;
            this.uiAreaLarge.CheckedChanged += new System.EventHandler(this.uiAreaLarge_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.uiCheckUpdates);
            this.groupBox1.Controls.Add(this.uiStartWithWindows);
            this.groupBox1.Location = new System.Drawing.Point(18, 23);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(530, 107);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Startup";
            // 
            // uiCheckUpdates
            // 
            this.uiCheckUpdates.AutoSize = true;
            this.uiCheckUpdates.Checked = true;
            this.uiCheckUpdates.CheckState = System.Windows.Forms.CheckState.Checked;
            this.uiCheckUpdates.Location = new System.Drawing.Point(29, 63);
            this.uiCheckUpdates.Name = "uiCheckUpdates";
            this.uiCheckUpdates.Size = new System.Drawing.Size(244, 29);
            this.uiCheckUpdates.TabIndex = 12;
            this.uiCheckUpdates.Text = "Check for updates online";
            this.uiCheckUpdates.UseVisualStyleBackColor = true;
            this.uiCheckUpdates.CheckedChanged += new System.EventHandler(this.uiCheckUpdates_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.uiPasteOnSelection);
            this.groupBox2.Controls.Add(this.uiCloseMinimize);
            this.groupBox2.Controls.Add(this.uiTrimWhitespaces);
            this.groupBox2.Controls.Add(this.uiAppEnabled);
            this.groupBox2.Controls.Add(this.uiHistoryEnabled);
            this.groupBox2.Location = new System.Drawing.Point(18, 150);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(530, 200);
            this.groupBox2.TabIndex = 20;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Behaviour";
            // 
            // uiPasteOnSelection
            // 
            this.uiPasteOnSelection.AutoSize = true;
            this.uiPasteOnSelection.Checked = true;
            this.uiPasteOnSelection.CheckState = System.Windows.Forms.CheckState.Checked;
            this.uiPasteOnSelection.Location = new System.Drawing.Point(29, 127);
            this.uiPasteOnSelection.Name = "uiPasteOnSelection";
            this.uiPasteOnSelection.Size = new System.Drawing.Size(247, 29);
            this.uiPasteOnSelection.TabIndex = 24;
            this.uiPasteOnSelection.Text = "Paste on history selection";
            this.trayIcon.SetToolTip(this.uiPasteOnSelection, "If checked then the application will minimize to tray when the Close button is cl" +
        "icked.\r\nIf not checked then the application will exit and terminate.");
            this.uiPasteOnSelection.UseVisualStyleBackColor = true;
            this.uiPasteOnSelection.CheckedChanged += new System.EventHandler(this.uiPasteOnSelection_CheckedChanged);
            // 
            // uiCloseMinimize
            // 
            this.uiCloseMinimize.AutoSize = true;
            this.uiCloseMinimize.Checked = true;
            this.uiCloseMinimize.CheckState = System.Windows.Forms.CheckState.Checked;
            this.uiCloseMinimize.Location = new System.Drawing.Point(29, 63);
            this.uiCloseMinimize.Name = "uiCloseMinimize";
            this.uiCloseMinimize.Size = new System.Drawing.Size(328, 29);
            this.uiCloseMinimize.TabIndex = 22;
            this.uiCloseMinimize.Text = "Close minimizes application to tray";
            this.trayIcon.SetToolTip(this.uiCloseMinimize, "If checked then the application will minimize to tray when the Close button is cl" +
        "icked.\r\nIf not checked then the application will exit and terminate.");
            this.uiCloseMinimize.UseVisualStyleBackColor = true;
            this.uiCloseMinimize.CheckedChanged += new System.EventHandler(this.uiCloseMinimize_CheckedChanged);
            // 
            // uiTrimWhitespaces
            // 
            this.uiTrimWhitespaces.AutoSize = true;
            this.uiTrimWhitespaces.Checked = true;
            this.uiTrimWhitespaces.CheckState = System.Windows.Forms.CheckState.Checked;
            this.uiTrimWhitespaces.Location = new System.Drawing.Point(29, 159);
            this.uiTrimWhitespaces.Name = "uiTrimWhitespaces";
            this.uiTrimWhitespaces.Size = new System.Drawing.Size(177, 29);
            this.uiTrimWhitespaces.TabIndex = 25;
            this.uiTrimWhitespaces.Text = "Trim whitespaces";
            this.uiTrimWhitespaces.UseVisualStyleBackColor = true;
            this.uiTrimWhitespaces.CheckedChanged += new System.EventHandler(this.uiTrimWhitespaces_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.uiAreaLarge);
            this.groupBox3.Controls.Add(this.uiAreaSmall);
            this.groupBox3.Controls.Add(this.uiAreaMedium);
            this.groupBox3.Location = new System.Drawing.Point(18, 23);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(231, 170);
            this.groupBox3.TabIndex = 60;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "History area";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.uiChangeFont);
            this.groupBox4.Controls.Add(this.uiShowFont);
            this.groupBox4.Location = new System.Drawing.Point(268, 23);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(281, 170);
            this.groupBox4.TabIndex = 80;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "History font";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.uiHistoryColorSelector);
            this.groupBox6.Location = new System.Drawing.Point(18, 212);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(231, 82);
            this.groupBox6.TabIndex = 70;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "History color theme";
            // 
            // uiHistoryColorSelector
            // 
            this.uiHistoryColorSelector.FormattingEnabled = true;
            this.uiHistoryColorSelector.Items.AddRange(new object[] {
            "Blue",
            "Brown",
            "Green",
            "White",
            "Yellow"});
            this.uiHistoryColorSelector.Location = new System.Drawing.Point(17, 36);
            this.uiHistoryColorSelector.Name = "uiHistoryColorSelector";
            this.uiHistoryColorSelector.Size = new System.Drawing.Size(197, 33);
            this.uiHistoryColorSelector.Sorted = true;
            this.uiHistoryColorSelector.TabIndex = 71;
            this.uiHistoryColorSelector.SelectedIndexChanged += new System.EventHandler(this.uiHistoryColorSelector_SelectedIndexChanged);
            // 
            // appVer
            // 
            this.appVer.AutoSize = true;
            this.appVer.Font = new System.Drawing.Font("Segoe UI", 11.12727F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.appVer.Location = new System.Drawing.Point(24, 63);
            this.appVer.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.appVer.Name = "appVer";
            this.appVer.Size = new System.Drawing.Size(23, 25);
            this.appVer.TabIndex = 49;
            this.appVer.Text = "#";
            this.appVer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Controls.Add(this.tabPage5);
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.Controls.Add(this.tabPage4);
            this.tabControl.Location = new System.Drawing.Point(12, 12);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(573, 441);
            this.tabControl.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 34);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(565, 403);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "General";
            // 
            // tabPage5
            // 
            this.tabPage5.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tabPage5.Controls.Add(this.groupBox7);
            this.tabPage5.Controls.Add(this.groupBox5);
            this.tabPage5.Location = new System.Drawing.Point(4, 34);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(565, 403);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Hotkeys";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.uiHotkeyBehaviourSystem);
            this.groupBox7.Controls.Add(this.uiHotkeyBehaviourPaste);
            this.groupBox7.Location = new System.Drawing.Point(19, 23);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(530, 107);
            this.groupBox7.TabIndex = 40;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Hotkey behaviour";
            // 
            // uiHotkeyBehaviourSystem
            // 
            this.uiHotkeyBehaviourSystem.AutoSize = true;
            this.uiHotkeyBehaviourSystem.Checked = true;
            this.uiHotkeyBehaviourSystem.Location = new System.Drawing.Point(29, 32);
            this.uiHotkeyBehaviourSystem.Name = "uiHotkeyBehaviourSystem";
            this.uiHotkeyBehaviourSystem.Size = new System.Drawing.Size(148, 29);
            this.uiHotkeyBehaviourSystem.TabIndex = 41;
            this.uiHotkeyBehaviourSystem.TabStop = true;
            this.uiHotkeyBehaviourSystem.Text = "Always action";
            this.uiHotkeyBehaviourSystem.UseVisualStyleBackColor = true;
            this.uiHotkeyBehaviourSystem.CheckedChanged += new System.EventHandler(this.uiHotkeyBehaviourSystem_CheckedChanged);
            // 
            // uiHotkeyBehaviourPaste
            // 
            this.uiHotkeyBehaviourPaste.AutoSize = true;
            this.uiHotkeyBehaviourPaste.Enabled = false;
            this.uiHotkeyBehaviourPaste.Location = new System.Drawing.Point(29, 64);
            this.uiHotkeyBehaviourPaste.Name = "uiHotkeyBehaviourPaste";
            this.uiHotkeyBehaviourPaste.Size = new System.Drawing.Size(216, 29);
            this.uiHotkeyBehaviourPaste.TabIndex = 42;
            this.uiHotkeyBehaviourPaste.Text = "Action only on hotkey";
            this.uiHotkeyBehaviourPaste.UseVisualStyleBackColor = true;
            this.uiHotkeyBehaviourPaste.CheckedChanged += new System.EventHandler(this.uiHotkeyBehaviourPaste_CheckedChanged);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.uiHotkeyPaste);
            this.groupBox5.Controls.Add(this.uiHotkeyNewer);
            this.groupBox5.Controls.Add(this.uiHotkeyOlder);
            this.groupBox5.Controls.Add(this.cancelHotkey);
            this.groupBox5.Controls.Add(this.uiHotkeyEnable);
            this.groupBox5.Controls.Add(this.applyHotkey);
            this.groupBox5.Controls.Add(this.label4);
            this.groupBox5.Controls.Add(this.label6);
            this.groupBox5.Controls.Add(this.label7);
            this.groupBox5.Controls.Add(this.label8);
            this.groupBox5.Location = new System.Drawing.Point(19, 150);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(530, 233);
            this.groupBox5.TabIndex = 50;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Hotkeys";
            // 
            // uiHotkeyPaste
            // 
            this.uiHotkeyPaste.BackColor = System.Drawing.SystemColors.Window;
            this.uiHotkeyPaste.Location = new System.Drawing.Point(274, 143);
            this.uiHotkeyPaste.Name = "uiHotkeyPaste";
            this.uiHotkeyPaste.ReadOnly = true;
            this.uiHotkeyPaste.Size = new System.Drawing.Size(222, 32);
            this.uiHotkeyPaste.TabIndex = 54;
            this.uiHotkeyPaste.Text = "Alt + O";
            this.uiHotkeyPaste.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.uiHotkeyPaste.Enter += new System.EventHandler(this.hotkeyPaste_Enter);
            this.uiHotkeyPaste.KeyDown += new System.Windows.Forms.KeyEventHandler(this.hotkeyPaste_KeyDown);
            // 
            // uiHotkeyNewer
            // 
            this.uiHotkeyNewer.BackColor = System.Drawing.SystemColors.Window;
            this.uiHotkeyNewer.Location = new System.Drawing.Point(274, 105);
            this.uiHotkeyNewer.Name = "uiHotkeyNewer";
            this.uiHotkeyNewer.ReadOnly = true;
            this.uiHotkeyNewer.Size = new System.Drawing.Size(222, 32);
            this.uiHotkeyNewer.TabIndex = 53;
            this.uiHotkeyNewer.Text = "Shift + Alt + H";
            this.uiHotkeyNewer.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.uiHotkeyNewer.Enter += new System.EventHandler(this.hotkeyNewer_Enter);
            this.uiHotkeyNewer.KeyDown += new System.Windows.Forms.KeyEventHandler(this.hotkeyNewer_KeyDown);
            // 
            // uiHotkeyOlder
            // 
            this.uiHotkeyOlder.BackColor = System.Drawing.SystemColors.Window;
            this.uiHotkeyOlder.Location = new System.Drawing.Point(274, 67);
            this.uiHotkeyOlder.Name = "uiHotkeyOlder";
            this.uiHotkeyOlder.ReadOnly = true;
            this.uiHotkeyOlder.Size = new System.Drawing.Size(222, 32);
            this.uiHotkeyOlder.TabIndex = 52;
            this.uiHotkeyOlder.Text = "Alt + H";
            this.uiHotkeyOlder.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.uiHotkeyOlder.Enter += new System.EventHandler(this.hotkeyOlder_Enter);
            this.uiHotkeyOlder.KeyDown += new System.Windows.Forms.KeyEventHandler(this.hotkeyOlder_KeyDown);
            // 
            // cancelHotkey
            // 
            this.cancelHotkey.Enabled = false;
            this.cancelHotkey.Location = new System.Drawing.Point(388, 180);
            this.cancelHotkey.Name = "cancelHotkey";
            this.cancelHotkey.Size = new System.Drawing.Size(108, 38);
            this.cancelHotkey.TabIndex = 56;
            this.cancelHotkey.Text = "Cancel";
            this.cancelHotkey.UseVisualStyleBackColor = true;
            this.cancelHotkey.Click += new System.EventHandler(this.cancelHotkey_Click);
            // 
            // uiHotkeyEnable
            // 
            this.uiHotkeyEnable.BackColor = System.Drawing.SystemColors.Window;
            this.uiHotkeyEnable.Location = new System.Drawing.Point(274, 29);
            this.uiHotkeyEnable.Name = "uiHotkeyEnable";
            this.uiHotkeyEnable.ReadOnly = true;
            this.uiHotkeyEnable.Size = new System.Drawing.Size(222, 32);
            this.uiHotkeyEnable.TabIndex = 51;
            this.uiHotkeyEnable.Text = "Control + Oem5";
            this.uiHotkeyEnable.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.uiHotkeyEnable.Enter += new System.EventHandler(this.HotkeyEnable_Enter);
            this.uiHotkeyEnable.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HotkeyEnable_KeyDown);
            // 
            // applyHotkey
            // 
            this.applyHotkey.Enabled = false;
            this.applyHotkey.Location = new System.Drawing.Point(274, 180);
            this.applyHotkey.Name = "applyHotkey";
            this.applyHotkey.Size = new System.Drawing.Size(108, 38);
            this.applyHotkey.TabIndex = 55;
            this.applyHotkey.Text = "Apply";
            this.applyHotkey.UseVisualStyleBackColor = true;
            this.applyHotkey.Click += new System.EventHandler(this.ApplyHotkeys_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 11.12727F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(24, 147);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(144, 25);
            this.label4.TabIndex = 48;
            this.label4.Text = "Paste on hotkey";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 11.12727F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(24, 71);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(155, 25);
            this.label6.TabIndex = 43;
            this.label6.Text = "Show older entry";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 11.12727F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(24, 109);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(163, 25);
            this.label7.TabIndex = 44;
            this.label7.Text = "Show newer entry";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Segoe UI", 11.12727F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(21, 32);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(224, 25);
            this.label8.TabIndex = 45;
            this.label8.Text = "Toggle application on/off";
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tabPage2.Controls.Add(this.groupBox8);
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Controls.Add(this.groupBox4);
            this.tabPage2.Controls.Add(this.groupBox6);
            this.tabPage2.Location = new System.Drawing.Point(4, 34);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(565, 403);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Apperance";
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.uiHistoryLocation);
            this.groupBox8.Location = new System.Drawing.Point(268, 212);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(281, 82);
            this.groupBox8.TabIndex = 72;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "History location";
            // 
            // uiHistoryLocation
            // 
            this.uiHistoryLocation.FormattingEnabled = true;
            this.uiHistoryLocation.Items.AddRange(new object[] {
            "Bottom Left",
            "Bottom Right",
            "Center",
            "Top Left",
            "Top Right"});
            this.uiHistoryLocation.Location = new System.Drawing.Point(17, 36);
            this.uiHistoryLocation.Name = "uiHistoryLocation";
            this.uiHistoryLocation.Size = new System.Drawing.Size(242, 33);
            this.uiHistoryLocation.Sorted = true;
            this.uiHistoryLocation.TabIndex = 71;
            this.uiHistoryLocation.SelectedIndexChanged += new System.EventHandler(this.uiHistoryLocation_SelectedIndexChanged);
            // 
            // tabPage4
            // 
            this.tabPage4.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tabPage4.Controls.Add(this.aboutBox);
            this.tabPage4.Controls.Add(this.label2);
            this.tabPage4.Controls.Add(this.appVer);
            this.tabPage4.Location = new System.Drawing.Point(4, 34);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(565, 403);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "About";
            // 
            // aboutBox
            // 
            this.aboutBox.BackColor = System.Drawing.Color.WhiteSmoke;
            this.aboutBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.aboutBox.Location = new System.Drawing.Point(28, 110);
            this.aboutBox.Name = "aboutBox";
            this.aboutBox.ReadOnly = true;
            this.aboutBox.Size = new System.Drawing.Size(510, 264);
            this.aboutBox.TabIndex = 91;
            this.aboutBox.Text = "#";
            this.aboutBox.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.aboutBox_LinkClicked);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(20, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(518, 54);
            this.label2.TabIndex = 50;
            this.label2.Text = "HovText # Rebooted edition";
            // 
            // uiHelp
            // 
            this.uiHelp.Location = new System.Drawing.Point(167, 461);
            this.uiHelp.Name = "uiHelp";
            this.uiHelp.Size = new System.Drawing.Size(262, 38);
            this.uiHelp.TabIndex = 30;
            this.uiHelp.Text = "Show online help for this tab";
            this.uiHelp.UseVisualStyleBackColor = true;
            this.uiHelp.Click += new System.EventHandler(this.uiHelp_Click);
            // 
            // updateTimer
            // 
            this.updateTimer.Interval = 5000;
            this.updateTimer.Tick += new System.EventHandler(this.updateTimer_Tick);
            // 
            // mouseClickTimer
            // 
            this.mouseClickTimer.Interval = 300;
            this.mouseClickTimer.Tick += new System.EventHandler(this.mouseClickTimer_Tick);
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(596, 507);
            this.Controls.Add(this.uiHelp);
            this.Controls.Add(this.tabControl);
            this.Font = new System.Drawing.Font("Segoe UI", 11.12727F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.Name = "Settings";
            this.Text = "HovText";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing_1);
            this.Shown += new System.EventHandler(this.MainWindow_Shown);
            this.Resize += new System.EventHandler(this.MainWindow_Resize);
            this.notifyIconMenuStrip.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.CheckBox uiAppEnabled;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.CheckBox uiHistoryEnabled;
        private System.Windows.Forms.CheckBox uiStartWithWindows;
        private System.Windows.Forms.FontDialog fontDialog;
        private System.Windows.Forms.Button uiChangeFont;
        private System.Windows.Forms.TextBox uiShowFont;
        private System.Windows.Forms.RadioButton uiAreaSmall;
        private System.Windows.Forms.RadioButton uiAreaMedium;
        private System.Windows.Forms.RadioButton uiAreaLarge;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.CheckBox uiTrimWhitespaces;
        private System.Windows.Forms.ComboBox uiHistoryColorSelector;
        private System.Windows.Forms.Label appVer;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ContextMenuStrip notifyIconMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem tabSettings;
        private System.Windows.Forms.ToolStripMenuItem exit;
        private System.Windows.Forms.ToolStripMenuItem tabAbout;
        private System.Windows.Forms.CheckBox uiCloseMinimize;
        private System.Windows.Forms.ToolTip trayIcon;
        private System.Windows.Forms.CheckBox uiPasteOnSelection;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.RadioButton uiHotkeyBehaviourSystem;
        private System.Windows.Forms.RadioButton uiHotkeyBehaviourPaste;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button applyHotkey;
        private System.Windows.Forms.Button uiHelp;
        private System.Windows.Forms.CheckBox uiCheckUpdates;
        private System.Windows.Forms.Timer updateTimer;
        private System.Windows.Forms.TextBox uiHotkeyEnable;
        private System.Windows.Forms.Button cancelHotkey;
        private System.Windows.Forms.TextBox uiHotkeyPaste;
        private System.Windows.Forms.TextBox uiHotkeyNewer;
        private System.Windows.Forms.TextBox uiHotkeyOlder;
        private System.Windows.Forms.Timer mouseClickTimer;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.ComboBox uiHistoryLocation;
        private System.Windows.Forms.RichTextBox aboutBox;
    }

}