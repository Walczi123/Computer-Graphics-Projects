using System;
using System.Windows.Media.Imaging;

namespace CG_Project_III
{
    public class Algorithms
    {
        public Algorithms() { }
        public void lineDDA(int x1, int y1, int x2, int y2, WriteableBitmap bitmap)
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
                MyBitmap.DrawPixel(bitmap, _x, _y);
            }
        }    
    }
}
