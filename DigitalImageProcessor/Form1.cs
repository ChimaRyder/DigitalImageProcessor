using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DigitalImageProcessor
{
    public partial class Form1 : Form
    {
        Form2 somthn;
        public Form1()
        {
            InitializeComponent();
            somthn = new Form2();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog(this);
        }

        Bitmap initial;
        Bitmap processed;

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            initial = new Bitmap(openFileDialog1.FileName);
            pictureBox1.Image = initial;
        }

        private void pixelCopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(initial.Width, initial.Height);
            Color pixel;

            for (int i = 0; i < initial.Width; i++)
            {
                for (int j = 0; j < initial.Height; j++)
                {
                    pixel = initial.GetPixel(i, j);

                    processed.SetPixel(i, j, pixel);
                }
            }

            pictureBox2.Image = processed;
        }

        private void grayscaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(initial.Width, initial.Height);
            Color pixel;
            int average;

            for (int i = 0; i < initial.Width; i++)
            {
                for (int j = 0; j < initial.Height; j++)
                {
                    pixel = initial.GetPixel(i, j);
                    average = (int) (pixel.R + pixel.G + pixel.B) / 3;
                    
                    Color gray = Color.FromArgb(average, average, average);
                    processed.SetPixel(i, j, gray);
                }
            }

            pictureBox2.Image = processed;
        }

        private void inversionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(initial.Width, initial.Height);
            Color pixel;

            for (int i = 0; i < initial.Width; i++)
            {
                for (int j = 0; j < initial.Height; j++)
                {
                    pixel = initial.GetPixel(i, j);

                    Color inverted = Color.FromArgb(255 - pixel.R, 255 - pixel.G, 255 - pixel.B);
                    processed.SetPixel(i, j, inverted);
                }
            }

            pictureBox2.Image = processed;
        }

        private void histogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Color sample;
            Color gray;
            Byte graydata;

            for (int i = 0; i < initial.Width; i++)
            {
                for (int j = 0; j < initial.Height; j++)
                {
                    sample = initial.GetPixel(i, j);
                    graydata = (byte)((sample.R + sample.G + sample.B) / 3);
                    gray = Color.FromArgb(graydata, graydata, graydata);
                    initial.SetPixel(i, j, gray);
                }
            }


            int[] histdata = new int[256];
            for (int i = 0; i < initial.Width; i++)
            {
                for (int j = 0; j < initial.Height; j++)
                {
                    sample = initial.GetPixel(i, j);
                    histdata[sample.R]++;
                }
            }


            processed = new Bitmap(256, 800);
            for (int x = 0; x < 256; x++)
            {
                for (int y = 0; y < 800; y++)
                {
                    processed.SetPixel(x, y, Color.White);
                }
            }

            for (int x = 0; x < 256; x++)
            {
                for (int y = 0; y < Math.Min(histdata[x] / 5, processed.Height - 1); y++)
                {
                    processed.SetPixel(x, (processed.Height - 1) - y, Color.Black);
                }
            }

            pictureBox2.Image = processed;
        }

        private void sepiaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(initial.Width, initial.Height);
            Color pixel, sp;
            int sr, sb, sg;
            int r, g, b;

            for (int i = 0; i < initial.Width; i++)
            {
                for (int j = 0; j < initial.Height; j++)
                {
                    pixel = initial.GetPixel(i, j);
                    sr = (int)(pixel.R * 0.393 + pixel.G * 0.769 + pixel.B * 0.189);
                    sg = (int)(pixel.R * 0.349 + pixel.G * 0.686 + pixel.B * 0.168);
                    sb = (int)(pixel.R * 0.272 + pixel.G * 0.534 + pixel.B * 0.131);

                    r = sr > 255 ? 255 : sr;
                    g = sg > 255 ? 255 : sg;
                    b = sb > 255 ? 255 : sb;

                    sp = Color.FromArgb(r, g, b);
                    processed.SetPixel(i, j, sp);
                }
            }

            pictureBox2.Image = processed;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            processed.Save(saveFileDialog1.FileName);
        }

        private void openChromaKeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            somthn.Owner = this;
            somthn.Show();
            this.Hide();
        }
    }
}
