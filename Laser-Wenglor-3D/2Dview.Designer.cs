namespace Laser_Wenglor_3D
{
    partial class _2Dview
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
            this.m_pictureBoxScanView = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.m_pictureBoxScanView)).BeginInit();
            this.SuspendLayout();
            // 
            // m_pictureBoxScanView
            // 
            this.m_pictureBoxScanView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_pictureBoxScanView.Location = new System.Drawing.Point(0, 0);
            this.m_pictureBoxScanView.Name = "m_pictureBoxScanView";
            this.m_pictureBoxScanView.Size = new System.Drawing.Size(800, 450);
            this.m_pictureBoxScanView.TabIndex = 0;
            this.m_pictureBoxScanView.TabStop = false;
            // 
            // _2Dview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.m_pictureBoxScanView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "_2Dview";
            this.Text = "2Dview";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this._2Dview_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.m_pictureBoxScanView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox m_pictureBoxScanView;
    }
}