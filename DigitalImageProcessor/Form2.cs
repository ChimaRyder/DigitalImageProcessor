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
    public partial class Form2 : Form
    {
        Bitmap chroma, background, result;
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Background
            openFileDialog2.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Chroma Image
            openFileDialog1.ShowDialog(this);
        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            background = new Bitmap(openFileDialog2.FileName);
            pictureBox2.Image = background;
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            chroma = new Bitmap(openFileDialog1.FileName);
            pictureBox1.Image = chroma;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            //Subtract
            result = new Bitmap(chroma.Width, chroma.Height);
            Color green = Color.FromArgb(0, 0, 255);
            int gray_green = (green.R + green.G + green.B) / 3;
            int threshold = 5;

            for (int i = 0; i < chroma.Width; i++)
            {
                for (int j = 0; j < chroma.Height; j++)
                {
                    Color pixel = chroma.GetPixel(i, j);
                    Color bg_pixel = background.GetPixel(i, j);

                    int gray = (pixel.R + pixel.G + pixel.B) / 3;
                    int subtractive = Math.Abs(gray - gray_green);

                    if (subtractive < threshold)
                    {
                        result.SetPixel(i, j, bg_pixel);
                    } else
                    {
                        result.SetPixel(i, j, pixel);
                    }
                }
            }

            pictureBox3.Image = result;
        }
    }
}
