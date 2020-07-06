using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CG_Project_IV
{
    public static class ClippingAlgorithm
    {
        #region Project IV
        enum Outcodes
        {
            LEFT = 1,
            RIGHT = 2,
            BOTTOM = 4,
            TOP = 8
        };
        private static byte ComputeOutcode(Point p, Rectangle clip)
        {
            byte outcode = 0;
            if (p.X > clip.Right()) outcode |= (byte)Outcodes.RIGHT;
            else if (p.X < clip.Left()) outcode |= (byte)Outcodes.LEFT;
            if (p.Y < clip.Top()) outcode |= (byte)Outcodes.TOP;
            else if (p.Y > clip.Bottom()) outcode |= (byte)Outcodes.BOTTOM;
            return outcode;
        }
        public static void CohenSutherland(Point p1, Point p2, Rectangle clip, int brushSize, Color color)
        {
            bool accept = false, done = false;
            byte outcode1 = ComputeOutcode(p1, clip);
            byte outcode2 = ComputeOutcode(p2, clip);
            do
            {
                if ((outcode1 | outcode2) == 0)
                { //trivially accepted
                    accept = true;
                    done = true;
                }
                else if ((outcode1 & outcode2) != 0)
                { //trivially rejected
                    accept = false;
                    done = true;
                }
                else
                {
                    byte outcodeOut = (outcode1 != 0) ? outcode1 : outcode2;
                    Point p = new Point();
                    if ((outcodeOut & (byte)Outcodes.TOP) != 0)
                    {
                        p.X = p1.X + (p2.X - p1.X) * (clip.Top() - p1.Y) / (p2.Y - p1.Y);
                        p.Y = clip.Top();
                    }
                    else if ((outcodeOut & (byte)Outcodes.BOTTOM) != 0)
                    {
                        p.X = p1.X + (p2.X - p1.X) * (clip.Bottom() - p1.Y) / (p2.Y - p1.Y);
                        p.Y = clip.Bottom();
                    }                    
                    else if ((outcodeOut & (byte)Outcodes.LEFT) != 0)
                    {
                        p.Y = p1.Y + (p2.Y - p1.Y) * (clip.Left() - p1.X) / (p2.X - p1.X);
                        p.X = clip.Left();
                    }                    
                    else if ((outcodeOut & (byte)Outcodes.RIGHT) != 0)
                    {
                        p.Y = p1.Y + (p2.Y - p1.Y) * (clip.Right() - p1.X) / (p2.X - p1.X);
                        p.X = clip.Right();
                    }
                    if (outcodeOut == outcode1)
                    {
                        p1 = p;
                        outcode1 = ComputeOutcode(p1, clip);
                    }
                    else
                    {
                        p2 = p;
                        outcode2 = ComputeOutcode(p2, clip);
                    }
                }
            } while (!done);
            if (accept)
                MyBitmap.DrawLine(p1.X, p1.Y, p2.X, p2.Y, brushSize + 3, color);
        }
        #endregion
    }
}
