using System;
using System.Drawing;

namespace ImageUtils
{
    public static class ColorExtensions
    {
        public static int Diff(this Color color, Color other)
        {
            var redDiff = Math.Abs(color.R - other.R);
            var greenDiff = Math.Abs(color.G - other.G);
            var blueDiff = Math.Abs(color.B - other.B);

            return (int) Math.Round((double) (redDiff + greenDiff + blueDiff) / 3);
        }
    }
}
