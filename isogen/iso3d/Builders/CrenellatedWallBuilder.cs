using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using IsoGen;
using static isogen.iso3d.Isometric3dExtensions;

namespace isogen.iso3d.Builders
{
    public class CrenellatedWallBuilder : IRender
    {
        internal Image TopImage { get; set; }
        internal Image LeftImage { get; set; }
        internal Image RightImage { get; set; }
        internal Image CrenellationImage { get; set; }
        internal int GridDiagonal { get; set; }
        internal float Height { get; set; } = 0;
        internal CrenellationVariant Variant { get; set; }
        internal float CrenellationHeight { get; set; } = 0.75f;
        internal float CrenellationThickness { get; set; } = 0.15f;
        internal float BottomTaper { get; set; } = 0.15f;

        public CrenellatedWallBuilder(Image baseImage, int gridDiagonal, CrenellationVariant variant)
        {
            TopImage = baseImage;
            LeftImage = baseImage;
            RightImage = baseImage;
            CrenellationImage = baseImage;
            GridDiagonal = gridDiagonal;
            Variant = variant;
        }

        [Flags]
        public enum CrenellationVariant
        {
            UpperLeft = 0x0001,
            UpperRight = 0x0010,
            LowerLeft = 0x0100,
            LowerRight = 0x1000,
        }
        public CrenellatedWallBuilder WithTop(Image image)
        {
            return new CrenellatedWallBuilder(image, GridDiagonal, Variant) {LeftImage = LeftImage, RightImage = RightImage, CrenellationImage = CrenellationImage, Height = Height};
        }
        
        public CrenellatedWallBuilder WithLeft(Image image)
        {
            return new CrenellatedWallBuilder(TopImage, GridDiagonal, Variant) {LeftImage = image, RightImage = RightImage, CrenellationImage = CrenellationImage, Height = Height};
        }
        
        public CrenellatedWallBuilder WithRight(Image image)
        {
            return new CrenellatedWallBuilder(TopImage, GridDiagonal, Variant) {LeftImage = LeftImage, RightImage = image, CrenellationImage = CrenellationImage, Height = Height};
        }
        
        public CrenellatedWallBuilder WithCrenellation(Image image)
        {
            return new CrenellatedWallBuilder(TopImage, GridDiagonal, Variant) {LeftImage = LeftImage, RightImage = RightImage, CrenellationImage = image, Height = Height};
        }

        public CrenellatedWallBuilder Extrude(float height)
        {
            return new CrenellatedWallBuilder(TopImage, GridDiagonal, Variant) {LeftImage = LeftImage, RightImage = RightImage, CrenellationImage = CrenellationImage, Height = height};
        }

        public CrenellatedWallBuilder WithVariant(CrenellationVariant variant)
        {
            return new CrenellatedWallBuilder(TopImage, GridDiagonal, variant) {LeftImage = LeftImage, RightImage = RightImage, CrenellationImage = CrenellationImage, Height = Height};
        }

        public Image Render()
        {
            Size s = CalculateBounds();
            Bitmap b = new Bitmap(s.Width, s.Height);
            using (Graphics g = Graphics.FromImage(b))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.SmoothingMode = SmoothingMode.None;
                if (Variant.HasFlag(CrenellationVariant.UpperLeft))
                {
                    RenderTopLeftCrenellation(g);
                }
                if (Variant.HasFlag(CrenellationVariant.UpperRight))
                {
                    RenderTopRightCrenellation(g);
                }
                if (Variant.HasFlag(CrenellationVariant.LowerLeft))
                {
                    RenderBottomLeftCrenellation(g);
                }
                if (Variant.HasFlag(CrenellationVariant.LowerRight))
                {
                    RenderBottomRightCrenellation(g);
                }
            }

            return b;
        }

        private Size CalculateBounds()
        {
            return Isometric3dExtensions.CalculateBounds(GridDiagonal, Height + CrenellationHeight);
        }

