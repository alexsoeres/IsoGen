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
        
        /// <summary>
        /// Shadings for top each orientation, see <code>Orientation</code>
        /// </summary>
        public static readonly Color?[] TopShadings =
        {
            ShadingTopColor,
            ShadingTopColor,
            ShadingTopColor,
            ShadingTopColor
        };
        
        /// <summary>
        /// Shadings for left front for each orientation, see <code>Orientation</code>
        /// </summary>
        public static readonly Color?[] LeftFrontShadings =
        {
            ShadingLeftColor,
            null,
            null,
            ShadingRightColor
        };
        
        #region RightFrontShadings, RightBackShadings, LeftBackShadings is same but offset, no need to edit
        
        /// <summary>
        /// Shadings for right front for each orientation, see <code>Orientation</code>
        /// </summary>
        public static readonly Color?[] RightFrontShadings =
        {
            LeftFrontShadings[3],
            LeftFrontShadings[0],
            LeftFrontShadings[1],
            LeftFrontShadings[2],
        };
        
        /// <summary>
        /// Shadings for right back for each orientation, see <code>Orientation</code>
        /// </summary>
        public static readonly Color?[] RightBackShadings =
        {
            RightFrontShadings[3],
            RightFrontShadings[0],
            RightFrontShadings[1],
            RightFrontShadings[2],
        };
        
        /// <summary>
        /// Shadings for left back for each orientation, see <code>Orientation</code>
        /// </summary>
        public static readonly Color?[] LeftBackShadings =
        {
            RightBackShadings[3],
            RightBackShadings[0],
            RightBackShadings[1],
            RightBackShadings[2],
        };
        #endregion
    }
}