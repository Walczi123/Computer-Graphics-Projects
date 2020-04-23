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
            ThicknessComboBox.ItemsSource= Enumerable.Range(2, 49).Select(i => (object)i).ToArray();
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
            if(current_drawing != 3)
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
                        MyBitmap.DrawPoint(x, y, brushSize);
                        shapes.Add(new Point(x, y, brushSize, MyBitmap.FirstColor));
                    }
                    else
                    {
                        MyBitmap.DrawPoint(x, y, 0);
                        shapes.Add(new Point(x, y, MyBitmap.FirstColor));
                    }
                    break;
                case 1:
                    if (thickness && ThicknessComboBox.SelectedItem != null)
                    {
                        var brushSize = Int32.Parse(ThicknessComboBox.SelectedItem.ToString());
                        MyBitmap.DrawLine(lastPosition.X, lastPosition.Y, x, y, brushSize);
                        shapes.Add(new Line(lastPosition.X, lastPosition.Y, x, y, brushSize, MyBitmap.FirstColor, MyBitmap.SecondColor));
                    }
                    else
                    {
                        shapes.Add(new Line(lastPosition.X, lastPosition.Y, x, y, MyBitmap.FirstColor, MyBitmap.SecondColor));
                        MyBitmap.DrawLine(lastPosition.X, lastPosition.Y, x, y, 0);
                    }                                        
                    break;
                case 2:
                    int R = (int)Math.Sqrt(Math.Pow(x - lastPosition.X, 2) + Math.Pow(y - lastPosition.Y, 2));
                    MyBitmap.DrawCircle(lastPosition.X, lastPosition.Y, R);
                    shapes.Add(new Circle(lastPosition.X, lastPosition.Y, R, MyBitmap.FirstColor, MyBitmap.SecondColor));
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
                            CurrentPolygon =  new Polygon(brushSize, MyBitmap.FirstColor, MyBitmap.SecondColor);
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
                        var d = Math.Sqrt(Math.Pow(x - firstPosition.X, 2) + Math.Pow(y - firstPosition.Y,2));
                        if (d < 10)
                        {
                            MyBitmap.DrawLine(lastPosition.X, lastPosition.Y, firstPosition.X, firstPosition.Y, CurrentPolygon.BrushSize);
                            this.shapes.Add(CurrentPolygon);
                            CurrentPolygon = null;
                            firstClick = !firstClick;
                        }
                        else
                        {
                            MyBitmap.DrawLine(lastPosition.X, lastPosition.Y, x, y, CurrentPolygon.BrushSize);
                            lastPosition.X = x;
                            lastPosition.Y = y;
                            CurrentPolygon.Add(new Point(x, y));
                        }
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
        }
        private void Line_Click(object sender, RoutedEventArgs e)
        {
            current_drawing = 1;
            ButtonPoint.IsEnabled = true;
            ButtonLine.IsEnabled = false;
            ButtonCircle.IsEnabled = true;
            ButtonPolygon.IsEnabled = true;
        }        
        private void Circle_Click(object sender, RoutedEventArgs e)
        {
            current_drawing = 2;
            ButtonPoint.IsEnabled = true;
            ButtonLine.IsEnabled = true;
            ButtonCircle.IsEnabled = false;
            ButtonPolygon.IsEnabled = true;
        }        
        private void Polygon_Click(object sender, RoutedEventArgs e)
        {
            current_drawing = 3;
            ButtonPoint.IsEnabled = true;
            ButtonLine.IsEnabled = true;
            ButtonCircle.IsEnabled = true;
            ButtonPolygon.IsEnabled = false;
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
        }

        private void Selected_Color2(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            if (cp2.SelectedColor.HasValue)
            {
                MyBitmap.SecondColor = new Color(cp2.SelectedColor.Value);
            }
        }

        private void OnlyNumbersValidationTextBox(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[0-9]$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void CheckBox_Checked_Thickness(object sender, RoutedEventArgs e)
        {
            thickness = true;
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
            FileStream fs = new FileStream("DataFile.dat", FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(fs, shapes);
            }
            catch (SerializationException error)
            {
                Console.WriteLine("Failed to serialize. Reason: " + error.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            FileStream fs = new FileStream("DataFile.dat", FileMode.Open);
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                shapes = (List<IShape>)formatter.Deserialize(fs);
            }
            catch (SerializationException error)
            {
                Console.WriteLine("Failed to deserialize. Reason: " + error.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }
            MyBitmap.Redraw(shapes);
        }
    }

    //public static class RewritableBitmap
    //{
    //    public static WriteableBitmap ResizeWritableBitmap(this WriteableBitmap wBitmap, int reqWidth, int reqHeight)
    //    {
    //        int Stride = wBitmap.PixelWidth * ((wBitmap.Format.BitsPerPixel + 7) / 8);
    //        int NumPixels = Stride * wBitmap.PixelHeight;
    //        ushort[] ArrayOfPixels = new ushort[NumPixels];


    //        wBitmap.CopyPixels(ArrayOfPixels, Stride, 0);

    //        int OriWidth = (int)wBitmap.PixelWidth;
    //        int OriHeight = (int)wBitmap.PixelHeight;

    //        double nXFactor = (double)OriWidth / (double)reqWidth;
    //        double nYFactor = (double)OriHeight / (double)reqHeight;

    //        double fraction_x, fraction_y, one_minus_x, one_minus_y;
    //        int ceil_x, ceil_y, floor_x, floor_y;

    //        ushort pix1, pix2, pix3, pix4;
    //        int nStride = reqWidth * ((wBitmap.Format.BitsPerPixel + 7) / 8);
    //        int nNumPixels = reqWidth * reqHeight;
    //        ushort[] newArrayOfPixels = new ushort[nNumPixels];
    //        /*Core Part*/
    //        /* Code project article :
    //Image Processing for Dummies with C# and GDI+ Part 2 - Convolution Filters By Christian Graus</a>

    //        href=<a href="http://www.codeproject.com/KB/GDI-plus/csharpfilters.aspx"></a>
    //        */
    //        for (int y = 0; y < reqHeight; y++)
    //        {
    //            for (int x = 0; x < reqWidth; x++)
    //            {
    //                // Setup
    //                floor_x = (int)Math.Floor(x * nXFactor);
    //                floor_y = (int)Math.Floor(y * nYFactor);

    //                ceil_x = floor_x + 1;
    //                if (ceil_x >= OriWidth) ceil_x = floor_x;

    //                ceil_y = floor_y + 1;
    //                if (ceil_y >= OriHeight) ceil_y = floor_y;

    //                fraction_x = x * nXFactor - floor_x;
    //                fraction_y = y * nYFactor - floor_y;

    //                one_minus_x = 1.0 - fraction_x;
    //                one_minus_y = 1.0 - fraction_y;

    //                pix1 = ArrayOfPixels[floor_x + floor_y * OriWidth];
    //                pix2 = ArrayOfPixels[ceil_x + floor_y * OriWidth];
    //                pix3 = ArrayOfPixels[floor_x + ceil_y * OriWidth];
    //                pix4 = ArrayOfPixels[ceil_x + ceil_y * OriWidth];

    //                ushort g1 = (ushort)(one_minus_x * pix1 + fraction_x * pix2);
    //                ushort g2 = (ushort)(one_minus_x * pix3 + fraction_x * pix4);
    //                ushort g = (ushort)(one_minus_y * (double)(g1) + fraction_y * (double)(g2));
    //                newArrayOfPixels[y * reqWidth + x] = g;
    //            }
    //        }
    //        /*End of Core Part*/
    //        WriteableBitmap newWBitmap = new WriteableBitmap(reqWidth, reqHeight, 96, 96, PixelFormats.Gray16, null);
    //        Int32Rect Imagerect = new Int32Rect(0, 0, reqWidth, reqHeight);
    //        int newStride = reqWidth * ((PixelFormats.Gray16.BitsPerPixel + 7) / 8);
    //        newWBitmap.WritePixels(Imagerect, newArrayOfPixels, newStride, 0);
    //        return newWBitmap;
    //    }
    //}
}