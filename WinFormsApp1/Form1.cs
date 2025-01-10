namespace WinFormsApp1;

using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

public partial class Form1 : Form
{
    private CancellationTokenSource cancellationTokenSource;
    private Thread downscaleThread;
    private Label lblCreator;

    public Form1()
    {
        InitializeComponent();
    }

    // Open file
    private void btnOpenFile_Click(object sender, EventArgs e)
    {
        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtFilePath.Text = openFileDialog.FileName;
            }
        }
    }

    // Downscale logic
    private void btnDownscale_Click(object sender, EventArgs e)
    {
        if (!float.TryParse(txtFactor.Text, out float factor) || factor <= 0 || factor > 100)
        {
            MessageBox.Show("Please enter a valid percentage (1-100)!", "Invalid input!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(txtFilePath.Text) || !File.Exists(txtFilePath.Text))
        {
            MessageBox.Show("Please select a valid image file!", "Invalid input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        cancellationTokenSource = new CancellationTokenSource();

        try
        {
            btnDownscale.Enabled = false;
            btnCancel.Enabled = true;
            progressBar.Value = 0;

            string outputFilePath = (
                Path.Combine(Path.GetDirectoryName(txtFilePath.Text),
                $"(Downscaled {txtFactor.Text}% of original) {Path.GetFileName(txtFilePath.Text)}")
            );

            Bitmap inputImage = new Bitmap(txtFilePath.Text);
            float scaleFactor = factor / 100f;

            // Start thread
            downscaleThread = new Thread(() => {
                
                Bitmap downscaledImage = DownscaleImage(inputImage, scaleFactor, cancellationTokenSource.Token);
                downscaledImage.Save(outputFilePath);
                downscaledImage.Dispose();

                Invoke(new Action(() => {
                    
                    MessageBox.Show($"The image is downscaled to:\n{outputFilePath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    string folderPath = Path.GetDirectoryName(outputFilePath);
                    
                    if (!string.IsNullOrEmpty(folderPath))
                    {
                        System.Diagnostics.Process.Start("explorer.exe", folderPath);
                    }

                    btnDownscale.Enabled = true;
                    btnCancel.Enabled = false;
                    cancellationTokenSource.Dispose();
                    
                }));
                
            });

            // Start
            downscaleThread.Start();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // Cancel logic
    private void btnCancel_Click(object sender, EventArgs e)
    {
        cancellationTokenSource?.Cancel();
        downscaleThread?.Abort();
    }

    // Get downscaled image
    private Bitmap DownscaleImage(Bitmap inputImage, float scaleFactor, CancellationToken cancellationToken)
    {
        int newWidth = (int)(inputImage.Width * scaleFactor);
        int newHeight = (int)(inputImage.Height * scaleFactor);

        Bitmap outputImage = new Bitmap(newWidth, newHeight);

        for (int y = 0; y < newHeight; y++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            for (int x = 0; x < newWidth; x++)
            {
                float gx = x / (float)newWidth * inputImage.Width;
                float gy = y / (float)newHeight * inputImage.Height;

                int x1 = (int)Math.Floor(gx);
                int y1 = (int)Math.Floor(gy);
                int x2 = Math.Min(x1 + 1, inputImage.Width - 1);
                int y2 = Math.Min(y1 + 1, inputImage.Height - 1);

                Color c1 = inputImage.GetPixel(x1, y1);
                Color c2 = inputImage.GetPixel(x2, y1);
                Color c3 = inputImage.GetPixel(x1, y2);
                Color c4 = inputImage.GetPixel(x2, y2);

                float dx = gx - x1;
                float dy = gy - y1;

                Color interpolated = InterpolateColors(c1, c2, c3, c4, dx, dy);
                outputImage.SetPixel(x, y, interpolated);
            }

            // Update
            Invoke(new Action(() => progressBar.Value = (int)((y / (float)newHeight) * 100)));
        }

        return outputImage;
    }

    // Get colors
    private Color InterpolateColors(Color c1, Color c2, Color c3, Color c4, float dx, float dy)
    {
        float r = c1.R * (1 - dx) * (1 - dy) + c2.R * dx * (1 - dy) + c3.R * (1 - dx) * dy + c4.R * dx * dy;
        float g = c1.G * (1 - dx) * (1 - dy) + c2.G * dx * (1 - dy) + c3.G * (1 - dx) * dy + c4.G * dx * dy;
        float b = c1.B * (1 - dx) * (1 - dy) + c2.B * dx * (1 - dy) + c3.B * (1 - dx) * dy + c4.B * dx * dy;

        return Color.FromArgb((int)r, (int)g, (int)b);
    }
}
