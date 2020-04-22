using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CG_Project_III
{
    public static class MyBitmap
    {
        public static WriteableBitmap Bitmap
        {
            get;
            set;
        }

        internal static void DrawPixel(int x, int y, Color color)
        {
            if (x < 0 || y < 0 || x >= Bitmap.PixelWidth || y >= Bitmap.PixelHeight)
                return;
            try
            {
                Bitmap.Lock();
                unsafe
                {
                    IntPtr pBackBuffer = Bitmap.BackBuffer + y * Bitmap.BackBufferStride + x * 4;

                    int color_data = 0;
                    color_data |= color.A << 24;    // A
                    color_data |= color.R << 16;    // R
                    color_data |= color.G << 8;     // G
                    color_data |= color.B << 0;     // B

                    *((int*)pBackBuffer) = color_data;
                }
                Bitmap.AddDirtyRect(new Int32Rect(x, y, 1, 1));
            }
            finally
            {
                Bitmap.Unlock();
            }
        }

        internal static void CleanDrawArea()
        {
            try
            {
                Bitmap.Lock();
                var backBuffer = Bitmap.BackBuffer;
                unsafe
                {
                    for (int y = 0; y < Bitmap.PixelHeight; y++)
                    {
                        for (int x = 0; x < Bitmap.PixelWidth; x++)
                        {
                            var bufPtr = backBuffer + Bitmap.BackBufferStride * y + x * 4;

                            int color_data = 0;
                            color_data |= 255 << 24;    // A
                            color_data |= 255 << 16;    // R
                            color_data |= 255 << 8;     // G
                            color_data |= 255 << 0;     // B

                            *((int*)bufPtr) = color_data;
                        }
                    }
                }
                Bitmap.AddDirtyRect(new Int32Rect(0, 0, Bitmap.PixelWidth, Bitmap.PixelHeight));
            }
            finally
            {
                Bitmap.Unlock();
            }
        }    
    }
}
