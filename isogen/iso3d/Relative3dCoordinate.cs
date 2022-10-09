using System;

namespace isogen.iso3d
{
    /// <summary>
    /// Represents a coordinate where the values are relative to its coordinate system's boundaries
    /// </summary>
    public readonly struct Relative3dCoordinate
    {
        /// <summary>
        /// X Coordinate as a relative unit, 0 corresponds to very left and 1 to very right
        /// </summary>
        public float X { get; }

        /// <summary>
        /// Y Coordinate as a relative unit, 0 corresponds to very left and 1 to very right
        /// </summary>
        public float Y { get; }

        /// <summary>
        /// Z Coordinate as a relative unit, 0 corresponds to very left and 1 to very right
        /// </summary>
        public float Z { get; }

        public Relative3dCoordinate(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public bool Equals(Relative3dCoordinate other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
        }

        public override bool Equals(object obj)
        {
            return obj is Relative3dCoordinate other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                return hashCode;
            }
        }

        public static Relative3dCoordinate operator +(Relative3dCoordinate first, Relative3dCoordinate second)
        {
            return new Relative3dCoordinate(first.X + second.X, first.Y + second.Y, first.Z + second.Z);
        }

        public static Relative3dCoordinate operator -(Relative3dCoordinate first, Relative3dCoordinate second)
        {
            return new Relative3dCoordinate(first.X - second.X, first.Y - second.Y, first.Z - second.Z);
        }

        public static Relative3dCoordinate operator *(Relative3dCoordinate first, Relative3dCoordinate second)
        {
            return new Relative3dCoordinate(first.X * second.X, first.Y * second.Y, first.Z * second.Z);
        }

        public static Relative3dCoordinate operator /(Relative3dCoordinate first, Relative3dCoordinate second)
        {
            return new Relative3dCoordinate(first.X / second.X, first.Y / second.Y, first.Z / second.Z);
        }

        public static bool operator ==(Relative3dCoordinate? first, Relative3dCoordinate? second)
        {
            return first?.Equals(second) ?? second.Equals(null);
        }

        public static bool operator !=(Relative3dCoordinate? first, Relative3dCoordinate? second)
        {
            return !(first?.Equals(second) ?? second.Equals(null));
        }

        /// <summary>
        /// Rotates a coordinate around the z axis
        /// </summary>
        /// <param name="angle">angle in degrees</param>
        /// <returns>new point</returns>
        public Relative3dCoordinate Rotate(float angle)
        {
            return Rotate(angle, new Relative3dCoordinate(0.5f, 0.5f, 0));
        }

        /// <summary>
        /// Rotates a coordinate around the point m in the direction of the z axis
        /// </summary>
        /// <param name="angle">angle in degrees</param>
        /// <param name="m">middle point</param>
        /// <returns>new point</returns>
        public Relative3dCoordinate Rotate(float angle, Relative3dCoordinate m)
        {
            double angleRad = angle * Math.PI * 2 / 360;
            float akX = this.X - m.X;
            float gkY = this.Y - m.Y;
            double radius = Math.Sqrt(Math.Pow(Math.Abs(akX), 2) + Math.Pow(Math.Abs(gkY), 2));
            //all angles from here are radians
            double alpha = Math.Atan2(gkY, akX);
            double newAlpha = angleRad + alpha;
            float newX = m.X + (float) (Math.Cos(newAlpha) * radius);
            float newY = m.Y + (float) (Math.Sin(newAlpha) * radius);
            return new Relative3dCoordinate(newX, newY, this.Z);
        }
    }
}