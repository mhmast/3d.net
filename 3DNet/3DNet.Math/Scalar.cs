namespace _3DNet.Math
{
    public class Scalar : MatrixBase<Scalar, Scalar, Scalar>, IVector
    {
        public override int Rows => 1;

        public override int Cols => 1;

        public override Scalar Instance => this;

        public Scalar(float value) : base(value)
        {

        }

        public static implicit operator float(Scalar s) => s[1][1];
        public static implicit operator Scalar(float s) => new Scalar(s);
        public static Scalar operator *(Scalar left, Scalar right) => left * right;

        public override Scalar CreateMatrix(params Scalar[] values)
        => values[0];

        public override Scalar CreateColumn(params Scalar[] values)
        => values[0];

        public override Scalar CreateColumnZeros()
        => 0f;

        public override Scalar CreateRow(params Scalar[] values)
        => values[0];

        public override Scalar CreateRowZeros()
        => 0f;

        public override Scalar CreateMatrixFromColumns(params IVector[] cols)
        => cols[0][0];

        public override Scalar CreateMatrixFromRows(params IVector[] rows)
        => rows[0][0];
    }
}
