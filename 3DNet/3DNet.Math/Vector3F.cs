namespace _3DNet.Math
{
    public class Vector3F : MatrixBase<Vector3F, Scalar, Vector3F>, IVector
    {
        public Vector3F(params Scalar[] values) : base(values)
        {
        }
        public Vector3F(Scalar value) : base(value)
        {
        }

        public static Vector3F Up => new Vector3F(0f, 1f, 0f);
        public float X => this[1][1];
        public float Y => this[2][1];
        public float Z => this[3][1];

        public override int Rows => 3;

        public override int Cols => 3;

        public override Vector3F Instance => this;

        public Vector3F Normalize()
        {
            var min = Min();
            return (X / min, Y / min, Z / min);
        }

        public Vector3F Cross(Vector3F vector2)
        => (Y * vector2.Z - Z * vector2.Y,
            Z * vector2.X - X * vector2.Z,
            X * vector2.Y - Y * vector2.X);


        public Vector4F ToVerctor4F(float w = 0)
        => new Vector4F(X, Y, Z, w);

        public override Vector3F CreateMatrix(params Scalar[] values)
        => new Vector3F(values);

        public override Vector3F CreateColumn(params Scalar[] values)
        => new Vector3F(values);

        public override Vector3F CreateColumnZeros()
        => new Vector3F(0);

        public override Scalar CreateRow(params Scalar[] values)
        => values[0];

        public override Scalar CreateRowZeros()
        =>0f;

        public override Vector3F CreateMatrixFromColumns(params IVector[] cols)
        => new Vector3F(cols[0].Data);

        public override Vector3F CreateMatrixFromRows(params IVector[] rows)
        => new Vector3F(rows[0].Data);

        public static implicit operator Vector3F((float, float, float) vals)
        {
            return new Vector3F(vals.Item1, vals.Item2, vals.Item3);
        }

    }
}
