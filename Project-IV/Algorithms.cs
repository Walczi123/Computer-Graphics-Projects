﻿using System;
using System.Collections.Generic;

namespace CG_Project_IV
{
    public static class Algorithms
    {
        #region Project III
        public static HashSet<(int, int)> lineDDA(int x1, int y1, int x2, int y2, Color color, int brushSize)
        {
            HashSet<(int, int)> result = new HashSet<(int, int)>();
            int steps, k, _x, _y;
            float mx, my, x, y;

            int dx = x2 - x1;
            int dy = y2 - y1;


            if (Math.Abs(dx) > Math.Abs(dy)) steps = Math.Abs(dx);
            else steps = Math.Abs(dy);

            mx = dx / (float)steps;
            my = dy / (float)steps;

            x = x1;
            y = y1;
            if( brushSize > 0)
            {
                for (k = 0; k < steps; k++)
                {
                    x += mx;
                    _x = (int)x;
                    y += my;
                    _y = (int)y;
                    var l = Algorithms.Brush(_x, _y, brushSize, color);
                    if (l != null)
                        result.UnionWith(l);
                }
            }
            else
            {
                for (k = 0; k < steps; k++)
                {
                    x += mx;
                    _x = (int)x;
                    y += my;
                    _y = (int)y;
                    MyBitmap.DrawPixel(_x, _y, color);
                    result.Add((_x, _y));
                }
            }
            return result;
        }
        public static HashSet<(int, int)> MidpointCircle(int origin_x, int origin_y,int R, Color color)
        {
            HashSet<(int, int)> result = new HashSet<(int, int)>();
            int dE = 3;
            int dSE = 5 - 2 * R;
            int d = 1 - R;
            int x = 0;
            int y = R;        
            MyBitmap.DrawPixel(x + origin_x, y + origin_y, color);
            MyBitmap.DrawPixel(origin_x + R, origin_y, color);
            MyBitmap.DrawPixel(origin_x - R, origin_y, color);
            while (y > x)
            {
                if (d < 0) 
                {
                    d += dE;
                    dE += 2;
                    dSE += 2;
                }
                else 
                {
                    d += dSE;
                    dE += 2;
                    dSE += 4;
                    --y;
                }
                ++x;
                MyBitmap.DrawPixel(origin_x + x, origin_y + y, color);
                MyBitmap.DrawPixel(origin_x + x, origin_y - y, color);
                MyBitmap.DrawPixel(origin_x - x, origin_y + y, color);
                MyBitmap.DrawPixel(origin_x - x, origin_y - y, color);
                MyBitmap.DrawPixel(origin_x + y, origin_y + x, color);
                MyBitmap.DrawPixel(origin_x + y, origin_y - x, color);
                MyBitmap.DrawPixel(origin_x - y, origin_y + x, color);
                MyBitmap.DrawPixel(origin_x - y, origin_y - x, color);
                result.Add((origin_x + x, origin_y + y));
                result.Add((origin_x + x, origin_y - y));
                result.Add((origin_x + x, origin_y + y));
                result.Add((origin_x - x, origin_y - y));
                result.Add((origin_x + y, origin_y + x));
                result.Add((origin_x + y, origin_y - x));
                result.Add((origin_x - y, origin_y + x));
                result.Add((origin_x - y, origin_y - x));
            }
            return result;
        }
        public static HashSet<(int, int)> Brush(int x, int y, int size, Color color)
        {
            if (size == 0)
            {
                MyBitmap.DrawPixel(x, y, color);
                return null;
            }
            HashSet<(int, int)> result = new HashSet<(int, int)>();
            for (int i = 0; i < size; i++)
            {
                for(int j = 0; j<size; j++)
                {
                    if (Math.Sqrt(Math.Pow(i - size / 2, 2) + Math.Pow(j - size / 2, 2)) <= size/2)
                    {
                        MyBitmap.DrawPixel(x - size / 2 + i, y - size / 2 + j, color);
                        result.Add((x - size / 2 + i, y - size / 2 + j));
                    }
                }
            }
            return result;
        }
        public static HashSet<(int, int)> WuLine(int x1, int y1, int x2, int y2, Color col1, Color col2)
        {
            HashSet<(int, int)> result = new HashSet<(int, int)>();
            var L = System.Windows.Media.Color.FromArgb((byte)col1.A, (byte)col1.R, (byte)col1.G, (byte)col1.B);
            var B= System.Windows.Media.Color.FromArgb((byte)col2.A, (byte)col2.R, (byte)col2.G, (byte)col2.B);
            int steps, k, _x, _y, tmp;
            float mx, my, x, y;

            int dx = x2 - x1;
            int dy = y2 - y1;


            if (Math.Abs(dx) > Math.Abs(dy)) steps = Math.Abs(dx);
            else steps = Math.Abs(dy);

            mx = dx / (float)steps;
            my = dy / (float)steps;

            if (mx > my) tmp = 1;
            else tmp = -1;

            x = x1;
            y = y1;
            for (k = 0; k < steps; k++)
            {
                x += mx;
                _x = (int)x;
                y += my;
                _y = (int)y;
                var c1 = L * (1 - (int)Math.Truncate(y)) + B * (int)Math.Truncate(y);
                var c2 = L * (int)Math.Truncate(y) + B * (1 - (int)Math.Truncate(y));
                MyBitmap.DrawPixel(_x,_y, c1);
                result.Add((_x, _y));
                MyBitmap.DrawPixel(_x, _y + tmp, c2);
                result.Add((_x, _y + tmp));

            }
            return result;
        }
        public static HashSet<(int, int)> WuCircle(int origin_x, int origin_y, int R, Color col1, Color col2)
        {
            HashSet<(int, int)> result = new HashSet<(int, int)>();
            var L = System.Windows.Media.Color.FromArgb((byte)col1.A, (byte)col1.R, (byte)col1.G, (byte)col1.B);
            var B = System.Windows.Media.Color.FromArgb((byte)col2.A, (byte)col2.R, (byte)col2.G, (byte)col2.B);
            int x = R;
            int y = 0;
            MyBitmap.DrawPixel(origin_x + x, origin_y + y, L);
            MyBitmap.DrawPixel(origin_x + R, origin_y, L);
            MyBitmap.DrawPixel(origin_x - R, origin_y, L);
            result.Add((origin_x + x, origin_y + y));
            result.Add((origin_x + R, origin_y));
            result.Add((origin_x - R, origin_y));
            while (x > y)
            {
                ++y;
                x = (int)Math.Ceiling(Math.Sqrt(R * R - y * y));
                float T = (float)(Math.Sqrt(R * R - y * y) - x);
                var c2 = L * (1 - T) + B * T;
                var c1 = L * T + B * (1 - T);
                MyBitmap.DrawPixel(origin_x + x, origin_y + y, c2);
                MyBitmap.DrawPixel(origin_x + x, origin_y - y, c2);
                MyBitmap.DrawPixel(origin_x - x, origin_y + y, c2);
                MyBitmap.DrawPixel(origin_x - x, origin_y - y, c2);
                MyBitmap.DrawPixel(origin_x + y, origin_y + x, c2);
                MyBitmap.DrawPixel(origin_x + y, origin_y - x, c2);
                MyBitmap.DrawPixel(origin_x - y, origin_y + x, c2);
                MyBitmap.DrawPixel(origin_x - y, origin_y - x, c2);
                result.Add((origin_x + x, origin_y + y));
                result.Add((origin_x + x, origin_y - y));
                result.Add((origin_x + x, origin_y + y));
                result.Add((origin_x - x, origin_y - y));
                result.Add((origin_x + y, origin_y + x));
                result.Add((origin_x + y, origin_y - x));
                result.Add((origin_x - y, origin_y + x));
                result.Add((origin_x - y, origin_y - x));

                MyBitmap.DrawPixel(origin_x + x - 1, origin_y + y, c1);
                MyBitmap.DrawPixel(origin_x + x - 1, origin_y - y, c1);
                MyBitmap.DrawPixel(origin_x - x + 1, origin_y + y, c1);
                MyBitmap.DrawPixel(origin_x - x + 1, origin_y - y, c1);
                MyBitmap.DrawPixel(origin_x + y, origin_y + x - 1, c1);
                MyBitmap.DrawPixel(origin_x + y, origin_y - x + 1, c1);
                MyBitmap.DrawPixel(origin_x - y, origin_y + x - 1, c1);
                MyBitmap.DrawPixel(origin_x - y, origin_y - x + 1, c1);
                result.Add((origin_x + x - 1, origin_y + y));
                result.Add((origin_x + x - 1, origin_y - y));
                result.Add((origin_x - x + 1, origin_y + y));
                result.Add((origin_x - x + 1, origin_y - y));
                result.Add((origin_x + y, origin_y + x - 1));
                result.Add((origin_x + y, origin_y - x + 1));
                result.Add((origin_x - y, origin_y + x - 1));
                result.Add((origin_x - y, origin_y - x + 1));
            }
            return result;
        }
        public static HashSet<(int, int)> Capsule(int x1, int y1, int x2, int y2 , int x3, int y3, Color col1)
        {
            HashSet<(int, int)> result = new HashSet<(int, int)>();
            var d1 = MyBitmap.PointDistance(x1,y1,x3,y3);
            var d2 = MyBitmap.PointDistance(x2,y2,x3,y3);
            if (d2 > d1)
            {
                int tmp = x1;
                x1 = x2;
                x2 = tmp;
                tmp = y1;
                y1 = y2;
                y2 = tmp;
            }
            var radius = MyBitmap.PointDistance(x2, y2, x3, y3);
            var V = ((double)x2 - x1, (double)y2 - y1);
            V = (V.Item2, -V.Item1);
            var length = MyBitmap.PointDistance(x1, y1, x2, y2);
            V = (V.Item1 / length, V.Item2 / length);
            V = (V.Item1 * radius, V.Item2 * radius);
            result.UnionWith(MyBitmap.DrawLine(x1+(int)V.Item1, y1 + (int)V.Item2, x2 + (int)V.Item1, y2 + (int)V.Item2,0));
            result.UnionWith(MyBitmap.DrawLine(x1 - (int)V.Item1, y1 - (int)V.Item2, x2 - (int)V.Item1, y2 - (int)V.Item2,0));
            result.UnionWith(CapsuleCircle(x1,y1, x1 + (int)V.Item1, y1 + (int)V.Item2,col1));
            result.UnionWith(CapsuleCircle(x2, y2, x2 - (int)V.Item1, y2 - (int)V.Item2,col1));
            return result;
        }
        public static HashSet<(int, int)> CapsuleCircle(int origin_x, int origin_y, int Ex, int Ey, Color color)
        {
            HashSet<(int, int)> result = new HashSet<(int, int)>();
            int R = (int)MyBitmap.PointDistance(origin_x, origin_y, Ex, Ey);
            int dE = 3;
            int dSE = 5 - 2 * R;
            int d = 1 - R;
            int x = 0;
            int y = R;
            if(Math.Sign((Ex - origin_x) * (y + origin_y - origin_y) - (Ey - origin_y) * (x + origin_x - origin_x))<0)
                MyBitmap.DrawPixel(x + origin_x, y + origin_y, color);
            if (Math.Sign((Ex - origin_x) * (origin_y - origin_y) - (Ey - origin_y) * (origin_x + R - origin_x)) < 0)
                MyBitmap.DrawPixel(origin_x + R, origin_y, color);
            if (Math.Sign((Ex - origin_x) * (origin_y - origin_y) - (Ey - origin_y) * (origin_x - R - origin_x)) < 0)
                MyBitmap.DrawPixel(origin_x - R, origin_y, color);
            while (y > x)
            {
                if (d < 0)
                {
                    d += dE;
                    dE += 2;
                    dSE += 2;
                }
                else
                {
                    d += dSE;
                    dE += 2;
                    dSE += 4;
                    --y;
                }
                ++x;
                if (Math.Sign((Ex - origin_x) * (origin_y + y - origin_y) - (Ey - origin_y) * (origin_x + x - origin_x)) < 0)
                {
                    MyBitmap.DrawPixel(origin_x + x, origin_y + y, color);
                    result.Add((origin_x + x, origin_y + y));
                }                  
                if (Math.Sign((Ex - origin_x) * (origin_y - y - origin_y) - (Ey - origin_y) * (origin_x + x - origin_x)) < 0)
                {
                    MyBitmap.DrawPixel(origin_x + x, origin_y - y, color);
                    result.Add((origin_x + x, origin_y - y));
                }
                    
                if (Math.Sign((Ex - origin_x) * (origin_y + y - origin_y) - (Ey - origin_y) * (origin_x - x - origin_x)) < 0)
                {
                    MyBitmap.DrawPixel(origin_x - x, origin_y + y, color);
                    result.Add((origin_x + x, origin_y + y));
                }
                    
                if (Math.Sign((Ex - origin_x) * (origin_y - y - origin_y) - (Ey - origin_y) * (origin_x - x - origin_x)) < 0)
                {
                    MyBitmap.DrawPixel(origin_x - x, origin_y - y, color);
                    result.Add((origin_x - x, origin_y - y));
                }
                    
                if (Math.Sign((Ex - origin_x) * (origin_y + x - origin_y) - (Ey - origin_y) * (origin_x + y - origin_x)) < 0)
                {
                    MyBitmap.DrawPixel(origin_x + y, origin_y + x, color);
                    result.Add((origin_x + y, origin_y + x));
                }
                   
                if (Math.Sign((Ex - origin_x) * (origin_y - x - origin_y) - (Ey - origin_y) * (origin_x + y - origin_x)) < 0)
                {
                    MyBitmap.DrawPixel(origin_x + y, origin_y - x, color);
                    result.Add((origin_x + y, origin_y - x));
                }
                    
                if (Math.Sign((Ex - origin_x) * (origin_y + x - origin_y) - (Ey - origin_y) * (origin_x - y - origin_x)) < 0)
                {
                    MyBitmap.DrawPixel(origin_x - y, origin_y + x, color);
                    result.Add((origin_x - y, origin_y + x));
                }
                    
                if (Math.Sign((Ex - origin_x) * (origin_y - x - origin_y) - (Ey - origin_y) * (origin_x - y - origin_x)) < 0)
                {
                    MyBitmap.DrawPixel(origin_x - y, origin_y - x, color);
                    result.Add((origin_x - y, origin_y - x));
                }                   
                
            }
            return result;
        }
        #endregion    
    }
}
