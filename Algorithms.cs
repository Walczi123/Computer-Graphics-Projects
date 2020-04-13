using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CG_Project_III
{
    public class LineDrawing
    {
        public LineDrawing() { }
        public void lineDDA(int x1, int y1, int x2, int y2, Canvas canvas)
        {
            float dy = y2 - y1;
            float dx = x2 - x1;
            float m = dy / dx;
            float y = y1;
            for (int x = x1; x <= x2; ++x)
            {
                this.putPixel((int)x, (int)y, canvas);
                y += m;
            }
        }

        public void putPixel(int x, int y, Canvas canvas)
        {
            Rectangle rec = new Rectangle();
            Canvas.SetTop(rec, y);
            Canvas.SetLeft(rec, x);
            rec.Width = 1;
            rec.Height = 1;
            rec.Fill = new SolidColorBrush(Colors.Red);
            canvas.Children.Add(rec);
        }
    }
}
