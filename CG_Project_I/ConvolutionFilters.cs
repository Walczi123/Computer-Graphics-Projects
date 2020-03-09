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
        public ConvolutionFilters() { }

        public byte Clamp(int value)
        {
            int result = value;
            if (value.CompareTo(255) > 0)
                result = 255;
            if (value.CompareTo(0) < 0)
                result = 0;
            return (byte)result;
        }

        public unsafe void ConvolutionFunction(Bitmap bmp, int[,] kernel)
        {        
            Bitmap clone = new Bitmap(bmp.Width, bmp.Height);
            using (Graphics graphics = Graphics.FromImage(clone))
                graphics.DrawImage(bmp, new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                    new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
            BitmapData cloneData = clone.LockBits(
                new Rectangle(0, 0, clone.Width, clone.Height), ImageLockMode.ReadWrite, clone.PixelFormat);
            byte* clone0 = (byte*)cloneData.Scan0.ToPointer();
            BitmapData bData = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
            byte* scan0 = (byte*)bData.Scan0.ToPointer();
            byte bitsPerPixel = (byte)(Image.GetPixelFormatSize(bData.PixelFormat));
            int kernelLen = kernel.GetLength(0);
            int kernelStep = kernelLen / 2;
            int red, green, blue, pixelCounter;        
            for (int i = 0; i < bData.Height; i++)
            {
                for (int j = 0; j < bData.Width; j++)
                {
                    byte* data = scan0 + i * bData.Stride + j * bitsPerPixel / 8;
                    red = 0;
                    green = 0;
                    blue = 0;
                    pixelCounter = 0;
                    for (int x = 0; x < kernelLen; x++)
                    {
                        for (int y = 0; y < kernelLen; y++)
                        { 
                            if(i - kernelStep + x >= 0 && i - kernelStep + x < bData.Height &&
                                j - kernelStep + y >= 0 && j - kernelStep + y < bData.Width)
                            {
                                byte* tmp = clone0 + (i - kernelStep + x) * cloneData.Stride + (j - kernelStep + y) * bitsPerPixel / 8;
                                red += tmp[0] * kernel[x,y];
                                green += tmp[1] * kernel[x, y];
                                blue += tmp[2] * kernel[x, y];
                                pixelCounter+=kernel[x, y];
                            }
                        }
                    }
                    if (pixelCounter == 0) pixelCounter = 1;
                    data[0] = (byte)(Clamp(red /pixelCounter));
                    data[1] = (byte)(Clamp(green / pixelCounter));
                    data[2] = (byte)(Clamp(blue / pixelCounter));
                }
            }
            bmp.UnlockBits(bData);
        }

        public unsafe void ConvolutionFunction(Bitmap bmp, int[,] kernel, int denominator)
        {
            if (denominator == 0) return;
            Bitmap clone = new Bitmap(bmp.Width, bmp.Height);
            using (Graphics graphics = Graphics.FromImage(clone))
                graphics.DrawImage(bmp, new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                    new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
            BitmapData cloneData = clone.LockBits(
                new Rectangle(0, 0, clone.Width, clone.Height), ImageLockMode.ReadWrite, clone.PixelFormat);
            byte* clone0 = (byte*)cloneData.Scan0.ToPointer();
            BitmapData bData = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
            byte* scan0 = (byte*)bData.Scan0.ToPointer();
            byte bitsPerPixel = (byte)(Image.GetPixelFormatSize(bData.PixelFormat));
            int kernelLen = kernel.GetLength(0);
            int kernelStep = kernelLen / 2;
            int red, green, blue;
            for (int i = 0; i < bData.Height; i++)
            {
                for (int j = 0; j < bData.Width; j++)
                {
                    byte* data = scan0 + i * bData.Stride + j * bitsPerPixel / 8;
                    red = 0;
                    green = 0;
                    blue = 0;
                    for (int x = 0; x < kernelLen; x++)
                    {
                        for (int y = 0; y < kernelLen; y++)
                        {
                            if (i - kernelStep + x >= 0 && i - kernelStep + x < bData.Height &&
                                j - kernelStep + y >= 0 && j - kernelStep + y < bData.Width)
                            {
                                byte* tmp = clone0 + (i - kernelStep + x) * cloneData.Stride + (j - kernelStep + y) * bitsPerPixel / 8;
                                red += tmp[0] * kernel[x, y];
                                green += tmp[1] * kernel[x, y];
                                blue += tmp[2] * kernel[x, y];
                            }
                        }
                    }
                    data[0] = (byte)(Clamp(red / denominator));
                    data[1] = (byte)(Clamp(green / denominator));
                    data[2] = (byte)(Clamp(blue / denominator));
                }
            }
            bmp.UnlockBits(bData);
        }

        public unsafe void ConvolutionFunction(Bitmap bmp, int[,] kernel, int denominator, int offset, int anchorX, int anchorY)
        {
            if (denominator == 0) return;
            Bitmap clone = new Bitmap(bmp.Width, bmp.Height);
            using (Graphics graphics = Graphics.FromImage(clone))
                graphics.DrawImage(bmp, new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                    new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
            BitmapData cloneData = clone.LockBits(
                new Rectangle(0, 0, clone.Width, clone.Height), ImageLockMode.ReadWrite, clone.PixelFormat);
            byte* clone0 = (byte*)cloneData.Scan0.ToPointer();
            BitmapData bData = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
            byte* scan0 = (byte*)bData.Scan0.ToPointer();
            byte bitsPerPixel = (byte)(Image.GetPixelFormatSize(bData.PixelFormat));
            int kernelLenX = kernel.GetLength(0);
            int kernelLenY = kernel.GetLength(1);
            int kernelStepX = anchorX;
            int kernelStepY = anchorX;
            int red, green, blue;
            for (int i = 0; i < bData.Height; i++)
            {
                for (int j = 0; j < bData.Width; j++)
                {
                    byte* data = scan0 + i * bData.Stride + j * bitsPerPixel / 8;
                    red = 0;
                    green = 0;
                    blue = 0;
                    for (int x = 0; x < kernelLenX; x++)
                    {
                        for (int y = 0; y < kernelLenY; y++)
                        {
                            if (i - kernelStepX + x >= 0 && i - kernelStepX + x < bData.Height &&
                                j - kernelStepY + y >= 0 && j - kernelStepY + y < bData.Width)
                            {
                                byte* tmp = clone0 + (i - kernelStepX + x) * cloneData.Stride + (j - kernelStepY + y) * bitsPerPixel / 8;
                                red += tmp[0] * kernel[x, y];
                                green += tmp[1] * kernel[x, y];
                                blue += tmp[2] * kernel[x, y];
                            }
                        }
                    }
                    data[0] = (byte)(Clamp(red / denominator + offset));
                    data[1] = (byte)(Clamp(green / denominator + offset));
                    data[2] = (byte)(Clamp(blue / denominator + offset));
                }
            }
            bmp.UnlockBits(bData);
        }
    }
}
