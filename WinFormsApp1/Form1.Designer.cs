namespace WinFormsApp1
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.btnOpenFile = new System.Windows.Forms.Button();
            this.txtFactor = new System.Windows.Forms.TextBox();
            this.lblFactor = new System.Windows.Forms.Label();
            this.lblCreator = new System.Windows.Forms.Label();
            this.btnDownscale = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            
            // File path field
            this.txtFilePath.Location = new System.Drawing.Point(12, 12);
            this.txtFilePath.Size = new System.Drawing.Size(245, 23);
            
            // Open file button
            this.btnOpenFile.Text = "Open file";
            this.btnOpenFile.AutoSize = true;
            this.btnOpenFile.Location = new System.Drawing.Point(264, 10);
            this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
            
            // Label downscale
            this.lblFactor.Text = "Downscale %:";
            this.lblFactor.Location = new System.Drawing.Point(12, 51);
            this.lblFactor.AutoSize = true;

            // Factor field
            this.txtFactor.Location = new System.Drawing.Point(18 + lblFactor.Width, 47);
            this.txtFactor.AutoSize = true;
            this.txtFactor.TabIndex = 1;
            
            // Downscale button
            this.btnDownscale.Text = "Start downscaling";
            this.btnDownscale.AutoSize = true;
            this.btnDownscale.Width = 230;
            this.btnDownscale.Location = new System.Drawing.Point(11, 80);
            this.btnDownscale.Click += new System.EventHandler(this.btnDownscale_Click);
            
            // Cancel button
            this.btnCancel.Text = "Cancel";
            this.btnCancel.AutoSize = true;
            this.btnCancel.Width = 100;
            this.btnCancel.Location = new System.Drawing.Point(343 - btnCancel.Width, 80);
            this.btnCancel.Enabled = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            
            // Progress bar
            this.progressBar.Location = new System.Drawing.Point(12, 115);
            this.progressBar.Size = new System.Drawing.Size(330, 23);
            this.progressBar.TabIndex = 2;
            
            // Label creator
            this.lblCreator.Text = "Made by Dimitar Gogov";
            this.lblCreator.Location = new System.Drawing.Point(12, 145);
            this.lblCreator.AutoSize = true;
            
            // Form
            this.ClientSize = new System.Drawing.Size(354, 175);
            this.Controls.Add(this.txtFilePath);
            this.Controls.Add(this.btnOpenFile);
            this.Controls.Add(this.lblFactor);
            this.Controls.Add(this.txtFactor);
            this.Controls.Add(this.btnDownscale);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.lblCreator);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Image Downscaler";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label label1;

        #endregion

        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.Button btnOpenFile;
        private System.Windows.Forms.TextBox txtFactor;
        private System.Windows.Forms.Label lblFactor;
        private System.Windows.Forms.Button btnDownscale;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ProgressBar progressBar;
    }
}
