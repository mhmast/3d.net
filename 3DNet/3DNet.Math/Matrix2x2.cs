using static System.Math;

namespace _3DNet.Math
{
    public class Matrix2x2 : MatrixBase<Matrix2x2, PointF>
    {
        public Matrix2x2(float value) : base(value)
        {
        }

        public Matrix2x2(float[] data) : base(data)
        {
        }
        public Matrix2x2(float[][] data) : base(data)
        {
        }

        public override int Rows => 2;

        public override int Cols => 2;

        public static Matrix2x2 Rotate(int degrees)
        {
            var radian = degrees * 0.0174532925;
            return new Matrix2x2( new[] { (float)Cos(radian), (float)-Sin(radian) , (float)Sin(radian), (float)Cos(radian) } );
        }

    }
}
