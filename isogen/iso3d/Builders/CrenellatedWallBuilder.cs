using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using IsoGen;

namespace isogen.iso3d.Builders
{
    public class CrenellatedWallBuilder : IRender
    {
        private Image TopImage { get; set; }
        private Image LeftImage { get; set; }
        private Image RightImage { get; set; }
        private Image CrenellationImage { get; set; }
        private int GridDiagonal { get; set; }
        private float Height { get; set; } = 0;
        private CrenellationVariant Variant { get; set; }
        private float CrenellationHeight { get; set; } = 0.75f;
        private float CrenellationThickness { get; set; } = 0.15f;

        public CrenellatedWallBuilder(Image baseImage, int gridDiagonal, CrenellationVariant variant)
        {
            TopImage = baseImage;
            LeftImage = baseImage;
            RightImage = baseImage;
            CrenellationImage = baseImage;
            GridDiagonal = gridDiagonal;
            Variant = variant;
        }

        public enum CrenellationVariant { Single, Corner }
        
        public CrenellatedWallBuilder WithTop(Image image)
        {
            return new CrenellatedWallBuilder(image, GridDiagonal, Variant)
            {
                LeftImage = LeftImage, RightImage = RightImage, CrenellationImage = CrenellationImage, Height = Height
            };
        }

        public CrenellatedWallBuilder WithLeft(Image image)
        {
            return new CrenellatedWallBuilder(TopImage, GridDiagonal, Variant)
            {
                LeftImage = image, RightImage = RightImage, CrenellationImage = CrenellationImage, Height = Height
            };
        }

        public CrenellatedWallBuilder WithRight(Image image)
        {
            return new CrenellatedWallBuilder(TopImage, GridDiagonal, Variant)
            {
                LeftImage = LeftImage, RightImage = image, CrenellationImage = CrenellationImage, Height = Height
            };
        }

        public CrenellatedWallBuilder WithCrenellation(Image image)
        {
            return new CrenellatedWallBuilder(TopImage, GridDiagonal, Variant)
            {
                LeftImage = LeftImage, RightImage = RightImage, CrenellationImage = image, Height = Height
            };
        }

        public CrenellatedWallBuilder Extrude(float height)
        {
            return new CrenellatedWallBuilder(TopImage, GridDiagonal, Variant)
            {
                LeftImage = LeftImage, RightImage = RightImage, CrenellationImage = CrenellationImage, Height = height
            };
        }

        public CrenellatedWallBuilder WithVariant(CrenellationVariant variant)
        {
            return new CrenellatedWallBuilder(TopImage, GridDiagonal, variant)
            {
                LeftImage = LeftImage, RightImage = RightImage, CrenellationImage = CrenellationImage, Height = Height
            };
        }

        public Image Render(Orientation o = Orientation.TopLeft)
        {
            Size s = CalculateBounds();
            Bitmap b = new Bitmap(s.Width, s.Height);
            using (Graphics g = Graphics.FromImage(b))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.SmoothingMode = SmoothingMode.None;
                
                switch (Variant)
                {
                    case CrenellationVariant.Single:
                        RenderSingle(g, o);
                        break;
                    case CrenellationVariant.Corner:
                        //todo: implement
                        break;
                }
            }

            return b;
        }

        private Size CalculateBounds()
        {
            return Isometric3dExtensions.CalculateBounds(GridDiagonal, Height + CrenellationHeight);
        }

