using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CG_Project_III
{
    public class Algorithms
    {
        public Algorithms() { }
        public void lineDDA(int x1, int y1, int x2, int y2, Color color, int brushSize)
        {
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
                    this.Brush(_x, _y, brushSize, color);
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
                }
            }
        }

        public void MidpointCircle(int origin_x, int origin_y,int R, Color color)
        {
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
            }
        }

        public void Brush(int x, int y, int size, Color color)
        {
            if(size == 0)
            {
                MyBitmap.DrawPixel(x, y, color);
                return;
            }
            for(int i = 0; i < size; i++)
            {
                for(int j = 0; j<size; j++)
                {
                    if (Math.Sqrt(Math.Pow(i - size / 2, 2) + Math.Pow(j - size / 2, 2)) <= size/2)
                    {
                        MyBitmap.DrawPixel(x - size / 2 + i, y - size / 2 + j, color);
                    }
                }
            }
        }

        public void WuLine(int x1, int y1, int x2, int y2, Color L, Color B)
        {
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

            for (k = 0; k < steps; k++)
            {
                x += mx;
                _x = (int)x;
                y += my;
                _y = (int)y;
                Color c1 = L * (1 - (int)Math.Truncate(y)) + B * (int)Math.Truncate(y);
                Color c2 = L * (int)Math.Truncate(y) + B * (1 - (int)Math.Truncate(y));
                MyBitmap.DrawPixel(_x,_y, c1);
                MyBitmap.DrawPixel(_x, _y + 1, c2);
            }
        }


        public void WuCircle(int origin_x, int origin_y, int R, Color L, Color B)
        {
            int x = R;
            int y = 0;
            MyBitmap.DrawPixel(origin_x + x, origin_y + y, L);
            MyBitmap.DrawPixel(origin_x + R, origin_y, L);
            MyBitmap.DrawPixel(origin_x - R, origin_y, L);
            while (x > y)
            {
                ++y;
                x = (int)Math.Ceiling(Math.Sqrt(R * R - y * y));
                float T = (float)(Math.Sqrt(R * R - y * y) - x);
                Color c2 = L * (1 - T) + B * T;
                Color c1 = L * T + B * (1 - T);
                MyBitmap.DrawPixel(origin_x + x, origin_y + y, c2);
                MyBitmap.DrawPixel(origin_x + x, origin_y - y, c2);
                MyBitmap.DrawPixel(origin_x - x, origin_y + y, c2);
                MyBitmap.DrawPixel(origin_x - x, origin_y - y, c2);
                MyBitmap.DrawPixel(origin_x + y, origin_y + x, c2);
                MyBitmap.DrawPixel(origin_x + y, origin_y - x, c2);
                MyBitmap.DrawPixel(origin_x - y, origin_y + x, c2);
                MyBitmap.DrawPixel(origin_x - y, origin_y - x, c2);

                MyBitmap.DrawPixel(origin_x + x - 1, origin_y + y, c1);
                MyBitmap.DrawPixel(origin_x + x - 1, origin_y - y, c1);
                MyBitmap.DrawPixel(origin_x - x + 1, origin_y + y, c1);
                MyBitmap.DrawPixel(origin_x - x + 1, origin_y - y, c1);
                MyBitmap.DrawPixel(origin_x + y, origin_y + x - 1, c1);
                MyBitmap.DrawPixel(origin_x + y, origin_y - x + 1, c1);
                MyBitmap.DrawPixel(origin_x - y, origin_y + x - 1, c1);
                MyBitmap.DrawPixel(origin_x - y, origin_y - x + 1, c1);
            }
        }
    }
}
