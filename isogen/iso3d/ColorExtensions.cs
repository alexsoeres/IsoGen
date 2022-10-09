using System.Drawing;

namespace IsoGen
{
    public static class ColorExtensions
    {
        public static Color WithAlpha(this Color color, byte alpha)
        {
            return Color.FromArgb(alpha, color.R, color.G, color.B);
        }
    }
}