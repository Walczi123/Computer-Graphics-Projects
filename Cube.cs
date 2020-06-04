using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace CG_Project_V
{
    public class Vertex
    {
        public double X;
        public double Y;
        public double Z;
        public double D;
        public Vertex(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.D = 1;
        }
        public void Change(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.D = 1;
        }
    }
    public class Cube
    {
        public List<Vertex> Vertices = new List<Vertex>();
        public HashSet<(int, int)> SceneArea = new HashSet<(int, int)>();
        public Camera camera = new Camera();
        public double value;
        private List<Color> colors = new List<Color>();
        public Cube(double val)
        {
            value = val;
            Vertices.Add(new Vertex(val, val, val));
            Vertices.Add(new Vertex(val, val, -val));
            Vertices.Add(new Vertex(val, -val, val));
            Vertices.Add(new Vertex(val, -val, -val));
            Vertices.Add(new Vertex(-val, val, val));
            Vertices.Add(new Vertex(-val, val, -val));
            Vertices.Add(new Vertex(-val, -val, val));
            Vertices.Add(new Vertex(-val, -val, -val));
            Random r = new Random();
            for (int i = 0; i < 6; i++)
                colors.Add(Color.FromRgb((byte)r.Next(1, 255), (byte)r.Next(1, 255), (byte)r.Next(1, 255)));
        }

        public Cube Clone()
        {
            return new Cube(this.value);
        }

        public void Draw()
        {
            this.Reset();
            this.Transofrm();
            //MyBitmap.DrawLine((int)(Vertices[0].X), (int)(Vertices[0].Y), (int)(Vertices[1].X), (int)(Vertices[1].Y));//0 1
            //MyBitmap.DrawLine((int)(Vertices[0].X), (int)(Vertices[0].Y), (int)(Vertices[2].X), (int)(Vertices[2].Y));//0 2
            //MyBitmap.DrawLine((int)(Vertices[0].X), (int)(Vertices[0].Y), (int)(Vertices[4].X), (int)(Vertices[4].Y));//0 4
            //MyBitmap.DrawLine((int)(Vertices[3].X), (int)(Vertices[3].Y), (int)(Vertices[1].X), (int)(Vertices[1].Y));//3 1
            //MyBitmap.DrawLine((int)(Vertices[3].X), (int)(Vertices[3].Y), (int)(Vertices[2].X), (int)(Vertices[2].Y));//3 2
            //MyBitmap.DrawLine((int)(Vertices[3].X), (int)(Vertices[3].Y), (int)(Vertices[7].X), (int)(Vertices[7].Y));//3 7
            //MyBitmap.DrawLine((int)(Vertices[5].X), (int)(Vertices[5].Y), (int)(Vertices[1].X), (int)(Vertices[1].Y));//5 1
            //MyBitmap.DrawLine((int)(Vertices[5].X), (int)(Vertices[5].Y), (int)(Vertices[4].X), (int)(Vertices[4].Y));//5 4
            //MyBitmap.DrawLine((int)(Vertices[5].X), (int)(Vertices[5].Y), (int)(Vertices[7].X), (int)(Vertices[7].Y));//5 7
            //MyBitmap.DrawLine((int)(Vertices[6].X), (int)(Vertices[6].Y), (int)(Vertices[2].X), (int)(Vertices[2].Y));//6 2
            //MyBitmap.DrawLine((int)(Vertices[6].X), (int)(Vertices[6].Y), (int)(Vertices[4].X), (int)(Vertices[4].Y));//6 4
            //MyBitmap.DrawLine((int)(Vertices[6].X), (int)(Vertices[6].Y), (int)(Vertices[7].X), (int)(Vertices[7].Y));//6 7
        }

        public void Transofrm()
        {
            double s = camera.width * (1 / Math.Tan(Math.PI / 8));
            foreach (var ver in Vertices)
            {
                double a = camera.alpha;
                double b = camera.beta;
                double x, y, z, d;
                //Rx
                y = ver.Y;
                z = ver.Z;
                ver.Y = Math.Cos(b) * y + Math.Sin(b) * z;
                ver.Z = -Math.Sin(b) * y + Math.Cos(b) * z;
                //Ry
                x = ver.X;
                z = ver.Z;
                ver.X = Math.Cos(a) * x + Math.Sin(a) * z;
                ver.Z = -Math.Sin(a) * x + Math.Cos(a) * z;
                //Tz
                ver.Z += camera.distance;
                //P
                x = ver.X;
                y = ver.Y;
                z = ver.Z;
                d = ver.D;
                ver.X = s * x + camera.width * z;
                ver.Y = s * y + camera.height * z;
                ver.Z = d;
                ver.D = z;

                ver.X /= ver.D;
                ver.Y /= ver.D;
                ver.Z /= ver.D;

                ver.X += camera.hori;
            }
            UpdateZBuffer();
        }

        public void Reset()
        {
            Vertices[0].Change(value, value, value);
            Vertices[1].Change(value, value, -value);
            Vertices[2].Change(value, -value, value);
            Vertices[3].Change(value, -value, -value);
            Vertices[4].Change(-value, value, value);
            Vertices[5].Change(-value, value, -value);
            Vertices[6].Change(-value, -value, value);
            Vertices[7].Change(-value, -value, -value);
        }
        private static Vertex LowwerY(Vertex p1, Vertex p2)
        {
            if (p1.Y <= p2.Y) return p1;
            return p2;
        }
        private static Vertex UpperY(Vertex p1, Vertex p2)
        {
            if (p1.Y >= p2.Y) return p1;
            return p2;
        }

        public void UpdateZBuffer()
        {
            List<Vertex> wallVertices = new List<Vertex>();
            //0-1-3-2
            wallVertices.Add(this.Vertices[0]);
            wallVertices.Add(this.Vertices[1]);
            wallVertices.Add(this.Vertices[3]);
            wallVertices.Add(this.Vertices[2]);
            var PlaneEquation = Algorithms.ComputePlaneEquation(this.Vertices[0], this.Vertices[1], this.Vertices[3]);
            double a = PlaneEquation.Item1, b = PlaneEquation.Item2, c = PlaneEquation.Item3, d = PlaneEquation.Item4;
            UpdateZbufferWall(wallVertices, a, b, c, d, colors[0]);
            //0-2-6-4
            wallVertices[0] = this.Vertices[0];
            wallVertices[1] = this.Vertices[2];
            wallVertices[2] = this.Vertices[6];
            wallVertices[3] = this.Vertices[4];
            PlaneEquation = Algorithms.ComputePlaneEquation(this.Vertices[0], this.Vertices[2], this.Vertices[6]);
            a = PlaneEquation.Item1;
            b = PlaneEquation.Item2;
            c = PlaneEquation.Item3;
            d = PlaneEquation.Item4;
            UpdateZbufferWall(wallVertices, a, b, c, d, colors[1]);            
            //0-1-5-4
            wallVertices[0] = this.Vertices[0];
            wallVertices[1] = this.Vertices[1];
            wallVertices[2] = this.Vertices[5];
            wallVertices[3] = this.Vertices[4];
            PlaneEquation = Algorithms.ComputePlaneEquation(this.Vertices[0], this.Vertices[1], this.Vertices[5]);
            a = PlaneEquation.Item1;
            b = PlaneEquation.Item2;
            c = PlaneEquation.Item3;
            d = PlaneEquation.Item4;
            UpdateZbufferWall(wallVertices, a, b, c, d, colors[2]);            
            //7-3-2-6
            wallVertices[0] = this.Vertices[7];
            wallVertices[1] = this.Vertices[3];
            wallVertices[2] = this.Vertices[2];
            wallVertices[3] = this.Vertices[6];
            PlaneEquation = Algorithms.ComputePlaneEquation(this.Vertices[7], this.Vertices[3], this.Vertices[2]);
            a = PlaneEquation.Item1;
            b = PlaneEquation.Item2;
            c = PlaneEquation.Item3;
            d = PlaneEquation.Item4;
            UpdateZbufferWall(wallVertices, a, b, c, d, colors[3]);            
            //7-3-1-5
            wallVertices[0] = this.Vertices[7];
            wallVertices[1] = this.Vertices[3];
            wallVertices[2] = this.Vertices[1];
            wallVertices[3] = this.Vertices[5];
            PlaneEquation = Algorithms.ComputePlaneEquation(this.Vertices[7], this.Vertices[3], this.Vertices[1]);
            a = PlaneEquation.Item1;
            b = PlaneEquation.Item2;
            c = PlaneEquation.Item3;
            d = PlaneEquation.Item4;
            UpdateZbufferWall(wallVertices, a, b, c, d, colors[4]);            
            //7-6-4-5
            wallVertices[0] = this.Vertices[7];
            wallVertices[1] = this.Vertices[6];
            wallVertices[2] = this.Vertices[4];
            wallVertices[3] = this.Vertices[5];
            PlaneEquation = Algorithms.ComputePlaneEquation(this.Vertices[7], this.Vertices[6], this.Vertices[4]);
            a = PlaneEquation.Item1;
            b = PlaneEquation.Item2;
            c = PlaneEquation.Item3;
            d = PlaneEquation.Item4;
            UpdateZbufferWall(wallVertices, a, b, c, d, colors[5]);            
        }

        private void UpdateZbufferWall(List<Vertex> vertices, double a, double b, double c, double d, Color color)
        {
            if (c == 0)
                return;
            int N = vertices.Count();
            List<(int, double, double)> AET = new List<(int, double, double)>();
            var P = vertices;
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
                            var l = LowwerY(P[i - 1], P[i]);
                            var u = UpperY((P[i - 1]), P[i]);
                            AET.Add(((int)u.Y, l.X, (double)(P[i - 1].X - P[i].X) / (P[i - 1].Y - P[i].Y)));
                        }
                    }
                    else
                    {
                        if ((int)P[N - 1].Y > (int)P[i].Y)
                        {
                            var l = LowwerY(P[N - 1], P[i]);
                            var u = UpperY(P[N - 1], P[i]);
                            AET.Add(((int)u.Y, l.X, (double)(P[N - 1].X - P[i].X) / (P[N - 1].Y - P[i].Y)));
                        }
                    }
                    if (i < N - 1)
                    {
                        if ((int)P[i + 1].Y > (int)P[i].Y)
                        {
                            var l = LowwerY(P[i + 1], P[i]);
                            var u = UpperY(P[i + 1], P[i]);
                            AET.Add(((int)u.Y, l.X, (double)(P[i + 1].X - P[i].X) / (P[i + 1].Y - P[i].Y)));
                        }
                    }
                    else
                    {
                        if ((int)P[0].Y > (int)P[i].Y)
                        {
                            var l = LowwerY(P[0], P[i]);
                            var u = UpperY(P[0], P[i]);
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
                            z = -(a*x + b*y + d)/c;
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
    }
}
