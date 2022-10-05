using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using IsoGen;
using static isogen.iso3d.Isometric3dExtensions;

namespace isogen.iso3d.Builders
{
    public class BlockBuilder : IRender
    {
        internal Image TopImage { get; set; }
        internal Image LeftImage { get; set; }
        internal Image RightImage { get; set; }
        internal int GridDiagonal { get; set; }
        internal float Height { get; set; }
        
        public BlockBuilder WithTopImage(Image image)
        {
            return new BlockBuilder() {TopImage = image, LeftImage = LeftImage, RightImage = RightImage, GridDiagonal = GridDiagonal, Height = Height};
        }
        
        public BlockBuilder WithLeftImage(Image image)
        {
            return new BlockBuilder() {TopImage = TopImage, LeftImage = image, RightImage = RightImage, GridDiagonal = GridDiagonal, Height = Height};
        }
        
        public BlockBuilder WithRightImage(Image image)
        {
            return new BlockBuilder() {TopImage = TopImage, LeftImage = LeftImage, RightImage = image, GridDiagonal = GridDiagonal, Height = Height};
        }

        public BlockBuilder WithHeight(float height)
        {
            return new BlockBuilder() {TopImage = TopImage, LeftImage = LeftImage, RightImage = RightImage, GridDiagonal = GridDiagonal, Height = height};
        }

        public Image Render()
        {
            Size s = CalculateBounds(GridDiagonal, Height);
            Bitmap b = new Bitmap(s.Width, s.Height);

            using (Graphics g = Graphics.FromImage(b))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.SmoothingMode = SmoothingMode.None;

                if (LeftImage != null)
                {
                    Relative3dPoint[] leftImageDestPoints =
                    {
                        new Relative3dPoint(0, 0, 0),
                        new Relative3dPoint(0, 1, 0),
                        new Relative3dPoint(0, 0, Height)
                    };
                    g.DrawImage(LeftImage, leftImageDestPoints.Take(3).ToArray(), GridDiagonal, BrushesAndColors.ShadingLeftColor);
                }

                if (RightImage != null)
                {
                    Relative3dPoint[] rightImageDestPoints =
                    {
                        new Relative3dPoint(0, 1, 0),
                        new Relative3dPoint(1, 1, 0),
                        new Relative3dPoint(0, 1, Height)
                    };
                    g.DrawImage(RightImage, rightImageDestPoints.Take(3).ToArray(), GridDiagonal, BrushesAndColors.ShadingRightColor);
                }

                if (TopImage != null)
                {
                    Relative3dPoint[] topImageDestPoints =
                    {
                        new Relative3dPoint(0, 0, 0),
                        new Relative3dPoint(1, 0, 0),
                        new Relative3dPoint(0, 1, 0)
                    };
                    g.DrawImage(TopImage, topImageDestPoints.Take(3).ToArray(), GridDiagonal, BrushesAndColors.ShadingTopColor);
                }
            }

            return b;
        }
    }
}