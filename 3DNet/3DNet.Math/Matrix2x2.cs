using System.Linq;
using static System.Math;

namespace _3DNet.Math
{
    public class Matrix2x2 : MatrixBase<Matrix2x2, Vector2F, Vector2F>
    {
        public Matrix2x2(params Vector2F[] values) : base(values)
        {

        }

        private Matrix2x2(params Scalar[] values) : base(values)
        {
        }

        public override int Rows => 2;

        public override int Cols => 2;

        public override Matrix2x2 Instance => this;

        public static Matrix2x2 Rotate(int degrees)
        {
            var radian = degrees * 0.0174532925;
            return new Matrix2x2(((float)Cos(radian), (float)-Sin(radian)), ((float)Sin(radian), (float)Cos(radian)));
        }

        public override Vector2F CreateColumn(params Scalar[] values)
        => new Vector2F(values);

        public override Vector2F CreateColumnZeros()
        => (0, 0);

        public override Matrix2x2 CreateMatrix(params Scalar[] values)
        => new Matrix2x2(values);

        public override Matrix2x2 CreateMatrixFromColumns(params IVector[] cols)
        => new Matrix2x2(cols.SelectMany(cols => cols.Data).ToArray()).Transpose();

        public override Matrix2x2 CreateMatrixFromRows(params IVector[] rows)
        => new Matrix2x2(rows.SelectMany(cols => cols.Data).ToArray());

        public override Vector2F CreateRow(params Scalar[] values)
       => new Vector2F(values);

        public override Vector2F CreateRowZeros()
        => (0,0);
    }
}
