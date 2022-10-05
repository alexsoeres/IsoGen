using System.Drawing;
using IsoGen;

namespace isogen.iso3d
{
    public static class BrushesAndColors
    {
        internal static readonly Color ShadingLeftColor = Color.FromArgb(200, 36, 22, 4);
        internal static readonly Color ShadingRightColor = ShadingLeftColor.WithAlpha(100);
        internal static readonly Color ShadingTopColor = Color.FromArgb(50, 255, 254, 172);
        internal static readonly Color ShadingRightDarkerColor = ShadingLeftColor.WithAlpha(150);
        internal static readonly Brush ShadingLeft = new SolidBrush(ShadingLeftColor);
        internal static readonly Brush ShadingRight = new SolidBrush(ShadingRightColor);
        internal static readonly Brush ShadingTop = new SolidBrush(ShadingTopColor);
    }
}