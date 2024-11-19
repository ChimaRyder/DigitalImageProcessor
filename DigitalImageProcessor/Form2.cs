using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;

namespace DigitalImageProcessor
{
    public partial class Form2 : Form
    {
        Bitmap chroma, background, result;
        private bool VideoOn = false;
        private FilterInfoCollection devices;
        private VideoCaptureDevice source;
        private readonly object imageLock = new object();

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

        private void Form2_Load(object sender, EventArgs e)
        {
            LoadDevices();
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopVideo();
            this.Owner.Dispose();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            chroma = new Bitmap(openFileDialog1.FileName);
            pictureBox1.Image = chroma;
            VideoOn = false;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            ////Subtract
            //result = new Bitmap(chroma.Width, chroma.Height);
            //Color green = Color.FromArgb(0, 0, 255);
            //int gray_green = (green.R + green.G + green.B) / 3;
            //int threshold = 5;

            //for (int i = 0; i < chroma.Width; i++)
            //{
            //    for (int j = 0; j < chroma.Height; j++)
            //    {
            //        Color pixel = chroma.GetPixel(i, j);
            //        Color bg_pixel = background.GetPixel(i, j);

            //        int gray = (pixel.R + pixel.G + pixel.B) / 3;
            //        int subtractive = Math.Abs(gray - gray_green);

            //        if (subtractive < threshold)
            //        {
            //            result.SetPixel(i, j, bg_pixel);
            //        } else
            //        {
            //            result.SetPixel(i, j, pixel);
            //        }
            //    }
            //}

            if (chroma == null && background == null)
            {
                return;
            }

            if (VideoOn)
            {
                timerToSubtract.Enabled = true;
            } else
            {
                BasicDIP.Subtract(ref chroma, ref background, ref result);
                pictureBox3.Image?.Dispose();
                pictureBox3.Image = result;
            }
        }

        private void LoadDevices()
        {
            devices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (devices.Count == 0)
            {
                MessageBox.Show("Camera not detected.");
                return;
            }

            enableVideoToolStripMenuItem.DropDown.Items.Clear();

            foreach (FilterInfo d in devices)
            {
                ToolStripMenuItem t = new ToolStripMenuItem(d.Name);
                t.Tag = d.MonikerString;
                t.Click += StartDeviceVideo;
                enableVideoToolStripMenuItem.DropDownItems.Add(t);
            }
        }

        private void StartDeviceVideo (object sender, EventArgs e)
        {
            StopVideo();

            ToolStripMenuItem item = sender as ToolStripMenuItem;
            string monikerString = item?.Tag.ToString();

            if (monikerString != null)
            {
                source = new VideoCaptureDevice(monikerString);
                source.NewFrame += FramesForVideo;
                source.Start();
                VideoOn = true;
            }
        }

        private void timerToSubtract_Tick(object sender, EventArgs e)
        {
            Bitmap vid;

            lock(imageLock)
            {
                vid = (Bitmap)chroma.Clone();
            }
            
            result?.Dispose();
            result = new Bitmap(vid.Width, vid.Height);

            BasicDIP.Subtract(ref vid, ref background, ref result);

            pictureBox3.Image?.Dispose();
            pictureBox3.Image = result;

            vid.Dispose();
        }

        private void FramesForVideo(object sender, NewFrameEventArgs e)
        {
            lock (imageLock)
            {
                chroma?.Dispose();
                chroma = (Bitmap) e.Frame.Clone();
            }

            pictureBox1.Image?.Dispose();
            pictureBox1.Image = (Bitmap) chroma.Clone();
        }

        private void StopVideo()
        {
            if(source != null && source.IsRunning)
            {
                source.SignalToStop();
                source.WaitForStop();
                source = null;
                VideoOn = false;

            }
        }
    }
}
