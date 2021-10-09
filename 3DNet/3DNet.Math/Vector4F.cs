namespace _3DNet.Math
{
    public class Vector4F : MatrixBase<Vector4F, Vector4F>
    {
        public Vector4F(float value) : base(value)
        {
        }

        public Vector4F(float[] data) : base(data)
        {
        }

        public float X => this[1, 1];
        public float Y => this[2, 1];
        public float Z => this[3, 1];
        public float U => this[4, 1];

        public override int Rows => 4;

        public override int Cols => 1;

    }
}