        private void RenderTopLeftCrenellation(Graphics g)
        {
            //only draw taper if enough space
            if (Height >= BottomTaper)
            {
                Relative3dPoint[] taperBFrontPoints =
                {
                    new Relative3dPoint(0, CrenellationThickness, CrenellationHeight),
                    new Relative3dPoint(1, CrenellationThickness, CrenellationHeight),
                    new Relative3dPoint(0, 0, CrenellationHeight + BottomTaper)
                };
                
                g.DrawImage(CrenellationImage, taperBFrontPoints, GridDiagonal, BrushesAndColors.ShadingRightDarkerColor);

                Point[] points =
                {
                    new Point(0, 0),
                    new Point(CrenellationImage.Width, 0),
                    new Point(0, CrenellationImage.Height),
                };
                Relative3dPoint[] taperBLeftPoints =
                {
                    new Relative3dPoint(0, 0, CrenellationHeight),
                    new Relative3dPoint(0, CrenellationThickness, CrenellationHeight),
                    new Relative3dPoint(0, 0, CrenellationHeight + BottomTaper)
                };
                using (Bitmap image = CrenellationImage.IntersectMask(points))
                {
                    g.DrawImage(image, taperBLeftPoints, GridDiagonal, BrushesAndColors.ShadingLeftColor);
                }
            }
            
            using (Bitmap frontImage = GetMaskedCrenellationFrontImage())
            using (Bitmap topImage = GetMaskedCrenellationTopImage())
            using (Bitmap top2Image = GetMaskedCrenellationTop2Image())
            {
                Relative3dPoint[] top1Points =
                {
                    new Relative3dPoint(0, 0, CrenellationHeight / 2),
                    new Relative3dPoint(1, 0, CrenellationHeight / 2),
                    new Relative3dPoint(0, CrenellationThickness, CrenellationHeight / 2)
                };
                g.DrawImage(topImage, top1Points, GridDiagonal, BrushesAndColors.ShadingTopColor);
                Relative3dPoint[] top2Points =
                {
                    new Relative3dPoint(0, 0, 0),
                    new Relative3dPoint(1, 0, 0),
                    new Relative3dPoint(0, CrenellationThickness, 0)
                };
                g.DrawImage(top2Image, top2Points, GridDiagonal, BrushesAndColors.ShadingTopColor);
                
                Relative3dPoint[] frontPoints = {
                    new Relative3dPoint(0, CrenellationThickness, 0),
                    new Relative3dPoint(1, CrenellationThickness, 0),
                    new Relative3dPoint(0, CrenellationThickness, CrenellationHeight),
                };
                g.DrawImage(frontImage, frontPoints, GridDiagonal, BrushesAndColors.ShadingRightColor);

                Relative3dPoint[] left1Points =
                {
                    new Relative3dPoint(0, 0, 0),
                    new Relative3dPoint(0, CrenellationThickness, 0),
                    new Relative3dPoint(0, 0, CrenellationHeight)
                };
                g.DrawImage(CrenellationImage, left1Points, GridDiagonal, BrushesAndColors.ShadingLeftColor);
                
                Relative3dPoint[] left2Points =
                {
                    new Relative3dPoint(3f / 8, 0, 0),
                    new Relative3dPoint(3f / 8, CrenellationThickness, 0),
                    new Relative3dPoint(3f / 8, 0, CrenellationHeight / 2)
                };
                g.DrawImage(CrenellationImage, left2Points, GridDiagonal, BrushesAndColors.ShadingLeftColor);
                
                Relative3dPoint[] left3Points =
                {
                    new Relative3dPoint(7f / 8, 0, 0),
                    new Relative3dPoint(7f / 8, CrenellationThickness, 0),
                    new Relative3dPoint(7f / 8, 0, CrenellationHeight / 2)
                };
                g.DrawImage(CrenellationImage, left3Points, GridDiagonal, BrushesAndColors.ShadingLeftColor);
            }
        }

