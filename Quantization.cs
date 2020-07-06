using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace CG_Project_II
{
    class Quantization
    {
        public Quantization() { }

        private void DivideByMedian(List<Dictionary<(int, int, int), List<PointerToColors>>> allBuckets)
        {
            Dictionary<(int, int, int), List<PointerToColors>> tmpBucket;
            int median;
            int rangeRed, rangeGreen, rangeBlue;
            int countOfList = allBuckets.Count;
            for (int i = 0; i < countOfList; i++)
            {
                tmpBucket = allBuckets[i];
                rangeRed = tmpBucket.Max(color => color.Key.Item1) - tmpBucket.Min(color => color.Key.Item1);
                rangeGreen = tmpBucket.Max(color => color.Key.Item2) - tmpBucket.Min(color => color.Key.Item2);
                rangeBlue = tmpBucket.Max(color => color.Key.Item3) - tmpBucket.Min(color => color.Key.Item3);
                if (rangeRed > rangeGreen)
                {
                    if (rangeBlue > rangeRed)
                        tmpBucket = tmpBucket.OrderBy(element => element.Key.Item3).ToDictionary(c => c.Key, c => c.Value); //sort by blue color
                    else
                        tmpBucket = tmpBucket.OrderBy(element => element.Key.Item1).ToDictionary(c => c.Key, c => c.Value); //sort by red color
                }
                else
                {
                    if (rangeBlue > rangeGreen)
                        tmpBucket = tmpBucket.OrderBy(element => element.Key.Item3).ToDictionary(c => c.Key, c => c.Value); //sort by blue color
                    else
                        tmpBucket = tmpBucket.OrderBy(element => element.Key.Item2).ToDictionary(c => c.Key, c => c.Value); //sort by green color
                }
                median = (int)Math.Ceiling((decimal)tmpBucket.Count / 2);
                allBuckets.Add(tmpBucket.Take(median).ToDictionary(c => c.Key, c => c.Value));
                allBuckets[i] = tmpBucket.Skip(median).ToDictionary(c => c.Key, c => c.Value);
            }
        }

        private unsafe int[] AverageOfColorList(Dictionary<(int, int, int), List<PointerToColors>> dict)
        {
            long sumR = 0, sumG = 0, sumB = 0;
            foreach (var element in dict.Keys)
            {
                sumR += element.Item1;
                sumG += element.Item2;
                sumB += element.Item3;
            }
            sumR /= dict.Count;
            sumG /= dict.Count;
            sumB /= dict.Count;
            int[] result = new int[] { (int)sumR, (int)sumG, (int)sumB };

            //sumR = list.Last().red - list.First().red;
            //sumG = list.Last().green - list.First().green;
            //sumB = list.Last().blue - list.First().blue;
            //int[] result = new int[] { (int)sumR / 2, (int)sumG / 2, (int)sumB / 2 };

            return result;
        }

        public unsafe void MedianCut(Bitmap bmp, int valueOfColors)
        {
            BitmapData bData = bmp.LockBits(
                            new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
            byte* scan0 = (byte*)bData.Scan0.ToPointer();
            byte bitsPerPixel = (byte)(Image.GetPixelFormatSize(bData.PixelFormat));
            int i;
            var bucket = new Dictionary<(int, int, int), List<PointerToColors>>();
            for (i = 0; i < bData.Height; i++)
            {
                for (int j = 0; j < bData.Width; j++)
                {
                    byte* data = scan0 + i * bData.Stride + j * bitsPerPixel / 8;
                    if (!bucket.ContainsKey((data[0], data[1], data[2])))
                        bucket[(data[0], data[1], data[2])] = new List<PointerToColors>() { new PointerToColors(&data[0], &data[1], &data[2]) };
                    else
                        bucket[(data[0], data[1], data[2])].Add(new PointerToColors(&data[0], &data[1], &data[2]));
                }
            }
            var allBuckets = new List<Dictionary<(int, int, int), List<PointerToColors>>>();
            allBuckets.Add(bucket);
            for (i = valueOfColors; i > 1; i /= 2)
            {             
                DivideByMedian(allBuckets);
            }     
            int[] averages;
            for (i = 0; i < valueOfColors; i++)
            {
                if (allBuckets[i].Count > 0)
                {
                    averages = AverageOfColorList(allBuckets[i]);
                    foreach (var setOfColors in allBuckets[i])
                    {
                        foreach (var color in setOfColors.Value)
                        {
                            *color.red = (byte)averages[0];
                            *color.green = (byte)averages[1];
                            *color.blue = (byte)averages[2];
                        }
                    }
                }
            }
            bmp.UnlockBits(bData);
        }

        private void DivideByMedian(object allBuckets)
        {
            throw new NotImplementedException();
        }
    }

    public unsafe struct PointerToColors
    {
        public byte* red;
        public byte* green;
        public byte* blue;

        public PointerToColors(byte* red, byte* green, byte* blue) : this ()
        {
            this.red = red;
            this.green = green;
            this.blue = blue;
        }
    }
}
