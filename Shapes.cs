using System;
using System.Collections.Generic;
using System.Linq;

namespace CG_Project_III
{
    [Serializable]
    public class Color
    {
        public int A
        {
            set;
            get;
        }
        public int R
        {
            set;
            get;
        }
        public int G
        {
            set;
            get;
        }
        public int B
        {
            set;
            get;
        }
        public  Color(System.Windows.Media.Color color)
        {
            this.A = color.A;
            this.R = color.R;
            this.G = color.G;
            this.B = color.B;
        }
    }

    public interface IShape
    {
        Color FirstColor
        {
            set;
            get;
        }
        void Draw();
    }
    [Serializable]
    public class Point : IShape
    {
        public Point() { }
        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
            this.BrushSize = 0;
        }
        public Point(int x, int y, Color color1)
        {
            this.X = x;
            this.Y = y;
            this.BrushSize = 0;
            this.FirstColor = color1;
        }
        public Point(int x, int y, int brushSize, Color color1)
        {
            this.X = x;
            this.Y = y;
            this.BrushSize = brushSize;
            this.FirstColor = color1;
        }
        public int X
        {
            get;
            set;
        }
        public int Y
        {
            get;
            set;
        }
        public int BrushSize
        {
            get;
            set;
        }
        public Color FirstColor
        {
            set;
            get;
        }
        public void Draw()
        {
            MyBitmap.DrawPoint(X, Y, BrushSize, FirstColor);
        }
    }
    [Serializable]
    public class Line : IShape
    {
        public Line(int x1, int y1, int x2, int y2, Color color1, Color color2)
        {
            this.X1 = x1;
            this.Y1 = y1; 
            this.X2 = x2;
            this.Y2 = y2;
            this.BrushSize = 0;
            this.FirstColor = color1;
            this.SecondColor = color2;
        }
        public Line(int x1, int y1, int x2, int y2, int brushSize, Color color1, Color color2)
        {
            this.X1 = x1;
            this.Y1 = y1;
            this.X2 = x2;
            this.Y2 = y2;
            this.BrushSize = brushSize;
            this.FirstColor = color1;
            this.SecondColor = color2;
        }
        public int X1
        {
            get;
            set;
        }
        public int Y1
        {
            get;
            set;
        }
        public int X2
        {
            get;
            set;
        }
        public int Y2
        {
            get;
            set;
        }
        public int BrushSize
        {
            get;
            set;
        }
        public Color FirstColor
        {
            set;
            get;
        }
        public Color SecondColor
        {
            set;
            get;
        }

        public void Draw()
        {
            MyBitmap.DrawLine(X1, Y1, X2, Y2, BrushSize, FirstColor, SecondColor);
        }
    }
    [Serializable]
    public class Circle : IShape
    {
        public Circle(int x, int y, int r, Color color1, Color color2)
        {
            this.OriginX = x;
            this.OriginY = y;
            this.Radius = r;
            this.FirstColor = color1;
            this.SecondColor = color2;
        }
        public int OriginX
        {
            get;
            set;
        }
        public int OriginY
        {
            get;
            set;
        }
        public int Radius
        {
            get;
            set;
        }
        public Color FirstColor
        {
            set;
            get;
        }
        public Color SecondColor
        {
            set;
            get;
        }

        public void Draw()
        {
            MyBitmap.DrawCircle(OriginX, OriginY, Radius, FirstColor, SecondColor);
        }
    }
    [Serializable]
    public class Polygon : IShape
    {
        public Polygon(Color color1, Color color2)
        {
            this.Vertices = new List<Point>() { };
            this.FirstColor = color1;
            this.SecondColor = color2;
            this.BrushSize = 0;
        }
        public Polygon(int brushSize, Color color1, Color color2)
        {
            this.Vertices = new List<Point>() { };
            this.FirstColor = color1;
            this.SecondColor = color2;
            this.BrushSize = brushSize;
        }
        public List<Point> Vertices
        {
            set;
            get;
        }
        public void Add(Point point)
        {
            Vertices.Add(point);
        }

        public void Draw()
        {
            int i;
            for (i=0; i < Vertices.Count() - 1; i++)
            {
                MyBitmap.DrawLine(Vertices[i].X, Vertices[i].Y, Vertices[i + 1].X, Vertices[i + 1].Y, BrushSize, FirstColor, SecondColor);
            }
            MyBitmap.DrawLine(Vertices[0].X, Vertices[0].Y, Vertices[i].X, Vertices[i].Y, BrushSize, FirstColor, SecondColor);
        }

        public int BrushSize
        {
            get;
            set;
        }
        public Color FirstColor
        {
            set;
            get;
        }
        public Color SecondColor
        {
            set;
            get;
        }
    }
}
