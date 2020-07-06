using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;


namespace CG_Project_II
{
    class FunctionalFilters
    {
        public FunctionalFilters() { }

        public byte Clamp(int value)
        {
            int result = value;
            if (value.CompareTo(255) > 0)
                result = 255;
            if (value.CompareTo(0) < 0)
                result = 0;
            return (byte)result;
        }

        public void inversion(Bitmap bmp)
        {
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData =
                bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                bmp.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] rgbValues = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
            for (int counter = 0; counter < rgbValues.Length; counter += 1) {
                rgbValues[counter] = (byte)(255 - rgbValues[counter]);
            }
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
            bmp.UnlockBits(bmpData);
        }

        public void brightness(Bitmap bmp, int step)
        {
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData =
                bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                bmp.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] rgbValues = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes); 
            for (int counter = 0; counter < rgbValues.Length; counter += 1)
            {
                rgbValues[counter] = Clamp(rgbValues[counter] + step);
            }
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
            bmp.UnlockBits(bmpData);
        }

        public void contrast(Bitmap bmp, double step)
        { 
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData =
                bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                bmp.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] rgbValues = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
            for (int counter = 0; counter < rgbValues.Length; counter += 1)
            {
                rgbValues[counter] = Clamp((int)((rgbValues[counter] - 127.5) * step + 127.5));
            }
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
            bmp.UnlockBits(bmpData);
        }

        public void gamma(Bitmap bmp, double step)
        {
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData =
                bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                bmp.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] rgbValues = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
            for (int counter = 0; counter < rgbValues.Length; counter += 1)
            {
                rgbValues[counter] = Clamp((int)Math.Pow(rgbValues[counter], step));
            }
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
            bmp.UnlockBits(bmpData);
        }

        #region ToSlow

        //public Bitmap inversion(Bitmap img) {
        //    for (int y = 0; y < img.Height ; y++)
        //    {
        //        for (int x = 0; x < img.Width ; x++)
        //        {
        //            Color tmp = img.GetPixel(x, y);
        //            tmp = Color.FromArgb((255 - tmp.R), (255 - tmp.G), (255 - tmp.B));
        //            img.SetPixel(x, y, tmp);
        //        }
        //    }
        //    return img;
        //}

        //public Bitmap brightness(Bitmap img, int step)
        //{
        //    for (int y = 0; y < img.Height; y++)
        //    {
        //        for (int x = 0; x < img.Width; x++)
        //        {
        //            Color tmp = img.GetPixel(x, y);
        //            tmp = Color.FromArgb(Clamp(tmp.R + step), Clamp(tmp.G + step), Clamp(tmp.B + step));
        //            img.SetPixel(x, y, tmp);
        //        }
        //    }
        //    return img;
        //}

        //public Bitmap contrast(Bitmap img, double step)
        //{
        //    for (int y = 0; y < img.Height; y++)
        //    {
        //        for (int x = 0; x < img.Width; x++)
        //        {
        //            var tmp = img.GetPixel(x, y);
        //            int red = (int)((tmp.R - 127.5) * step);
        //            int green = (int)((tmp.G - 127.5) * step);
        //            int blue = (int)((tmp.B - 127.5) * step);

        //            tmp = Color.FromArgb(Clamp(red), Clamp(green), Clamp(blue));
        //            img.SetPixel(x, y, tmp);
        //        }
        //    }
        //    return img;
        //}

        //public Bitmap gamma(Bitmap img, double step)
        //{
        //    for (int y = 0; y < img.Height; y++)
        //    {
        //        for (int x = 0; x < img.Width; x++)
        //        {
        //            var tmp = img.GetPixel(x, y);
        //            int red = (int)Math.Pow(tmp.R,step);
        //            int green = (int)Math.Pow(tmp.G, step);
        //            int blue = (int)Math.Pow(tmp.B, step);
        //            var newColor = Color.FromArgb(Clamp(red), Clamp(green), Clamp(blue));
        //            img.SetPixel(x, y, newColor);
        //        }
        //    }
        //    return img;
        //}

        #endregion
    }
}
