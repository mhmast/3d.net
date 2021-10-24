using System;
using static System.Math;

namespace _3DNet.Math
{
    public class Matrix4x4 : MatrixBase<Matrix4x4, Vector4F>
    {
        public Matrix4x4(float value) : base(value)
        {
        }

        public Matrix4x4(float[] data) : base(data)
        {
        }

        public Matrix4x4(params Vector4F[] rows) : base(rows)
        {
        }

        public override int Rows => 4;

        public override int Cols => 4;

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
    }
}
