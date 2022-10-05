using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using isogen.iso3d;
using Brushes = System.Drawing.Brushes;

namespace IsoGen
{
    public static class ImageExtensions
    {
        public static Bitmap[,] ExtractTileSet(this Image image, Size tileSize)
        {
            int cols = image.Width / tileSize.Width;
            int rows = image.Height / tileSize.Height;

            Bitmap[,] images = new Bitmap[rows, cols];
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    int x = tileSize.Width * c;
                    int y = tileSize.Height * r;
                    images[r, c] = image.Crop(new Rectangle(x, y, tileSize.Width, tileSize.Height));
                }
            }

            return images;
        }
        
        public static Bitmap Crop(this Image image, Rectangle r)
        {
            Bitmap nb = new Bitmap(r.Width, r.Height);
            using (Graphics g = Graphics.FromImage(nb))
            {
                g.DrawImage(image, -r.X, -r.Y);
                return nb;
            }
        }

        public static void DrawImage(this Graphics g, Image image, Point[] destinationPoints, Color overlay)
        {
            if (destinationPoints.Length != 3)
            {
                throw new ArgumentException("destinationPoints needs to have exactly 3 elements");
            }

            Point upperLeft = destinationPoints[0];
            Point upperRight = destinationPoints[1];
            Point lowerLeft = destinationPoints[2];
            
            Size delta = new Size(upperRight.X - upperLeft.X, upperRight.Y - upperLeft.Y);

            //Point lowerRight = lowerLeft + delta;
            
            g.DrawImageV2(image.Overlay(overlay), destinationPoints);
        }

        public static Image Overlay(this Image image, Color o)
        {
            Bitmap i = new Bitmap(image);
            for(int y = 0; y < i.Height; y++)
            {
                for(int x = 0; x < i.Width; x++)
                {
                    Color p = i.GetPixel(x, y);

                    float aF = o.A / 255f;
                    short newR = (short)(p.R * (1 - aF) + o.R * aF);
                    short newG = (short)(p.G * (1 - aF) + o.G * aF);
                    short newB = (short)(p.B * (1 - aF) + o.B * aF);

                    Color pNew = Color.FromArgb(p.A, newR, newG, newB);
                    i.SetPixel(x, y, pNew);
                }
            }

            return i;
        }

        public static Bitmap Render3dTileSet(this Image[,] tiles)
        {
            int tileHeight = Flatten(tiles).Max(x => x.Height);
            int tileWidth = Flatten(tiles).Max(x => x.Width);
            int rows = tiles.GetLength(0);
            int cols = tiles.GetLength(1);

            Bitmap res = new Bitmap(tileWidth * cols, tileHeight * rows);
            
            Graphics g = Graphics.FromImage(res);
            for (int r = 0; r < tiles.GetLength(0); r++) {
                for (int c = 0; c < tiles.GetLength(1); c++)
                {
                    Image tile = tiles[r, c];
                    int x = c * tileWidth;
                    int y = (r * tileHeight) + tileHeight - tile.Height;
                    
                    g.DrawImage(tile, x, y);
                }
            }

            return res;
        }
        
        //todo: move somewhere else
        private static IEnumerable<T> Flatten<T>(T[,] map) {
            for (int row = 0; row < map.GetLength(0); row++) {
                for (int col = 0; col < map.GetLength(1); col++) {
                    yield return map[row,col];
                }
            }
        }

        public static Bitmap IntersectMask(this Image image, Point[] polygon, bool inverse = false)
        {
            Bitmap mask = new Bitmap(image.Width, image.Height);
            using (Graphics g = Graphics.FromImage(mask))
            {
                g.SmoothingMode = SmoothingMode.None;
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.FillPolygon(Brushes.Black, polygon);
            }
            return image.IntersectMask(mask, inverse);
        }
        
        public static Bitmap IntersectMask(this Image image, Image mask, bool inverse = false)
        {
            using (Bitmap m = new Bitmap(mask))
            using (Bitmap i = new Bitmap(image))
            {
                if (image.Size != mask.Size)
                {
                    throw new NotSupportedException("this configuration of image sizes is not supported yet");
                }

                Bitmap b = new Bitmap(image.Width, image.Height);
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Height; x++)
                    {
                        byte alpha = m.GetPixel(x, y).A;
                        if (inverse)
                        {
                            alpha = (byte)(byte.MaxValue - alpha);
                        }
                        if (alpha != 0)
                        {
                            b.SetPixel(x, y, i.GetPixel(x, y).WithAlpha(alpha));
                        }
                    }
                }

                return b;
            }
        }
        public static void DrawImage(this Graphics g, Image image, Relative3dPoint[] ps, int d)
        {
            if (ps.Length != 3)
            {
                throw new NotSupportedException("only three points supported for DrawImage with Relative3dPoints");
            }

            Point[] translated = ps.Select(x => x.Translate3dCoordinate(d)).ToArray();

            g.DrawImageV2(image, translated);
        }

        public static void DrawImage(this Graphics g, Image image, Relative3dPoint[] ps, int d, Color overlay)
        {
            if (ps.Length != 3)
            {
                throw new NotSupportedException("only three points supported for DrawImage with Relative3dPoints");
            }

            Point[] translated = ps.Select(x => x.Translate3dCoordinate(d)).ToArray();

            g.DrawImage(image, translated, overlay);
        }

        public static void DrawImageV2(this Graphics g, Image image, Point[] ps)
        {
            using (Bitmap canvas = new Bitmap((int) g.VisibleClipBounds.Width, (int) g.VisibleClipBounds.Height))
            {
                canvas.DrawImageV2(image, ps);
                g.DrawImage(canvas, 0, 0);
            }
        }

        /**
         * Here's where the magic happens
         */
        public static void DrawImageV2(this Bitmap canvas, Image image, Point[] ps)
        {
            using (Bitmap imgRef = new Bitmap(image))
            {
                if (ps.Length != 3)
                {
                    throw new NotSupportedException("only three points supported for DrawImageV2");
                }

                int lineLength = ps[1].X - ps[0].X; //positive = moves right
                int lineCount = ps[2].Y - ps[0].Y; //positive = moves down
                int xInc = lineLength > 0 ? 1 : -1; //todo: might get error for lineLength = 0
                int yInc = lineCount > 0 ? 1 : -1; //todo: might get error for lineLength = 0

                float m = ((float) ps[1].Y - ps[0].Y) / (ps[1].X - ps[0].X);
                float m2 = ((float) ps[2].Y - ps[0].Y) / (ps[2].X - ps[0].X);
                float b = ps[0].Y - (m * ps[0].X); //I think this should be an integer number, not sure tho

                //loop through lines and then through y offset //actually opposite nevermind

                for (int yOffset = 0; Math.Abs(yOffset) < Math.Abs(lineCount); yOffset += yInc)
                {
                    for (int xOffset = 0; Math.Abs(xOffset) < Math.Abs(lineLength); xOffset += xInc)
                    {
                        int xOffsetOffset = (int)((yOffset) / m2);
                        //xOffsetOffset *= 0;
                        int xC = ps[0].X + xOffset;
                        float yCRaw = (int) (m * xC + b + yOffset); //should be an integer number aswell //ommitted  + yOffset
                        int yC = Convert.ToInt32(yCRaw); //round to nearest int value

                        int xImg = (xOffset * image.Width) / lineLength;
                        int yImg = (yOffset * image.Height) / lineCount;

                        Color currentPixel = imgRef.GetPixel(xImg, yImg);

                        canvas.SetPixel(Math.Min(Math.Max(xC + xOffsetOffset, 0), canvas.Width - 1),
                            Math.Min(Math.Max(yC, 0), canvas.Height - 1), currentPixel);
                        canvas.SetPixel(Math.Min(Math.Max(xC + xOffsetOffset, 0), canvas.Width - 1), Math.Min(Math.Max(yC + 1, 0), canvas.Height - 1), currentPixel); //also set bottom pixel (quick fix for skipped pixels
                        canvas.SetPixel(Math.Min(Math.Max(xC + xOffsetOffset + 1, 0), canvas.Width - 1), Math.Min(Math.Max(yC, 0), canvas.Height - 1), currentPixel); //also set right pixel (quick fix for skipped pixels
                    }
                }
            }
        }
        
        public static Point Translate3dCoordinate(this Relative3dPoint p, int d)
        {
            int hd = d / 2;
            
            //starting with x coordinate
            float resX = p.X * hd;
            float resY = hd - (p.X * hd);
            
            //add y coordinate
            resX += p.Y * hd;
            resY += p.Y * hd;
            
            //add z coordinate
            resY += hd * p.Z;

            return new Point((int) resX, (int) resY);
        }

        public static void Buffer(this Graphics g, Action<Bitmap, Graphics> bufferFunc)
        {
            using (Bitmap buffer = new Bitmap((int) g.VisibleClipBounds.Width, (int) g.VisibleClipBounds.Height))
            using (Graphics gBuffer = Graphics.FromImage(buffer))
            {
                bufferFunc(buffer, gBuffer);
                g.DrawImage(buffer, 0, 0);
            }
        }
    }
}