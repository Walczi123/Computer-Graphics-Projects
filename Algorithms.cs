using System;
using System.Collections.Generic;
using System.Numerics;
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
        public static double VertexDistance(Vertex a, Vertex b)
        {
            return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2) + Math.Pow(a.Z - b.Z, 2));
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

        public static void Transofrm(List<Vertex> vertices, Camera camera)
        {
            double a = camera.alpha;
            double b = camera.beta;
            double s = camera.width * (1 / Math.Tan(Math.PI / 8));
            Matrix4x4 Ry = new Matrix4x4((float)Math.Cos(a), 0, (float)Math.Sin(a), 0,
                                        0, 1, 0, 0,
                                        (float)-Math.Sin(a), 0, (float)Math.Cos(a), 0,
                                        0, 0, 0, 1  );
            Matrix4x4 Rx = new Matrix4x4(1, 0, 0, 0,
                                        0, (float)Math.Cos(b), (float)Math.Sin(b), 0,
                                        0, (float)-Math.Sin(b), (float)Math.Cos(b), 0,
                                        0, 0, 0, 1  );
            Matrix4x4 Tz = new Matrix4x4(1, 0, 0, 0,
                                        0, 1, 0, 0,
                                        0, 0, 1, (float)camera.distance,
                                        0, 0, 0, 1  );
            Matrix4x4 P  = new Matrix4x4((float)s, 0, (float)camera.width, 0,
                                        0, (float)s, (float)camera.height, 0,
                                        0, 0, 0, 1,
                                        0, 0, 1, 0  );
            Matrix4x4 result = P * Tz * Rx * Ry;
            double x, y, z, d;
            foreach (var ver in vertices)
            {
                x = ver.X;
                y = ver.Y;
                z = ver.Z;
                d = ver.D;
                ver.X = result.M11 * x + result.M12 * y + result.M13 * z + result.M14 * d;
                ver.Y = result.M21 * x + result.M22 * y + result.M23 * z + result.M24 * d;
                ver.Z = result.M31 * x + result.M32 * y + result.M33 * z + result.M34 * d;
                ver.D = result.M41 * x + result.M42 * y + result.M43 * z + result.M44 * d;

                ver.X /= ver.D;
                ver.Y /= ver.D;
                ver.Z /= ver.D;

                ver.X += camera.hori;
            }
        }

        //public static void Transofrm(List<Vertex> vertices, Camera camera)
        //{
        //    double a = camera.alpha;
        //    double b = camera.beta;
        //    double s = camera.width * (1 / Math.Tan(Math.PI / 8));
        //    foreach (var ver in vertices)
        //    {

        //        double x, y, z, d;
        //        //Rx
        //        y = ver.Y;
        //        z = ver.Z;
        //        ver.Y = Math.Cos(b) * y + Math.Sin(b) * z;
        //        ver.Z = -Math.Sin(b) * y + Math.Cos(b) * z;
        //        //Ry
        //        x = ver.X;
        //        z = ver.Z;
        //        ver.X = Math.Cos(a) * x + Math.Sin(a) * z;
        //        ver.Z = -Math.Sin(a) * x + Math.Cos(a) * z;
        //        //Tz
        //        ver.Z += camera.distance;
        //        //P
        //        x = ver.X;
        //        y = ver.Y;
        //        z = ver.Z;
        //        d = ver.D;
        //        //ver.X = s * x + camera.width * z;
        //        ver.Y = s * y + camera.height * z;
        //        ver.Z = d;
        //        ver.D = z;

        //        ver.X /= ver.D;
        //        ver.Y /= ver.D;
        //        ver.Z /= ver.D;

        //        ver.X += camera.hori;
        //    }
        //}

        public static Vertex LowwerY(Vertex p1, Vertex p2)
        {
            if (p1.Y <= p2.Y) return p1;
            return p2;
        }
        public static Vertex UpperY(Vertex p1, Vertex p2)
        {
            if (p1.Y >= p2.Y) return p1;
            return p2;
        }
    }
}
