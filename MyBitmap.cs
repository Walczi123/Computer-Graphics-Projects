using System;
using System.Collections.Generic;
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
        public static bool AntiAliasing
        {
            get;
            set;
        }
        public static Color FirstColor
        {
            get;
            set;
        }
        public static Color SecondColor
        {
            get;
            set;
        }

        internal static void DrawPoint(int x, int y, int brushSize)
        {
            if (brushSize > 0)
                Algorithms.Brush(x, y, brushSize, FirstColor);
            else
                MyBitmap.DrawPixel(x, y, FirstColor);
        }
        internal static void DrawLine(int x1, int y1, int x2, int y2, int brushSize)
        {
            if (AntiAliasing)
            {
                Algorithms.WuLine(x1, y1, x2, y2, FirstColor, SecondColor);
            }
            else
            {
                Algorithms.lineDDA(x1, y1, x2, y2, FirstColor, brushSize);
            }
        }
        internal static void DrawCircle(int origin_x, int origin_y, int R)
        {
            if (AntiAliasing)
            {
                Algorithms.WuCircle(origin_x, origin_y, R, FirstColor, SecondColor);
            }
            else
            {
                Algorithms.MidpointCircle(origin_x, origin_y, R, FirstColor);
            }
        }

        internal static void DrawPoint(int x, int y, int brushSize, Color col)
        {
            if (brushSize > 0)
                Algorithms.Brush(x, y, brushSize, col);
            else
                MyBitmap.DrawPixel(x, y, col);
        }
        internal static void DrawLine(int x1, int y1, int x2, int y2, int brushSize, Color col1, Color col2)
        {
            if (AntiAliasing)
            {
                Algorithms.WuLine(x1, y1, x2, y2, col1, col2);
            }
            else
            {
                Algorithms.lineDDA(x1, y1, x2, y2, col1, brushSize);
            }
        }
        internal static void DrawCircle(int origin_x, int origin_y, int R, Color col1, Color col2)
        {
            if (AntiAliasing)
            {
                Algorithms.WuCircle(origin_x, origin_y, R, col1, col2);
            }
            else
            {
                Algorithms.MidpointCircle(origin_x, origin_y, R, col1);
            }
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

        internal static void DrawPixel(int x, int y, System.Windows.Media.Color color)
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

        internal static void Redraw(List<IShape> shapes)
        {
            MyBitmap.CleanDrawArea();
            foreach (var shape in shapes)
                shape.Draw();
        }
    }
}