        private void RenderBottomRightCrenellation(Graphics g)
        {
            float yOffset = 1 - CrenellationThickness;
            //only draw taper if enough space
            if (Height >= BottomTaper)
            {
                Point[] points =
                {
                    new Point(0, 0),
                    new Point(CrenellationImage.Width, 0),
                    new Point(CrenellationImage.Width, CrenellationImage.Height),
                };
                Relative3dPoint[] taperBLeftPoints =
                {
                    new Relative3dPoint(0, yOffset, CrenellationHeight),
                    new Relative3dPoint(0, 1, CrenellationHeight), //yOffset + CrenellationThickness
                    new Relative3dPoint(0, yOffset, CrenellationHeight + BottomTaper)
                };
                using (Bitmap image = CrenellationImage.IntersectMask(points))
                {
                    g.DrawImage(image, taperBLeftPoints, GridDiagonal, BrushesAndColors.ShadingLeftColor);
                }
            }
            
            using (Bitmap frontImage = GetMaskedCrenellationFrontImage())
            using (Bitmap topImage = GetMaskedCrenellationTopImage())
            using (Bitmap top2Image = GetMaskedCrenellationTop2Image())
            {
                Relative3dPoint[] top1Points =
                {
                    new Relative3dPoint(0, yOffset, CrenellationHeight / 2),
                    new Relative3dPoint(1, yOffset, CrenellationHeight / 2),
                    new Relative3dPoint(0, 1, CrenellationHeight / 2)
                };
                g.DrawImage(topImage, top1Points, GridDiagonal, BrushesAndColors.ShadingTopColor);
                Relative3dPoint[] top2Points =
                {
                    new Relative3dPoint(0, yOffset, 0),
                    new Relative3dPoint(1, yOffset, 0),
                    new Relative3dPoint(0, 1, 0)
                };
                g.DrawImage(top2Image, top2Points, GridDiagonal, BrushesAndColors.ShadingTopColor);
                
                Relative3dPoint[] frontPoints = {
                    new Relative3dPoint(0, 1, 0),
                    new Relative3dPoint(1, 1, 0),
                    new Relative3dPoint(0, 1, CrenellationHeight),
                };
                g.DrawImage(frontImage, frontPoints, GridDiagonal, BrushesAndColors.ShadingRightColor);

                Relative3dPoint[] left1Points =
                {
                    new Relative3dPoint(0, yOffset, 0),
                    new Relative3dPoint(0,  1, 0),
                    new Relative3dPoint(0, yOffset, CrenellationHeight)
                };
                g.DrawImage(CrenellationImage, left1Points, GridDiagonal, BrushesAndColors.ShadingLeftColor);
                
                Relative3dPoint[] left2Points =
                {
                    new Relative3dPoint(3f / 8, yOffset, 0),
                    new Relative3dPoint(3f / 8, 1, 0),
                    new Relative3dPoint(3f / 8, yOffset, CrenellationHeight / 2)
                };
                g.DrawImage(CrenellationImage, left2Points, GridDiagonal, BrushesAndColors.ShadingLeftColor);
                
                Relative3dPoint[] left3Points =
                {
                    new Relative3dPoint(7f / 8, yOffset, 0),
                    new Relative3dPoint(7f / 8, 1, 0),
                    new Relative3dPoint(7f / 8, yOffset, CrenellationHeight / 2)
                };
                g.DrawImage(CrenellationImage, left3Points, GridDiagonal, BrushesAndColors.ShadingLeftColor);
            }
        }
        
