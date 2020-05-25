using System;
using System.Collections.Generic;

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
    }
    public class Cube
    {
        public List<Vertex> Vertices = new List<Vertex>();
        public Cube(double x, double y, double z)
        {
            Vertices.Add(new Vertex(x, y, z));
            Vertices.Add(new Vertex(x, y, -z));
            Vertices.Add(new Vertex(x, -y, z));
            Vertices.Add(new Vertex(x, -y, -z));
            Vertices.Add(new Vertex(-x, y, z));
            Vertices.Add(new Vertex(-x, y, -z));
            Vertices.Add(new Vertex(-x, -y, z));
            Vertices.Add(new Vertex(-x, -y, -z));
            //Vertices.Add(new Vertex(x, y, z));
            //Vertices.Add(new Vertex(x, -y, -z));
            //Vertices.Add(new Vertex(x, -y, z));
            //Vertices.Add(new Vertex(x, y, -z));
            //Vertices.Add(new Vertex(-x, y, z));
            //Vertices.Add(new Vertex(-x, y, -z));
            //Vertices.Add(new Vertex(-x, -y, z));
            //Vertices.Add(new Vertex(-x, -y, -z));
        }

        internal Cube Clone()
        {
            return new Cube(Vertices[0].X, Vertices[0].Y, Vertices[0].Z);
        }
    }
}
