using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CG_Project_II
{
    public class ConvertionYCbCr
    {
        public ConvertionYCbCr() { }

        private double Lerp(int a, int b, double t)
        {
            return (1 - t) * a + t * b;
        }
        public unsafe void Y(Bitmap bmp)
        {
            BitmapData bData = bmp.LockBits(
                            new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
            byte* scan0 = (byte*)bData.Scan0.ToPointer();
            byte bitsPerPixel = (byte)(Image.GetPixelFormatSize(bData.PixelFormat));
            int y;
            for (int i = 0; i < bData.Height; i++)
            {
                for (int j = 0; j < bData.Width; j++)
                {
                    byte* data = scan0 + i * bData.Stride + j * bitsPerPixel / 8;
                    y =(int)(0.299 * data[2] + 0.587 * data[1] + 0.114 * data[1]);
                    data[0] = (byte)y;
                    data[1] = (byte)y;
                    data[2] = (byte)y;
                }
            }
            bmp.UnlockBits(bData);
        }

        public unsafe void Cb(Bitmap bmp)
        {
            BitmapData bData = bmp.LockBits(
                            new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
            byte* scan0 = (byte*)bData.Scan0.ToPointer();
            byte bitsPerPixel = (byte)(Image.GetPixelFormatSize(bData.PixelFormat));
            double cb;
            for (int i = 0; i < bData.Height; i++)
            {
                for (int j = 0; j < bData.Width; j++)
                {
                    byte* data = scan0 + i * bData.Stride + j * bitsPerPixel / 8;
                    cb = 0.168736 * data[2] + 0.331264 * data[1] + 0.5 * data[0];
                    cb /= 255;
                    data[2] = 127;
                    data[1] = (byte)Lerp(255,0,cb);
                    data[0] = (byte)Lerp(0, 255, cb);
                }
            }
            bmp.UnlockBits(bData);
        }

        public unsafe void Cr(Bitmap bmp)
        {
            BitmapData bData = bmp.LockBits(
                            new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
            byte* scan0 = (byte*)bData.Scan0.ToPointer();
            byte bitsPerPixel = (byte)(Image.GetPixelFormatSize(bData.PixelFormat));
            double cr;
            for (int i = 0; i < bData.Height; i++)
            {
                for (int j = 0; j < bData.Width; j++)
                {
                    byte* data = scan0 + i * bData.Stride + j * bitsPerPixel / 8;
                    cr = 0.5 * data[2] + 0.418688 * data[1] + 0.081312 * data[0];
                    cr /= 255;
                    data[2] = (byte)Lerp(0, 255, cr);
                    data[1] = (byte)Lerp(255, 0, cr);
                    data[0] = 127;
                }
            }
            bmp.UnlockBits(bData);
        }
    }
}
