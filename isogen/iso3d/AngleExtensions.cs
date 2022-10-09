namespace isogen.iso3d
{
    public static class AngleExtensions
    {
        public static float GetOffset(this Orientation orientation)
        {
            switch (orientation)
            {
                case Orientation.TopLeft:
                    return 0;
                case Orientation.BottomLeft:
                    return 270; //rotations are inverted in code cause 
                case Orientation.BottomRight:
                    return 180;
                case Orientation.TopRight:
                    return 90;
                default:
                    return 0;
            }
        }
    }
}