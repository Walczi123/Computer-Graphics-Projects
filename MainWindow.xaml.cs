using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace CG_Project_II
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private Fields
        #region Project I
        private FunctionalFilters functionalFilters = new FunctionalFilters();
        private ConvolutionFilters convolutionFilters = new ConvolutionFilters();
        private int brightnessStep = 50;
        private double contrastStep = 1.3;
        private double gammaStep = 1.2;
        private string text;
        private Tuple<int, int> anchor = new Tuple<int, int>(1, 1);
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
        private int[,] kernelCustom = {{ 1, 1, 1 },
                                       { 1, 1, 1 },
                                       { 1, 1, 1 }};
        #endregion
        #region Project II
        private Dithering dithering = new Dithering();
        private Quantization quantization = new Quantization();
        #endregion
        #endregion
        public MainWindow()
        {
            InitializeComponent();
            #region Project I
            SetGrid(kernelCustom);
            setAnchor();
            #endregion
        }
        #region Private Methods
        #region Common Methods
        private void loadImage(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png;*bmp|" +
              "JPEG|*.jpg;*.jpeg|" +
              "PNG|*.png|" +
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
        #endregion
        #region Project I Methods
        private void InversionButtonClick(object sender, RoutedEventArgs e)
        {
            if (resultImage.Source != null)
            {
                System.Drawing.Bitmap tmp = this.convertToBitmap(resultImage.Source);
                this.functionalFilters.inversion(tmp);
                resultImage.Source = (ImageSource)this.ImageSourceFromBitmap(tmp);
            }
            else
                MessageBox.Show("Please load an image", "No Image loaded");
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
                double step = gammaStep;
                if (GammaValue.Text.ToString() != "")
                {
                    if(!Double.TryParse(GammaValue.Text.ToString(), out step)) { step = gammaStep; }
                }
                System.Drawing.Bitmap tmp = this.convertToBitmap(resultImage.Source);
                this.functionalFilters.gamma(tmp, step);
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
                this.convolutionFilters.ConvolutionFunction(tmp, kernelEmboss, 1);
                resultImage.Source = (ImageSource)this.ImageSourceFromBitmap(tmp);
            }
            else
                MessageBox.Show("Please load an image", "No Image loaded");
        }
        private void ChangeGrid(int col, int row)
        {
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
            setAnchor();
        }
        private void SetGrid(int[,] kernel)
        {
            if (GridKernel == null) return;
            GridKernel.Children.Clear();
            int diff = GridKernel.ColumnDefinitions.Count - kernel.GetLength(0);
            diff /= 2;
            for (int i = 0; i < GridKernel.ColumnDefinitions.Count; i++)
            {
                for (int j = 0; j < GridKernel.RowDefinitions.Count; j++)
                {
                    var elem = new TextBox();
                    if (i >= diff && j >= diff &&
                        i < GridKernel.ColumnDefinitions.Count - diff && j < GridKernel.ColumnDefinitions.Count - diff)
                    {
                        elem.Text = kernel[i - diff, j - diff].ToString();
                    }
                    else
                    {
                        elem.Text = "0";
                    }

                    elem.HorizontalContentAlignment = HorizontalAlignment.Center;
                    elem.VerticalContentAlignment = VerticalAlignment.Center;
                    elem.TextChanged += (TextChangedEventHandler)TextBoxPasting;
                    elem.PreviewTextInput += TextBefore;
                    Grid.SetRow(elem, i);
                    Grid.SetColumn(elem, j);
                    GridKernel.Children.Add(elem);
                }
            }
            setAnchor();
        }
        private static readonly Regex _regex = new Regex("[^0-9.-]+");
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }
        private void TextBoxPasting(object sender, TextChangedEventArgs e)
        {
            TextBox t = (TextBox)(sender as TextBox);
            String[] str = sender.ToString().Split(' ');
            if (str.Length > 1)
            {
                if (!IsTextAllowed(sender.ToString().Split(' ')[1]))
                {
                    t.Text = this.text;
                }
            }
            else
            {
                t.Text = "1";
            }
        }
        private void TextBefore(object sender, EventArgs e)
        {
            String[] str = sender.ToString().Split(' ');
            if (str.Length > 1)
            {
                this.text = sender.ToString().Split(' ')[1];
            }
            else
            {
                this.text = "";
            }

        }
        private int[,] readGrid()
        {
            int size = GridKernel.ColumnDefinitions.Count, i = 0, j = 0;
            int[,] kernel = new int[size, size];
            foreach (TextBox c in GridKernel.Children)
            {
                kernel[j, i++] = Int32.Parse(c.Text);
                if (i >= size)
                {
                    i = 0; j++;
                }
            }
            return kernel;
        }
        private void KernelSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (GridKernel != null)
            {
                var kernel = readGrid();
                this.ChangeGrid((int)KernelSizeSlider.Value, (int)KernelSizeSlider.Value);
                this.SetGrid(kernel);
            }
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
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem cbi = (ComboBoxItem)(sender as ComboBox).SelectedItem;
            switch (cbi.Content.ToString())
            {
                case "Blur":
                    SetGrid(kernelBlur);
                    break;
                case "Gaussian Blur":
                    SetGrid(kernelGaussianBlur);
                    break;
                case "Sharpen":
                    SetGrid(kernelSharpen);
                    break;
                case "Edge Detection":
                    SetGrid(kernelEdgeDetection);
                    break;
                case "Emboss":
                    SetGrid(kernelEmboss);
                    break;
                case "Custom":
                    SetGrid(kernelCustom);
                    break;
            }
        }
        private void SaveKernelButton_Click(object sender, RoutedEventArgs e)
        {

            switch (ComboBox.SelectedIndex.ToString())
            {
                case "0":
                    kernelCustom = readGrid();
                    SetGrid(kernelCustom);
                    break;
                case "1":
                    kernelBlur = readGrid();
                    SetGrid(kernelBlur);
                    break;
                case "2":
                    kernelGaussianBlur = readGrid();
                    SetGrid(kernelGaussianBlur);
                    break;
                case "3":
                    kernelSharpen = readGrid();
                    SetGrid(kernelSharpen);
                    break;
                case "4":
                    kernelEdgeDetection = readGrid();
                    SetGrid(kernelEdgeDetection);
                    break;
                case "5":
                    kernelEmboss = readGrid();
                    SetGrid(kernelEmboss);
                    break;
            }
        }
        private void AnchorX_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox t = (TextBox)(sender as TextBox);
            String[] str = sender.ToString().Split(' ');
            if (str.Length > 1)
            {
                if (!IsTextAllowed(sender.ToString().Split(' ')[1]))
                {
                    t.Text = this.text;
                }
            }
            else
            {
                t.Text = "1";
            }
            setAnchor();
        }
        private void AnchorY_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox t = (TextBox)(sender as TextBox);
            String[] str = sender.ToString().Split(' ');
            if (str.Length > 1)
            {
                if (!IsTextAllowed(sender.ToString().Split(' ')[1]))
                {
                    t.Text = this.text;
                }
            }
            else
            {
                t.Text = "1";
            }
            setAnchor();
        }
        private bool setAnchor()
        {
            if (AnchorX != null && AnchorY != null && GridKernel != null
                && AnchorX.Text.ToString() != "" && AnchorY.Text.ToString() != "")
            {
                int x = Int32.Parse(AnchorX.Text.ToString());
                int y = Int32.Parse(AnchorY.Text.ToString());
                int col = GridKernel.ColumnDefinitions.Count;
                if (x < col && y < col)
                {
                    foreach (TextBox elem in GridKernel.Children)
                    {
                        elem.Background = System.Windows.Media.Brushes.White;
                    }
                    x *= GridKernel.ColumnDefinitions.Count;
                    TextBox t = (TextBox)(GridKernel.Children[x + y] as TextBox);
                    t.Background = System.Windows.Media.Brushes.Red;
                    return true;
                }
            }
            return false;
        }
        private void RunKernelButton_Click(object sender, RoutedEventArgs e)
        {
            if (resultImage.Source != null)
            {
                var ker = readGrid();
                int divisor = 1;
                int offset = 0;
                int anchorX = 0, anchorY = 0;
                if (Divisor.Text.ToString() != "")
                    divisor = Int32.Parse(Divisor.Text.ToString());
                if (Offset.Text.ToString() != "")
                    offset = Int32.Parse(Offset.Text.ToString());
                if (AnchorX.Text.ToString() != "")
                    anchorX = Int32.Parse(AnchorX.Text.ToString());
                if (AnchorY.Text.ToString() != "")
                    anchorY = Int32.Parse(AnchorY.Text.ToString());
                System.Drawing.Bitmap tmp = this.convertToBitmap(resultImage.Source);
                this.convolutionFilters.ConvolutionFunction(tmp, ker, divisor, offset, anchorX, anchorY);
                resultImage.Source = (ImageSource)this.ImageSourceFromBitmap(tmp);
            }
            else
                MessageBox.Show("Please load an image", "No Image loaded");
        }
        private void NumberValidationTextBox(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[a-z]");
            double result;
            if (!double.TryParse(sender.ToString(), out result))
            {
                e.Handled = regex.IsMatch(e.Text);
            }
        }
        #endregion

        #endregion

        private void selectP1(object sender, RoutedEventArgs e)
        {
            secondColumn.Width = new GridLength(150, GridUnitType.Pixel);
            thirdColumn.Width = new GridLength(0, GridUnitType.Pixel);
            secondRow.Height = new GridLength(5, GridUnitType.Pixel);
            thirdRow.MinHeight = 300;
            thirdRow.Height = new GridLength(1, GridUnitType.Star);
            Application.Current.MainWindow.MinHeight = 750;
            Application.Current.MainWindow.Height = 600;
        }

        private void selectP2(object sender, RoutedEventArgs e)
        {
            secondColumn.Width = new GridLength(0, GridUnitType.Pixel);
            thirdColumn.Width = new GridLength(150, GridUnitType.Pixel);
            secondRow.Height = new GridLength(0, GridUnitType.Pixel);
            thirdRow.MinHeight = 0;
            thirdRow.Height = new GridLength(0, GridUnitType.Pixel);
            Application.Current.MainWindow.MinHeight = 450;
            Application.Current.MainWindow.Height = 450;          
        }

        private void OnlyNumbersValidationTextBox(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[0-9]*$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void ColorDitheringClick(object sender, RoutedEventArgs e)
        {
            if (resultImage.Source != null)
            {
                int valuePerRed=2;
                if (ValuePerRed.Text.ToString() != "")
                {
                    if (!Int32.TryParse(ValuePerRed.Text.ToString(), out valuePerRed)) { valuePerRed = 2; }
                }
                int valuePerGreen= 2;
                if (ValuePerGreen.Text.ToString() != "")
                {
                    if (!Int32.TryParse(ValuePerGreen.Text.ToString(), out valuePerGreen)) { valuePerGreen = 2; }
                }
                int valuePerBlue = 2;
                if (ValuePerBlue.Text.ToString() != "")
                {
                    if (!Int32.TryParse(ValuePerBlue.Text.ToString(), out valuePerBlue)) { valuePerBlue = 2; }
                }
                System.Drawing.Bitmap tmp = this.convertToBitmap(resultImage.Source);
                this.dithering.AverageColor(tmp, valuePerRed, valuePerGreen, valuePerBlue);
                resultImage.Source = (ImageSource)this.ImageSourceFromBitmap(tmp);
            }
            else
                MessageBox.Show("Please load an image", "No Image loaded");
        }

        private void BWDitheringClick(object sender, RoutedEventArgs e)
        {
            if (resultImage.Source != null)
            {
                int valuePerChannel = 2;
                if (ValuePerChannel.Text.ToString() != "")
                {
                    if (!Int32.TryParse(ValuePerChannel.Text.ToString(), out valuePerChannel)) { valuePerChannel = 2; }
                }
                System.Drawing.Bitmap tmp = this.convertToBitmap(resultImage.Source);
                this.dithering.AverageBW(tmp, valuePerChannel);
                resultImage.Source = (ImageSource)this.ImageSourceFromBitmap(tmp);
            }
            else
                MessageBox.Show("Please load an image", "No Image loaded");
        }

        private void ConvertToBW(object sender, RoutedEventArgs e)
        {
            if (resultImage.Source != null)
            {              
                System.Drawing.Bitmap tmp = this.convertToBitmap(resultImage.Source);
                this.dithering.ToGrayScale(tmp);
                resultImage.Source = (ImageSource)this.ImageSourceFromBitmap(tmp);
            }
            else
                MessageBox.Show("Please load an image", "No Image loaded");
        }

        private void MedianCutClick(object sender, RoutedEventArgs e)
        {
            if (resultImage.Source != null)
            {
                int numberOfColors = Int32.Parse(ComboCoxMedianCut.SelectedItem.ToString().Split(' ')[1]);
                System.Drawing.Bitmap tmp = this.convertToBitmap(resultImage.Source);
                this.quantization.MedianCut(tmp, numberOfColors);
                resultImage.Source = (ImageSource)this.ImageSourceFromBitmap(tmp);
            }
            else
                MessageBox.Show("Please load an image", "No Image loaded");
        }
    }
}
