namespace HovText
{
    partial class TooBigLogfile
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TooBigLogfile));
            this.TooBigInfo = new System.Windows.Forms.Label();
            this.guna2Button6 = new Guna.UI2.WinForms.Guna2Button();
            this.guna2Button1 = new Guna.UI2.WinForms.Guna2Button();
            this.SuspendLayout();
            // 
            // TooBigInfo
            // 
            this.TooBigInfo.Location = new System.Drawing.Point(41, 32);
            this.TooBigInfo.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.TooBigInfo.Name = "TooBigInfo";
            this.TooBigInfo.Size = new System.Drawing.Size(579, 122);
            this.TooBigInfo.TabIndex = 3;
            this.TooBigInfo.Text = resources.GetString("TooBigInfo.Text");
            // 
            // guna2Button6
            // 
            this.guna2Button6.AutoRoundedCorners = true;
            this.guna2Button6.BorderRadius = 15;
            this.guna2Button6.Cursor = System.Windows.Forms.Cursors.Hand;
            this.guna2Button6.DisabledState.BorderColor = System.Drawing.Color.Silver;
            this.guna2Button6.DisabledState.CustomBorderColor = System.Drawing.Color.Silver;
            this.guna2Button6.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.guna2Button6.DisabledState.ForeColor = System.Drawing.Color.Gray;
            this.guna2Button6.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(97)))), ((int)(((byte)(55)))));
            this.guna2Button6.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2Button6.ForeColor = System.Drawing.Color.White;
            this.guna2Button6.Location = new System.Drawing.Point(56, 150);
            this.guna2Button6.Name = "guna2Button6";
            this.guna2Button6.Size = new System.Drawing.Size(270, 32);
            this.guna2Button6.TabIndex = 1005;
            this.guna2Button6.Text = "Disable troubleshoot logging";
            this.guna2Button6.Click += new System.EventHandler(this.GuiTooBigDisable_Click);
            // 
            // guna2Button1
            // 
            this.guna2Button1.AutoRoundedCorners = true;
            this.guna2Button1.BorderRadius = 15;
            this.guna2Button1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.guna2Button1.DisabledState.BorderColor = System.Drawing.Color.Silver;
            this.guna2Button1.DisabledState.CustomBorderColor = System.Drawing.Color.Silver;
            this.guna2Button1.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.guna2Button1.DisabledState.ForeColor = System.Drawing.Color.Gray;
            this.guna2Button1.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(97)))), ((int)(((byte)(55)))));
            this.guna2Button1.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2Button1.ForeColor = System.Drawing.Color.White;
            this.guna2Button1.Location = new System.Drawing.Point(332, 150);
            this.guna2Button1.Name = "guna2Button1";
            this.guna2Button1.Size = new System.Drawing.Size(270, 32);
            this.guna2Button1.TabIndex = 1006;
            this.guna2Button1.Text = "Keep logging but truncate file";
            this.guna2Button1.Click += new System.EventHandler(this.GuiTooBigKeep_Click);
            // 
            // TooBigLogfile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(661, 201);
            this.ControlBox = false;
            this.Controls.Add(this.guna2Button1);
            this.Controls.Add(this.guna2Button6);
            this.Controls.Add(this.TooBigInfo);
            this.Font = new System.Drawing.Font("Segoe UI", 10.8F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TooBigLogfile";
            this.Text = "HovText troubleshoot logfile is getting too big";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label TooBigInfo;
        private Guna.UI2.WinForms.Guna2Button guna2Button6;
        private Guna.UI2.WinForms.Guna2Button guna2Button1;
    }
}