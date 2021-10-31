using static System.Math;

namespace _3DNet.Math
{
    public class Matrix3x3 : MatrixBase<Matrix3x3, Vector3F>
    {
        public Matrix3x3(float value) : base(value)
        {
        }

        public Matrix3x3(float[] data) : base(data)
        {
        }
        public Matrix3x3(float[][] data) : base(data)
        {
        }

        public override int Rows => 3;

        public override int Cols => 3;

        public Matrix4x4 To4x4()
        => new Matrix4x4(Row(1).ToVerctor4F(), Row(2).ToVerctor4F(), Row(3).ToVerctor4F(), new Vector4F(new[] { 0f, 0f, 0f, 1f }));

        public static Matrix3x3 RotateX(int degrees)
        {
            var radian = degrees * 0.0174532925;
            return new Matrix3x3(new[]
            {
                1f, 0f, 0f,
                0f,  (float)Cos(radian), (float)-Sin(radian),
                0f,  (float)Sin(radian), (float)Cos(radian)
            });
        }

        public static Matrix3x3 RotateY(int degrees)
        {
            var radian = degrees * 0.0174532925;
            return new Matrix3x3(new[]
            {
                (float)Cos(radian), 0f,(float)-Sin(radian),
                0f, 1f  ,0f,
                (float)Sin(radian), 0f,  (float)Cos(radian)
            });
        }

        public static Matrix3x3 RotateZ(int degrees)
        {
            var radian = degrees * 0.0174532925;
            return new Matrix3x3(new[]
            {
                (float)Cos(radian), (float)-Sin(radian), 0f,
                (float)Sin(radian), (float)Cos(radian),0f,
                0f,  0f,1f
            });
        }

    }
}