        private void RenderSingle(Graphics g, Orientation o)
        {
            
            List<Relative3dFace> faces = new List<Relative3dFace>();

            using (Bitmap frontImage = GetMaskedCrenellationFrontImage())
            using (Bitmap topImage = GetMaskedCrenellationTopImage())
            using (Bitmap top2Image = GetMaskedCrenellationTop2Image())
            {
                //top 1
                faces.Add(new Relative3dFace(new[]
                {
                    new Relative3dCoordinate(0, 0, CrenellationHeight / 2),
                    new Relative3dCoordinate(1, 0, CrenellationHeight / 2),
                    new Relative3dCoordinate(0, CrenellationThickness, CrenellationHeight / 2)
                }, topImage, new[] {0, 0, 0, 0}, BrushesAndColors.TopShadings));
                
                
                //top 2
                faces.Add(new Relative3dFace(new[]
                {
                    new Relative3dCoordinate(0, 0, 0),
                    new Relative3dCoordinate(1, 0, 0),
                    new Relative3dCoordinate(0, CrenellationThickness, 0)
                }, top2Image, new[] {1, 1, 1, 1}, BrushesAndColors.TopShadings));

                //right front
                faces.Add(new Relative3dFace(new[]
                {
                    new Relative3dCoordinate(0, CrenellationThickness, 0),
                    new Relative3dCoordinate(1, CrenellationThickness, 0),
                    new Relative3dCoordinate(0, CrenellationThickness, CrenellationHeight),
                }, frontImage, new[] {2, 2, -1, -1}, BrushesAndColors.RightFrontShadings));
                
                //left back
                faces.Add(new Relative3dFace(new[]
                {
                    new Relative3dCoordinate(0, 0, 0),
                    new Relative3dCoordinate(1, 0, 0),
                    new Relative3dCoordinate(0, 0, CrenellationHeight),
                }, frontImage, new[] {-1, -1, 2, 2}, BrushesAndColors.LeftBackShadings));

                //right back 1
                faces.Add(new Relative3dFace(new[]
                {
                    new Relative3dCoordinate(1, 0, 0),
                    new Relative3dCoordinate(1, CrenellationThickness, 0),
                    new Relative3dCoordinate(1, 0, CrenellationHeight)
                }, CrenellationImage, new[] {-1, 3, 3, -1}, BrushesAndColors.RightBackShadings));
                
                //left front 1
                faces.Add(new Relative3dFace(new[]
                {
                    new Relative3dCoordinate(7f / 8, 0, 0),
                    new Relative3dCoordinate(7f / 8, CrenellationThickness, 0),
                    new Relative3dCoordinate(7f / 8, 0, CrenellationHeight / 2)
                }, CrenellationImage, new[] {3, -1, -1, 3}, BrushesAndColors.LeftFrontShadings));

                //right back 2
                faces.Add(new Relative3dFace(new[]
                {
                    new Relative3dCoordinate(5f / 8, 0, 0),
                    new Relative3dCoordinate(5f / 8, CrenellationThickness, 0),
                    new Relative3dCoordinate(5f / 8, 0, CrenellationHeight / 2)
                }, CrenellationImage, new[] {-1, 4, 4, -1}, BrushesAndColors.RightBackShadings));

                //left front 2
                faces.Add(new Relative3dFace(new[]
                {
                    new Relative3dCoordinate(3f / 8, 0, 0),
                    new Relative3dCoordinate(3f / 8, CrenellationThickness, 0),
                    new Relative3dCoordinate(3f / 8, 0, CrenellationHeight / 2)
                }, CrenellationImage, new[] {4, -1, -1, 4}, BrushesAndColors.LeftFrontShadings));
                
                //right back 3
                faces.Add(new Relative3dFace(new[]
                {
                    new Relative3dCoordinate(1f / 8, 0, 0),
                    new Relative3dCoordinate(1f / 8, CrenellationThickness, 0),
                    new Relative3dCoordinate(1f / 8, 0, CrenellationHeight / 2)
                }, CrenellationImage, new[] {-1, 5, 5, -1}, BrushesAndColors.RightBackShadings));

                //left front 3
                faces.Add(new Relative3dFace(new[]
                {
                    new Relative3dCoordinate(0, 0, 0),
                    new Relative3dCoordinate(0, CrenellationThickness, 0),
                    new Relative3dCoordinate(0, 0, CrenellationHeight)
                }, CrenellationImage, new[] {5, -1, -1, 5}, BrushesAndColors.LeftFrontShadings));
                
                //render inside here, cause idiot me forgot that you can't draw disposed images :)
                g.Draw3dFaces(faces, o, GridDiagonal);
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