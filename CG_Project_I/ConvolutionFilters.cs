using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CG_Project_I
{
    class ConvolutionFilters
    {
        private double[,] kernel = { {  1, 1, 1, },
                                    {  1, 1, 1, },
                                    {  1, 1, 1, } };

        public ConvolutionFilters() { }

        //public Bitmap blur(Bitmap img)
        //{
        //    Bitmap copy = (Bitmap) img.Clone();
        //    int xx, yy;
        //    int R, G, B;
        //    int PixelCount;
        //    Color tmp;
        //    for (int y = 0; y < img.Height; y++)
        //    {
        //        for (int x = 0; x < img.Width; x++)
        //        {
        //            R = 0; G = 0; B = 0;
        //            PixelCount = 0;
        //            for ( yy = -1; yy < 2 ; yy++)
        //            {
        //                for ( xx = -1; xx < 2; xx++)
        //                {
        //                    if(yy + y >= 0 && yy + y <= img.Height && xx + x >= 0 && xx + x <= img.Width)
        //                    {
        //                        tmp = copy.GetPixel(x + xx, y + yy);
        //                        PixelCount++;
        //                        R += tmp.R;
        //                        G += tmp.G;
        //                        B += tmp.B;
        //                    }
        //                }
        //            }
        //            tmp = Color.FromArgb(R / PixelCount, G / PixelCount, B / PixelCount);
        //            img.SetPixel(x, y, tmp);
        //        }
        //    }
        //    return img;
        //}

        public void blur(Bitmap image, int blurSize) => image = Blur(image, new Rectangle(0, 0, image.Width, image.Height), blurSize);

        private unsafe static Bitmap Blur(Bitmap image, Rectangle rectangle, Int32 blurSize)
        {
            Bitmap blurred = new Bitmap(image.Width, image.Height);

            // make an exact copy of the bitmap provided
            using (Graphics graphics = Graphics.FromImage(blurred))
                graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
                    new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);

            // Lock the bitmap's bits
            BitmapData blurredData = blurred.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, blurred.PixelFormat);

            // Get bits per pixel for current PixelFormat
            int bitsPerPixel = Image.GetPixelFormatSize(blurred.PixelFormat);

            // Get pointer to first line
            byte* scan0 = (byte*)blurredData.Scan0.ToPointer();

            // look at every pixel in the blur rectangle
            for (int xx = rectangle.X; xx < rectangle.X + rectangle.Width; xx++)
            {
                for (int yy = rectangle.Y; yy < rectangle.Y + rectangle.Height; yy++)
                {
                    int avgR = 0, avgG = 0, avgB = 0;
                    int blurPixelCount = 0;

                    // average the color of the red, green and blue for each pixel in the
                    // blur size while making sure you don't go outside the image bounds
                    for (int x = xx; (x < xx + blurSize && x < image.Width); x++)
                    {
                        for (int y = yy; (y < yy + blurSize && y < image.Height); y++)
                        {
                            // Get pointer to RGB
                            byte* data = scan0 + y * blurredData.Stride + x * bitsPerPixel / 8;

                            avgB += data[0]; // Blue
                            avgG += data[1]; // Green
                            avgR += data[2]; // Red

                            blurPixelCount++;
                        }
                    }

                    avgR = avgR / blurPixelCount;
                    avgG = avgG / blurPixelCount;
                    avgB = avgB / blurPixelCount;

                    // now that we know the average for the blur size, set each pixel to that color
                    for (int x = xx; x < xx + blurSize && x < image.Width && x < rectangle.Width; x++)
                    {
                        for (int y = yy; y < yy + blurSize && y < image.Height && y < rectangle.Height; y++)
                        {
                            // Get pointer to RGB
                            byte* data = scan0 + y * blurredData.Stride + x * bitsPerPixel / 8;

                            // Change values
                            data[0] = (byte)avgB;
                            data[1] = (byte)avgG;
                            data[2] = (byte)avgR;
                        }
                    }
                }
            }

            // Unlock the bits
            blurred.UnlockBits(blurredData);

            return blurred;
        }
    }
}
