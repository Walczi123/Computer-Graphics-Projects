using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace CG_Project_III
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WriteableBitmap bitmap = new WriteableBitmap(100, 100, 96, 96, System.Windows.Media.PixelFormats.Bgra32, null);
        private int current_drawing = 0;
        private bool thickness = false;
        private bool firstClick = false;
        private Point lastPosition = new Point();
        private Point firstPosition = new Point();
        private Polygon CurrentPolygon = null;
        private IShape CurrentEditableShape = null;
        private List<IShape> shapes = new List<IShape>() { };
        public MainWindow()
        {
            InitializeComponent();
            image.Source = bitmap;
            MyBitmap.Bitmap = bitmap;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            this.bitmap = new WriteableBitmap((int)image.ActualWidth + 1, (int)image.ActualHeight + 1, 96, 96, System.Windows.Media.PixelFormats.Bgra32, null);
            image.Source = bitmap;
            MyBitmap.Bitmap = bitmap;
            MyBitmap.CleanDrawArea();
            ThicknessComboBox.ItemsSource = Enumerable.Range(1, 30).Select(i => (object)i).ToArray();
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if ((int)image.ActualWidth > 0 && (int)image.ActualHeight > 0)
            {
                this.bitmap = new WriteableBitmap((int)image.ActualWidth + 1, (int)image.ActualHeight + 1, 96, 96, System.Windows.Media.PixelFormats.Bgra32, null);
                image.Source = bitmap;
                MyBitmap.Bitmap = bitmap;
                MyBitmap.Redraw(shapes);
            }
        }

        private void menu_shutdown(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void menu_clear(object sender, RoutedEventArgs e)
        {
            MyBitmap.CleanDrawArea();
        }
        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            var position = Mouse.GetPosition(image);
            MouseX.Text = position.X.ToString();
            MouseY.Text = position.Y.ToString();
        }
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (current_drawing != 3)
            {
                lastPosition.X = (int)e.GetPosition(image).X;
                lastPosition.Y = (int)e.GetPosition(image).Y;
            }
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            int x = (int)e.GetPosition(image).X;
            int y = (int)e.GetPosition(image).Y;
            switch (current_drawing)
            {
                case 0:
                    if (thickness && ThicknessComboBox.SelectedItem != null)
                    {
                        var brushSize = Int32.Parse(ThicknessComboBox.SelectedItem.ToString());
                        var p = new Point(x, y, brushSize, MyBitmap.FirstColor);
                        p.Draw();
                        shapes.Add(p);
                    }
                    else
                    {
                        var p = new Point(x, y, 0, MyBitmap.FirstColor);
                        p.Draw();
                        shapes.Add(p);
                    }
                    break;
                case 1:
                    if (thickness && ThicknessComboBox.SelectedItem != null)
                    {
                        var brushSize = Int32.Parse(ThicknessComboBox.SelectedItem.ToString());
                        var l = new Line(lastPosition.X, lastPosition.Y, x, y, brushSize, MyBitmap.FirstColor, MyBitmap.SecondColor);
                        l.Draw();
                        shapes.Add(l);
                    }
                    else
                    {
                        var l = new Line(lastPosition.X, lastPosition.Y, x, y, MyBitmap.FirstColor, MyBitmap.SecondColor);
                        l.Draw();
                        shapes.Add(l);
                    }
                    break;
                case 2:
                    int R = (int)Math.Sqrt(Math.Pow(x - lastPosition.X, 2) + Math.Pow(y - lastPosition.Y, 2));
                    var c = new Circle(lastPosition.X, lastPosition.Y, R, MyBitmap.FirstColor, MyBitmap.SecondColor);
                    c.Draw();
                    shapes.Add(c);
                    break;
                case 3:
                    if (!firstClick)
                    {
                        firstPosition.X = x;
                        firstPosition.Y = y;
                        lastPosition.X = x;
                        lastPosition.Y = y;
                        if (thickness && ThicknessComboBox.SelectedItem != null)
                        {
                            var brushSize = Int32.Parse(ThicknessComboBox.SelectedItem.ToString());
                            MyBitmap.DrawPoint(x, y, brushSize);
                            CurrentPolygon = new Polygon(brushSize, MyBitmap.FirstColor, MyBitmap.SecondColor);
                            CurrentPolygon.Add(new Point(x, y));
                        }
                        else
                        {
                            MyBitmap.DrawPoint(firstPosition.X, firstPosition.Y, 0);
                            CurrentPolygon = new Polygon(MyBitmap.FirstColor, MyBitmap.SecondColor);
                            CurrentPolygon.Add(new Point(x, y));
                        }
                        firstClick = !firstClick;
                    }
                    else
                    {
                        var d = Math.Sqrt(Math.Pow(x - firstPosition.X, 2) + Math.Pow(y - firstPosition.Y, 2));
                        if (d < 5)
                        {
                            MyBitmap.DrawLine(lastPosition.X, lastPosition.Y, firstPosition.X, firstPosition.Y, CurrentPolygon.GetBrushSize());
                            CurrentPolygon.Draw();
                            this.shapes.Add(CurrentPolygon);
                            CurrentPolygon = null;
                            firstClick = !firstClick;
                            //lastPosition = new Point();
                            //firstPosition = new Point();
                        }
                        else
                        {
                            MyBitmap.DrawLine(lastPosition.X, lastPosition.Y, x, y, CurrentPolygon.GetBrushSize());
                            lastPosition.X = x;
                            lastPosition.Y = y;
                            CurrentPolygon.Add(new Point(x, y));
                        }
                    }
                    break;
                case 4:
                    var edit = MyBitmap.FindShape(shapes, x, y);
                    var edit2 = MyBitmap.FindShape(shapes, lastPosition.X, lastPosition.Y);
                    if (CurrentEditableShape != null && edit2 == CurrentEditableShape)
                    {
                        if (CurrentEditableShape is Point)
                        {
                            //MessageBox.Show("Point");
                            var p = CurrentEditableShape as Point;
                            p.X = x;
                            p.Y = y;
                            MyBitmap.Redraw(shapes);
                        }
                        else if (CurrentEditableShape is Circle)
                        {
                            //MessageBox.Show("Circle");
                            var cir= CurrentEditableShape as Circle;
                            var d = MyBitmap.PointDistance(cir.OriginX, cir.OriginY, lastPosition.X, lastPosition.Y);
                            if (d < 10)
                            {
                                cir.OriginX = x;
                                cir.OriginY = y;
                                MyBitmap.Redraw(shapes);
                            }
                            else
                            {
                                var r = MyBitmap.PointDistance(cir.OriginX, cir.OriginY, x, y);
                                cir.Radius = (int)r;
                                MyBitmap.Redraw(shapes);
                            }
                        }
                        else if (CurrentEditableShape is Line)
                        {
                            //MessageBox.Show("Line");
                            var l = CurrentEditableShape as Line;
                            var d1 = MyBitmap.PointDistance(l.X1, l.Y1, lastPosition.X, lastPosition.Y);
                            var d2 = MyBitmap.PointDistance(l.X2, l.Y2, lastPosition.X, lastPosition.Y);
                            if (d1 < 10)
                            {
                                l.X1 = x;
                                l.Y1 = y;
                                MyBitmap.Redraw(shapes);
                            }
                            else if (d2 < 10)
                            {
                                l.X2 = x;
                                l.Y2 = y;
                                MyBitmap.Redraw(shapes);
                            }
                        }
                        else if (CurrentEditableShape is Polygon)
                        {
                            //MessageBox.Show("Polygon");
                            var pol = CurrentEditableShape as Polygon;
                            for(int i=0; i<pol.Vertices.Count;i++)
                            {
                                var d = MyBitmap.PointDistance(pol.Vertices[i].X, pol.Vertices[i].Y, lastPosition.X, lastPosition.Y);
                                if (d < 10)
                                {
                                    pol.Vertices[i].X = x;
                                    pol.Vertices[i].Y = y;
                                    MyBitmap.Redraw(shapes);
                                }
                            }
                        }
                    }
                    if (edit != null)
                    {
                        if (CurrentEditableShape != null)
                        {      
                            if (CurrentEditableShape != edit)
                            {
                                CurrentEditableShape.EditModeStop();
                                CurrentEditableShape = edit;
                                CurrentEditableShape.EditModeStart();
                                MyBitmap.Redraw(shapes);
                            }   
                        }                  
                        else
                        {
                            CurrentEditableShape = edit;
                            CurrentEditableShape.EditModeStart();
                            MyBitmap.Redraw(shapes);
                        }
                    }
                    break;
                case 5:
                    var del = MyBitmap.FindShape(shapes, x, y);
                    if (del != null)
                    {
                        shapes.Remove(del);
                        MyBitmap.Redraw(shapes);
                    }
                    break;
            }

        }

        private void Point_Click(object sender, RoutedEventArgs e)
        {
            current_drawing = 0;
            ButtonPoint.IsEnabled = false;
            ButtonLine.IsEnabled = true;
            ButtonCircle.IsEnabled = true;
            ButtonPolygon.IsEnabled = true;
            ButtonEdit.IsEnabled = true;
            ButtonDelete.IsEnabled = true;
            if(CurrentEditableShape != null)
            {
                CurrentEditableShape.EditModeStop();
                CurrentEditableShape = null;
                MyBitmap.Redraw(shapes);
            }

        }
        private void Line_Click(object sender, RoutedEventArgs e)
        {
            current_drawing = 1;
            ButtonPoint.IsEnabled = true;
            ButtonLine.IsEnabled = false;
            ButtonCircle.IsEnabled = true;
            ButtonPolygon.IsEnabled = true;
            ButtonEdit.IsEnabled = true;
            ButtonDelete.IsEnabled = true;
            if (CurrentEditableShape != null)
            {
                CurrentEditableShape.EditModeStop();
                CurrentEditableShape = null;
                MyBitmap.Redraw(shapes);
            }
        }
        private void Circle_Click(object sender, RoutedEventArgs e)
        {
            current_drawing = 2;
            ButtonPoint.IsEnabled = true;
            ButtonLine.IsEnabled = true;
            ButtonCircle.IsEnabled = false;
            ButtonPolygon.IsEnabled = true;
            ButtonEdit.IsEnabled = true;
            ButtonDelete.IsEnabled = true;
            if (CurrentEditableShape != null)
            {
                CurrentEditableShape.EditModeStop();
                CurrentEditableShape = null;
                MyBitmap.Redraw(shapes);
            }
        }
        private void Polygon_Click(object sender, RoutedEventArgs e)
        {
            current_drawing = 3;
            ButtonPoint.IsEnabled = true;
            ButtonLine.IsEnabled = true;
            ButtonCircle.IsEnabled = true;
            ButtonPolygon.IsEnabled = false;
            ButtonEdit.IsEnabled = true;
            ButtonDelete.IsEnabled = true;
            if (CurrentEditableShape != null)
            {
                CurrentEditableShape.EditModeStop();
                CurrentEditableShape = null;
                MyBitmap.Redraw(shapes);
            }
        }
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            current_drawing = 4;
            ButtonPoint.IsEnabled = true;
            ButtonLine.IsEnabled = true;
            ButtonCircle.IsEnabled = true;
            ButtonPolygon.IsEnabled = true;
            ButtonEdit.IsEnabled = false;
            ButtonDelete.IsEnabled = true;
            if (CurrentEditableShape != null)
            {
                CurrentEditableShape.EditModeStop();
                CurrentEditableShape = null;
                MyBitmap.Redraw(shapes);
            }
        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            current_drawing = 5;
            ButtonPoint.IsEnabled = true;
            ButtonLine.IsEnabled = true;
            ButtonCircle.IsEnabled = true;
            ButtonPolygon.IsEnabled = true;
            ButtonEdit.IsEnabled = true;
            ButtonDelete.IsEnabled = false;
            if (CurrentEditableShape != null)
            {
                CurrentEditableShape.EditModeStop();
                CurrentEditableShape = null;
                MyBitmap.Redraw(shapes);
            }
        }
        private void Selected_Color1(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            if (cp1.SelectedColor.HasValue)
            {
                var mainColor = new Color(cp1.SelectedColor.Value);
                MyBitmap.FirstColor = mainColor;
                if (cp2 != null)
                {
                    cp2.SelectedColor = System.Windows.Media.Color.FromArgb(127, (byte)mainColor.R, (byte)mainColor.G, (byte)mainColor.B);
                    MyBitmap.SecondColor = new Color(cp2.SelectedColor.Value);
                }
            }
            if (CurrentEditableShape != null)
            {
                CurrentEditableShape.FirstColor = MyBitmap.FirstColor;
                CurrentEditableShape.Draw();
            }
        }

        private void Selected_Color2(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            if (cp2.SelectedColor.HasValue)
            {
                MyBitmap.SecondColor = new Color(cp2.SelectedColor.Value);
            }
            if (CurrentEditableShape?.SecondColor != null)
            {
                CurrentEditableShape.SecondColor = MyBitmap.FirstColor;
                CurrentEditableShape.Draw();
            }
        }
        private void ThicknessComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (CurrentEditableShape != null && ThicknessComboBox.SelectedItem != null)
            {
                var brushSize = Int32.Parse(ThicknessComboBox.SelectedItem.ToString());
                CurrentEditableShape.ChangeBrushSize(brushSize);
                MyBitmap.Redraw(shapes);
            }
        }

        private void CheckBox_Checked_Thickness(object sender, RoutedEventArgs e)
        {
            thickness = true;
            if (CurrentEditableShape != null && ThicknessComboBox.SelectedItem != null)
            {
                var brushSize = Int32.Parse(ThicknessComboBox.SelectedItem.ToString());
                CurrentEditableShape.ChangeBrushSize(brushSize);
                MyBitmap.Redraw(shapes);
            }
        }

        private void CheckBox_Unchecked_Thickness(object sender, RoutedEventArgs e)
        {
            thickness = false;
        }

        private void CheckBox_Checked_AA(object sender, RoutedEventArgs e)
        {
            MyBitmap.AntiAliasing = true;
            MyBitmap.Redraw(shapes);
        }

        private void CheckBox_Unchecked_AA(object sender, RoutedEventArgs e)
        {
            MyBitmap.AntiAliasing = false;
            MyBitmap.Redraw(shapes);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "My Image data|*.dat";
            saveFileDialog1.Title = "Save an image";
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {
                System.IO.FileStream fs =(System.IO.FileStream)saveFileDialog1.OpenFile();
                BinaryFormatter formatter = new BinaryFormatter();
                try
                {
                    if(shapes != null)
                        formatter.Serialize(fs, shapes);
                }
                catch (SerializationException error)
                {
                    MessageBox.Show("Saving return SerializationException " + error.Message, "Eroor");
                }
                finally
                {
                    fs.Close();
                }
            }
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "My graphics|*.dat;";
            if (op.ShowDialog() == true)
            {
                FileStream fs = new FileStream(op.FileName, FileMode.Open);
                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    shapes = (List<IShape>)formatter.Deserialize(fs);
                }
                catch (SerializationException error)
                {
                    MessageBox.Show("Loading return SerializationException " + error.Message, "Eroor");
                }
                finally
                {
                    fs.Close();
                }
                MyBitmap.Redraw(shapes);
            }
        }
    }
}