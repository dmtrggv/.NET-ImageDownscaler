using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private CancellationTokenSource cancellationTokenSource;
        private Bitmap inputImage;
        private string outputFilePath;
        private int rows = 8;  // Number of chunks for parallel downscaling
        private int totalProgress = 0; // Variable to hold the overall progress
        private object progressLock = new object(); // Lock to ensure thread safety for progress updates

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

        // Linear Downscale
        private void btnDownscaleLnr_Click(object sender, EventArgs e)
        {
            StartDownscale(false);
        }

        // Parallel Downscale
        private void btnDownscalePrl_Click(object sender, EventArgs e)
        {
            StartDownscale(true);
        }

        private void StartDownscale(bool isParallel)
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
            totalProgress = 0;
            progressBarLnr.Value = 0;
            progressBarPrl.Value = 0;

            try
            {
                inputImage = new Bitmap(txtFilePath.Text);
                float scaleFactor = factor / 100f;
                int newPercent = 100 - (int)factor;

                // Determine the compression type
                string compressionType = isParallel ? "Parallel" : "Linear";

                // Prepare the output file path with the compression type
                outputFilePath = Path.Combine(
                    Path.GetDirectoryName(txtFilePath.Text),
                    $"(Downscaled by {newPercent}% using {compressionType} compression) {Path.GetFileName(txtFilePath.Text)}"
                );

                // Start compression on a background task
                Task.Run(() =>
                {
                    if (isParallel)
                    {
                        DownscaleImageParallel(inputImage, scaleFactor, cancellationTokenSource.Token);
                    }
                    else
                    {
                        DownscaleImageLinear(inputImage, scaleFactor, cancellationTokenSource.Token);
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Linear Downscale method
        private void DownscaleImageLinear(Bitmap inputImage, float scaleFactor, CancellationToken cancellationToken)
        {
            try
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

                        Color interpolated = InterpolateColors_Linar(c1, c2, c3, c4, dx, dy);
                        outputImage.SetPixel(x, y, interpolated);
                    }

                    // Update progress
                    UpdateProgress((int)((float)(y + 1) / newHeight * 100));

                    // Update the progress bar in UI thread
                    Invoke(new Action(() => progressBarLnr.Value = totalProgress));
                }

                // After compression, save and notify
                outputImage.Save(outputFilePath);
                Invoke(new Action(() =>
                {
                    MessageBox.Show($"Image saved at: {outputFilePath}", "Compression Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    System.Diagnostics.Process.Start("explorer.exe", Path.GetDirectoryName(outputFilePath));
                }));

                outputImage.Dispose();
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("The downscaling operation was canceled.", "Operation Canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during downscaling: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Parallel Downscale method
        private void DownscaleImageParallel(Bitmap inputImage, float scaleFactor, CancellationToken cancellationToken)
        {
            try
            {
                // Calculate the new width and height based on the scale factor
                int newWidth = (int)(inputImage.Width * scaleFactor);
                int newHeight = (int)(inputImage.Height * scaleFactor);

                // Block size (400x400)
                int blockWidth = 400;
                int blockHeight = 400;

                // Calculate the number of chunks horizontally and vertically
                int horizontalChunks = (int)Math.Ceiling((double)inputImage.Width / blockWidth);
                int verticalChunks = (int)Math.Ceiling((double)inputImage.Height / blockHeight);

                // Create an output image with the new dimensions
                Bitmap outputImage = new Bitmap(newWidth, newHeight);

                // Prepare for progress tracking
                int totalChunks = horizontalChunks * verticalChunks;
                int chunksProcessed = 0;

                // Create tasks for each chunk
                Task[] tasks = new Task[totalChunks];
                int taskIndex = 0;

                for (int chunkY = 0; chunkY < verticalChunks; chunkY++)
                {
                    for (int chunkX = 0; chunkX < horizontalChunks; chunkX++)
                    {
                        int startX = chunkX * blockWidth;
                        int startY = chunkY * blockHeight;
                        int chunkWidthAdjusted = Math.Min(blockWidth, inputImage.Width - startX);
                        int chunkHeightAdjusted = Math.Min(blockHeight, inputImage.Height - startY);

                        tasks[taskIndex] = Task.Run(() =>
                        {
                            // Crop the chunk from the original image
                            Rectangle chunkRect = new Rectangle(startX, startY, chunkWidthAdjusted, chunkHeightAdjusted);
                            using (Bitmap chunkImage = inputImage.Clone(chunkRect, inputImage.PixelFormat))
                            {
                                // Resize (downscale) the chunk
                                Bitmap downscaledChunk = new Bitmap(chunkImage, (int)(chunkImage.Width * scaleFactor), (int)(chunkImage.Height * scaleFactor));

                                // Update the output image with the downscaled chunk
                                using (Graphics g = Graphics.FromImage(outputImage))
                                {
                                    g.DrawImage(downscaledChunk, startX * scaleFactor, startY * scaleFactor);
                                }

                                // Track progress
                                Interlocked.Increment(ref chunksProcessed);
                                int progress = (int)((float)chunksProcessed / totalChunks * 100);

                                // Update the progress bar
                                Invoke(new Action(() =>
                                {
                                    progressBarPrl.Value = progress;
                                }));
                            }

                            // Check for cancellation
                            cancellationToken.ThrowIfCancellationRequested();
                        });

                        taskIndex++;
                    }
                }

                // Wait for all tasks to finish
                Task.WhenAll(tasks).Wait();

                // After all chunks are processed, save the image
                string outputFilePath = Path.Combine(
                    Path.GetDirectoryName(txtFilePath.Text),
                    $"(Downscaled {100 - Int32.Parse(txtFactor.Text)}%) {Path.GetFileName(txtFilePath.Text)}"
                );
                outputImage.Save(outputFilePath);

                // Notify the user and open the folder in Explorer
                Invoke(new Action(() =>
                {
                    MessageBox.Show($"Image saved at: {outputFilePath}", "Compression Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    System.Diagnostics.Process.Start("explorer.exe", Path.GetDirectoryName(outputFilePath));
                }));

                outputImage.Dispose();
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("The downscaling operation was canceled.", "Operation Canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during downscaling: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Get interpolated colors for downscaling
        private Color InterpolateColors_Linar(Color c1, Color c2, Color c3, Color c4, float dx, float dy)
        {
            float r = c1.R * (1 - dx) * (1 - dy) + c2.R * dx * (1 - dy) + c3.R * (1 - dx) * dy + c4.R * dx * dy;
            float g = c1.G * (1 - dx) * (1 - dy) + c2.G * dx * (1 - dy) + c3.G * (1 - dx) * dy + c4.G * dx * dy;
            float b = c1.B * (1 - dx) * (1 - dy) + c2.B * dx * (1 - dy) + c3.B * (1 - dx) * dy + c4.B * dx * dy;

            return Color.FromArgb((int)r, (int)g, (int)b);
        }

        // Update progress value
        private void UpdateProgress(int progress)
        {
            lock (progressLock)
            {
                totalProgress = progress;
            }
        }

        // Cancel Compression
        private void btnCancelLnr_Click(object sender, EventArgs e)
        {
            cancellationTokenSource?.Cancel();
        }

        private void btnCancelPrl_Click(object sender, EventArgs e)
        {
            cancellationTokenSource?.Cancel();
        }
    }
}
