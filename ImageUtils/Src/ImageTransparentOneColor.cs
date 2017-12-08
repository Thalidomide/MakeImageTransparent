using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ImageUtils
{
    public class DetectColorResult
    {
        public Color MainColor { get; }
        public Color MostDifferentColor { get; }

        public DetectColorResult(Color mainColor, Color mostDifferentColor)
        {
            MainColor = mainColor;
            MostDifferentColor = mostDifferentColor;
        }
    }

    public enum MainColorStrategy
    {
        MostUsed,
        SecondMostUsed,
        MostDifferentFromMostUsed,
        DifferentAndWellUsed
    }

    public class ImageTransparentOneColor
    {
        public Bitmap KeepBlack(Image image)
        {
            return KeepColorAndMakeOtherColorsTransparent(new Bitmap(image), Color.Black, Color.White);
        }

        public Bitmap DetectMainColor(Image image, MainColorStrategy mainColorStrategy)
        {
            Console.WriteLine($"Transform image with strategy {mainColorStrategy}");

            var bitmap = new Bitmap(image);

            var colorResult = DetectMainColor(bitmap, mainColorStrategy);

            Console.WriteLine($"Main color detected: {colorResult.MainColor}. Most different color: {colorResult.MostDifferentColor}");

            return KeepColorAndMakeOtherColorsTransparent(bitmap, colorResult.MainColor, colorResult.MostDifferentColor);
        }
        
        private static Bitmap KeepColorAndMakeOtherColorsTransparent(Bitmap bitmap, Color mainColor, Color mostDifferent)
        {
            var maxDiff = mainColor.Diff(mostDifferent);

            for (var x = 0; x < bitmap.Width; x++)
            {
                for (var y = 0; y < bitmap.Height; y++)
                {
                    var color = bitmap.GetPixel(x, y);
                    var diff = mainColor.Diff(color);
                    
                    // Calculate alpha ranging from 0-255, with the most different color scoring 0
                    var alpha = (maxDiff - diff) * 255 / maxDiff;
                    var newColor = Color.FromArgb(alpha, mainColor.R, mainColor.G, mainColor.B);

                    bitmap.SetPixel(x, y, newColor);
                }
            }

            return bitmap;
        }

        private static DetectColorResult DetectMainColor(Bitmap bitmap, MainColorStrategy mainColorStrategy)
        {
            var colorsCount = new Dictionary<Color, int>();
            
            for (var x = 0; x < bitmap.Width; x++)
            {
                for (var y = 0; y < bitmap.Height; y++)
                {
                    var color = bitmap.GetPixel(x, y);
                    
                    if (colorsCount.ContainsKey(color))
                    {
                        colorsCount[color]++;
                    }
                    else
                    {
                        colorsCount.Add(color, 1);
                    }
                }
            }

            // Assume most used color is background and second most used is main
            var ordered = colorsCount.OrderByDescending(c => c.Value).ToArray();
            var orderedColors = ordered.Select(o => o.Key).ToArray();

            Color mainColor;
            switch (mainColorStrategy)
            {
                case MainColorStrategy.MostUsed:
                    mainColor = orderedColors[0];
                    break;
                case MainColorStrategy.SecondMostUsed:
                    mainColor = ordered.Length > 1 ? orderedColors[1] : orderedColors[0];
                    break;
                case MainColorStrategy.MostDifferentFromMostUsed:
                    mainColor = FindMostDifferent(orderedColors[0], orderedColors);
                    break;
                case MainColorStrategy.DifferentAndWellUsed:
                    mainColor = FindDifferentAndWellUsed(orderedColors);
                    break;
                default:
                    throw new Exception($"Unhandled main color strategy: {mainColorStrategy}");
            }

            // Find most different color
            var mostDifferent = FindMostDifferent(mainColor, ordered.Select(i => i.Key));

            return new DetectColorResult(mainColor, mostDifferent);
        }

        private static Color FindDifferentAndWellUsed(Color[] orderedColors)
        {
            var mostUsed = orderedColors[0];
            var mostDifferent = FindMostDifferent(mostUsed, orderedColors);
            var maxDiff = mostUsed.Diff(mostDifferent);

            foreach (var color in orderedColors)
            {
                var diff = color.Diff(mostUsed);

                if (diff > maxDiff / 2)
                {
                    return color;
                }
            }

            // Default to most different color
            return mostDifferent;
        }

        private static Color FindMostDifferent(Color mainColor, IEnumerable<Color> otherColors)
        {
            Color? mostDifferent = null;
            var maxDiff = 0;

            foreach (var color in otherColors)
            {
                var diff = mainColor.Diff(color);
                if (diff > maxDiff)
                {
                    mostDifferent = color;
                    maxDiff = diff;
                }
            }

            return mostDifferent.Value;
        }
    }
}