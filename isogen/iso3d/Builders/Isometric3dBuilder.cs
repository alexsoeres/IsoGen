using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using IsoGen;

namespace isogen.iso3d.Builders
{
    public class Isometric3d
    {
        
        public class Isometric3dBuilder
        {
            internal Image Base { get; set; }
            internal int GridDiagonal { get; set; }

            public BlockBuilder Block()
            {
                return new BlockBuilder() {TopImage = Base, LeftImage = Base, RightImage = Base, GridDiagonal = GridDiagonal, Height = 1};
            }

            public CrenellatedWallBuilder CrenellatedWall()
            {
                return new CrenellatedWallBuilder(Base, GridDiagonal,
                    CrenellatedWallBuilder.CrenellationVariant.UpperLeft) {LeftImage = Base};
            }
        }
    }
}