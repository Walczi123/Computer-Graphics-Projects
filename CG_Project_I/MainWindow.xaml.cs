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
using System.Text.RegularExpressions;

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
        private double gammaStep = 1.2;

        private int[,] kernelBlur = {{ 1, 1, 1 },
                                     { 1, 1, 1 },
                                     { 1, 1, 1 }};
        private int[,] kernelGaussianBlur = {{ 0, 1, 0 },
                                             { 1, 4, 1 },
                                             { 0, 1, 0 }};
        private int[,] kernelSharpen = {{  0, -1,  0 },
                                        { -1,  5, -1 },
                                        {  0, -1,  0 }};
        private int[,] kernelEdgeDetection = {{ 0, -1, 0 },
                                              { 0,  1, 0 },
                                              { 0,  0, 0 }};
        private int[,] kernelEmboss = {{ -1, 0, 1 },
                                       { -1, 1, 1 },
                                       { -1, 0, 1 }};

        public MainWindow()
        {
            InitializeComponent();
            SetGrid(kernelBlur);
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
                this.convolutionFilters.ConvolutionFunction(tmp, kernelBlur);
                resultImage.Source = (ImageSource)this.ImageSourceFromBitmap(tmp);
            }
            else
                MessageBox.Show("Please load an image", "No Image loaded");
        }

        private void GaussianBlurButtonClick(object sender, RoutedEventArgs e)
        {
            if (resultImage.Source != null)
            {
                System.Drawing.Bitmap tmp = this.convertToBitmap(resultImage.Source);
                this.convolutionFilters.ConvolutionFunction(tmp, kernelGaussianBlur);
                resultImage.Source = (ImageSource)this.ImageSourceFromBitmap(tmp);
            }
            else
                MessageBox.Show("Please load an image", "No Image loaded");
        }

        private void SharpenButtonClick(object sender, RoutedEventArgs e)
        {
            if (resultImage.Source != null)
            {
                System.Drawing.Bitmap tmp = this.convertToBitmap(resultImage.Source);
                this.convolutionFilters.ConvolutionFunction(tmp, kernelSharpen, 1);
                resultImage.Source = (ImageSource)this.ImageSourceFromBitmap(tmp);
            }
            else
                MessageBox.Show("Please load an image", "No Image loaded");
        }

        private void EdgeDetectionButtonClick(object sender, RoutedEventArgs e)
        {
            if (resultImage.Source != null)
            {
                System.Drawing.Bitmap tmp = this.convertToBitmap(resultImage.Source);
                this.convolutionFilters.ConvolutionFunction(tmp, kernelEdgeDetection, 1);
                resultImage.Source = (ImageSource)this.ImageSourceFromBitmap(tmp);
            }
            else
                MessageBox.Show("Please load an image", "No Image loaded");
        }

        private void EmbossButtonClick(object sender, RoutedEventArgs e)
        {
            if (resultImage.Source != null)
            {
                System.Drawing.Bitmap tmp = this.convertToBitmap(resultImage.Source);
                this.convolutionFilters.ConvolutionFunction(tmp, kernelEmboss,1);
                resultImage.Source = (ImageSource)this.ImageSourceFromBitmap(tmp);
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

        private void ChangeGrid(int col, int row)
        {
            //for(int i = 0; i< col; i++)
            //{
            //    GridKernel.RowDefinitions.Add(new RowDefinition());
            //    GridKernel.ColumnDefinitions.Add(new ColumnDefinition());
            //}

            if (this.GridKernel.ColumnDefinitions.Count != col)
            {
                int diffCol = col - GridKernel.ColumnDefinitions.Count;
                if (diffCol < 0)
                {
                    GridKernel.ColumnDefinitions.RemoveRange(GridKernel.ColumnDefinitions.Count + diffCol, -diffCol);
                }
                else
                {
                    for (int i = 0; i < diffCol; i++)
                    {
                        GridKernel.ColumnDefinitions.Add(new ColumnDefinition());
                    }
                }
            }
            if (GridKernel.RowDefinitions.Count != row)
            {
                int diffRow = row - GridKernel.RowDefinitions.Count;
                if (diffRow < 0)
                {
                    GridKernel.RowDefinitions.RemoveRange(GridKernel.RowDefinitions.Count + diffRow, -diffRow);
                }
                else
                {
                    for (int i = 0; i < diffRow; i++)
                        GridKernel.RowDefinitions.Add(new RowDefinition());
                }
            }
        }

        private void SetGrid(int[,] kernel)
        {
            GridKernel.Children.Clear();
            int diff = GridKernel.ColumnDefinitions.Count - kernel.GetLength(0);
            diff /= 2;
            for (int i = 0; i < GridKernel.ColumnDefinitions.Count; i++)
            {
                for (int j = 0; j < GridKernel.RowDefinitions.Count; j++)
                {
                    var elem = new TextBox();
                    if(i >= diff && j >= diff && 
                        i < GridKernel.ColumnDefinitions.Count-diff && j< GridKernel.ColumnDefinitions.Count - diff)
                    {
                        elem.Text = kernel[i - diff, j - diff].ToString();
                    }
                    else
                    {
                        elem.Text = "0";
                    }
                    
                    elem.HorizontalContentAlignment = HorizontalAlignment.Center;
                    elem.VerticalContentAlignment = VerticalAlignment.Center;
                    //elem.PreviewTextInput = new PreviewTextInput(TextBoxPasting);
                    //elem.InputBindings.Add(TextBoxPasting);
                    Grid.SetRow(elem, i);
                    Grid.SetColumn(elem, j);
                    GridKernel.Children.Add(elem);
                }
            }
        }

        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void TextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private int[,] readGrid()
        {
            int size = GridKernel.ColumnDefinitions.Count, i = 0, j=0;
            int[,] kernel = new int[size, size];
            foreach (TextBox c in GridKernel.Children)
            {
                kernel[i++, j] = Int32.Parse(c.Text);
                if (i >= size)
                {
                    i = 0; j++;
                }
            }
            return kernel;
        }

        private void KernelSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(GridKernel != null)
            {
                var kernel = readGrid();
                this.ChangeGrid((int)KernelSizeSlider.Value, (int)KernelSizeSlider.Value);
                this.SetGrid(kernel);
            }
        }

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

        private void ComputeDivisor_Click(object sender, RoutedEventArgs e)
        {
            int sum = 0;
            foreach (TextBox c in GridKernel.Children)
            {
                sum += Int32.Parse(c.Text);
            }
            if (sum == 0) sum = 1;
            Divisor.Text = sum.ToString();
        }
    }
}
