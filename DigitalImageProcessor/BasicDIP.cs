using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Xsl;

namespace DigitalImageProcessor
{
    internal class BasicDIP
    {
        public static void PhotoCopy(ref Bitmap original, ref Bitmap processed)
        {
            processed = new Bitmap(original.Width, original.Height);
            Color pixel;

            for (int i = 0; i < original.Width; i++)
            {
                for (int j = 0; j < original.Height; j++)
                {
                    pixel = original.GetPixel(i, j);

                    processed.SetPixel(i, j, pixel);
                }
            }
        }
        public static void GreyScale(ref Bitmap original, ref Bitmap processed)
        {
            processed = new Bitmap(original.Width, original.Height);
            Color pixel;
            int average;

            for (int i = 0; i < original.Width; i++)
            {
                for (int j = 0; j < original.Height; j++)
                {
                    pixel = original.GetPixel(i, j);
                    average = (int) (pixel.R + pixel.G + pixel.B) / 3;
                    
                    Color gray = Color.FromArgb(average, average, average);
                    processed.SetPixel(i, j, gray);
                }
            }
        }

        public static void Inversion(ref Bitmap original, ref Bitmap processed)
        {
            processed = new Bitmap(original.Width, original.Height);
            Color pixel;

            for (int i = 0; i < original.Width; i++)
            {
                for (int j = 0; j < original.Height; j++)
                {
                    pixel = original.GetPixel(i, j);

                    Color inverted = Color.FromArgb(255 - pixel.R, 255 - pixel.G, 255 - pixel.B);
                    processed.SetPixel(i, j, inverted);
                }
            }
        }

        public static void Histogram(ref Bitmap original, ref Bitmap processed)
        {
            Color sample;
            Color gray;
            Byte graydata;

            for (int i = 0; i < original.Width; i++)
            {
                for (int j = 0; j < original.Height; j++)
                {
                    sample = original.GetPixel(i, j);
                    graydata = (byte)((sample.R + sample.G + sample.B) / 3);
                    gray = Color.FromArgb(graydata, graydata, graydata);
                    original.SetPixel(i, j, gray);
                }
            }


            int[] histdata = new int[256];
            for (int i = 0; i < original.Width; i++)
            {
                for (int j = 0; j < original.Height; j++)
                {
                    sample = original.GetPixel(i, j);
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
        }

        public static void Sepia(ref Bitmap original, ref Bitmap processed)
        {
            processed = new Bitmap(original.Width, original.Height);
            Color pixel, sp;
            int sr, sb, sg;
            int r, g, b;

            for (int i = 0; i < original.Width; i++)
            {
                for (int j = 0; j < original.Height; j++)
                {
                    pixel = original.GetPixel(i, j);
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
        } 

        public static void MirrorVertical(ref Bitmap original, ref Bitmap processed)
        {
            processed = new Bitmap(original.Width, original.Height);

            for (int i = 0; i < original.Width; i++) {
                for (int j = 0; j < original.Height; j++)
                {
                    Color pixel = original.GetPixel(i, j);

                    processed.SetPixel(i, original.Height - j - 1, pixel);
                }
            }
        }
        public static void MirrorHorizontal(ref Bitmap original, ref Bitmap processed)
        {
            processed = new Bitmap(original.Width, original.Height);

            for (int i = 0; i < original.Width; i++) {
                for (int j = 0; j < original.Height; j++)
                {
                    Color pixel = original.GetPixel(i, j);

                    processed.SetPixel(original.Width - i - 1, j, pixel);
                }
            }
        }

        public static void Brightness(ref Bitmap original, ref Bitmap processed, int value)
        {
            processed = new Bitmap(original.Width, original.Height);

            for (int i = 0; i < original.Width; i++)
            {
                for (int j = 0; j < original.Height; j++)
                {
                    Color pixel = original.GetPixel(i, j);
                    Color new_pixel;

                    if (value > 0)
                    {
                        new_pixel = Color.FromArgb(Math.Min(pixel.R + value, 255), Math.Min(pixel.G + value, 255), Math.Min(pixel.B + value, 255));
                    }
                    else
                    {
                        new_pixel = Color.FromArgb(Math.Max(pixel.R + value, 0), Math.Max(pixel.G + value, 0), Math.Max(pixel.B + value, 0));
                    }

                    processed.SetPixel(i, j, new_pixel);
                }
            }
        }

        public static void Contrast(ref Bitmap original, ref Bitmap processed, int degree)
        {
            int[] histmap = new int[256];
            int[] ymap = new int[256];

            Bitmap histImage = new Bitmap(original.Width, original.Height);
            GreyScale(ref original, ref histImage);

            for (int i = 0; i < original.Width; i++)
            {
                for (int j = 0; j < original.Height; j++)
                {
                    Color grey = histImage.GetPixel(i, j);
                    histmap[grey.R]++;
                }
            }

            int samples = original.Width * original.Height;
            int sum = 0;
            for (int i = 0; i < 256; i++) 
            {
                sum += histmap[i];
                ymap[i] = sum * 255 / samples;
            }

            if (degree < 100)
            {
                for (int i = 0; i < 256; i++)
                {
                    sum += histmap[i];
                    ymap[i] = i + ((int) ymap[i] - i) * degree / 100;
                }
            }

            processed = new Bitmap(original.Width, original.Height);
            for (int i = 0; i < original.Width; i++)
            {
                for (int j = 0; j < original.Height; j++)
                {
                    Color pros = Color.FromArgb(ymap[histImage.GetPixel(i, j).R], ymap[histImage.GetPixel(i, j).G], ymap[histImage.GetPixel(i, j).B]);
                    processed.SetPixel(i, j, pros);
                }
            }
        }

        public static void Rotate(ref Bitmap original, ref Bitmap processed, int degree)
        {
            float angle = degree * (float)Math.PI / 180;
            int x_center = (int) (original.Width / 2);
            int y_center = (int) (original.Height / 2);

            float cosA = (float)Math.Cos(angle);
            float sinA = (float)Math.Sin(angle);

            processed = new Bitmap(original.Width, original.Height);

            for (int i = 0; i < original.Width; i++)
            {
                for(int j = 0; j < original.Height; j++)
                {
                    int x0 = i - x_center;
                    int y0 = j - y_center;

                    int x = (int)(x0 * cosA + y0 * sinA);
                    int y = (int)(-x0 * sinA + y0 * cosA);

                    x = (int)(x + x_center);
                    y = (int)(y + y_center);

                    x = Math.Max(0, Math.Min(original.Width - 1, x));
                    y = Math.Max(0, Math.Min(original.Height - 1, y));

                    processed.SetPixel(i, j, original.GetPixel(x, y));
                }
            }
        }

        public static void Subtract(ref Bitmap chroma, ref Bitmap background, ref Bitmap result)
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
        }
    }
}
