using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private bool cancelLinear = false;
        private bool cancelParallel = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.bmp;*.png;*.jpg;*.jpeg"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtFilePath.Text = openFileDialog.FileName;
            }
        }

        private Bitmap CreateCopy(Bitmap source)
        {
            return new Bitmap(source);
        }

        private async void btnDownscaleLnr_Click(object sender, EventArgs e)
        {
            cancelLinear = false;
            progressBarLnr.Value = 0;
            ToggleButtons(false, true);

            try
            {
                string filePath = txtFilePath.Text;
                float scaleFactor = float.Parse(txtFactor.Text) / 100f;
                string originalDirectory = Path.GetDirectoryName(filePath);
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string extension = Path.GetExtension(filePath);
                string outputFile = Path.Combine(originalDirectory, $"({100 - int.Parse(txtFactor.Text)} downsized) (linear) {fileName}{extension}");

                Bitmap originalImage = new Bitmap(filePath);

                await Task.Run(() => DownscaleLinear(originalImage, scaleFactor, outputFile));

                MessageBox.Show("Linear downscaling completed successfully.");
                OpenContainingFolder(outputFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during downscaling: {ex.Message}");
            }
            finally
            {
                progressBarLnr.Value = 0; // Нулиране на прогрес бара
                ToggleButtons(true, false); // Активира всички бутони
            }
        }

        private void DownscaleLinear(Bitmap original, float scaleFactor, string outputFile)
        {
            int newWidth = (int)(original.Width * scaleFactor);
            int newHeight = (int)(original.Height * scaleFactor);

            Bitmap downscaled = new Bitmap(newWidth, newHeight);

            for (int y = 0; y < newHeight; y++)
            {
                if (cancelLinear) return;

                for (int x = 0; x < newWidth; x++)
                {
                    int srcX = (int)(x / scaleFactor);
                    int srcY = (int)(y / scaleFactor);

                    Color pixelColor = original.GetPixel(srcX, srcY);
                    downscaled.SetPixel(x, y, pixelColor);
                }

                Invoke(new Action(() =>
                {
                    progressBarLnr.Value = (int)((y / (float)newHeight) * 100);
                }));
            }

            downscaled.Save(outputFile);
            downscaled.Dispose();
        }

        private async void btnDownscalePrl_Click(object sender, EventArgs e)
        {
            cancelParallel = false;
            progressBarPrl.Value = 0;
            ToggleButtons(false, true);

            try
            {
                string filePath = txtFilePath.Text;
                float scaleFactor = float.Parse(txtFactor.Text) / 100f;
                string originalDirectory = Path.GetDirectoryName(filePath);
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string extension = Path.GetExtension(filePath);
                string outputFile = Path.Combine(originalDirectory, $"({100 - int.Parse(txtFactor.Text)} downsized) (parallel) {fileName}{extension}");

                Bitmap originalImage = new Bitmap(filePath);

                await ParallelDownscaleAsync(originalImage, scaleFactor, outputFile);

                MessageBox.Show("Parallel downscaling completed successfully.");
                OpenContainingFolder(outputFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during downscaling: {ex.Message}");
            }
            finally
            {
                progressBarPrl.Value = 0;
                ToggleButtons(true, false);
            }
        }

        private async Task ParallelDownscaleAsync(Bitmap original, float scaleFactor, string outputFile)
        {
            int processorCount = Environment.ProcessorCount;
            int partHeight = original.Height / processorCount;

            Bitmap downscaled = new Bitmap((int)(original.Width * scaleFactor), (int)(original.Height * scaleFactor));

            var tasks = new List<Task>();

            for (int i = 0; i < processorCount; i++)
            {
                int startRow = i * partHeight;
                int endRow = (i == processorCount - 1) ? original.Height : startRow + partHeight;

                Bitmap partCopy = CreateCopy(original);

                tasks.Add(Task.Run(() =>
                {
                    DownscalePart(partCopy, downscaled, startRow, endRow, scaleFactor);
                }));
            }

            await Task.WhenAll(tasks);

            downscaled.Save(outputFile);
            downscaled.Dispose();
        }

        private void DownscalePart(Bitmap original, Bitmap downscaled, int startRow, int endRow, float scaleFactor)
        {
            for (int y = startRow; y < endRow; y++)
            {
                if (cancelParallel) return;

                for (int x = 0; x < original.Width; x++)
                {
                    int newX = (int)(x * scaleFactor);
                    int newY = (int)(y * scaleFactor);

                    Color pixelColor = original.GetPixel(x, y);

                    lock (downscaled)
                    {
                        downscaled.SetPixel(newX, newY, pixelColor);
                    }
                }

                Invoke(new Action(() =>
                {
                    progressBarPrl.Value = (int)((y - startRow) / (float)(endRow - startRow) * 100);
                }));
            }
        }

        private void btnCancelLnr_Click(object sender, EventArgs e)
        {
            cancelLinear = true;
        }

        private void btnCancelPrl_Click(object sender, EventArgs e)
        {
            cancelParallel = true;
        }

        private void OpenContainingFolder(string filePath)
        {
            string argument = $"/select, \"{filePath}\"";
            Process.Start("explorer.exe", argument);
        }
        
        private void ToggleButtons(bool enable, bool cancelOnly)
        {
            btnOpenFile.Enabled = enable;
            btnDownscaleLnr.Enabled = enable;
            btnDownscalePrl.Enabled = enable;
            txtFilePath.Enabled = enable;
            txtFactor.Enabled = enable;

            btnCancelLnr.Enabled = !enable && !cancelOnly;
            btnCancelPrl.Enabled = !enable && cancelOnly;
        }
    }
}