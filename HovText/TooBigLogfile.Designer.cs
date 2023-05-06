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
            this.GuiDev2StableDisable = new System.Windows.Forms.Button();
            this.Dev2StableInfo = new System.Windows.Forms.Label();
            this.GuiDev2StableKeep = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // GuiDev2StableDisable
            // 
            this.GuiDev2StableDisable.Location = new System.Drawing.Point(56, 142);
            this.GuiDev2StableDisable.Margin = new System.Windows.Forms.Padding(5, 8, 5, 8);
            this.GuiDev2StableDisable.Name = "GuiDev2StableDisable";
            this.GuiDev2StableDisable.Size = new System.Drawing.Size(270, 37);
            this.GuiDev2StableDisable.TabIndex = 1;
            this.GuiDev2StableDisable.Text = "Disable troubleshoot logging";
            this.GuiDev2StableDisable.UseVisualStyleBackColor = true;
            this.GuiDev2StableDisable.Click += new System.EventHandler(this.GuiDev2StableDisable_Click);
            // 
            // Dev2StableInfo
            // 
            this.Dev2StableInfo.Location = new System.Drawing.Point(41, 32);
            this.Dev2StableInfo.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Dev2StableInfo.Name = "Dev2StableInfo";
            this.Dev2StableInfo.Size = new System.Drawing.Size(579, 122);
            this.Dev2StableInfo.TabIndex = 3;
            this.Dev2StableInfo.Text = resources.GetString("Dev2StableInfo.Text");
            // 
            // GuiDev2StableKeep
            // 
            this.GuiDev2StableKeep.Location = new System.Drawing.Point(334, 142);
            this.GuiDev2StableKeep.Margin = new System.Windows.Forms.Padding(5, 8, 5, 8);
            this.GuiDev2StableKeep.Name = "GuiDev2StableKeep";
            this.GuiDev2StableKeep.Size = new System.Drawing.Size(270, 37);
            this.GuiDev2StableKeep.TabIndex = 2;
            this.GuiDev2StableKeep.Text = "Keep logging but truncate file";
            this.GuiDev2StableKeep.UseVisualStyleBackColor = true;
            this.GuiDev2StableKeep.Click += new System.EventHandler(this.GuiDev2StableKeep_Click);
            // 
            // DevToStable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(661, 201);
            this.ControlBox = false;
            this.Controls.Add(this.GuiDev2StableKeep);
            this.Controls.Add(this.GuiDev2StableDisable);
            this.Controls.Add(this.Dev2StableInfo);
            this.Font = new System.Drawing.Font("Segoe UI", 10.8F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DevToStable";
            this.Text = "HovText troubleshoot logfile is getting too big";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button GuiDev2StableDisable;
        private System.Windows.Forms.Label Dev2StableInfo;
        private System.Windows.Forms.Button GuiDev2StableKeep;
    }
}