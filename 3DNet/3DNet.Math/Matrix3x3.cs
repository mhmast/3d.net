using System;
using System.Linq;
using static System.Math;

namespace _3DNet.Math
{
    public class Matrix3x3 : MatrixBase<Matrix3x3, Vector3F, Vector3F>
    {
        public override int Rows => 3;

        public override int Cols => 3;

        public override Matrix3x3 Instance => this;

        private Matrix3x3(params Scalar[] data) : base(data) { }
        public Matrix3x3(params Vector3F[] rows) : base(rows)
        {

        }

        public Matrix4x4 To4x4()
        => new Matrix4x4(this[1].ToVerctor4F(), this[2].ToVerctor4F(), this[3].ToVerctor4F(), (0f, 0f, 0f, 1f ));

        public static Matrix3x3 RotateX(int degrees)
        {
            var radian = degrees * 0.0174532925;
            return new Matrix3x3(
                (1f, 0f, 0f),
                (0f, (float)Cos(radian), (float)-Sin(radian)),
                (0f, (float)Sin(radian), (float)Cos(radian))
            );
        }

        public static Matrix3x3 RotateY(int degrees)
        {
            var radian = degrees * 0.0174532925;
            return new Matrix3x3(
                ((float)Cos(radian), 0f, (float)-Sin(radian)),
                (0f, 1f, 0f),
                ((float)Sin(radian), 0f, (float)Cos(radian))
            );
        }

        public static Matrix3x3 RotateZ(int degrees)
        {
            var radian = degrees * 0.0174532925;
            return new Matrix3x3(
                ((float)Cos(radian), (float)-Sin(radian), 0f),
                ((float)Sin(radian), (float)Cos(radian), 0f),
                (0f, 0f, 1f)
            );
        }

        public override Matrix3x3 CreateMatrix(params Scalar[] values)
        => new Matrix3x3(values);

        public override Vector3F CreateColumn(params Scalar[] values)
        => new Vector3F(values);

        public override Vector3F CreateColumnZeros()
        => new Vector3F(0, 0, 0);

        public override Vector3F CreateRow(params Scalar[] values)
        => new Vector3F(values);

        public override Vector3F CreateRowZeros()
        => new Vector3F(0, 0, 0);

        public override Matrix3x3 CreateMatrixFromColumns(params IVector[] cols)
        => new Matrix3x3(cols.SelectMany(c=>c.Data).ToArray()).Transpose();

        public override Matrix3x3 CreateMatrixFromRows(params IVector[] rows)
         => new Matrix3x3(rows.SelectMany(c => c.Data).ToArray());
    }
}
