using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace CG_Project_V
{
    class Face
    {
        public Triangle triangleA;
        public Triangle triangleB;
        public Color color;
        public Face(Vertex a, Vertex b, Vertex c, Vertex d, Color col)
        {
            triangleA = new Triangle(a, b, c);
            triangleB = new Triangle(a, d, c);
            color = col;
        }
        public void Draw(Transformation transformation)
        {
            triangleA.Draw(color, transformation);
            triangleB.Draw(color, transformation);
        }
        public double MaxX()
        {
            return Math.Max(triangleA.MaxX(),triangleB.MaxX());
        }

        public double MaxY()
        {
            return Math.Max(triangleA.MaxY(), triangleB.MaxY());
        }

        public double MinX()
        {
            return Math.Min(triangleA.MinX(), triangleB.MinX());
        }

        public double MinY()
        {
            return Math.Min(triangleA.MinY(), triangleB.MinY());
        }
    }
    class Triangle
    {
        public List<Vertex> Vertices;
        public readonly List<Vertex> VerticesPattern;
        public Triangle(Vertex a, Vertex b, Vertex c)
        {
            Vertices = new List<Vertex>();
            Vertices.Add(a);
            Vertices.Add(b);
            Vertices.Add(c);
            VerticesPattern = Vertices.Select(item => item.Clone()).ToList();
        }
        public Triangle(List<Vertex> vertices)
        {
            this.Vertices = vertices;
        }
        public double MaxX()
        {
            return Vertices.Max(item => item.X);
        }

        public double MaxY()
        {
            return Vertices.Max(item => item.Y);
        }

        public double MinX()
        {
            return Vertices.Min(item => item.X);
        }

        public double MinY()
        {
            return Vertices.Min(item => item.Y);
        }

        private void ResetVertices()
        {
            for (int i = 0; i < 3; i++)
                Vertices[i].Change(VerticesPattern[i]);
        }
        public void Draw(Color color, Transformation transformation)
        {
            ResetVertices();
            Algorithms.Transofrm(Vertices, transformation);
            var PlaneEquation = Algorithms.ComputePlaneEquation(this.Vertices[0], this.Vertices[1], this.Vertices[2]);
            double a = PlaneEquation.Item1, b = PlaneEquation.Item2, c = PlaneEquation.Item3, d = PlaneEquation.Item4;
            if (c == 0)
                return;
            int N = Vertices.Count();
            List<(int, double, double)> AET = new List<(int, double, double)>();
            var P = Vertices;
            var P1 = P.OrderBy(p => p.Y).ToList();
            int[] indices = new int[N];
            for (int j = 0; j < N; j++)
                indices[j] = P.IndexOf(P.Find(x => x == P1[j]));
            int k = 0;
            double z;
            int i = indices[k];
            int y, ymin, ymax;
            y = ymin = (int)P[indices[0]].Y;
            ymax = (int)P[indices[N - 1]].Y;
            MyBitmap.Bitmap.Lock();
            while (y < ymax)
            {
                while ((int)P[i].Y == y)
                {
                    if (i > 0)
                    {
                        if ((int)P[i - 1].Y > (int)P[i].Y)
                        {
                            var l = Algorithms.LowwerY(P[i - 1], P[i]);
                            var u = Algorithms.UpperY((P[i - 1]), P[i]);
                            AET.Add(((int)u.Y, l.X, (double)(P[i - 1].X - P[i].X) / (P[i - 1].Y - P[i].Y)));
                        }
                    }
                    else
                    {
                        if ((int)P[N - 1].Y > (int)P[i].Y)
                        {
                            var l = Algorithms.LowwerY(P[N - 1], P[i]);
                            var u = Algorithms.UpperY(P[N - 1], P[i]);
                            AET.Add(((int)u.Y, l.X, (double)(P[N - 1].X - P[i].X) / (P[N - 1].Y - P[i].Y)));
                        }
                    }
                    if (i < N - 1)
                    {
                        if ((int)P[i + 1].Y > (int)P[i].Y)
                        {
                            var l = Algorithms.LowwerY(P[i + 1], P[i]);
                            var u = Algorithms.UpperY(P[i + 1], P[i]);
                            AET.Add(((int)u.Y, l.X, (double)(P[i + 1].X - P[i].X) / (P[i + 1].Y - P[i].Y)));
                        }
                    }
                    else
                    {
                        if ((int)P[0].Y > (int)P[i].Y)
                        {
                            var l = Algorithms.LowwerY(P[0], P[i]);
                            var u = Algorithms.UpperY(P[0], P[i]);
                            AET.Add(((int)u.Y, l.X, (double)(P[0].X - P[i].X) / (P[0].Y - P[i].Y)));
                        }
                    }
                    ++k;
                    i = indices[k];
                }
                //sort AET by x value
                AET = AET.OrderBy(item => item.Item2).ToList();
                //fill pixels between pairs of intersections
                for (int j = 0; j < AET.Count; j += 2)
                {
                    if (j + 1 < AET.Count)
                    {
                        for (int x = (int)AET[j].Item2; x <= (int)AET[j + 1].Item2; x++)
                        {
                            z = -(a * x + b * y + d) / c;
                            if (x > 0 && y > 0 && x < MyBitmap.Bitmap.Width && y < MyBitmap.Bitmap.Height && z > MyBitmap.ZBuffer[x, y])
                            {
                                MyBitmap.ZBuffer[x, y] = z;
                                MyBitmap.DrawPixel(x, y, color);
                            }
                        }
                    }
                }
                ++y;
                //remove from AET edges for which ymax = y
                AET.RemoveAll(x => x.Item1 == y);
                //x += 1 / m
                for (int j = 0; j < AET.Count; j++)
                    AET[j] = (AET[j].Item1, AET[j].Item2 + AET[j].Item3, AET[j].Item3);
            }
            MyBitmap.Bitmap.Unlock();
        }
    }
}
