using System.Drawing;
using isogen.iso3d.Builders;

namespace isogen.iso3d
{
    public static class Isometric3dExtensions
    {
        public static Isometric3d.Isometric3dBuilder Iso3d(this Image image, int diagonal)
        {
            return new Isometric3d.Isometric3dBuilder() {Base = image, GridDiagonal = diagonal};
        }

        public static Size CalculateBounds(int diagonal, float height)
        {
            return new Size(diagonal, diagonal + (int) (diagonal * 0.5 * height));
        }
    }
}