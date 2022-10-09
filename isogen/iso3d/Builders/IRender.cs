using System.Drawing;

namespace isogen.iso3d.Builders
{
    public interface IRender
    {
        Image Render(Orientation o = Orientation.TopLeft);
    }
}