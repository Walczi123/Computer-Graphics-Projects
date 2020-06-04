using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CG_Project_V
{
    public static class MyBitmap
    {
        public static bool clipping = false;
        private static WriteableBitmap _bitmap;
        public static WriteableBitmap Bitmap
        {
            get => _bitmap;
            set
            {
                if (value != null)
                {
                    _bitmap = value;
                    ZBuffer = new double[(int)Bitmap.Width, (int)Bitmap.Height];
                    ResetZBuffer();
                }
            }
        }

        public static List<Cube> drawingCubes = new List<Cube>();
        public static double[,] ZBuffer;
        public static void ResetZBuffer()
        {
            for (int i = 0; i < Bitmap.Width; i++)
                for (int j = 0; j < Bitmap.Height; j++)
                    MyBitmap.ZBuffer[i, j] = -10;
        }
        internal static HashSet<(int, int)> DrawPoint(int x, int y, Color color)
        {
            HashSet<(int, int)> result;
            Bitmap.Lock();
            result = new HashSet<(int, int)>() { (x, y) };
            MyBitmap.DrawPixel(x, y, color);         
            Bitmap.Unlock();
            return result;
        }
        internal static HashSet<(int, int)> DrawPoint(int x, int y)
        {
            HashSet<(int, int)> result;
            Bitmap.Lock();
            result = new HashSet<(int, int)>() { (x, y) };
            MyBitmap.DrawPixel(x, y, Color.FromRgb(0, 0, 0));
            Bitmap.Unlock();
            return result;
        }
        internal static HashSet<(int, int)> DrawLine(int x1, int y1, int x2, int y2, Color color)
        {
            HashSet<(int, int)> result;
            Bitmap.Lock();
            result = Algorithms.lineDDA(x1, y1, x2, y2, color);
            Bitmap.Unlock();
            return result;
        }

        internal static HashSet<(int, int)> DrawLine(int x1, int y1, int x2, int y2)
        {
            HashSet<(int, int)> result;
            Bitmap.Lock();
            result = Algorithms.lineDDA(x1, y1, x2, y2, Color.FromRgb(0,0,0));
            Bitmap.Unlock();
            return result;
        }

        internal static void DrawPixel(int x, int y, Color color)
        {
            if (x < 0 || y < 0 || x >= Bitmap.PixelWidth || y >= Bitmap.PixelHeight)
                return;
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

        internal static void Redraw()
        {
            if (MyBitmap.Bitmap == null)
                return;
            MyBitmap.CleanDrawArea();
            MyBitmap.ResetZBuffer();
            foreach (var cube in drawingCubes)
                cube.Draw();
        }
    }
}
