using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CG_Project_II
{
    class Dithering
    {
        public Dithering() { }

        public unsafe void ToGrayScale(Bitmap bmp)
        {
            BitmapData bData = bmp.LockBits(
                            new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
            byte* scan0 = (byte*)bData.Scan0.ToPointer();
            byte bitsPerPixel = (byte)(Image.GetPixelFormatSize(bData.PixelFormat));
            int sum;
            for (int i = 0; i < bData.Height; i++)
            {
                for (int j = 0; j < bData.Width; j++)
                {
                    sum = 0;
                    byte* data = scan0 + i * bData.Stride + j * bitsPerPixel / 8;
                    sum = data[0] + data[1] + data[2];

                    data[0] = (byte)(sum/3);
                    data[1] = (byte)(sum/3);
                    data[2] = (byte)(sum/3);
                }
            }         
            bmp.UnlockBits(bData);
        }

        public unsafe void AverageColor(Bitmap bmp, int valuePerRed, int valuePerGreen, int valuePerBlue)
        {
            BitmapData bData = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
            byte* scan0 = (byte*)bData.Scan0.ToPointer();
            byte bitsPerPixel = (byte)(Image.GetPixelFormatSize(bData.PixelFormat));
            int i;
            int[,] red = new int[valuePerRed - 1,2];
            for (i = 0; i < (valuePerRed - 1); i++)
            {
                red[i, 0] = 0;
                red[i, 1] = 0;
            }
            int[,] green = new int[ (valuePerGreen - 1),2];
            for (i = 0; i < (valuePerGreen - 1); i++)
            {
                green[i, 0] = 0;
                green[i, 1] = 0;
            }           
            int[,] blue = new int[ (valuePerBlue - 1),2];
            for (i = 0; i < (valuePerBlue - 1); i++)
            {
                blue[i, 0] = 0;
                blue[i, 1] = 0;
            }             
            int redStep = 255 / (valuePerRed - 1) + 1;
            int greenStep = 255 / (valuePerGreen - 1) + 1;
            int blueStep = 255 / (valuePerBlue - 1) + 1;
            for (i = 0; i < bData.Height; i++)
            {
                for (int j = 0; j < bData.Width; j++)
                {
                    byte* data = scan0 + i * bData.Stride + j * bitsPerPixel / 8;
                    red[(int)data[0] / redStep, 0] += data[0];
                    red[(int)data[0] / redStep, 1] += 1;
                    green[(int)data[1] / greenStep,0] += data[1];
                    green[(int)data[1] / greenStep,1] += 1;
                    blue[(int)data[2] / blueStep,0 ] += data[2];
                    blue[(int)data[2] / blueStep,1] += 1;
                }
            }
            for (i = 0; i < (valuePerRed - 1); i++)
                red[i,0]/= red[i,1];
            for (i = 0; i < (valuePerGreen - 1); i++)
                green[i,0] /= green[i, 1];
            for (i = 0; i < (valuePerBlue - 1); i++)
                blue[i, 0] /= blue[i, 1];
            for (i = 0; i < bData.Height; i++)
            {
                for (int j = 0; j < bData.Width; j++)
                {
                    byte* data = scan0 + i * bData.Stride + j * bitsPerPixel / 8;

                    if (data[0] > red[(int)data[0] / redStep,0]) data[0] = (byte)(((int)data[0] / redStep + 1) * (redStep-1));
                    else data[0] = (byte)(((int)data[0] / redStep) * (redStep - 1));

                    if (data[1] > green[(int)data[1] / greenStep, 0]) data[1] = (byte)(((int)data[1] / greenStep + 1) * (greenStep-1));
                    else data[1] = (byte)(((int)data[1] / greenStep) * (greenStep - 1));

                    if (data[2] > blue[(int)data[2] / blueStep, 0]) data[2] = (byte)(((int)data[2] / blueStep + 1) * (blueStep-1));
                    else data[2] = (byte)(((int)data[2] / blueStep) * (blueStep - 1));
                }
            }
            bmp.UnlockBits(bData);
        }

        public unsafe void AverageBW(Bitmap bmp, int valuePerChannel)
        {
            BitmapData bData = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
            byte* scan0 = (byte*)bData.Scan0.ToPointer();
            byte bitsPerPixel = (byte)(Image.GetPixelFormatSize(bData.PixelFormat));
            int i;
            int[,] red = new int[valuePerChannel - 1, 2];
            int[,] green = new int[(valuePerChannel - 1), 2];
            int[,] blue = new int[(valuePerChannel - 1), 2];
            for (i = 0; i < (valuePerChannel - 1); i++)
            {
                red[i, 0] = 0;
                red[i, 1] = 0;
                green[i, 0] = 0;
                green[i, 1] = 0;
                blue[i, 0] = 0;
                blue[i, 1] = 0;
            }     
            int redStep = 255 / (valuePerChannel - 1) + 1;
            int greenStep = 255 / (valuePerChannel - 1) + 1;
            int blueStep = 255 / (valuePerChannel - 1) + 1;
            for (i = 0; i < bData.Height; i++)
            {
                for (int j = 0; j < bData.Width; j++)
                {
                    byte* data = scan0 + i * bData.Stride + j * bitsPerPixel / 8;
                    red[(int)data[0] / redStep, 0] += data[0];
                    red[(int)data[0] / redStep, 1] += 1;
                    green[(int)data[1] / greenStep, 0] += data[1];
                    green[(int)data[1] / greenStep, 1] += 1;
                    blue[(int)data[2] / blueStep, 0] += data[2];
                    blue[(int)data[2] / blueStep, 1] += 1;
                }
            }
            for (i = 0; i < (valuePerChannel - 1); i++)
            {
                red[i, 0] /= red[i, 1];
                green[i, 0] /= green[i, 1];
                blue[i, 0] /= blue[i, 1];
            }
            for (i = 0; i < bData.Height; i++)
            {
                for (int j = 0; j < bData.Width; j++)
                {
                    byte* data = scan0 + i * bData.Stride + j * bitsPerPixel / 8;

                    if (data[0] > red[(int)data[0] / redStep, 0]) data[0] = (byte)(((int)data[0] / redStep + 1) * (redStep - 1));
                    else data[0] = (byte)(((int)data[0] / redStep) * (redStep - 1));

                    if (data[1] > green[(int)data[1] / greenStep, 0]) data[1] = (byte)(((int)data[1] / greenStep + 1) * (greenStep - 1));
                    else data[1] = (byte)(((int)data[1] / greenStep) * (greenStep - 1));

                    if (data[2] > blue[(int)data[2] / blueStep, 0]) data[2] = (byte)(((int)data[2] / blueStep + 1) * (blueStep - 1));
                    else data[2] = (byte)(((int)data[2] / blueStep) * (blueStep - 1));
                }
            }
            bmp.UnlockBits(bData);
        }
    }
}
