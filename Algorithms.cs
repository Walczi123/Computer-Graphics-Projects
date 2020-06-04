using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace CG_Project_V
{
    public static class Algorithms
    {
        public static HashSet<(int, int)> lineDDA(int x1, int y1, int x2, int y2, Color color)
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

            for (k = 0; k < steps; k++)
            {
                x += mx;
                _x = (int)x;
                y += my;
                _y = (int)y;
                MyBitmap.DrawPixel(_x, _y, color);
                result.Add((_x, _y));
            }
            return result;
        }
        public static double PointDistance(int x1, int y1, int x2, int y2)
        {
            return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }

        public static (double, double, double, double) ComputePlaneEquation(Vertex A, Vertex B, Vertex C)
        {
            double a1 = B.X - A.X;
            double b1 = B.Y - A.Y;
            double c1 = B.Z - A.Z;
            double a2 = C.X - A.X;
            double b2 = C.Y - A.Y;
            double c2 = C.Z - A.Z;
            double a = b1 * c2 - b2 * c1;
            double b = a2 * c1 - a1 * c2;
            double c = a1 * b2 - b1 * a2;
            double d = (-a * A.X - b * A.Y - c * A.Z);
            return (a, b, c, d);
        }
    }
}