        private void RenderTopRightCrenellation(Graphics g)
        {
            float xOffset = 1 - CrenellationThickness;
            //only draw taper if enough space
            if (Height >= BottomTaper)
            {
                Point[] points =
                {
                    new Point(0, 0),
                    new Point(CrenellationImage.Width, 0),
                    new Point(CrenellationImage.Width, CrenellationImage.Height),
                };
                Relative3dPoint[] taperBLeftPoints =
                {
                    new Relative3dPoint(xOffset, 1, CrenellationHeight),
                    new Relative3dPoint(1, 1, CrenellationHeight),
                    new Relative3dPoint(xOffset, 1, CrenellationHeight + BottomTaper)
                };
                using (Bitmap image = CrenellationImage.IntersectMask(points))
                {
                    g.DrawImage(image, taperBLeftPoints, GridDiagonal, BrushesAndColors.ShadingLeftColor);
                }
            }
            
            using (Bitmap frontImage = GetMaskedCrenellationFrontImage())
            using (Bitmap topImage = GetMaskedCrenellationTopImage())
            using (Bitmap top2Image = GetMaskedCrenellationTop2Image())
            {
                Relative3dPoint[] top1Points =
                {
                    new Relative3dPoint(1, 0, CrenellationHeight / 2),
                    new Relative3dPoint(1, 1, CrenellationHeight / 2),
                    new Relative3dPoint(xOffset, 0, CrenellationHeight / 2)
                };
                g.DrawImage(topImage, top1Points, GridDiagonal, BrushesAndColors.ShadingTopColor);
                Relative3dPoint[] top2Points =
                {
                    new Relative3dPoint(1, 0, 0),
                    new Relative3dPoint(1, 1, 0),
                    new Relative3dPoint(xOffset, 0, 0)
                };
                g.DrawImage(top2Image, top2Points, GridDiagonal, BrushesAndColors.ShadingTopColor);
                
                Relative3dPoint[] frontPoints = {
                    new Relative3dPoint(xOffset, 0, 0),
                    new Relative3dPoint(xOffset, 1, 0),
                    new Relative3dPoint(xOffset, 0, CrenellationHeight),
                };
                g.DrawImage(frontImage, frontPoints, GridDiagonal, BrushesAndColors.ShadingLeftColor);

                Relative3dPoint[] right1Points =
                {
                    new Relative3dPoint(xOffset, 1, 0),
                    new Relative3dPoint(1,  1, 0),
                    new Relative3dPoint(xOffset, 1, CrenellationHeight)
                };
                g.DrawImage(CrenellationImage, right1Points, GridDiagonal, BrushesAndColors.ShadingRightColor);
                
                Relative3dPoint[] right2Points =
                {
                    new Relative3dPoint(xOffset, 5f / 8, 0),
                    new Relative3dPoint(1, 5f / 8, 0),
                    new Relative3dPoint(xOffset, 5f / 8, CrenellationHeight / 2)
                };
                g.DrawImage(CrenellationImage, right2Points, GridDiagonal, BrushesAndColors.ShadingRightColor);
                
                Relative3dPoint[] right3Points =
                {
                    new Relative3dPoint(xOffset, 1f / 8, 0),
                    new Relative3dPoint(1, 1f / 8 , 0),
                    new Relative3dPoint(xOffset, 1f / 8, CrenellationHeight / 2)
                };
                g.DrawImage(CrenellationImage, right3Points, GridDiagonal, BrushesAndColors.ShadingRightColor);
            }
        }
        
