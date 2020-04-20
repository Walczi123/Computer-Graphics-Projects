using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace CG_Project_III
{
    public static class MyBitmap
    { 
        internal static void DrawPixel(WriteableBitmap bitmap, int x, int y)
        {
            try
            {
                bitmap.Lock();
                unsafe
                {
                    IntPtr pBackBuffer = bitmap.BackBuffer + y * bitmap.BackBufferStride + x * 4;

                    int color_data = 0 << 16; // R
                    color_data |= 128 << 8;   // G
                    color_data |= 0 << 0;   // B

                    *((int*)pBackBuffer) = color_data;
                }
                bitmap.AddDirtyRect(new Int32Rect(x, y, 1, 1));
            }
            finally
            {
                bitmap.Unlock();
            }
        }

        internal static void CleanDrawArea(WriteableBitmap bitmap)
        {
            try
            {
                bitmap.Lock();
                var backBuffer = bitmap.BackBuffer;
                unsafe
                {
                    for (int y = 0; y < bitmap.PixelHeight; y++)
                    {
                        for (int x = 0; x < bitmap.PixelWidth; x++)
                        {
                            var bufPtr = backBuffer + bitmap.BackBufferStride * y + x * 4;

                            int color_data = 255 << 16; // R
                            color_data |= 255 << 8;   // G
                            color_data |= 255 << 0;   // B

                            *((int*)bufPtr) = color_data;
                        }
                    }
                }
                bitmap.AddDirtyRect(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));
            }
            finally
            {
                bitmap.Unlock();
            }
        }

        internal static void CleanDrawArea(WriteableBitmap bitmap, int color)
        {
            try
            {
                bitmap.Lock();
                var backBuffer = bitmap.BackBuffer;
                unsafe
                {
                    for (int y = 0; y < bitmap.PixelHeight; y++)
                    {
                        for (int x = 0; x < bitmap.PixelWidth; x++)
                        {
                            var bufPtr = backBuffer + bitmap.BackBufferStride * y + x * 4;
                            *((int*)bufPtr) = color;
                        }
                    }
                }
                bitmap.AddDirtyRect(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));
            }
            finally
            {
                bitmap.Unlock();
            }
        }
    }
}
