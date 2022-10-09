using System;
using System.Drawing;

namespace isogen.iso3d.Builders
{
    public struct Relative3dFace
    {
        /// <summary>
        /// Needs to be exactly 3 Points
        /// </summary>
        public Relative3dCoordinate[] Points { get; }
        public Image Image { get; }

        /// <summary>
        /// Order per orientation starting from TopLeft, the lower numbers are rendered first, negative number = deactivated (when not visible for example)
        /// </summary>
        public int[] RenderingOrders { get; }
        
        /// <summary>
        /// Shading per orientation starting from TopLeft, if null none is applied
        /// </summary>
        public Color?[] Shadings { get; }

        public Relative3dFace(Relative3dCoordinate[] points, Image image, int[] renderingOrders)
        {
            Points = points;
            Image = image;
            RenderingOrders = renderingOrders;
            Shadings = new Color?[] { null, null, null, null};
        }
        
        public Relative3dFace(Relative3dCoordinate[] points, Image image, int[] renderingOrders, Color?[] shadings)
        {
            Points = points;
            Image = image;
            RenderingOrders = renderingOrders;
            Shadings = shadings;
        }
    }
}