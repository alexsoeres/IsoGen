using System.Drawing;
using isogen.iso3d;
using isogen.iso3d.Builders;

namespace IsoGen
{
    internal class Program
    {
        private const int DIAGONAL = 1000;
        
        public static void Main(string[] args)
        {
            /*Image tileSet = Image.FromFile(@"C:\Users\Alex\Desktop\dnd stuff\jungle_fever.png");
            int i = 0;

            Bitmap[,] tiles = tileSet.ExtractTileSet(new Size(64, 64));

            tiles[12, 0].Iso3d(DIAGONAL).Block().Top(tiles[15, 3]).Render().Save("render.png");*/
            
            
            BuildJungle3dTileSet().Render3dTileSet().Save("jungle.png");
            //IntersectTest();
        }

        static void IntersectTest()
        {
            Bitmap[,] source = Image.FromFile(@"C:\Users\Alex\Desktop\dnd stuff\jungle_fever.png")
                .ExtractTileSet(new Size(64, 64));

            source[0, 0].IntersectMask(new[]
            {
                new Point(0, 0),
                new Point(8, 0),
                new Point(8, 8),
                new Point(24, 8),
                new Point(24, 0),
                new Point(40, 0),
                new Point(40, 8),
                new Point(56, 8),
                new Point(56, 0),
                new Point(64, 0),
                new Point(64, 64),
                new Point(0, 64),
            }).Save("intersect.png");
        }

        static Image[,] BuildJungle3dTileSet()
        {
            Bitmap[,] source = Image.FromFile(@"C:\Users\Alex\Desktop\dnd stuff\jungle_fever.png")
                .ExtractTileSet(new Size(64, 64));

            var mossBlock = source[15, 3].Iso3d(DIAGONAL).Block();
            var mossDirtBlock = mossBlock.WithLeftImage(source[12, 0]).WithRightImage(source[12, 1]);
            var dirtBlock = source[10, 1].Iso3d(DIAGONAL).Block().WithRightImage(source[10, 2]).WithTopImage(source[11, 2]);
            var cobbleBlock = source[0, 2].Iso3d(DIAGONAL).Block().WithRightImage(source[0, 3]).WithTopImage(source[1, 3]);
            var rockBlock = source[19, 0].Iso3d(DIAGONAL).Block();
            var rockCrenellatedWall = source[19, 0].Iso3d(DIAGONAL).CrenellatedWall()
                //.WithCrenellation(source[0, 0])
                ;

            return new [,]
            {
                {
                    mossBlock.Render(),
                    mossBlock.WithHeight(0.75f).Render(),
                    mossBlock.WithHeight(0.5f).Render(),
                    mossBlock.WithHeight(0.25f).Render(),
                },
                {
                    mossDirtBlock.Render(),
                    mossDirtBlock.WithHeight(0.75f).Render(),
                    mossDirtBlock.WithHeight(0.5f).Render(),
                    mossDirtBlock.WithHeight(0.25f).Render(),
                },
                {
                    dirtBlock.Render(),
                    dirtBlock.WithHeight(0.75f).Render(),
                    dirtBlock.WithHeight(0.5f).Render(),
                    dirtBlock.WithHeight(0.25f).Render(),
                },
                {
                    cobbleBlock.Render(),
                    cobbleBlock.WithHeight(0.75f).Render(),
                    cobbleBlock.WithHeight(0.5f).Render(),
                    cobbleBlock.WithHeight(0.25f).Render(),
                },
                {
                    rockBlock.Render(),
                    rockBlock.WithHeight(0.75f).Render(),
                    rockBlock.WithHeight(0.5f).Render(),
                    rockBlock.WithHeight(0.25f).Render(),
                },
                {
                    rockCrenellatedWall.Render(),
                    rockCrenellatedWall.WithVariant(CrenellatedWallBuilder.CrenellationVariant.UpperRight).Render(),
                    rockCrenellatedWall.WithVariant(CrenellatedWallBuilder.CrenellationVariant.LowerLeft).Render(),
                    rockCrenellatedWall.WithVariant(CrenellatedWallBuilder.CrenellationVariant.LowerRight).Render(),
                },
                {
                    rockCrenellatedWall.WithVariant(CrenellatedWallBuilder.CrenellationVariant.UpperLeft | CrenellatedWallBuilder.CrenellationVariant.UpperRight).Render(),
                    rockCrenellatedWall.WithVariant(CrenellatedWallBuilder.CrenellationVariant.UpperRight | CrenellatedWallBuilder.CrenellationVariant.LowerRight).Render(),
                    rockCrenellatedWall.WithVariant(CrenellatedWallBuilder.CrenellationVariant.LowerRight | CrenellatedWallBuilder.CrenellationVariant.LowerLeft).Render(),
                    rockCrenellatedWall.WithVariant(CrenellatedWallBuilder.CrenellationVariant.LowerLeft | CrenellatedWallBuilder.CrenellationVariant.UpperLeft).Render(),
                },
            };
        }
    }
}