        private void RenderBottomLeftCrenellation(Graphics g)
        {
            //only draw taper if enough space
            if (Height >= BottomTaper)
            {
                Point[] points =
                {
                    new Point(0, 0),
                    new Point(CrenellationImage.Width, 0),
                    new Point(CrenellationImage.Width, CrenellationImage.Height),
                };
                Relative3dPoint[] taperBLeftPoints =
                {
                    new Relative3dPoint(0, 1, CrenellationHeight),
                    new Relative3dPoint(CrenellationThickness, 1, CrenellationHeight),
                    new Relative3dPoint(0, 1, CrenellationHeight + BottomTaper)
                };
                using (Bitmap image = CrenellationImage.IntersectMask(points))
                {
                    g.DrawImage(image, taperBLeftPoints, GridDiagonal, BrushesAndColors.ShadingLeftColor);
                }
            }
            
            using (Bitmap frontImage = GetMaskedCrenellationFrontImage())
            using (Bitmap topImage = GetMaskedCrenellationTopImage())
            using (Bitmap top2Image = GetMaskedCrenellationTop2Image())
            {
                Relative3dPoint[] top1Points =
                {
                    new Relative3dPoint(CrenellationThickness, 0, CrenellationHeight / 2),
                    new Relative3dPoint(CrenellationThickness, 1, CrenellationHeight / 2),
                    new Relative3dPoint(0, 0, CrenellationHeight / 2)
                };
                g.DrawImage(topImage, top1Points, GridDiagonal, BrushesAndColors.ShadingTopColor);
                Relative3dPoint[] top2Points =
                {
                    new Relative3dPoint(CrenellationThickness, 0, 0),
                    new Relative3dPoint(CrenellationThickness, 1, 0),
                    new Relative3dPoint(0, 0, 0)
                };
                g.DrawImage(top2Image, top2Points, GridDiagonal, BrushesAndColors.ShadingTopColor);
                
                Relative3dPoint[] frontPoints = {
                    new Relative3dPoint(0, 0, 0),
                    new Relative3dPoint(0, 1, 0),
                    new Relative3dPoint(0, 0, CrenellationHeight),
                };
                g.DrawImage(frontImage, frontPoints, GridDiagonal, BrushesAndColors.ShadingLeftColor);

                Relative3dPoint[] right1Points =
                {
                    new Relative3dPoint(0, 1, 0),
                    new Relative3dPoint(CrenellationThickness,  1, 0),
                    new Relative3dPoint(0, 1, CrenellationHeight)
                };
                g.DrawImage(CrenellationImage, right1Points, GridDiagonal, BrushesAndColors.ShadingRightColor);
                
                Relative3dPoint[] right2Points =
                {
                    new Relative3dPoint(0, 5f / 8, 0),
                    new Relative3dPoint(CrenellationThickness, 5f / 8, 0),
                    new Relative3dPoint(0, 5f / 8, CrenellationHeight / 2)
                };
                g.DrawImage(CrenellationImage, right2Points, GridDiagonal, BrushesAndColors.ShadingRightColor);
                
                Relative3dPoint[] right3Points =
                {
                    new Relative3dPoint(0, 1f / 8, 0),
                    new Relative3dPoint(CrenellationThickness, 1f / 8 , 0),
                    new Relative3dPoint(0, 1f / 8, CrenellationHeight / 2)
                };
                g.DrawImage(CrenellationImage, right3Points, GridDiagonal, BrushesAndColors.ShadingRightColor);
            }
        }
        
        private Bitmap GetMaskedCrenellationFrontImage()
        {
            int wUnit = CrenellationImage.Width / 8;
            int hUnit = CrenellationImage.Height / 2;
            return CrenellationImage.IntersectMask(new[]
            {
                new Point(0, 0),
                new Point(wUnit, 0),
                new Point(wUnit, hUnit),
                new Point(wUnit * 3, hUnit),
                new Point(wUnit * 3, 0),
                new Point(wUnit * 5, 0),
                new Point(wUnit * 5, hUnit),
                new Point(wUnit * 7, hUnit),
                new Point(wUnit * 7, 0),
                new Point(wUnit * 8, 0),
                new Point(wUnit * 8, hUnit * 2),
                new Point(0, hUnit * 2),
            });
        }
        
        private Bitmap GetMaskedCrenellationTopImage()
        {
            int wUnit = CrenellationImage.Width / 8;
            int hUnit = CrenellationImage.Height;
            return CrenellationImage.IntersectMask(new[]
            {
                new Point(wUnit, hUnit),
                new Point(wUnit, 0),
                new Point(wUnit * 3, 0),
                new Point(wUnit * 3, hUnit),
                new Point(wUnit * 5, hUnit),
                new Point(wUnit * 5, 0),
                new Point(wUnit * 7, 0),
                new Point(wUnit * 7, hUnit),
            });
        }
        
        private Bitmap GetMaskedCrenellationTop2Image()
        {
            int wUnit = CrenellationImage.Width / 8;
            int hUnit = CrenellationImage.Height;
            return CrenellationImage.IntersectMask(new[]
            {
                new Point(0, 0),
                new Point(wUnit, 0),
                new Point(wUnit, hUnit),
                new Point(wUnit * 3, hUnit),
                new Point(wUnit * 3, 0),
                new Point(wUnit * 5, 0),
                new Point(wUnit * 5, hUnit),
                new Point(wUnit * 7, hUnit),
                new Point(wUnit * 7, 0),
                new Point(wUnit * 8, 0),
                new Point(wUnit * 8, hUnit),
                new Point(0, hUnit),
            });
        }
    }
}