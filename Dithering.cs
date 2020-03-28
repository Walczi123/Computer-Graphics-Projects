using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CG_Project_II
{
    class Dithering
    {
        public Dithering() { }

        private int changeColor(int inputColor, int valuePerChannel)
        {
            int step = 255 / valuePerChannel;
            int result = 255;
            for (int i=1; i< valuePerChannel; i++)
            {
                if (inputColor < step * i )
                {
                    result = step * (i-1);
                    break;
                }    
            }
            return result;
        }
        public void Average(Bitmap bmp, int valuePerChannel)
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
                rgbValues[counter] = (byte)(changeColor(rgbValues[counter],valuePerChannel));
            }
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
            bmp.UnlockBits(bmpData);

        }
    }
}
