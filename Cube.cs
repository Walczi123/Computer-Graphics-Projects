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
        public Vertex Clone()
        {
            return new Vertex(X, Y, Z);
        }
        public void Change(Vertex v)
        {
            this.X = v.X;
            this.Y = v.Y;
            this.Z = v.Z;
            this.D = v.D;
        }
    }
    public class Cube
    {        
        private List<Face> Faces = new List<Face>();
        public Transformation transformation = new Transformation();
        public Cube(double val)
        {
            Random r = new Random();
            //1
            Faces.Add(new Face(new Vertex(val, val, val), new Vertex(val, val, -val), new Vertex(val, -val, -val), new Vertex(val, -val, val),
                Color.FromRgb((byte)r.Next(1, 255), (byte)r.Next(1, 255), (byte)r.Next(1, 255))));
            //2
            Faces.Add(new Face(new Vertex(val, val, val), new Vertex(val, val, -val), new Vertex(-val, val, -val), new Vertex(-val, val, val),
              Color.FromRgb((byte)r.Next(1, 255), (byte)r.Next(1, 255), (byte)r.Next(1, 255))));
            //3
            Faces.Add(new Face(new Vertex(val, val, val), new Vertex(val, -val, val), new Vertex(-val, -val, val), new Vertex(-val, val, val),
              Color.FromRgb((byte)r.Next(1, 255), (byte)r.Next(1, 255), (byte)r.Next(1, 255))));
            //4
            Faces.Add(new Face(new Vertex(-val, -val, -val), new Vertex(val, -val, -val), new Vertex(val, val, -val), new Vertex(-val, val, -val),
              Color.FromRgb((byte)r.Next(1, 255), (byte)r.Next(1, 255), (byte)r.Next(1, 255))));
            //5
            Faces.Add(new Face(new Vertex(-val, -val, -val), new Vertex(-val, val, -val), new Vertex(-val, val, val), new Vertex(-val, -val, val),
              Color.FromRgb((byte)r.Next(1, 255), (byte)r.Next(1, 255), (byte)r.Next(1, 255))));
            //6
            Faces.Add(new Face(new Vertex(-val, -val, -val), new Vertex(val, -val, -val), new Vertex(val, -val, val), new Vertex(-val, -val, val),
              Color.FromRgb((byte)r.Next(1, 255), (byte)r.Next(1, 255), (byte)r.Next(1, 255))));               
        }
        public void Draw()
        {
            foreach (var f in Faces)
                f.Draw(transformation);
        }
        public double MaxX()
        {
            return Faces.Max(item => item.MaxX());
        }

        public double MaxY()
        {
            return Faces.Max(item => item.MaxY());
        }

        public double MinX()
        {
            return Faces.Min(item => item.MinX());
        }

        public double MinY()
        {
            return Faces.Min(item => item.MinY());
        }
    }
}
