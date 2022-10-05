namespace isogen.iso3d
{
    public readonly struct Relative3dPoint
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

        public Relative3dPoint(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public bool Equals(Relative3dPoint other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
        }

        public override bool Equals(object obj)
        {
            return obj is Relative3dPoint other && Equals(other);
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

        public static Relative3dPoint operator +(Relative3dPoint first, Relative3dPoint second)
        {
            return new Relative3dPoint(first.X + second.X, first.Y + second.Y, first.Z + second.Z);
        }

        public static Relative3dPoint operator -(Relative3dPoint first, Relative3dPoint second)
        {
            return new Relative3dPoint(first.X - second.X, first.Y - second.Y, first.Z - second.Z);
        }

        public static Relative3dPoint operator *(Relative3dPoint first, Relative3dPoint second)
        {
            return new Relative3dPoint(first.X * second.X, first.Y * second.Y, first.Z * second.Z);
        }

        public static Relative3dPoint operator /(Relative3dPoint first, Relative3dPoint second)
        {
            return new Relative3dPoint(first.X / second.X, first.Y / second.Y, first.Z / second.Z);
        }

        public static bool operator ==(Relative3dPoint? first, Relative3dPoint? second)
        {
            return first?.Equals(second) ?? second == null;
        }

        public static bool operator !=(Relative3dPoint? first, Relative3dPoint? second)
        {
            return !(first?.Equals(second) ?? second == null);
        }
    }
}