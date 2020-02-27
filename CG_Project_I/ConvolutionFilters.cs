using System;
using System.Collections.Generic;
using System.Drawing;
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

        public Bitmap blur(Bitmap img)
        {
            Bitmap copy = (Bitmap) img.Clone();
            int xx, yy;
            int R, G, B;
            int PixelCount;
            Color tmp;
            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    R = 0; G = 0; B = 0;
                    PixelCount = 0;
                    for ( yy = -1; yy < 2 ; yy++)
                    {
                        for ( xx = -1; xx < 2; xx++)
                        {
                            if(yy + y >= 0 && yy + y <= img.Height && xx + x >= 0 && xx + x <= img.Width)
                            {
                                tmp = copy.GetPixel(x + xx, y + yy);
                                PixelCount++;
                                R += tmp.R;
                                G += tmp.G;
                                B += tmp.B;
                            }
                        }
                    }
                    tmp = Color.FromArgb(R / PixelCount, G / PixelCount, B / PixelCount);
                    img.SetPixel(x, y, tmp);
                }
            }
            return img;
        }

        ////https://gist.github.com/superic/8165746
        //public void blur(Bitmap image, int blurSize)
        //{
        //    // look at every pixel in the blur rectangle
        //    for (int xx = rectangle.X; xx < rectangle.X + rectangle.Width; xx++)
        //    {
        //        for (int yy = rectangle.Y; yy < rectangle.Y + rectangle.Height; yy++)
        //        {
        //            int avgR = 0, avgG = 0, avgB = 0;
        //            int blurPixelCount = 0;

        //            // average the color of the red, green and blue for each pixel in the
        //            // blur size while making sure you don't go outside the image bounds
        //            for (int x = xx; (x < xx + blurSize && x < image.Width); x++)
        //            {
        //                for (int y = yy; (y < yy + blurSize && y < image.Height); y++)
        //                {
        //                    Color pixel = image.GetPixel(x, y); ;

        //                    avgR += pixel.R;
        //                    avgG += pixel.G;
        //                    avgB += pixel.B;

        //                    blurPixelCount++;
        //                }
        //            }

        //            avgR = avgR / blurPixelCount;
        //            avgG = avgG / blurPixelCount;
        //            avgB = avgB / blurPixelCount;

        //            // now that we know the average for the blur size, set each pixel to that color
        //            for (int x = xx; x < xx + blurSize && x < image.Width && x < rectangle.Width; x++)
        //                for (int y = yy; y < yy + blurSize && y < image.Height && y < rectangle.Height; y++)
        //                    blurred.SetPixel(x, y, Color.FromArgb(avgR, avgG, avgB));
        //        }
        //    }

        //    image =  blurred;
        //}

    }
}
