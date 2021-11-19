using System;
using static System.Math;

namespace _3DNet.Math
{
    public class Matrix4x4 : MatrixBase<Matrix4x4, Vector4F,Vector4F>
    {
        public Matrix4x4(params Vector4F[] rows) : base(rows)
        {
        }
        public static Matrix4x4 Identity => new Matrix4x4((1, 0, 0, 0), (0, 1, 0, 0), (0, 0, 1, 0), (0, 0, 0, 1));

        public override int Rows => 4;

        public override int Cols => 4;

        public override Matrix4x4 Instance => this;

        public static Matrix4x4 Translate(Vector3F position)
        => new Matrix4x4((1, 0, 0, position.X), (0, 1, 0, position.Y), (0, 0, 1, position.Z), (0, 0, 0, 1));

        public static Matrix4x4 Scale(Vector3F scaleFactors)
        => new Matrix4x4((scaleFactors.X, 0, 0, 0), (0, scaleFactors.Y, 0, 0), (0, 0, scaleFactors.Z, 0), (0, 0, 0, 1));

        public static Matrix4x4 PerspectiveFovLH(float fovY, float aspectRatio, float zn, float zf)
        {
            var yScale = Cot(fovY / 2);
            var xScale = yScale / aspectRatio;
            return new Matrix4x4(
                (xScale, 0, 0, 0),
                (0, yScale, 0, 0),
                (0, 0, zf / (zf - zn), 1),
                (0, 0, -zn * zf / (zf - zn), 0)
            );
        }

        private static float Cot(float v)
        => 1f / (float)Tan(v);

        public override Matrix4x4 CreateMatrix(params Scalar[] values)
        {
            throw new NotImplementedException();
        }

        public override Vector4F CreateColumn(params Scalar[] values)
        {
            throw new NotImplementedException();
        }

        public override Vector4F CreateColumnZeros()
        {
            throw new NotImplementedException();
        }

        public override Vector4F CreateRow(params Scalar[] values)
        {
            throw new NotImplementedException();
        }

        public override Vector4F CreateRowZeros()
        {
            throw new NotImplementedException();
        }

        public override Matrix4x4 CreateMatrixFromColumns(params IVector[] cols)
        {
            throw new NotImplementedException();
        }

        public override Matrix4x4 CreateMatrixFromRows(params IVector[] rows)
        {
            throw new NotImplementedException();
        }
    }
}
