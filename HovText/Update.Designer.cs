namespace HovText
{
    partial class Update
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Update));
            this.GuiUpdateButton1 = new System.Windows.Forms.Button();
            this.GuiUpdateButton2 = new System.Windows.Forms.Button();
            this.Label1 = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.Label3 = new System.Windows.Forms.Label();
            this.GuiAppVerYours = new System.Windows.Forms.Label();
            this.GuiAppVerOnline = new System.Windows.Forms.Label();
            this.GuiUpdateButton3 = new System.Windows.Forms.Button();
            this.TextBox1 = new System.Windows.Forms.TextBox();
            this.UpdateGoToHomepage = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // GuiUpdateButton1
            // 
            this.GuiUpdateButton1.Location = new System.Drawing.Point(400, 228);
            this.GuiUpdateButton1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.GuiUpdateButton1.Name = "GuiUpdateButton1";
            this.GuiUpdateButton1.Size = new System.Drawing.Size(182, 37);
            this.GuiUpdateButton1.TabIndex = 2;
            this.GuiUpdateButton1.Text = "Skip this version";
            this.GuiUpdateButton1.UseVisualStyleBackColor = true;
            this.GuiUpdateButton1.Click += new System.EventHandler(this.SkipVersion_Click);
            // 
            // GuiUpdateButton2
            // 
            this.GuiUpdateButton2.Location = new System.Drawing.Point(210, 228);
            this.GuiUpdateButton2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.GuiUpdateButton2.Name = "GuiUpdateButton2";
            this.GuiUpdateButton2.Size = new System.Drawing.Size(182, 37);
            this.GuiUpdateButton2.TabIndex = 1;
            this.GuiUpdateButton2.Text = "Manual download";
            this.GuiUpdateButton2.UseVisualStyleBackColor = true;
            this.GuiUpdateButton2.Click += new System.EventHandler(this.Download_Click);
            // 
            // Label1
            // 
            this.Label1.Location = new System.Drawing.Point(14, 30);
            this.Label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(560, 58);
            this.Label1.TabIndex = 2;
            this.Label1.Text = "The newest stable version available on the HovText home page is different from th" +
    "e version you use.";
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label2.Location = new System.Drawing.Point(43, 88);
            this.Label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(161, 31);
            this.Label2.TabIndex = 3;
            this.Label2.Text = "Stable version:";
            this.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label3.Location = new System.Drawing.Point(43, 122);
            this.Label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(143, 31);
            this.Label3.TabIndex = 4;
            this.Label3.Text = "Your version:";
            this.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // GuiAppVerYours
            // 
            this.GuiAppVerYours.AutoSize = true;
            this.GuiAppVerYours.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GuiAppVerYours.Location = new System.Drawing.Point(177, 122);
            this.GuiAppVerYours.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.GuiAppVerYours.Name = "GuiAppVerYours";
            this.GuiAppVerYours.Size = new System.Drawing.Size(28, 31);
            this.GuiAppVerYours.TabIndex = 5;
            this.GuiAppVerYours.Text = "#";
            this.GuiAppVerYours.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // GuiAppVerOnline
            // 
            this.GuiAppVerOnline.AutoSize = true;
            this.GuiAppVerOnline.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GuiAppVerOnline.Location = new System.Drawing.Point(177, 88);
            this.GuiAppVerOnline.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.GuiAppVerOnline.Name = "GuiAppVerOnline";
            this.GuiAppVerOnline.Size = new System.Drawing.Size(28, 31);
            this.GuiAppVerOnline.TabIndex = 6;
            this.GuiAppVerOnline.Text = "#";
            this.GuiAppVerOnline.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // GuiUpdateButton3
            // 
            this.GuiUpdateButton3.Location = new System.Drawing.Point(19, 228);
            this.GuiUpdateButton3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.GuiUpdateButton3.Name = "GuiUpdateButton3";
            this.GuiUpdateButton3.Size = new System.Drawing.Size(183, 37);
            this.GuiUpdateButton3.TabIndex = 0;
            this.GuiUpdateButton3.Text = "Auto-install";
            this.GuiUpdateButton3.UseVisualStyleBackColor = true;
            this.GuiUpdateButton3.Click += new System.EventHandler(this.GuiUpdateButton3_Click);
            // 
            // TextBox1
            // 
            this.TextBox1.BackColor = System.Drawing.SystemColors.Control;
            this.TextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextBox1.Location = new System.Drawing.Point(19, 188);
            this.TextBox1.Multiline = true;
            this.TextBox1.Name = "TextBox1";
            this.TextBox1.Size = new System.Drawing.Size(555, 32);
            this.TextBox1.TabIndex = 8;
            this.TextBox1.Text = "Please choose one of the options below to proceed:";
            // 
            // UpdateGoToHomepage
            // 
            this.UpdateGoToHomepage.BackColor = System.Drawing.SystemColors.Control;
            this.UpdateGoToHomepage.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.UpdateGoToHomepage.Location = new System.Drawing.Point(22, 166);
            this.UpdateGoToHomepage.Name = "UpdateGoToHomepage";
            this.UpdateGoToHomepage.ReadOnly = true;
            this.UpdateGoToHomepage.Size = new System.Drawing.Size(552, 42);
            this.UpdateGoToHomepage.TabIndex = 9;
            this.UpdateGoToHomepage.TabStop = false;
            this.UpdateGoToHomepage.Text = "#";
            this.UpdateGoToHomepage.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.UpdateGoToHomepage_LinkClicked);
            // 
            // Update
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(601, 288);
            this.Controls.Add(this.GuiUpdateButton3);
            this.Controls.Add(this.GuiUpdateButton2);
            this.Controls.Add(this.GuiUpdateButton1);
            this.Controls.Add(this.TextBox1);
            this.Controls.Add(this.UpdateGoToHomepage);
            this.Controls.Add(this.GuiAppVerOnline);
            this.Controls.Add(this.GuiAppVerYours);
            this.Controls.Add(this.Label3);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.Label1);
            this.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Update";
            this.Text = "HovText version difference found";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button GuiUpdateButton1;
        private System.Windows.Forms.Button GuiUpdateButton2;
        private System.Windows.Forms.Label Label1;
        private System.Windows.Forms.Label Label2;
        private System.Windows.Forms.Label Label3;
        public System.Windows.Forms.Label GuiAppVerYours;
        public System.Windows.Forms.Label GuiAppVerOnline;
        private System.Windows.Forms.Button GuiUpdateButton3;
        private System.Windows.Forms.TextBox TextBox1;
        private System.Windows.Forms.RichTextBox UpdateGoToHomepage;
    }
}