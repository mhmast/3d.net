using System;

namespace _3DNet.Math
{
    public class Vector4F : MatrixBase<Vector4F, Scalar, Scalar>, IVector
    {

        public Vector4F(params Scalar[] scalars) : base(scalars)
        {
        }

        public float X => this[1][1];
        public float Y => this[2][1];
        public float Z => this[3][1];
        public float W => this[4][1];

        public override int Rows => 4;

        public override int Cols => 1;

        public override Vector4F Instance => throw new NotImplementedException();

        public static Vector4F operator -(Vector4F me) => (-me.X, -me.Y, -me.Z, -me.W);

        public static implicit operator Vector4F((float, float, float, float) input)
        => new Vector4F(input.Item1, input.Item2, input.Item3, input.Item4);

        public Vector3F ToVector3F()
        => (X, Y, Z);

        public override Vector4F CreateMatrix(params Scalar[] values)
        => new Vector4F(values);

        public override Scalar CreateColumn(params Scalar[] values)
        => values[0];

        public override Scalar CreateColumnZeros()
        => 0f;

        public override Scalar CreateRow(params Scalar[] values)
        => values[0];

        public override Scalar CreateRowZeros()
        => 0f;

        public override Vector4F CreateMatrixFromColumns(params IVector[] cols)
        => new Vector4F(cols[0].Data);

        public override Vector4F CreateMatrixFromRows(params IVector[] rows)
        => new Vector4F(rows[0].Data);

    }
}
