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

    }
}
