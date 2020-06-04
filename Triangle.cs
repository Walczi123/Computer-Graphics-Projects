using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CG_Project_V
{
    class Triangle
    {
        public List<Vertex> Vertices;

        public Triangle(Vertex a, Vertex b, Vertex c)
        {
            Vertices = new List<Vertex>();
            Vertices.Add(a);
            Vertices.Add(b);
            Vertices.Add(c);
        }
        public Triangle(List<Vertex> vertices)
        {
            this.Vertices = vertices;
        }
    }
}
