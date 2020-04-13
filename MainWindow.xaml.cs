using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CG_Project_III
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Point currentPoint = new Point();
        LineDrawing lineDrawing = new LineDrawing();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Canvas_MouseDown_1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            currentPoint = Mouse.GetPosition(paintSurface);
            lineDrawing.putPixel((int)currentPoint.X, (int)currentPoint.Y,paintSurface);
            //if (e.ButtonState == MouseButtonState.Pressed)
                //currentPoint = e.GetPosition(this);
        }

        private void Canvas_MouseMove_1(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //if (e.LeftButton == MouseButtonState.Pressed)
            //{
            //    Line line = new Line();

            //    line.Stroke = SystemColors.WindowFrameBrush;
            //    line.X1 = currentPoint.X;
            //    line.Y1 = currentPoint.Y - 50;
            //    line.X2 = e.GetPosition(this).X;
            //    line.Y2 = e.GetPosition(this).Y - 50;
                

            //    currentPoint = e.GetPosition(this);

            //    paintSurface.Children.Add(line);
            //}
        }

        private void Canvas_MouseUp_1(object sender, MouseButtonEventArgs e)
        {
            var secondPoint = Mouse.GetPosition(paintSurface);
            lineDrawing.lineDDA((int)currentPoint.X, (int)currentPoint.Y, (int)secondPoint.X, (int)secondPoint.Y, paintSurface);
        }
    }
}
