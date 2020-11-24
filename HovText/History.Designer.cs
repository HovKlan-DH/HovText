namespace HovText
{
    partial class History
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
            this.textBoxHistory = new System.Windows.Forms.Label();
            this.pictureHistory = new System.Windows.Forms.PictureBox();
            this.topline = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureHistory)).BeginInit();
            this.SuspendLayout();
            // 
            // textBoxHistory
            // 
            this.textBoxHistory.BackColor = System.Drawing.SystemColors.Info;
            this.textBoxHistory.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxHistory.Location = new System.Drawing.Point(-5, 34);
            this.textBoxHistory.Name = "textBoxHistory";
            this.textBoxHistory.Padding = new System.Windows.Forms.Padding(7);
            this.textBoxHistory.Size = new System.Drawing.Size(523, 335);
            this.textBoxHistory.TabIndex = 25;
            this.textBoxHistory.Text = "#";
            // 
            // pictureHistory
            // 
            this.pictureHistory.BackColor = System.Drawing.SystemColors.Info;
            this.pictureHistory.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureHistory.Location = new System.Drawing.Point(-5, 34);
            this.pictureHistory.Margin = new System.Windows.Forms.Padding(8, 12, 8, 12);
            this.pictureHistory.Name = "pictureHistory";
            this.pictureHistory.Padding = new System.Windows.Forms.Padding(5);
            this.pictureHistory.Size = new System.Drawing.Size(547, 320);
            this.pictureHistory.TabIndex = 22;
            this.pictureHistory.TabStop = false;
            // 
            // topline
            // 
            this.topline.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.topline.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.topline.Location = new System.Drawing.Point(-3, -3);
            this.topline.Name = "topline";
            this.topline.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.topline.Size = new System.Drawing.Size(521, 37);
            this.topline.TabIndex = 26;
            this.topline.Text = "#";
            this.topline.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // History
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.ClientSize = new System.Drawing.Size(508, 321);
            this.ControlBox = false;
            this.Controls.Add(this.topline);
            this.Controls.Add(this.textBoxHistory);
            this.Controls.Add(this.pictureHistory);
            this.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Location = new System.Drawing.Point(1365, 785);
            this.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.Name = "History";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.TopMost = true;
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.History_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.pictureHistory)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label textBoxHistory;
        private System.Windows.Forms.PictureBox pictureHistory;
        private System.Windows.Forms.Label topline;

    }
}