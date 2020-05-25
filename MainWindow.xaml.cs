using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CG_Project_V
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Cube cube = new Cube(1,1,1);
        private Camera camera = new Camera();
        private int hChange = 10;
        private int wChange = 10;        
        private double rotHorChange = Math.PI / 16;
        private double rotVerChange = Math.PI / 16;
        private static DispatcherTimer dispatcherTimer;
        private double Vx = 2;
        private double Vy = 1;

        public MainWindow()
        {
            InitializeComponent();
            DistanceComboBox.ItemsSource = Enumerable.Range(5, 50).Select(i => (object)i).ToArray();
            DistanceComboBox.SelectedIndex = 5;
            SetTimer();
        }
        private void DrawButton_Click(object sender, RoutedEventArgs e)
        {
            ComputeCube();
        }
        void Redraw()
        {
            Clear();
            ComputeCube();
        }

        void Clear()
        {
            MyCanvas.Children.Clear();
        }

        void ComputeCube()
        {
            double s = camera.width * (1 / Math.Tan(Math.PI / 8));
            Cube tmpCube = cube.Clone();
            foreach (var ver in tmpCube.Vertices)
            {
                double a = camera.alpha;
                double b = camera.beta;
                double x, y, z, d;
                //Rx
                x = ver.X;
                y = ver.Y;
                z = ver.Z;
                d = ver.D;
                ver.Y = Math.Cos(b) * y + Math.Sin(b) * z;
                ver.Z = - Math.Sin(b) * y + Math.Cos(b) * z;
                //Ry
                x = ver.X;
                y = ver.Y;
                z = ver.Z;
                d = ver.D;
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

                ver.X = ver.X / ver.D;
                ver.Y = ver.Y / ver.D;
                ver.Z = ver.Z / ver.D;

                ver.X += camera.hori;
            }
            DrawCube(tmpCube.Vertices);         
        }
        void DrawCube( List<Vertex> tmp)
        {
            DrawLine((int)(tmp[0].X), (int)(tmp[0].Y), (int)(tmp[1].X), (int)(tmp[1].Y));//0 1
            DrawLine((int)(tmp[0].X), (int)(tmp[0].Y), (int)(tmp[2].X), (int)(tmp[2].Y));//0 2
            DrawLine((int)(tmp[0].X), (int)(tmp[0].Y), (int)(tmp[4].X), (int)(tmp[4].Y));//0 4
            DrawLine((int)(tmp[3].X), (int)(tmp[3].Y), (int)(tmp[1].X), (int)(tmp[1].Y));//3 1
            DrawLine((int)(tmp[3].X), (int)(tmp[3].Y), (int)(tmp[2].X), (int)(tmp[2].Y));//3 2
            DrawLine((int)(tmp[3].X), (int)(tmp[3].Y), (int)(tmp[7].X), (int)(tmp[7].Y));//3 7
            DrawLine((int)(tmp[5].X), (int)(tmp[5].Y), (int)(tmp[1].X), (int)(tmp[1].Y));//5 1
            DrawLine((int)(tmp[5].X), (int)(tmp[5].Y), (int)(tmp[4].X), (int)(tmp[4].Y));//5 4
            DrawLine((int)(tmp[5].X), (int)(tmp[5].Y), (int)(tmp[7].X), (int)(tmp[7].Y));//5 7
            DrawLine((int)(tmp[6].X), (int)(tmp[6].Y), (int)(tmp[2].X), (int)(tmp[2].Y));//6 2
            DrawLine((int)(tmp[6].X), (int)(tmp[6].Y), (int)(tmp[4].X), (int)(tmp[4].Y));//6 4
            DrawLine((int)(tmp[6].X), (int)(tmp[6].Y), (int)(tmp[7].X), (int)(tmp[7].Y));//6 7
        }

        void DrawLine(int x1, int y1, int x2, int y2)
        {
            Line myLine = new Line();
            myLine.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
            myLine.X1 = x1;
            myLine.X2 = x2;
            myLine.Y1 = y1;
            myLine.Y2 = y2;
            myLine.StrokeThickness = 2;
            MyCanvas.Children.Add(myLine);
        }

        private void DistanceComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (DistanceComboBox.SelectedItem != null)
            {
                camera.distance = Int32.Parse(DistanceComboBox.SelectedItem.ToString());
                Redraw();
            }
        }

        private void EraseButton_Click(object sender, RoutedEventArgs e)
        {
            if (dispatcherTimer.IsEnabled)
            {
                AnimationButton.Content = "Start Animation";
                dispatcherTimer.Stop();
            }
            Clear();
        }

        private void UpButton_Click(object sender, RoutedEventArgs e)
        {
            camera.height -= hChange;
            Redraw();
        }

        private void DownButton_Click(object sender, RoutedEventArgs e)
        {
            camera.height += hChange;
            Redraw();
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            camera.hori += wChange;
            Redraw();
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            camera.hori -= wChange;
            Redraw();
        }

        private void RotDownButton_Click(object sender, RoutedEventArgs e)
        {
            camera.beta -= rotHorChange;
            Redraw();
        }

        private void RotUpButton_Click(object sender, RoutedEventArgs e)
        {
            camera.beta += rotHorChange;
            Redraw();
        }

        private void RotRightButton_Click(object sender, RoutedEventArgs e)
        {
            camera.alpha += rotVerChange;
            Redraw();
        }

        private void RotLeftButton_Click(object sender, RoutedEventArgs e)
        {
            camera.alpha -= rotVerChange;
            Redraw();
        }

        private void AnimationButton_Click(object sender, RoutedEventArgs e)
        {
            if (!dispatcherTimer.IsEnabled)
            {
                AnimationButton.Content = "Stop Animation";
                dispatcherTimer.Start();
            }
            else
            {
                AnimationButton.Content = "Start Animation";
                dispatcherTimer.Stop();
            }
        }
        private void SetTimer()
        {
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);
        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            camera.alpha -= Math.PI / 180;
            camera.beta -= Math.PI / 360;
            List<(double, double)> tmp = new List<(double, double)>();
            foreach(var elem in MyCanvas.Children)
            {
                if (elem is Line)
                {
                    var l = elem as Line;
                    tmp.Add((l.X1, l.Y1));
                    tmp.Add((l.X2, l.Y2));
                }
            }
            var maxX = tmp.Max(item => item.Item1);
            var maxY = tmp.Max(item => item.Item2);
            var minX = tmp.Min(item => item.Item1);
            var minY = tmp.Min(item => item.Item2);
            if (minX < 2)
                Vx = Math.Abs(Vx);            
            if (maxX > MyCanvas.ActualWidth - 2)
                Vx = -Math.Abs(Vx);
            if (minY < 2)
                Vy = Math.Abs(Vy);
            if (maxY > MyCanvas.ActualHeight - 2)
                Vy = -Math.Abs(Vy);
            camera.height += Vy;
            camera.hori += Vx;

            Redraw();
        }
    }
}