namespace _3DNet.Math
{
    public class Vector3F : MatrixBase<Vector3F, Vector3F>
    {
        public Vector3F(float value) : base(value)
        {
        }

        public Vector3F(float[] data) : base(data)
        {
        }
        public Vector3F(float[][] data) : base(data)
        {
        }

        public static Vector3F Up => new Vector3F(new[] { 0f, 1f, 0f });
        public float X => this[1, 1];
        public float Y => this[2, 1];

        public Vector3F Normalize()
        {
            var min = Min();
            return (X / min, Y / min, Z / min);
        }

        public Vector3F Cross(Vector3F vector2)
        => (Y * vector2.Z - Z * vector2.Y,
            Z * vector2.X - X * vector2.Z,
            X * vector2.Y - Y * vector2.X);

        public float Z => this[3, 1];

        public Vector4F ToVerctor4F(float w=0)
        => new Vector4F(new[] { X, Y, Z, w });

        public override int Rows => 3;

        public override int Cols => 1;

        public static Vector3F operator -(Vector3F me) => (-me.X, -me.Y, -me.Z);

        public static implicit operator Vector3F((float, float, float) vals)
        {
            return new Vector3F(new[] { vals.Item1, vals.Item2, vals.Item3 });
        }

    }
}
