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
            this.GuiTooBigDisable = new System.Windows.Forms.Button();
            this.TooBigInfo = new System.Windows.Forms.Label();
            this.GuiTooBigKeep = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // GuiTooBigDisable
            // 
            this.GuiTooBigDisable.Location = new System.Drawing.Point(56, 142);
            this.GuiTooBigDisable.Margin = new System.Windows.Forms.Padding(5, 8, 5, 8);
            this.GuiTooBigDisable.Name = "GuiTooBigDisable";
            this.GuiTooBigDisable.Size = new System.Drawing.Size(270, 37);
            this.GuiTooBigDisable.TabIndex = 1;
            this.GuiTooBigDisable.Text = "Disable troubleshoot logging";
            this.GuiTooBigDisable.UseVisualStyleBackColor = true;
            this.GuiTooBigDisable.Click += new System.EventHandler(this.GuiTooBigDisable_Click);
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
            // GuiTooBigKeep
            // 
            this.GuiTooBigKeep.Location = new System.Drawing.Point(334, 142);
            this.GuiTooBigKeep.Margin = new System.Windows.Forms.Padding(5, 8, 5, 8);
            this.GuiTooBigKeep.Name = "GuiTooBigKeep";
            this.GuiTooBigKeep.Size = new System.Drawing.Size(270, 37);
            this.GuiTooBigKeep.TabIndex = 2;
            this.GuiTooBigKeep.Text = "Keep logging but truncate file";
            this.GuiTooBigKeep.UseVisualStyleBackColor = true;
            this.GuiTooBigKeep.Click += new System.EventHandler(this.GuiTooBigKeep_Click);
            // 
            // TooBigLogfile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(661, 201);
            this.ControlBox = false;
            this.Controls.Add(this.GuiTooBigKeep);
            this.Controls.Add(this.GuiTooBigDisable);
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

        private System.Windows.Forms.Button GuiTooBigDisable;
        private System.Windows.Forms.Label TooBigInfo;
        private System.Windows.Forms.Button GuiTooBigKeep;
    }
}