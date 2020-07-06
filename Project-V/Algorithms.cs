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

        public static void Transofrm(List<Vertex> vertices, Transformation transformation)
        {
            double a = transformation.alpha;
            double b = transformation.beta;
            double s = Camera.width * (1 / Math.Tan(Math.PI / 8));
            Matrix4x4 Ry = new Matrix4x4((float)Math.Cos(a), 0, (float)Math.Sin(a), 0,
                                        0, 1, 0, 0,
                                        (float)-Math.Sin(a), 0, (float)Math.Cos(a), 0,
                                        0, 0, 0, 1  );
            Matrix4x4 Rx = new Matrix4x4(1, 0, 0, 0,
                                        0, (float)Math.Cos(b), (float)Math.Sin(b), 0,
                                        0, (float)-Math.Sin(b), (float)Math.Cos(b), 0,
                                        0, 0, 0, 1  );
            //Matrix4x4 Tz = new Matrix4x4(1, 0, 0, 0,
            //                            0, 1, 0, 0,
            //                            0, 0, 1, (float)Camera.distance,
            //                            0, 0, 0, 1  );
            Matrix4x4 P  = new Matrix4x4((float)s, 0, (float)(Camera.width), 0,
                                        0, (float)s, (float)(Camera.height), 0,
                                        0, 0, 0, 1,
                                        0, 0, 1, 0  );
            Matrix4x4 M = Camera.Matrix();
            Matrix4x4 result1 = Rx * Ry;
            //Matrix4x4 result2 = P * M * Tz;
            Matrix4x4 result2 = P * M;
            double x, y, z, d;
            foreach (var ver in vertices)
            {
                //rotate about the origin
                x = ver.X;
                y = ver.Y;
                z = ver.Z;
                d = ver.D;              
                ver.X = result1.M11 * x + result1.M12 * y + result1.M13 * z + result1.M14 * d;
                ver.Y = result1.M21 * x + result1.M22 * y + result1.M23 * z + result1.M24 * d;
                ver.Z = result1.M31 * x + result1.M32 * y + result1.M33 * z + result1.M34 * d;
                ver.D = result1.M41 * x + result1.M42 * y + result1.M43 * z + result1.M44 * d;

                //move
                ver.X += transformation.hori;
                ver.Y += transformation.vert;

                //projection and distance
                x = ver.X;
                y = ver.Y;
                z = ver.Z;
                d = ver.D;               
                ver.X = result2.M11 * x + result2.M12 * y + result2.M13 * z + result2.M14 * d;
                ver.Y = result2.M21 * x + result2.M22 * y + result2.M23 * z + result2.M24 * d;
                ver.Z = result2.M31 * x + result2.M32 * y + result2.M33 * z + result2.M34 * d;
                ver.D = result2.M41 * x + result2.M42 * y + result2.M43 * z + result2.M44 * d;

                ver.X /= ver.D;
                ver.Y /= ver.D;
                ver.Z /= ver.D;
            }
        }

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

        public static double LengthOfVector((double, double, double) vec)
        {
            return Math.Pow(Math.Pow(vec.Item1,2) + Math.Pow(vec.Item2, 2) + Math.Pow(vec.Item3, 2), (double)1/3);
        }

        public static (double, double, double) CrossProdOfVectors((double, double, double) vec1, (double, double, double) vec2)
        {
            (double, double, double) result = ((vec1.Item2 * vec2.Item3) - (vec1.Item3 * vec2.Item2),
                                            (vec1.Item3 * vec2.Item1) - (vec1.Item1 * vec2.Item3),
                                            (vec1.Item1 * vec2.Item2) - (vec1.Item2 * vec2.Item1));
            return result;
        }
        public static double MultiplicationOfVectors((double, double, double) vec1, (double, double, double) vec2)
        {
            double result = vec1.Item1 * vec2.Item1 + vec1.Item2 * vec2.Item2 + vec1.Item3 * vec2.Item3;
            return result;
        }
    }
}
