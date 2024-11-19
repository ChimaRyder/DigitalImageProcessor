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
using ImageProcess2;

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
            LoadDevices();
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
            BasicDIP.PhotoCopy(ref initial, ref processed);

            pictureBox2.Image = processed;
        }

        private void grayscaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BasicDIP.GreyScale(ref initial, ref processed);

            pictureBox2.Image = processed;
        }

        private void inversionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BasicDIP.Inversion(ref initial, ref processed);

            pictureBox2.Image = processed;
        }

        private void histogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BasicDIP.Histogram(ref initial, ref processed);

            pictureBox2.Image = processed;
        }

        private void sepiaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BasicDIP.Sepia(ref initial, ref processed);

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

        private void mirrorVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BasicDIP.MirrorVertical(ref initial, ref processed);

            pictureBox2.Image = processed;
        }

        private void mirrorHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BasicDIP.MirrorHorizontal(ref initial, ref processed);

            pictureBox2.Image = processed;
        }

        private void brightnessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (trackBar1.Visible == true)
            {
                trackBar1.Visible = false;
            } else
            {
                trackBar1.Visible = true;
                trackBar2.Visible = false;
                trackBar3.Visible = false;
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            int x = trackBar1.Value;

            BasicDIP.Brightness(ref initial, ref processed, x);

            pictureBox2.Image = processed;
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            int x = trackBar2.Value;

            BasicDIP.Contrast(ref initial, ref processed, x);

            pictureBox2.Image = processed;

        }

        private void contrastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (trackBar2.Visible == true)
            {
                trackBar2.Visible = false;
            } else
            {
                trackBar2.Visible = true;
                trackBar1.Visible = false;
                trackBar3.Visible= false;
            }
 
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            int x = trackBar3.Value;

            BasicDIP.Rotate(ref initial, ref processed, x);

            pictureBox2.Image = processed;

        }

        private void rotateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (trackBar3.Visible == true)
            {
                trackBar3.Visible = false;
            } else
            {
                trackBar3.Visible = true;
                trackBar1.Visible = false;
                trackBar2.Visible = false;
            }
 

        }

        public enum Filter
        {
            GrayScale,
            Inversion,
            Histogram,
            Sepia
        }

        private bool VideoOn = false;
        private bool vFilterOn = false;
        private Filter currentFilter;
        private FilterInfoCollection devices; 
        private VideoCaptureDevice source;
        private readonly object imageLock = new object();

        private void LoadDevices()
        {
            devices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (devices.Count == 0)
            {
                MessageBox.Show("Camera not detected.");
                return;
            }

            turnOnVideoToolStripMenuItem.DropDown.Items.Clear();

            foreach (FilterInfo d in devices)
            {
                ToolStripMenuItem t = new ToolStripMenuItem(d.Name);
                t.Tag = d.MonikerString;
                t.Click += StartDeviceVideo;
                turnOnVideoToolStripMenuItem.DropDownItems.Add(t);
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

        private void FramesForVideo(object sender, NewFrameEventArgs e)
        {
            lock (imageLock)
            {
                initial?.Dispose();
                initial = (Bitmap) e.Frame.Clone();
            }

            pictureBox1.Image?.Dispose();
            pictureBox1.Image = (Bitmap) initial.Clone();
        }

        private void StopVideo()
        {
            if(source != null && source.IsRunning)
            {
                source.SignalToStop();
                source.WaitForStop();
                source = null;
                VideoOn = false;

                StopvFilter();
            }
        }

        private void turnOffVideoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StopVideo();
        }

        private void video_timer_Tick(object sender, EventArgs e)
        {
            ProcessFrame();
        }

        private void StartvFilter(Filter filter)
        {
            if (VideoOn == true)
            {
                currentFilter = filter;
                vFilterOn = true;
                video_timer.Enabled = true;
            }
        }
        private void StopvFilter()
        {
            if (vFilterOn == true)
            {
                video_timer.Stop();
                vFilterOn = false;
            }
        }

        private void ProcessFrame()
        {
            if (initial == null)
            {
                return;
            }

            Bitmap clone;
            lock (imageLock)
            {
                clone = (Bitmap) initial.Clone();
            }

            switch(currentFilter)
            {
                case Filter.GrayScale:
                    BitmapFilter.GrayScale(clone);
                    break;
                case Filter.Inversion:
                    BitmapFilter.Invert(clone);
                    break;
                case Filter.Histogram:
                    BasicDIP.Histogram(ref clone, ref clone);
                    break;
                case Filter.Sepia:
                    BitmapFilter.Sepia(clone);
                    break;
            }

            processed?.Dispose();
            processed = (Bitmap) clone.Clone();

            pictureBox2.Image?.Dispose();
            pictureBox2.Image = processed;

            clone.Dispose();
        }

        private void video_grayScaleToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            StopvFilter();
            StartvFilter(Filter.GrayScale);
        }

        private void video_inversionToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            StopvFilter();
            StartvFilter(Filter.Inversion);
        }

        private void video_histogramToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            StopvFilter();
            StartvFilter(Filter.Histogram);
        }

        private void video_sepiaToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            StopvFilter();
            StartvFilter(Filter.Sepia);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopVideo();
            somthn.Dispose();
        }

        private void gaussianBlurToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (initial == null)
            {
                return;
            }

            processed = new Bitmap(initial);
            BitmapFilter.GaussianBlur(processed, 4);
            pictureBox2.Image = processed;
        }

        private void sharpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (initial == null)
            {
                return;
            }

            processed = new Bitmap(initial);
            BitmapFilter.Sharpen(processed, 11);
            pictureBox2.Image = processed;

        }

        private void smoothenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (initial == null)
            {
                return;
            }

            processed = new Bitmap(initial);
            BitmapFilter.Smooth(processed, 4);
            pictureBox2.Image = processed;

        }

        private void embossToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (initial == null)
            {
                return;
            }

            processed = new Bitmap(initial);
            BitmapFilter.EmbossLaplacian(processed);
            pictureBox2.Image = processed;

        }

        private void meanRemovalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (initial == null)
            {
                return;
            }

            processed = new Bitmap(initial);
            BitmapFilter.MeanRemoval(processed, 9);
            pictureBox2.Image = processed;

        }
    }
}
