using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace CG_Project_IV
{
    public static class MyBitmap
    {
        public static List<IShape> Shapes = new List<IShape>() { };
        public static Rectangle ClippingShape
        {
            get;
            set;
        }            
        public static IShape ClippedShape
        {
            get;
            set;
        }
        public static bool clipping = false;       
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
        public static Color ClippingColor
        {
            get;
            set;
        }

        internal static HashSet<(int, int)> DrawPoint(int x, int y, int brushSize)
        {
            HashSet<(int, int)> result;
            Bitmap.Lock();
            if (brushSize > 0)
                 result = Algorithms.Brush(x, y, brushSize, FirstColor);
            else
            {
                result = new HashSet<(int, int)>() { (x, y) };
                MyBitmap.DrawPixel(x, y, FirstColor);
            }              
            Bitmap.Unlock();
            return result;
        }
        internal static HashSet<(int, int)> DrawPoint(int x, int y, int brushSize, Color col)
        {
            HashSet<(int, int)> result;
            Bitmap.Lock();
            if (brushSize > 0)
                result = Algorithms.Brush(x, y, brushSize, col);
            else
            {
                result = new HashSet<(int, int)>() { (x, y) };
                MyBitmap.DrawPixel(x, y, col);
            }             
            Bitmap.Unlock();
            return result;
        }
        internal static HashSet<(int, int)> DrawLine(int x1, int y1, int x2, int y2, int brushSize)
        {
            HashSet<(int, int)> result;
            Bitmap.Lock();
            if (AntiAliasing)
            {
                result = Algorithms.WuLine(x1, y1, x2, y2, FirstColor, SecondColor);
            }
            else
            {
                result = Algorithms.lineDDA(x1, y1, x2, y2, FirstColor, brushSize);
            }
            Bitmap.Unlock();
            return result;
        }        
        internal static HashSet<(int, int)> DrawLine(int x1, int y1, int x2, int y2, int brushSize, Color color)
        {
            HashSet<(int, int)> result;
            Bitmap.Lock();
            if (AntiAliasing)
            {
                result = Algorithms.WuLine(x1, y1, x2, y2, color, SecondColor);
            }
            else
            {
                result = Algorithms.lineDDA(x1, y1, x2, y2, color, brushSize);
            }
            Bitmap.Unlock();
            return result;
        }


        internal static HashSet<(int, int)> DrawCircle(int origin_x, int origin_y, int R)
        {
            HashSet<(int, int)> result;
            Bitmap.Lock();
            if (AntiAliasing)
            {
                result = Algorithms.WuCircle(origin_x, origin_y, R, FirstColor, SecondColor);
            }
            else
            {
                result = Algorithms.MidpointCircle(origin_x, origin_y, R, FirstColor);
            }
            Bitmap.Unlock();
            return result;
        }

        internal static HashSet<(int, int)> DrawLine(int x1, int y1, int x2, int y2, int brushSize, Color col1, Color col2)
        {
            HashSet<(int, int)> result;
            Bitmap.Lock();
            if (AntiAliasing)
            {
                result = Algorithms.WuLine(x1, y1, x2, y2, col1, col2);
            }
            else
            {
                result = Algorithms.lineDDA(x1, y1, x2, y2, col1, brushSize);
            }
            Bitmap.Unlock();
            return result;
        }
        internal static HashSet<(int, int)> DrawCircle(int origin_x, int origin_y, int R, Color col1, Color col2)
        {
            HashSet<(int, int)> result;
            Bitmap.Lock();
            if (AntiAliasing)
            {
                result = Algorithms.WuCircle(origin_x, origin_y, R, col1, col2);
            }
            else
            {
                result = Algorithms.MidpointCircle(origin_x, origin_y, R, col1);
            }
            Bitmap.Unlock();
            return result;
        }

        internal static HashSet<(int, int)> DrawCapsule(int x1, int y1, int x2, int y2, int x3, int y3, Color col1)
        {
            Bitmap.Lock();
            var result = Algorithms.Capsule(x1, y1, x2, y2, x3, y3, col1);
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

        internal static void DrawPixel(int x, int y, System.Windows.Media.Color color)
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
            foreach (var shape in Shapes)
                shape.Draw();
            if ( clipping && ClippedShape != null && ClippingShape != null)
            {
                if (ClippedShape is Line)
                {
                    var line = ClippedShape as Line;
                    ClippingAlgorithm.CohenSutherland(new Point(line.X1, line.Y1), new Point(line.X2, line.Y2), ClippingShape, line.GetBrushSize(), MyBitmap.ClippingColor);
                }
                else if (ClippedShape is Polygon)
                {
                    var polygon = ClippedShape as Polygon;
                    var vertices = polygon.Vertices;
                    int i = 0;
                    for (; i < vertices.Count() - 1; i++)
                        ClippingAlgorithm.CohenSutherland(vertices[i], vertices[i + 1], ClippingShape, polygon.GetBrushSize(), MyBitmap.ClippingColor);
                    ClippingAlgorithm.CohenSutherland(vertices[0], vertices[i], ClippingShape, polygon.GetBrushSize(), MyBitmap.ClippingColor);
                }
                else if (ClippedShape is Rectangle)
                {
                    var rectangle = ClippedShape as Rectangle;
                    var vertices = rectangle.Vertices;
                    int i = 0;
                    for (; i < vertices.Count() - 1; i++)
                        ClippingAlgorithm.CohenSutherland(vertices[i], vertices[i + 1], ClippingShape, rectangle.GetBrushSize(), MyBitmap.ClippingColor);
                    ClippingAlgorithm.CohenSutherland(vertices[0], vertices[i], ClippingShape, rectangle.GetBrushSize(), MyBitmap.ClippingColor);
                }
            }
        }

        internal static IShape FindShape(List<IShape> shapes, int x, int y)
        {
            int searchArea = 5, distance = searchArea;
            IShape result = null;
            foreach (var shape in shapes)
            {
                if (shape.Pixels != null)
                {
                    foreach(var pixel in shape.Pixels)
                    {
                        for (int i = 0; i < searchArea; i++)
                        {
                            for (int j = 0; j < searchArea; j++)
                            {
                                int d = (int)Math.Sqrt(Math.Pow( pixel.Item1 - x , 2) + Math.Pow(pixel.Item2 - y, 2));
                                if ( d < distance)
                                {
                                    distance = d;
                                    result = shape;
                                    if ( d == 0)
                                    {
                                        return result;
                                    }
                                }
                            }
                        }
                    }
                }
            }            
            return result;
        }
        internal static double PointDistance(int x1, int y1, int x2, int y2)
        {
            return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1-y2, 2));
        }
    }
}
