using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CG_Project_IV
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields Common
        private WriteableBitmap bitmap = new WriteableBitmap(100, 100, 96, 96, System.Windows.Media.PixelFormats.Bgra32, BitmapPalettes.Halftone256);
        private int current_drawing = 0;
        private bool thickness = false;
        private bool firstClick = false;
        private Point lastPosition = new Point();
        private Point firstPosition = new Point();
        private Polygon CurrentPolygon = null;
        private IShape CurrentEditableShape = null;
        #endregion
        #region Fields Project IV
        private Rectangle CurrentRectangle = null;
        private Color FillingColor;
        private Bitmap FillingPattern = null;
        #endregion

        public void Image(ImageSource bitmap)
        {
            image.Source = bitmap;
        }


        public MainWindow()
        {
            InitializeComponent();
            image.Source = bitmap;
            MyBitmap.Bitmap = bitmap;
        }

        #region Methods Project III
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            this.bitmap = new WriteableBitmap((int)image.ActualWidth + 1, (int)image.ActualHeight + 1, 96, 96, System.Windows.Media.PixelFormats.Bgra32, BitmapPalettes.Halftone256);
            image.Source = bitmap;
            MyBitmap.Bitmap = bitmap;
            MyBitmap.CleanDrawArea();
            MyBitmap.Shapes = new List<IShape>() { };
            ThicknessComboBox.ItemsSource = Enumerable.Range(1, 30).Select(i => (object)i).ToArray();
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
        private void Point_Click(object sender, RoutedEventArgs e)
        {
            current_drawing = 0;
            ButtonPoint.IsEnabled = false;
            ButtonLine.IsEnabled = true;
            ButtonCircle.IsEnabled = true;
            ButtonPolygon.IsEnabled = true;
            ButtonEdit.IsEnabled = true;
            ButtonDelete.IsEnabled = true;
            ButtonCapsule.IsEnabled = true;
            ButtonRectangle.IsEnabled = true;
            ButtonClipping.IsEnabled = true;
            ButtonFillColor.IsEnabled = true;
            if (CurrentEditableShape != null)
            {
                CurrentEditableShape.EditModeStop();
                CurrentEditableShape = null;
            }
            if (MyBitmap.ClippingShape != null)
            {
                MyBitmap.ClippingShape.ClippingModeStop();
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
            ButtonCapsule.IsEnabled = true;
            ButtonRectangle.IsEnabled = true;
            ButtonClipping.IsEnabled = true;
            ButtonFillColor.IsEnabled = true;
            ButtonFillPattern.IsEnabled = true;
            if (CurrentEditableShape != null)
            {
                CurrentEditableShape.EditModeStop();
                CurrentEditableShape = null;
            }
            if (MyBitmap.ClippingShape != null)
            {
                MyBitmap.ClippingShape.ClippingModeStop();
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
            ButtonCapsule.IsEnabled = true;
            ButtonRectangle.IsEnabled = true;
            ButtonClipping.IsEnabled = true;
            ButtonFillColor.IsEnabled = true;
            ButtonFillPattern.IsEnabled = true;
            if (CurrentEditableShape != null)
            {
                CurrentEditableShape.EditModeStop();
                CurrentEditableShape = null;
                MyBitmap.Redraw();
            }
            if (MyBitmap.ClippingShape != null)
            {
                MyBitmap.ClippingShape.ClippingModeStop();
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
            ButtonCapsule.IsEnabled = true;
            ButtonRectangle.IsEnabled = true;
            ButtonClipping.IsEnabled = true;
            ButtonFillColor.IsEnabled = true;
            ButtonFillPattern.IsEnabled = true;
            if (CurrentEditableShape != null)
            {
                CurrentEditableShape.EditModeStop();
                CurrentEditableShape = null;
            }
            if (MyBitmap.ClippingShape != null)
            {
                MyBitmap.ClippingShape.ClippingModeStop();
            }
            firstPosition = null;
            lastPosition = null;
        }
        private void Capsule_Click(object sender, RoutedEventArgs e)
        {
            current_drawing = 4;
            ButtonPoint.IsEnabled = true;
            ButtonLine.IsEnabled = true;
            ButtonCircle.IsEnabled = true;
            ButtonPolygon.IsEnabled = true;
            ButtonEdit.IsEnabled = true;
            ButtonDelete.IsEnabled = true;
            ButtonCapsule.IsEnabled = false;
            ButtonRectangle.IsEnabled = true;
            ButtonClipping.IsEnabled = true;
            ButtonFillColor.IsEnabled = true;
            ButtonFillPattern.IsEnabled = true;
            if (CurrentEditableShape != null)
            {
                CurrentEditableShape.EditModeStop();
                CurrentEditableShape = null;
            }
            if (MyBitmap.ClippingShape != null)
            {
                MyBitmap.ClippingShape.ClippingModeStop();
            }
        }       
        private void Rectangle_Click(object sender, RoutedEventArgs e)
        {
            current_drawing = 5;
            ButtonPoint.IsEnabled = true;
            ButtonLine.IsEnabled = true;
            ButtonCircle.IsEnabled = true;
            ButtonPolygon.IsEnabled = true;
            ButtonEdit.IsEnabled = true;
            ButtonDelete.IsEnabled = true;
            ButtonCapsule.IsEnabled = true;
            ButtonRectangle.IsEnabled = false;
            ButtonClipping.IsEnabled = true;
            ButtonFillColor.IsEnabled = true;
            ButtonFillPattern.IsEnabled = true;
            if (CurrentEditableShape != null)
            {
                CurrentEditableShape.EditModeStop();
                CurrentEditableShape = null;
            }
            if (MyBitmap.ClippingShape != null)
            {
                MyBitmap.ClippingShape.ClippingModeStop();
            }
        }
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            current_drawing = 6;
            ButtonPoint.IsEnabled = true;
            ButtonLine.IsEnabled = true;
            ButtonCircle.IsEnabled = true;
            ButtonPolygon.IsEnabled = true;
            ButtonEdit.IsEnabled = false;
            ButtonDelete.IsEnabled = true;
            ButtonCapsule.IsEnabled = true;
            ButtonRectangle.IsEnabled = true;
            ButtonClipping.IsEnabled = true;
            ButtonFillColor.IsEnabled = true;
            ButtonFillPattern.IsEnabled = true;
            if (CurrentEditableShape != null)
            {
                CurrentEditableShape.EditModeStop();
                CurrentEditableShape = null;
            }
            if (MyBitmap.ClippingShape != null)
            {
                MyBitmap.ClippingShape.ClippingModeStop();
            }
        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            current_drawing = 7;
            ButtonPoint.IsEnabled = true;
            ButtonLine.IsEnabled = true;
            ButtonCircle.IsEnabled = true;
            ButtonPolygon.IsEnabled = true;
            ButtonEdit.IsEnabled = true;
            ButtonDelete.IsEnabled = false;
            ButtonCapsule.IsEnabled = true;
            ButtonRectangle.IsEnabled = true;
            ButtonClipping.IsEnabled = true;
            ButtonFillColor.IsEnabled = true;
            ButtonFillPattern.IsEnabled = true;
            if (CurrentEditableShape != null)
            {
                CurrentEditableShape.EditModeStop();
                CurrentEditableShape = null;
            }
            if (MyBitmap.ClippingShape != null)
            {
                MyBitmap.ClippingShape.ClippingModeStop();
            }
        }
        private void ThicknessComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (CurrentEditableShape != null && ThicknessComboBox.SelectedItem != null)
            {
                var brushSize = Int32.Parse(ThicknessComboBox.SelectedItem.ToString());
                CurrentEditableShape.ChangeBrushSize(brushSize);
                MyBitmap.Redraw();
            }
        }
        private void CheckBox_Checked_Thickness(object sender, RoutedEventArgs e)
        {
            thickness = true;
            if (CurrentEditableShape != null && ThicknessComboBox.SelectedItem != null)
            {
                var brushSize = Int32.Parse(ThicknessComboBox.SelectedItem.ToString());
                CurrentEditableShape.ChangeBrushSize(brushSize);
                MyBitmap.Redraw();
            }
        }
        private void CheckBox_Unchecked_Thickness(object sender, RoutedEventArgs e)
        {
            thickness = false;
        }
        private void CheckBox_Checked_AA(object sender, RoutedEventArgs e)
        {
            MyBitmap.AntiAliasing = true;
            MyBitmap.Redraw();
        }
        private void CheckBox_Unchecked_AA(object sender, RoutedEventArgs e)
        {
            MyBitmap.AntiAliasing = false;
            MyBitmap.Redraw();
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
                    if(MyBitmap.Shapes != null)
                        formatter.Serialize(fs, MyBitmap.Shapes);
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
                    MyBitmap.Shapes = (List<IShape>)formatter.Deserialize(fs);
                }
                catch (SerializationException error)
                {
                    MessageBox.Show("Loading return SerializationException " + error.Message, "Eroor");
                }
                finally
                {
                    fs.Close();
                }
                MyBitmap.Redraw();
            }
        }
        private void CreateCaplsule(int x, int y)
        {
            if (firstPosition == null)
            {
                firstPosition = new Point(x, y);
            }
            else if (lastPosition == null)
            {
                lastPosition = new Point(x, y);
            }
            else
            {
                var cap = new Capsule(firstPosition.X, firstPosition.Y, lastPosition.X, lastPosition.Y, x, y, MyBitmap.FirstColor);
                MyBitmap.Shapes.Add(cap);
                cap.Draw();
                firstPosition = null;
                lastPosition = null;
            }
        }
        private void CreatePolygon(int x, int y)
        {
            if (!firstClick)
            {
                if (firstPosition == null)
                    firstPosition = new Point();                
                if (lastPosition == null)
                    lastPosition = new Point();
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
                    MyBitmap.Shapes.Add(CurrentPolygon);
                    CurrentPolygon = null;
                    firstClick = !firstClick;
                }
                else
                {
                    MyBitmap.DrawLine(lastPosition.X, lastPosition.Y, x, y, CurrentPolygon.GetBrushSize());
                    lastPosition.X = x;
                    lastPosition.Y = y;
                    CurrentPolygon.Add(new Point(x, y));
                }
            }
        }
        private void CreateCircle(int x, int y)
        {
            int R = (int)Math.Sqrt(Math.Pow(x - lastPosition.X, 2) + Math.Pow(y - lastPosition.Y, 2));
            var c = new Circle(lastPosition.X, lastPosition.Y, R, MyBitmap.FirstColor, MyBitmap.SecondColor);
            c.Draw();
            MyBitmap.Shapes.Add(c);
        }
        private void CreateLine(int x, int y)
        {
            if (thickness && ThicknessComboBox.SelectedItem != null)
            {
                var brushSize = Int32.Parse(ThicknessComboBox.SelectedItem.ToString());
                var l = new Line(lastPosition.X, lastPosition.Y, x, y, brushSize, MyBitmap.FirstColor, MyBitmap.SecondColor);
                l.Draw();
                MyBitmap.Shapes.Add(l);
            }
            else
            {
                var l = new Line(lastPosition.X, lastPosition.Y, x, y, MyBitmap.FirstColor, MyBitmap.SecondColor);
                l.Draw();
                MyBitmap.Shapes.Add(l);
            }
        }
        private void CreatePoint(int x, int y)
        {
            if (thickness && ThicknessComboBox.SelectedItem != null)
            {
                var brushSize = Int32.Parse(ThicknessComboBox.SelectedItem.ToString());
                var p = new Point(x, y, brushSize, MyBitmap.FirstColor);
                p.Draw();
                MyBitmap.Shapes.Add(p);
            }
            else
            {
                var p = new Point(x, y, 0, MyBitmap.FirstColor);
                p.Draw();
                MyBitmap.Shapes.Add(p);
            }
        }
        #endregion

        #region Common Methods
        private void menu_shutdown(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void menu_clear(object sender, RoutedEventArgs e)
        {
            MyBitmap.CleanDrawArea();
            MyBitmap.Shapes = new List<IShape>() { };
        }
        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            var position = Mouse.GetPosition(image);
            MouseX.Text = position.X.ToString();
            MouseY.Text = position.Y.ToString();
        }
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (current_drawing != 3 && current_drawing != 5)
            {
                if (lastPosition == null)
                    lastPosition = new Point();
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
                    CreatePoint(x, y);
                    break;
                case 1:
                    CreateLine(x, y);
                    break;
                case 2:
                    CreateCircle(x, y);
                    break;
                case 3:
                    CreatePolygon(x, y);                  
                    break;
                case 4:
                    CreateCaplsule(x, y);
                    break;                
                case 5:
                    CreateRectangle(x, y);
                    break;
                case 6:
                    Edit(x,y);
                    break;
                case 7:
                    Delete(x, y);
                    break;
                case 8:
                    Clipping(x, y);
                    break;
                case 9:
                    ColorFilling(x, y);
                    break;                
                case 10:
                    PatternFilling(x, y);
                    break;
            }
        }
        private void Delete(int x, int y)
        {
            var del = MyBitmap.FindShape(MyBitmap.Shapes, x, y);
            if (del != null)
            {
                if(del.Equals(MyBitmap.ClippedShape) || del.Equals(MyBitmap.ClippingShape))
                {
                    MyBitmap.ClippedShape = null;
                    MyBitmap.ClippingShape = null; 
                }
                MyBitmap.Shapes.Remove(del);
                MyBitmap.Redraw();
            }
        }
        private void Edit(int x, int y)
        {
            var edit = MyBitmap.FindShape(MyBitmap.Shapes, x, y);
            var edit2 = MyBitmap.FindShape(MyBitmap.Shapes, lastPosition.X, lastPosition.Y);
            if (CurrentEditableShape != null && edit2 == CurrentEditableShape)
            {
                if (CurrentEditableShape is Point)
                {
                    var p = CurrentEditableShape as Point;
                    p.X = x;
                    p.Y = y;
                    MyBitmap.Redraw();
                }
                else if (CurrentEditableShape is Circle)
                {
                    var cir = CurrentEditableShape as Circle;
                    var d = MyBitmap.PointDistance(cir.OriginX, cir.OriginY, lastPosition.X, lastPosition.Y);
                    if (d < 10)
                    {
                        cir.OriginX = x;
                        cir.OriginY = y;
                        MyBitmap.Redraw();
                    }
                    else
                    {
                        var r = MyBitmap.PointDistance(cir.OriginX, cir.OriginY, x, y);
                        cir.Radius = (int)r;
                        MyBitmap.Redraw();
                    }
                }
                else if (CurrentEditableShape is Line)
                {
                    var l = CurrentEditableShape as Line;
                    var d1 = MyBitmap.PointDistance(l.X1, l.Y1, lastPosition.X, lastPosition.Y);
                    var d2 = MyBitmap.PointDistance(l.X2, l.Y2, lastPosition.X, lastPosition.Y);
                    if (d1 < 10)
                    {
                        l.X1 = x;
                        l.Y1 = y;
                        MyBitmap.Redraw();
                    }
                    else if (d2 < 10)
                    {
                        l.X2 = x;
                        l.Y2 = y;
                        MyBitmap.Redraw();
                    }
                }
                else if (CurrentEditableShape is Polygon)
                {
                    var pol = CurrentEditableShape as Polygon;
                    var center = pol.Center();
                    if (MyBitmap.PointDistance(center.Item1, center.Item2, lastPosition.X, lastPosition.Y) < 10)
                    {
                        var dx = lastPosition.X - x;
                        var dy = lastPosition.Y - y;
                        foreach (var vert in pol.Vertices)
                        {
                            vert.X -= dx;
                            vert.Y -= dy;
                        }
                        MyBitmap.Redraw();
                        return;
                    }
                    for (int i = 0; i < pol.Vertices.Count; i++)
                    {
                        var d = MyBitmap.PointDistance(pol.Vertices[i].X, pol.Vertices[i].Y, lastPosition.X, lastPosition.Y);
                        if (d < 10)
                        {
                            pol.Vertices[i].X = x;
                            pol.Vertices[i].Y = y;
                            MyBitmap.Redraw();
                            return;
                        }
                    }
                    var line = pol.WhichLine(lastPosition.X, lastPosition.Y);
                    if (line.Item1 != -1)
                    {
                        pol.Vertices[line.Item1].X -= lastPosition.X - x;
                        pol.Vertices[line.Item1].Y -= lastPosition.Y - y;
                        pol.Vertices[line.Item2].X -= lastPosition.X - x;
                        pol.Vertices[line.Item2].Y -= lastPosition.Y - y;
                        MyBitmap.Redraw();
                        return;
                    }
                } 
                else if (CurrentEditableShape is Rectangle)
                {
                    var rec = CurrentEditableShape as Rectangle;
                    var center = rec.Center();
                    if (MyBitmap.PointDistance(center.Item1, center.Item2, lastPosition.X, lastPosition.Y) < 10)
                    {
                        var dx = lastPosition.X - x;
                        var dy = lastPosition.Y - y;
                        foreach (var vert in rec.Vertices)
                        {
                            vert.X -= dx;
                            vert.Y -= dy;
                        }
                        MyBitmap.Redraw();
                        return;
                    }
                    for (int i = 0; i < rec.Vertices.Count; i++)
                    {
                        var d = MyBitmap.PointDistance(rec.Vertices[i].X, rec.Vertices[i].Y, lastPosition.X, lastPosition.Y);
                        if (d < 10)
                        {
                            rec.Vertices[i].X = x;
                            rec.Vertices[i].Y = y;
                            var tmp1 = (i - 1) % 4;
                            if (tmp1 == -1)
                                tmp1 = 3;
                            var tmp2 = (i + 1) % 4;
                            if ( i%2 == 0)
                            {
                                rec.Vertices[tmp1].Y = y;
                                rec.Vertices[tmp2].X = x;
                            }
                            else
                            {
                                rec.Vertices[tmp1].X = x;
                                rec.Vertices[tmp2].Y = y;
                            }                 
                            MyBitmap.Redraw();
                            return;
                        }
                    }
                    var line = rec.WhichLine(lastPosition.X, lastPosition.Y);
                    if (line.Item1 != -1)
                    {
                        if (line.Item1 % 2 == 0)
                        {
                            rec.Vertices[line.Item1].X -= lastPosition.X - x;
                            rec.Vertices[line.Item2].X -= lastPosition.X - x;
                        }
                        else
                        {
                            rec.Vertices[line.Item1].Y -= lastPosition.Y - y;
                            rec.Vertices[line.Item2].Y -= lastPosition.Y - y;
                        }
                        MyBitmap.Redraw();
                        return;
                    }
                }
                else if (CurrentEditableShape is Capsule)
                {
                    var cap = CurrentEditableShape as Capsule;
                    var d1 = MyBitmap.PointDistance(cap.X1, cap.Y1, lastPosition.X, lastPosition.Y);
                    var d2 = MyBitmap.PointDistance(cap.X2, cap.Y2, lastPosition.X, lastPosition.Y);
                    MyBitmap.DrawPoint(lastPosition.X, lastPosition.Y, 20);
                    if (d1 < 10)
                    {
                        cap.X1 = x;
                        cap.Y1 = y;
                        MyBitmap.Redraw();
                    }
                    else if (d2 < 10)
                    {
                        cap.X2 = x;
                        cap.Y2 = y;
                        MyBitmap.Redraw();
                    }
                    else
                    {
                        cap.X3 = x;
                        cap.Y3 = y;
                        MyBitmap.Redraw();
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
                    }
                }
                else
                {
                    CurrentEditableShape = edit;
                    CurrentEditableShape.EditModeStart();
                    MyBitmap.Redraw();
                }
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
        #endregion
        #region Methods Project IV
        private void CreateRectangle(int x, int y)
        {
            if (!firstClick)
            {
                if (firstPosition == null)
                    firstPosition = new Point();
                firstPosition.X = x;
                firstPosition.Y = y;
                if (thickness && ThicknessComboBox.SelectedItem != null)
                {
                    var brushSize = Int32.Parse(ThicknessComboBox.SelectedItem.ToString());
                    MyBitmap.DrawPoint(x, y, brushSize);
                    CurrentRectangle = new Rectangle(brushSize, MyBitmap.FirstColor, MyBitmap.SecondColor);
                    CurrentRectangle.Add(new Point(x, y));
                }
                else
                {
                    MyBitmap.DrawPoint(firstPosition.X, firstPosition.Y, 0);
                    CurrentRectangle = new Rectangle(MyBitmap.FirstColor, MyBitmap.SecondColor);
                    CurrentRectangle.Add(new Point(x, y));
                }
                firstClick = !firstClick;
            }
            else
            {
                CurrentRectangle.Add(new Point(firstPosition.X, y));
                CurrentRectangle.Add(new Point(x, y));
                CurrentRectangle.Add(new Point(x, firstPosition.Y));
                CurrentRectangle.Draw();
                MyBitmap.Shapes.Add(CurrentRectangle);
                CurrentRectangle = null;
                firstClick = !firstClick;
                firstPosition = null;
            }
        }
        private void Clipping_Click(object sender, RoutedEventArgs e)
        {
            current_drawing = 8;
            ButtonPoint.IsEnabled = true;
            ButtonLine.IsEnabled = true;
            ButtonCircle.IsEnabled = true;
            ButtonPolygon.IsEnabled = true;
            ButtonEdit.IsEnabled = true;
            ButtonDelete.IsEnabled = true;
            ButtonCapsule.IsEnabled = true;
            ButtonRectangle.IsEnabled = true;
            ButtonClipping.IsEnabled = false;
            ButtonFillColor.IsEnabled = true;
            if (CurrentEditableShape != null)
            {
                CurrentEditableShape.EditModeStop();
                CurrentEditableShape = null;
            }
            if (MyBitmap.ClippingShape != null)
            {              
                MyBitmap.ClippingShape.ClippingModeStop();
                MyBitmap.ClippingShape = null;
                MyBitmap.ClippedShape = null;
            }
        }
        private void FillColor_Click(object sender, RoutedEventArgs e)
        {
            current_drawing = 9;
            ButtonPoint.IsEnabled = true;
            ButtonLine.IsEnabled = true;
            ButtonCircle.IsEnabled = true;
            ButtonPolygon.IsEnabled = true;
            ButtonEdit.IsEnabled = true;
            ButtonDelete.IsEnabled = true;
            ButtonCapsule.IsEnabled = true;
            ButtonRectangle.IsEnabled = true;
            ButtonClipping.IsEnabled = true;
            ButtonFillColor.IsEnabled = false;
            ButtonFillPattern.IsEnabled = true;
            if (CurrentEditableShape != null)
            {
                CurrentEditableShape.EditModeStop();
                CurrentEditableShape = null;
            }
            if (MyBitmap.ClippingShape != null)
            {
                MyBitmap.ClippingShape.ClippingModeStop();
            }
        }
        private void ChoosePattern_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*bmp|" +
              "JPEG|*.jpg;*.jpeg|" +
              "Bitmap|*.bmp";
            if (op.ShowDialog() == true)
            {
                FillingPattern = new System.Drawing.Bitmap(op.FileName);            
            }
        }   
        private void FillPattern_Click(object sender, RoutedEventArgs e)
        {
            current_drawing = 10;
            ButtonPoint.IsEnabled = false;
            ButtonLine.IsEnabled = true;
            ButtonCircle.IsEnabled = true;
            ButtonPolygon.IsEnabled = true;
            ButtonEdit.IsEnabled = true;
            ButtonDelete.IsEnabled = true;
            ButtonCapsule.IsEnabled = true;
            ButtonRectangle.IsEnabled = true;
            ButtonClipping.IsEnabled = true;
            ButtonFillColor.IsEnabled = true;
            if (CurrentEditableShape != null)
            {
                CurrentEditableShape.EditModeStop();
                CurrentEditableShape = null;
            }
            if (MyBitmap.ClippingShape != null)
            {
                MyBitmap.ClippingShape.ClippingModeStop();
            }
        }
        private void Clipping(int x, int y)
        {
            var shape = MyBitmap.FindShape(MyBitmap.Shapes, x, y);
            if (MyBitmap.ClippingShape == null)
            {
                if (shape is Rectangle)
                {
                    MyBitmap.ClippingShape = shape as Rectangle;
                    MyBitmap.ClippingShape.ClippingModeStart();
                }
            }
            else
            {
                if (!shape.Equals(MyBitmap.ClippingShape))
                {
                    MyBitmap.ClippedShape = shape;
                    MyBitmap.ClippingShape.ClippingModeStop();
                    ButtonClipping.IsEnabled = true;
                }
            }
        }
        private void ColorFilling(int x, int y)
        {
            var shape = MyBitmap.FindShape(MyBitmap.Shapes, x, y);
            if ( shape is Rectangle)
            {
                var rect = shape as Rectangle;
                rect.FillColor = FillingColor;
                rect.FillPattern = null;
                rect.Draw();
            }
            else if (shape is Polygon)
            {
                var poly = shape as Polygon;
                poly.FillColor = FillingColor;
                poly.FillPattern = null;
                poly.Draw();
            }
        } 
        private void PatternFilling(int x, int y)
        {
            if (FillingPattern == null)
                MessageBox.Show("Firstly choose pattern");
            else
            {
                var shape = MyBitmap.FindShape(MyBitmap.Shapes, x, y);
                if (shape is Rectangle)
                {
                    var rect = shape as Rectangle;
                    rect.FillColor = null;
                    rect.FillPattern = FillingPattern;
                    rect.Draw();
                }
                else if (shape is Polygon)
                {
                    var poly = shape as Polygon;
                    poly.FillColor = null;
                    poly.FillPattern = FillingPattern;
                    poly.Draw();
                }
            }
        }     
        private void CheckBox_Checked_Clipping(object sender, RoutedEventArgs e)
        {
            MyBitmap.clipping = true;
            MyBitmap.Redraw();
        }
        private void CheckBox_Unchecked_Clipping(object sender, RoutedEventArgs e)
        {
            MyBitmap.clipping = false;
            MyBitmap.Redraw();
        }
        private void Selected_ClippingColor(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            if (clippingColor.SelectedColor.HasValue)
            {
                var color = new Color(clippingColor.SelectedColor.Value);
                MyBitmap.ClippingColor = color;
                MyBitmap.Redraw();
            }
        }
        private void Selected_FillingColor(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            if (fillingColor.SelectedColor.HasValue)
            {
                var color = new Color(fillingColor.SelectedColor.Value);
                FillingColor = color;
            }
        }
        #endregion
    }
}