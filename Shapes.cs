using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

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
    [Serializable]
    public abstract class IShape
    {
        protected int editSize = 10;
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
        public HashSet<(int, int)> Pixels
        {
            set;
            get;
        }
        protected int BrushSize;
        public void ChangeBrushSize(int change)
        {
            if (BrushSize != -1)
            {
                BrushSize = change;
                if (BrushSize < 0)
                    BrushSize = 0;
            }             
        }
        public int GetBrushSize()
        {
            return BrushSize;
        }

        public abstract void Draw();
        public abstract void EditModeStart();
        public abstract void EditModeStop();

    }
    [Serializable]
    public class Point : IShape
    {
        private bool editMode = false;
        public Point() { }
        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
            this.BrushSize = 0;
            this.Pixels = null;
        }
        public Point(int x, int y, Color color1)
        {
            this.X = x;
            this.Y = y;
            this.BrushSize = 0;
            this.FirstColor = color1;
            this.Pixels = null;
        }
        public Point(int x, int y, int brushSize, Color color1)
        {
            this.X = x;
            this.Y = y;
            this.BrushSize = brushSize;
            this.FirstColor = color1;
            this.Pixels = null;
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
        public override void Draw()
        {
            Pixels = MyBitmap.DrawPoint(X, Y, BrushSize, FirstColor);
            if (editMode)
            {
                Pixels.UnionWith(MyBitmap.DrawPoint(X, Y, BrushSize + editSize, FirstColor));
            }
        }
        public override void EditModeStart()
        {
            editMode = true;
            this.Draw();
        }
        public override void EditModeStop()
        {
            editMode = false;
        }
    }
    [Serializable]
    public class Line : IShape
    {
        private bool editMode = false;
        public Line(int x1, int y1, int x2, int y2, Color color1, Color color2)
        {
            this.X1 = x1;
            this.Y1 = y1; 
            this.X2 = x2;
            this.Y2 = y2;
            this.BrushSize = 0;
            this.FirstColor = color1;
            this.SecondColor = color2;
            this.Pixels = null;
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
            this.Pixels = null;
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
        public override void Draw()
        {
            Pixels = MyBitmap.DrawLine(X1, Y1, X2, Y2, BrushSize, FirstColor, SecondColor);
            if (editMode)
            {
                Pixels.UnionWith(MyBitmap.DrawPoint(X1, Y1, BrushSize + editSize, FirstColor));
                Pixels.UnionWith(MyBitmap.DrawPoint(X2, Y2, BrushSize + editSize, FirstColor));
            }
        }
        public override void EditModeStart()
        {
            editMode = true;
            this.Draw();
        }
        public override void EditModeStop()
        {
            editMode = false;
        }
    }
    [Serializable]
    public class Circle : IShape
    {
        private bool editMode = false;
        public Circle(int x, int y, int r, Color color1, Color color2)
        {
            this.OriginX = x;
            this.OriginY = y;
            this.Radius = r;
            this.BrushSize = -1;
            this.FirstColor = color1;
            this.SecondColor = color2;
            this.Pixels = null;
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
        public override void Draw()
        {
            Pixels = MyBitmap.DrawCircle(OriginX, OriginY, Radius, FirstColor, SecondColor);
            if (editMode)
            {
                Pixels.UnionWith(MyBitmap.DrawPoint(OriginX, OriginY, editSize, FirstColor));
            }
        }
        public override void EditModeStart()
        {
            editMode = true;
            this.Draw();
        }
        public override void EditModeStop()
        {
            editMode = false;
        }
    }
    [Serializable]
    public class Polygon : IShape
    {
        private bool editMode = false;
        public Polygon(Color color1, Color color2)
        {
            this.Vertices = new List<Point>() { };
            this.FirstColor = color1;
            this.SecondColor = color2;
            this.BrushSize = 0;
            this.Pixels = null;
        }
        public Polygon(int brushSize, Color color1, Color color2)
        {
            this.Vertices = new List<Point>() { };
            this.FirstColor = color1;
            this.SecondColor = color2;
            this.BrushSize = brushSize;
            this.Pixels = null;
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
        public override void Draw()
        {
            int i;
            Pixels = new HashSet<(int, int)>();
            for (i = 0; i < Vertices.Count() - 1; i++)
            {
                Pixels.UnionWith(MyBitmap.DrawLine(Vertices[i].X, Vertices[i].Y, Vertices[i + 1].X, Vertices[i + 1].Y, BrushSize, FirstColor, SecondColor));
                if (editMode)
                {
                    Pixels.UnionWith(MyBitmap.DrawPoint(Vertices[i].X, Vertices[i].Y, editSize, FirstColor));
                }
            }
            Pixels.UnionWith(MyBitmap.DrawLine(Vertices[0].X, Vertices[0].Y, Vertices[i].X, Vertices[i].Y, BrushSize, FirstColor, SecondColor));
            if (editMode)
            {
                Pixels.UnionWith(MyBitmap.DrawPoint(Vertices[i].X, Vertices[i].Y, editSize, FirstColor));
                var xy = this.Center();
                Pixels.UnionWith(MyBitmap.DrawPoint(xy.Item1, xy.Item2, editSize, FirstColor));
            }
             
        }
        public override void EditModeStart()
        {
            editMode = true;
            this.Draw();
        }
        public override void EditModeStop()
        {
            editMode = false;
        }
        public (int,int) Center()
        {
            int sum_x = 0, sum_y = 0;
           foreach( var vertex in Vertices)
           {
                sum_x += vertex.X;
                sum_y += vertex.Y;
           }
            sum_x /= Vertices.Count();
            sum_y /= Vertices.Count();
            return (sum_x, sum_y);
        }
        public (int,int) WhichLine(int x, int y)
        {
            int i;
            for (i = 0; i < Vertices.Count() - 1; i++)
            {
                var set = MyBitmap.DrawLine(Vertices[i].X, Vertices[i].Y, Vertices[i + 1].X, Vertices[i + 1].Y, BrushSize, FirstColor, SecondColor);
                foreach( var element in set)
                {
                    if (MyBitmap.PointDistance(element.Item1, element.Item2, x, y) < 5)
                    {
                        return (i, i + 1);
                    }
                }    
            }
            var set2 = MyBitmap.DrawLine(Vertices[0].X, Vertices[0].Y, Vertices[i].X, Vertices[i].Y, BrushSize, FirstColor, SecondColor);
            foreach (var element in set2)
            {
                if (MyBitmap.PointDistance(element.Item1, element.Item2, x, y) < 5)
                {
                    return (0, i );
                }
            }
            return (-1, -1);
        }
    }
}
