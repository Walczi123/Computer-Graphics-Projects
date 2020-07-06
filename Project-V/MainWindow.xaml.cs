using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace CG_Project_V
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int hChange = 1;
        private int wChange = 1;
        private int leftBound = -10;
        private int rightBound = 10;
        private double vel = 0.2;
        private double rotHorChange = Math.PI / 16;
        private double rotVerChange = Math.PI / 16;
        private static DispatcherTimer dispatcherTimer;
        //private double Vx = 0.05;
        //private double Vy = 0.05;
        private WriteableBitmap bitmap = new WriteableBitmap(100, 100, 96, 96, System.Windows.Media.PixelFormats.Bgra32, BitmapPalettes.Halftone256);
        private Cube EditingCube = null;
        //private List<Cube> AnimationList = new List<Cube>();

        public MainWindow()
        {
            InitializeComponent();
            DistanceComboBox.ItemsSource = Enumerable.Range(5, 50).Select(i => (object)i).ToArray();
            DistanceComboBox.SelectedIndex = 5;
            SetTimer();
            image.Source = bitmap;
            MyBitmap.Bitmap = bitmap;
        }
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            this.bitmap = new WriteableBitmap((int)image.ActualWidth + 1, (int)image.ActualHeight + 1, 96, 96, System.Windows.Media.PixelFormats.Bgra32, BitmapPalettes.Halftone256);
            image.Source = bitmap;
            MyBitmap.Bitmap = bitmap;
            MyBitmap.CleanDrawArea();
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if ((int)image.ActualWidth > 0 && (int)image.ActualHeight > 0)
            {
                this.bitmap = new WriteableBitmap((int)image.ActualWidth + 1, (int)image.ActualHeight + 1, 96, 96, System.Windows.Media.PixelFormats.Bgra32, BitmapPalettes.Halftone256);
                image.Source = bitmap;
                MyBitmap.Bitmap = bitmap;
                MyBitmap.Redraw();
            }
        }
        private void DrawButton_Click(object sender, RoutedEventArgs e)
        {
            Cube c = new Cube(1);
            c.Draw();
            MyBitmap.drawingCubes.Add(c);
            ComboBoxControll.Items.Add(ComboBoxControll.Items.Count + 1);
            ComboBoxControll.SelectedIndex = ComboBoxControll.Items.IndexOf(ComboBoxControll.Items.Count);
            EditingCube = c;
        }

        private void DistanceComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (DistanceComboBox.SelectedItem != null && EditingCube!=null)
            {
                Camera.distance = Int32.Parse(DistanceComboBox.SelectedItem.ToString());
                MyBitmap.Redraw();
            }
        }        
        private void ControllComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ComboBoxControll.SelectedItem != null)
            {
                var i = Int32.Parse(ComboBoxControll.SelectedItem.ToString());
                EditingCube = MyBitmap.drawingCubes[i - 1];
                //if (AnimationList.Contains(EditingCube))
                //{
                //    AnimationButton.Content = "Stop Animation";
                //}
                //else
                //{
                //    AnimationButton.Content = "Start Animation";
                //}
            }
        }
        private void EraseAllButton_Click(object sender, RoutedEventArgs e)
        {
            if (dispatcherTimer.IsEnabled)
            {
                AnimationButton.Content = "Start Animation";
                dispatcherTimer.Stop();
            }
            MyBitmap.CleanDrawArea();
            MyBitmap.drawingCubes = new List<Cube>();
            //AnimationList.Clear();
            ComboBoxControll.Items.Clear();
        }
        private void EraseButton_Click(object sender, RoutedEventArgs e)
        {
            if (EditingCube != null)
            {
                if (dispatcherTimer.IsEnabled)
                {
                    AnimationButton.Content = "Start Animation";
                    dispatcherTimer.Stop();
                }
                MyBitmap.drawingCubes.Remove(EditingCube);
                //AnimationList.Remove(EditingCube);
                int selected = ComboBoxControll.SelectedIndex;
                ComboBoxControll.Items.RemoveAt(selected);
                MyBitmap.Redraw();
                for(int i=0; i<ComboBoxControll.Items.Count;i++)
                {
                    if (ComboBoxControll.Items.IndexOf(ComboBoxControll.Items[i]) >= selected)
                        ComboBoxControll.Items[i] = (int)ComboBoxControll.Items[i] - 1;
                }
                EditingCube = null;
            }
        }

        private void UpButton_Click(object sender, RoutedEventArgs e)
        {
            if(EditingCube!=null)
                EditingCube.transformation.vert -= hChange;
            MyBitmap.Redraw();
        }

        private void DownButton_Click(object sender, RoutedEventArgs e)
        {
            if (EditingCube != null)
                EditingCube.transformation.vert += hChange;
            MyBitmap.Redraw();
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            if (EditingCube != null)
                EditingCube.transformation.hori += wChange;
            MyBitmap.Redraw();
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            if (EditingCube != null)
                EditingCube.transformation.hori -= wChange;
            MyBitmap.Redraw();
        }

        private void RotDownButton_Click(object sender, RoutedEventArgs e)
        {
            if (EditingCube != null)
                EditingCube.transformation.beta -= rotHorChange;
            MyBitmap.Redraw();
        }

        private void RotUpButton_Click(object sender, RoutedEventArgs e)
        {
            if (EditingCube != null)
                EditingCube.transformation.beta += rotHorChange;
            MyBitmap.Redraw();
        }

        private void RotRightButton_Click(object sender, RoutedEventArgs e)
        {
            if (EditingCube != null)
                EditingCube.transformation.alpha += rotVerChange;
            MyBitmap.Redraw();
        }

        private void RotLeftButton_Click(object sender, RoutedEventArgs e)
        {
            if (EditingCube != null)
                EditingCube.transformation.alpha -= rotVerChange;
            MyBitmap.Redraw();
        }

        private void AnimationButton_Click(object sender, RoutedEventArgs e)
        {
            if (!dispatcherTimer.IsEnabled)
            {
                dispatcherTimer.Start();
                AnimationButton.Content = "Stop Animation";
            }          
            else
            {
                dispatcherTimer.Stop();
                AnimationButton.Content = "Start Animation";
            }
               
            //if (EditingCube != null)
            //{
            //    if (!AnimationList.Contains(EditingCube))
            //    {
            //        AnimationButton.Content = "Stop Animation";
            //        AnimationList.Add(EditingCube);
            //        if (!dispatcherTimer.IsEnabled && AnimationList.Count == 1)
            //            dispatcherTimer.Start();
            //    }
            //    else
            //    {
            //        AnimationButton.Content = "Start Animation";
            //        AnimationList.Remove(EditingCube);
            //        if (dispatcherTimer.IsEnabled && AnimationList.Count == 0)
            //            dispatcherTimer.Stop();
            //    }
            //}
        }
        private void SetTimer()
        {
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if(Camera.x <= leftBound) vel = Math.Abs(vel);
            if(Camera.x >= rightBound) vel = -Math.Abs(vel);
            Camera.x += vel;
            MyBitmap.Redraw();
            //foreach(var cube in AnimationList) { 
            //    cube.transformation.alpha -= Math.PI / 180;
            //    cube.transformation.beta -= Math.PI / 360;
            //    double maxX = cube.MaxX();
            //    double maxY = cube.MaxY();
            //    double minX = cube.MinX();
            //    double minY = cube.MinY();
            //    if (minX < 2)
            //        Vx = Math.Abs(Vx);
            //    if (maxX > image.ActualWidth - 2)
            //        Vx = -Math.Abs(Vx);
            //    if (minY < 2)
            //        Vy = Math.Abs(Vy);
            //    if (maxY > image.ActualHeight - 2)
            //        Vy = -Math.Abs(Vy);
            //    cube.transformation.vert += Vy;
            //    cube.transformation.hori += Vx;

            //    MyBitmap.Redraw();
            //}
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = Mouse.GetPosition(image);
            MouseX.Text = p.X.ToString();
            MouseY.Text = p.Y.ToString();
            if(p.X < image.ActualWidth && p.Y < image.ActualHeight)
            {
                if (MyBitmap.ZBuffer[(int)p.X, (int)p.Y] != -10)
                    MouseZ.Text = Math.Round(MyBitmap.ZBuffer[(int)p.X, (int)p.Y], 4).ToString();
                else
                    MouseZ.Text = "-";
            }   
        }

        private void ProjectExampleButton_Click(object sender, RoutedEventArgs e)
        {
            Cube c1 = new Cube(1);
            Thread.Sleep(20); //sleep to random new colors
            Cube c2 = new Cube(1);
            MyBitmap.drawingCubes.Add(c1);
            MyBitmap.drawingCubes.Add(c2);
            c1.transformation.alpha = Math.PI / 8;
            c1.transformation.beta = Math.PI / 4;
            c1.transformation.hori -= 0.5;
            c1.transformation.alpha += Math.PI / 8;
            c2.transformation.beta -= Math.PI / 16;
            c2.transformation.hori += 0.5;
            MyBitmap.Redraw();
            ComboBoxControll.Items.Add(ComboBoxControll.Items.Count + 1);
            ComboBoxControll.Items.Add(ComboBoxControll.Items.Count + 1);
            ComboBoxControll.SelectedIndex = ComboBoxControll.Items.IndexOf(ComboBoxControll.Items.Count);
            EditingCube = c2;
        }

        private void UpButtonCamera_Click(object sender, RoutedEventArgs e)
        {
            Camera.y += 1;           
            MyBitmap.Redraw();
        }

        private void DownButtonCamera_Click(object sender, RoutedEventArgs e)
        {
            Camera.y -= 1;
            MyBitmap.Redraw();
        }

        private void RightButtonCamera_Click(object sender, RoutedEventArgs e)
        {
            Camera.x -= 1;
            MyBitmap.Redraw();
        }

        private void LeftButtonCamera_Click(object sender, RoutedEventArgs e)
        {
            Camera.x += 1;
            MyBitmap.Redraw();
        }

        private void RotDownButtonCamera_Click(object sender, RoutedEventArgs e)
        {
            Camera.targetY -=1;
            MyBitmap.Redraw();
        }

        private void RotUpButtonCamera_Click(object sender, RoutedEventArgs e)
        {
            Camera.targetY += 1;
            MyBitmap.Redraw();
        }

        private void RotRightButtonCamera_Click(object sender, RoutedEventArgs e)
        {
            Camera.targetX -= 1;
            MyBitmap.Redraw();
        }

        private void RotLeftButtonCamera_Click(object sender, RoutedEventArgs e)
        {
            Camera.targetX += 1;
            MyBitmap.Redraw();
        }

        private void ResetCameraButton_Click(object sender, RoutedEventArgs e)
        {
            Camera.y = 0;
            Camera.x = 0;
            Camera.distance = 10;
            Camera.targetX = Math.PI / 2;
            Camera.targetY = Math.PI / 2;
            MyBitmap.Redraw();
        }
    }
}