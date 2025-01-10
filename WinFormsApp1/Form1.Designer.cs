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

        private void InitializeComponent()
        {
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.btnOpenFile = new System.Windows.Forms.Button();
            this.txtFactor = new System.Windows.Forms.TextBox();
            this.lblFactor = new System.Windows.Forms.Label();
            this.lblCreator = new System.Windows.Forms.Label();

            this.lblTitleLnr = new System.Windows.Forms.Label();
            this.btnDownscaleLnr = new System.Windows.Forms.Button();
            this.btnCancelLnr = new System.Windows.Forms.Button();
            this.progressBarLnr = new System.Windows.Forms.ProgressBar();

            this.lblTitlePrl = new System.Windows.Forms.Label();
            this.btnDownscalePrl = new System.Windows.Forms.Button();
            this.btnCancelPrl = new System.Windows.Forms.Button();
            this.progressBarPrl = new System.Windows.Forms.ProgressBar();

            this.SuspendLayout();

            // File Path TextBox
            this.txtFilePath.Location = new System.Drawing.Point(12, 12);
            this.txtFilePath.Size = new System.Drawing.Size(245, 23);

            // Open File Button
            this.btnOpenFile.Text = "Open file";
            this.btnOpenFile.AutoSize = true;
            this.btnOpenFile.Location = new System.Drawing.Point(264, 10);
            this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);

            // Downscale Percentage Label
            this.lblFactor.Text = "Downscale %:";
            this.lblFactor.Location = new System.Drawing.Point(12, 51);
            this.lblFactor.AutoSize = true;

            // Factor TextBox
            this.txtFactor.Location = new System.Drawing.Point(18 + lblFactor.Width, 47);
            this.txtFactor.Size = new System.Drawing.Size(100, 23);

            // Linear Downscale Section
            this.lblTitleLnr.Text = "Linear Downscale:";
            this.lblTitleLnr.Location = new System.Drawing.Point(12, 80);
            this.lblTitleLnr.AutoSize = true;

            this.btnDownscaleLnr.Text = "Start Linear Downscaling";
            this.btnDownscaleLnr.AutoSize = true;
            this.btnDownscaleLnr.Width = 230;
            this.btnDownscaleLnr.Location = new System.Drawing.Point(12, 110);
            this.btnDownscaleLnr.Click += new System.EventHandler(this.btnDownscaleLnr_Click);

            this.btnCancelLnr.Text = "Cancel";
            this.btnCancelLnr.AutoSize = true;
            this.btnCancelLnr.Width = 100;
            this.btnCancelLnr.Location = new System.Drawing.Point(343 - this.btnCancelLnr.Width, 110);

            this.progressBarLnr.Location = new System.Drawing.Point(12, 145);
            this.progressBarLnr.Size = new System.Drawing.Size(330, 23);

            // Parallel Downscale Section
            this.lblTitlePrl.Text = "Parallel Downscale:";
            this.lblTitlePrl.Location = new System.Drawing.Point(12, 180);
            this.lblTitlePrl.AutoSize = true;

            this.btnDownscalePrl.Text = "Start Parallel Downscaling";
            this.btnDownscalePrl.AutoSize = true;
            this.btnDownscalePrl.Width = 230;
            this.btnDownscalePrl.Location = new System.Drawing.Point(12, 210);
            this.btnDownscalePrl.Click += new System.EventHandler(this.btnDownscalePrl_Click);

            this.btnCancelPrl.Text = "Cancel";
            this.btnCancelPrl.AutoSize = true;
            this.btnCancelPrl.Width = 100;
            this.btnCancelPrl.Location = new System.Drawing.Point(343 - this.btnCancelPrl.Width, 210);

            this.progressBarPrl.Location = new System.Drawing.Point(12, 245);
            this.progressBarPrl.Size = new System.Drawing.Size(330, 23);

            // Creator Label
            this.lblCreator.Text = "Made by Dimitar Gogov";
            this.lblCreator.Location = new System.Drawing.Point(12, 275);
            this.lblCreator.AutoSize = true;

            // Form Settings
            this.ClientSize = new System.Drawing.Size(354, 315);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Image Downscaler";

            // Adding Controls
            this.Controls.Add(this.txtFilePath);
            this.Controls.Add(this.btnOpenFile);
            this.Controls.Add(this.txtFactor);
            this.Controls.Add(this.lblFactor);
            this.Controls.Add(this.lblCreator);
            this.Controls.Add(this.lblTitleLnr);
            this.Controls.Add(this.btnDownscaleLnr);
            this.Controls.Add(this.btnCancelLnr);
            this.Controls.Add(this.progressBarLnr);
            this.Controls.Add(this.lblTitlePrl);
            this.Controls.Add(this.btnDownscalePrl);
            this.Controls.Add(this.btnCancelPrl);
            this.Controls.Add(this.progressBarPrl);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        // Controls
        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.Button btnOpenFile;
        private System.Windows.Forms.TextBox txtFactor;
        private System.Windows.Forms.Label lblFactor;
        private System.Windows.Forms.Label lblCreator;

        private System.Windows.Forms.Label lblTitleLnr;
        private System.Windows.Forms.Button btnDownscaleLnr;
        private System.Windows.Forms.Button btnCancelLnr;
        private System.Windows.Forms.ProgressBar progressBarLnr;

        private System.Windows.Forms.Label lblTitlePrl;
        private System.Windows.Forms.Button btnDownscalePrl;
        private System.Windows.Forms.Button btnCancelPrl;
        private System.Windows.Forms.ProgressBar progressBarPrl;
    }
}
