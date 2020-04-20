using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CG_Project_III
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WriteableBitmap bitmap = new WriteableBitmap(100, 100, 96, 96, PixelFormats.Bgr32, null);
        private int current_drawing = 0;
        private Point lastPosition = new Point();
        private Algorithms algorithms = new Algorithms();
        public MainWindow()
        {
            InitializeComponent();
            image.Source = bitmap;
           
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            this.bitmap = new WriteableBitmap((int)image.ActualWidth + 1, (int)image.ActualHeight + 1, 96, 96, PixelFormats.Bgr32, null);
            MyBitmap.CleanDrawArea(bitmap);
            image.Source = bitmap;
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if ((int)image.ActualWidth > 0 && (int)image.ActualHeight > 0)
            {
                this.bitmap = new WriteableBitmap((int)image.ActualWidth + 1, (int)image.ActualHeight + 1, 96, 96, PixelFormats.Bgr32, null);
                //    this.bitmap = RewritableBitmap.ResizeWritableBitmap(bitmap, (int)image.ActualWidth, (int)image.ActualHeight);
                MyBitmap.CleanDrawArea(bitmap);
                image.Source = bitmap;
            }
        }

        private void menu_shutdown(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void menu_clear(object sender, RoutedEventArgs e)
        {
            MyBitmap.CleanDrawArea(bitmap);
        }     
        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            var position = Mouse.GetPosition(image);
            MouseX.Text = position.X.ToString();
            MouseY.Text = position.Y.ToString();
        }
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            lastPosition.X = e.GetPosition(image).X;
            lastPosition.Y = e.GetPosition(image).Y;          
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            int x = (int)e.GetPosition(image).X;
            int y = (int)e.GetPosition(image).Y;
            switch (current_drawing)
            {
                case 0:
                    MyBitmap.DrawPixel(bitmap, x, y);
                    break;
                case 1:
                    this.algorithms.lineDDA((int)lastPosition.X, (int)lastPosition.Y, x, y, bitmap);
                    break;
                case 2: break;
            }
        }

        private void Point_Click(object sender, RoutedEventArgs e)
        {
            current_drawing = 0;
        }
        private void Line_Click(object sender, RoutedEventArgs e)
        {
            current_drawing = 1;
        }        
        private void Circle_Click(object sender, RoutedEventArgs e)
        {
            current_drawing = 2;
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