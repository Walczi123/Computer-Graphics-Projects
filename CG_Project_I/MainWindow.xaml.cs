using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.Drawing;
using System.Windows.Interop;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace CG_Project_I
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FunctionalFilters functionalFilters = new FunctionalFilters();
        private ConvolutionFilters convolutionFilters = new ConvolutionFilters();
        private int brightnessStep = 50;
        private double contrastStep = 1.3;
        private double gammaStep = 1.3;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void loadImage(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png;*bmp|" +
              "JPEG|*.jpg;*.jpeg|" +
              "PNG|*.png|"+
              "Bitmap|*.bmp";
            if (op.ShowDialog() == true)
            {
                orginalImage.Source = new BitmapImage(new Uri(op.FileName));
                resultImage.Source = new BitmapImage(new Uri(op.FileName));
            }
        }

        private void saveImage(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "JPEG Image|*.jpg|PNG Image|*.png |Bitmap|*.bmp";
            saveFileDialog1.Title = "Save an image";
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {
                System.IO.FileStream fs =
                    (System.IO.FileStream)saveFileDialog1.OpenFile();
                BitmapEncoder encoder = null;
                switch (saveFileDialog1.FilterIndex)
                {
                    case 1:
                        encoder = new JpegBitmapEncoder();
                        break;
                    case 2:
                        encoder = new PngBitmapEncoder();
                        break;
                    case 3:
                        encoder = new BmpBitmapEncoder();
                        break;
                }
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)this.resultImage.Source));
                encoder.Save(fs);
                fs.Close();
            }
        }

        private void resetImage(object sender, RoutedEventArgs e)
        {
            resultImage.Source = orginalImage.Source.Clone();
        }

        private void shutdown(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void InversionButtonClick(object sender, RoutedEventArgs e)
        {
            if (resultImage.Source != null)
            {
                System.Drawing.Bitmap tmp = this.convertToBitmap(resultImage.Source);
                this.functionalFilters.inversion(tmp);
                resultImage.Source = (ImageSource)this.ImageSourceFromBitmap(tmp);
            }
            else
                MessageBox.Show("Please load an image","No Image loaded");
        }

        private void BrightnessButtonClick(object sender, RoutedEventArgs e)
        {
            if (resultImage.Source != null)
            {
                System.Drawing.Bitmap tmp = this.convertToBitmap(resultImage.Source);
                this.functionalFilters.brightness(tmp, brightnessStep);
                resultImage.Source = (ImageSource)this.ImageSourceFromBitmap(tmp);
            }
            else
                MessageBox.Show("Please load an image", "No Image loaded");
        }

        private void ContrastButtonClick(object sender, RoutedEventArgs e)
        {
            if (resultImage.Source != null)
            {
                System.Drawing.Bitmap tmp = this.convertToBitmap(resultImage.Source);
                this.functionalFilters.contrast(tmp, contrastStep);
                resultImage.Source = (ImageSource)this.ImageSourceFromBitmap(tmp);
            }
            else
                MessageBox.Show("Please load an image", "No Image loaded");
        }

        private void GammaButtonClick(object sender, RoutedEventArgs e)
        {
            if (resultImage.Source != null)
            {
                System.Drawing.Bitmap tmp = this.convertToBitmap(resultImage.Source);
                this.functionalFilters.gamma(tmp, gammaStep);
                resultImage.Source = (ImageSource)this.ImageSourceFromBitmap(tmp);
            }
            else
                MessageBox.Show("Please load an image", "No Image loaded");
        }

        private void BlurButtonClick(object sender, RoutedEventArgs e)
        {
            if (resultImage.Source != null)
            {
                System.Drawing.Bitmap tmp = this.convertToBitmap(resultImage.Source);
                this.convolutionFilters.blur(tmp,3);
                resultImage.Source = (ImageSource)this.ImageSourceFromBitmap(tmp);
                MessageBox.Show("Done", "blurred");
            }
            else
                MessageBox.Show("Please load an image", "No Image loaded");
        }

        private System.Drawing.Bitmap convertToBitmap(ImageSource bitmapSource)
        {
            System.Drawing.Bitmap bitmap;
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create((BitmapSource)bitmapSource));
                enc.Save(outStream);
                bitmap = new System.Drawing.Bitmap(outStream);
            }
            return bitmap;
        }

        //https://stackoverflow.com/questions/26260654/wpf-converting-bitmap-to-imagesource
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        public ImageSource ImageSourceFromBitmap(System.Drawing.Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObject(handle); }
        }
    }
}
