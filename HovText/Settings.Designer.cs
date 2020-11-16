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
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.uiAppEnabled = new System.Windows.Forms.CheckBox();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.notifyIconMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.about = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.exit = new System.Windows.Forms.ToolStripMenuItem();
            this.uiHistoryEnabled = new System.Windows.Forms.CheckBox();
            this.uiStartWithWindows = new System.Windows.Forms.CheckBox();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
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
            this.uiThemeColorSelector = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.uiEntries = new System.Windows.Forms.Label();
            this.appVer = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.uiHotkeySystem = new System.Windows.Forms.RadioButton();
            this.uiHotkeyPaste = new System.Windows.Forms.RadioButton();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.hotkeyPaste = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.hotkeyOlder = new System.Windows.Forms.TextBox();
            this.hotkeyEnable = new System.Windows.Forms.TextBox();
            this.hotkeyNewer = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.button4 = new System.Windows.Forms.Button();
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            this.notifyIconMenuStrip.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Font = new System.Drawing.Font("Segoe UI", 11.12727F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabel1.Location = new System.Drawing.Point(24, 248);
            this.linkLabel1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(173, 25);
            this.linkLabel1.TabIndex = 17;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "http://hovtext.com/";
            this.linkLabel1.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // uiAppEnabled
            // 
            this.uiAppEnabled.AutoSize = true;
            this.uiAppEnabled.Checked = true;
            this.uiAppEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.uiAppEnabled.Location = new System.Drawing.Point(29, 31);
            this.uiAppEnabled.Name = "uiAppEnabled";
            this.uiAppEnabled.Size = new System.Drawing.Size(190, 29);
            this.uiAppEnabled.TabIndex = 18;
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
            this.about,
            this.settingsMenu,
            this.exit});
            this.notifyIconMenuStrip.Name = "notofyIconMenuStrip";
            this.notifyIconMenuStrip.Size = new System.Drawing.Size(178, 76);
            // 
            // about
            // 
            this.about.Name = "about";
            this.about.Size = new System.Drawing.Size(177, 24);
            this.about.Text = "About HovText";
            this.about.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
            // 
            // settingsMenu
            // 
            this.settingsMenu.Name = "settingsMenu";
            this.settingsMenu.Size = new System.Drawing.Size(177, 24);
            this.settingsMenu.Text = "Settings";
            this.settingsMenu.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // exit
            // 
            this.exit.Name = "exit";
            this.exit.Size = new System.Drawing.Size(177, 24);
            this.exit.Text = "Exit";
            this.exit.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // uiHistoryEnabled
            // 
            this.uiHistoryEnabled.AutoSize = true;
            this.uiHistoryEnabled.Checked = true;
            this.uiHistoryEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.uiHistoryEnabled.Location = new System.Drawing.Point(29, 95);
            this.uiHistoryEnabled.Name = "uiHistoryEnabled";
            this.uiHistoryEnabled.Size = new System.Drawing.Size(153, 29);
            this.uiHistoryEnabled.TabIndex = 27;
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
            this.uiStartWithWindows.TabIndex = 33;
            this.uiStartWithWindows.Text = "Start with Windows";
            this.uiStartWithWindows.UseVisualStyleBackColor = true;
            this.uiStartWithWindows.CheckedChanged += new System.EventHandler(this.uiStartWithWindows_CheckedChanged);
            // 
            // uiChangeFont
            // 
            this.uiChangeFont.Location = new System.Drawing.Point(6, 33);
            this.uiChangeFont.Name = "uiChangeFont";
            this.uiChangeFont.Size = new System.Drawing.Size(129, 34);
            this.uiChangeFont.TabIndex = 34;
            this.uiChangeFont.Text = "Change font";
            this.uiChangeFont.UseVisualStyleBackColor = true;
            this.uiChangeFont.Click += new System.EventHandler(this.uiChangeFont_Click);
            // 
            // uiShowFont
            // 
            this.uiShowFont.Location = new System.Drawing.Point(6, 73);
            this.uiShowFont.Multiline = true;
            this.uiShowFont.Name = "uiShowFont";
            this.uiShowFont.Size = new System.Drawing.Size(236, 71);
            this.uiShowFont.TabIndex = 35;
            // 
            // uiAreaSmall
            // 
            this.uiAreaSmall.AutoSize = true;
            this.uiAreaSmall.Checked = true;
            this.uiAreaSmall.Location = new System.Drawing.Point(29, 36);
            this.uiAreaSmall.Name = "uiAreaSmall";
            this.uiAreaSmall.Size = new System.Drawing.Size(79, 29);
            this.uiAreaSmall.TabIndex = 36;
            this.uiAreaSmall.TabStop = true;
            this.uiAreaSmall.Text = "Small";
            this.uiAreaSmall.UseVisualStyleBackColor = true;
            this.uiAreaSmall.CheckedChanged += new System.EventHandler(this.uiAreaSmall_CheckedChanged);
            // 
            // uiAreaMedium
            // 
            this.uiAreaMedium.AutoSize = true;
            this.uiAreaMedium.Location = new System.Drawing.Point(29, 67);
            this.uiAreaMedium.Name = "uiAreaMedium";
            this.uiAreaMedium.Size = new System.Drawing.Size(103, 29);
            this.uiAreaMedium.TabIndex = 37;
            this.uiAreaMedium.Text = "Medium";
            this.uiAreaMedium.UseVisualStyleBackColor = true;
            this.uiAreaMedium.CheckedChanged += new System.EventHandler(this.uiAreaMedium_CheckedChanged);
            // 
            // uiAreaLarge
            // 
            this.uiAreaLarge.AutoSize = true;
            this.uiAreaLarge.Location = new System.Drawing.Point(29, 98);
            this.uiAreaLarge.Name = "uiAreaLarge";
            this.uiAreaLarge.Size = new System.Drawing.Size(80, 29);
            this.uiAreaLarge.TabIndex = 38;
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
            this.groupBox1.Size = new System.Drawing.Size(527, 107);
            this.groupBox1.TabIndex = 39;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Startup";
            // 
            // uiCheckUpdates
            // 
            this.uiCheckUpdates.AutoSize = true;
            this.uiCheckUpdates.Checked = true;
            this.uiCheckUpdates.CheckState = System.Windows.Forms.CheckState.Checked;
            this.uiCheckUpdates.Location = new System.Drawing.Point(29, 66);
            this.uiCheckUpdates.Name = "uiCheckUpdates";
            this.uiCheckUpdates.Size = new System.Drawing.Size(244, 29);
            this.uiCheckUpdates.TabIndex = 34;
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
            this.groupBox2.Size = new System.Drawing.Size(527, 200);
            this.groupBox2.TabIndex = 40;
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
            this.uiPasteOnSelection.TabIndex = 31;
            this.uiPasteOnSelection.Text = "Paste on history selection";
            this.toolTip1.SetToolTip(this.uiPasteOnSelection, "If checked then the application will minimize to tray when the Close button is cl" +
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
            this.uiCloseMinimize.TabIndex = 30;
            this.uiCloseMinimize.Text = "Close minimizes application to tray";
            this.toolTip1.SetToolTip(this.uiCloseMinimize, "If checked then the application will minimize to tray when the Close button is cl" +
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
            this.uiTrimWhitespaces.TabIndex = 29;
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
            this.groupBox3.Size = new System.Drawing.Size(231, 158);
            this.groupBox3.TabIndex = 41;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Notification area";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.uiChangeFont);
            this.groupBox4.Controls.Add(this.uiShowFont);
            this.groupBox4.Location = new System.Drawing.Point(268, 23);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(248, 158);
            this.groupBox4.TabIndex = 42;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Notification font";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.uiThemeColorSelector);
            this.groupBox6.Location = new System.Drawing.Point(18, 201);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(231, 82);
            this.groupBox6.TabIndex = 42;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Notification color theme";
            // 
            // uiThemeColorSelector
            // 
            this.uiThemeColorSelector.FormattingEnabled = true;
            this.uiThemeColorSelector.Items.AddRange(new object[] {
            "Blue",
            "Brown",
            "Green",
            "White",
            "Yellow"});
            this.uiThemeColorSelector.Location = new System.Drawing.Point(6, 36);
            this.uiThemeColorSelector.Name = "uiThemeColorSelector";
            this.uiThemeColorSelector.Size = new System.Drawing.Size(219, 33);
            this.uiThemeColorSelector.Sorted = true;
            this.uiThemeColorSelector.TabIndex = 49;
            this.uiThemeColorSelector.SelectedIndexChanged += new System.EventHandler(this.uiThemeColorSelector_SelectedIndexChanged_1);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 11.12727F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(17, 22);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(175, 25);
            this.label5.TabIndex = 25;
            this.label5.Text = "Entries in clipboard";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // uiEntries
            // 
            this.uiEntries.AutoSize = true;
            this.uiEntries.Font = new System.Drawing.Font("Segoe UI", 11.12727F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiEntries.Location = new System.Drawing.Point(184, 22);
            this.uiEntries.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.uiEntries.Name = "uiEntries";
            this.uiEntries.Size = new System.Drawing.Size(23, 25);
            this.uiEntries.TabIndex = 22;
            this.uiEntries.Text = "#";
            this.uiEntries.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.Window;
            this.textBox1.Location = new System.Drawing.Point(22, 89);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(516, 202);
            this.textBox1.TabIndex = 50;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 11.12727F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(17, 61);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(183, 25);
            this.label1.TabIndex = 48;
            this.label1.Text = "Debug for now [key]";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(573, 441);
            this.tabControl1.TabIndex = 51;
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
            this.groupBox7.Controls.Add(this.uiHotkeySystem);
            this.groupBox7.Controls.Add(this.uiHotkeyPaste);
            this.groupBox7.Location = new System.Drawing.Point(19, 23);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(522, 107);
            this.groupBox7.TabIndex = 50;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Hotkey behaviour";
            // 
            // uiHotkeySystem
            // 
            this.uiHotkeySystem.AutoSize = true;
            this.uiHotkeySystem.Checked = true;
            this.uiHotkeySystem.Location = new System.Drawing.Point(29, 36);
            this.uiHotkeySystem.Name = "uiHotkeySystem";
            this.uiHotkeySystem.Size = new System.Drawing.Size(148, 29);
            this.uiHotkeySystem.TabIndex = 36;
            this.uiHotkeySystem.TabStop = true;
            this.uiHotkeySystem.Text = "Always action";
            this.uiHotkeySystem.UseVisualStyleBackColor = true;
            this.uiHotkeySystem.CheckedChanged += new System.EventHandler(this.uiHotkeySystem_CheckedChanged);
            // 
            // uiHotkeyPaste
            // 
            this.uiHotkeyPaste.AutoSize = true;
            this.uiHotkeyPaste.Enabled = false;
            this.uiHotkeyPaste.Location = new System.Drawing.Point(29, 67);
            this.uiHotkeyPaste.Name = "uiHotkeyPaste";
            this.uiHotkeyPaste.Size = new System.Drawing.Size(216, 29);
            this.uiHotkeyPaste.TabIndex = 37;
            this.uiHotkeyPaste.Text = "Action only on hotkey";
            this.uiHotkeyPaste.UseVisualStyleBackColor = true;
            this.uiHotkeyPaste.CheckedChanged += new System.EventHandler(this.uiHotkeyPaste_CheckedChanged);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.button1);
            this.groupBox5.Controls.Add(this.hotkeyPaste);
            this.groupBox5.Controls.Add(this.label4);
            this.groupBox5.Controls.Add(this.hotkeyOlder);
            this.groupBox5.Controls.Add(this.hotkeyEnable);
            this.groupBox5.Controls.Add(this.hotkeyNewer);
            this.groupBox5.Controls.Add(this.label6);
            this.groupBox5.Controls.Add(this.label7);
            this.groupBox5.Controls.Add(this.label8);
            this.groupBox5.Location = new System.Drawing.Point(19, 150);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(522, 228);
            this.groupBox5.TabIndex = 49;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Hotkeys";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(274, 182);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(195, 38);
            this.button1.TabIndex = 50;
            this.button1.Text = "Apply hotkeys";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // hotkeyPaste
            // 
            this.hotkeyPaste.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.hotkeyPaste.Location = new System.Drawing.Point(274, 144);
            this.hotkeyPaste.Name = "hotkeyPaste";
            this.hotkeyPaste.ReadOnly = true;
            this.hotkeyPaste.Size = new System.Drawing.Size(195, 32);
            this.hotkeyPaste.TabIndex = 49;
            this.hotkeyPaste.Text = "ALT + O";
            this.hotkeyPaste.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
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
            // hotkeyOlder
            // 
            this.hotkeyOlder.BackColor = System.Drawing.SystemColors.HighlightText;
            this.hotkeyOlder.Location = new System.Drawing.Point(274, 68);
            this.hotkeyOlder.Name = "hotkeyOlder";
            this.hotkeyOlder.ReadOnly = true;
            this.hotkeyOlder.Size = new System.Drawing.Size(195, 32);
            this.hotkeyOlder.TabIndex = 36;
            this.hotkeyOlder.Text = "ALT + H";
            this.hotkeyOlder.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // hotkeyEnable
            // 
            this.hotkeyEnable.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.hotkeyEnable.Location = new System.Drawing.Point(274, 29);
            this.hotkeyEnable.Name = "hotkeyEnable";
            this.hotkeyEnable.ReadOnly = true;
            this.hotkeyEnable.Size = new System.Drawing.Size(195, 32);
            this.hotkeyEnable.TabIndex = 47;
            this.hotkeyEnable.Text = "CTRL + ½";
            this.hotkeyEnable.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // hotkeyNewer
            // 
            this.hotkeyNewer.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.hotkeyNewer.Location = new System.Drawing.Point(274, 106);
            this.hotkeyNewer.Name = "hotkeyNewer";
            this.hotkeyNewer.ReadOnly = true;
            this.hotkeyNewer.Size = new System.Drawing.Size(195, 32);
            this.hotkeyNewer.TabIndex = 46;
            this.hotkeyNewer.Text = "SHIFT + ALT + H";
            this.hotkeyNewer.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
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
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tabPage3.Controls.Add(this.label5);
            this.tabPage3.Controls.Add(this.textBox1);
            this.tabPage3.Controls.Add(this.label1);
            this.tabPage3.Controls.Add(this.uiEntries);
            this.tabPage3.Location = new System.Drawing.Point(4, 34);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(565, 403);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Debug";
            // 
            // tabPage4
            // 
            this.tabPage4.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tabPage4.Controls.Add(this.linkLabel1);
            this.tabPage4.Controls.Add(this.label3);
            this.tabPage4.Controls.Add(this.label2);
            this.tabPage4.Controls.Add(this.appVer);
            this.tabPage4.Location = new System.Drawing.Point(4, 34);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(565, 403);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "About";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(24, 115);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(515, 89);
            this.label3.TabIndex = 51;
            this.label3.Text = "This application is open source and you are allowed to use it on any computer you" +
    " want. It is not allowed to sell this application or distribute it in any commer" +
    "cial regard.";
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
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(145, 459);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(294, 38);
            this.button4.TabIndex = 52;
            this.button4.Text = "Show online help for this tab";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // timerUpdate
            // 
            this.timerUpdate.Interval = 2000;
            this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(596, 507);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("Segoe UI", 11.12727F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.Name = "Settings";
            this.Text = "HovText";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing_1);
            this.Shown += new System.EventHandler(this.MainWindow_Shown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Settings_KeyUp);
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
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.CheckBox uiAppEnabled;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.CheckBox uiHistoryEnabled;
        private System.Windows.Forms.CheckBox uiStartWithWindows;
        private System.Windows.Forms.FontDialog fontDialog1;
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
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label uiEntries;
        private System.Windows.Forms.ComboBox uiThemeColorSelector;
        private System.Windows.Forms.Label appVer;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ContextMenuStrip notifyIconMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem settingsMenu;
        private System.Windows.Forms.ToolStripMenuItem exit;
        private System.Windows.Forms.ToolStripMenuItem about;
        private System.Windows.Forms.CheckBox uiCloseMinimize;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox uiPasteOnSelection;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.RadioButton uiHotkeySystem;
        private System.Windows.Forms.RadioButton uiHotkeyPaste;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox hotkeyPaste;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox hotkeyOlder;
        private System.Windows.Forms.TextBox hotkeyEnable;
        private System.Windows.Forms.TextBox hotkeyNewer;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.CheckBox uiCheckUpdates;
        private System.Windows.Forms.Timer timerUpdate;
    }

}