using System;

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
        public Vector4F(float[][] data) : base(data)
        {
        }

        public float X => this[1, 1];
        public float Y => this[2, 1];
        public float Z => this[3, 1];
        public float W => this[4, 1];

        public override int Rows => 4;
        public override int Cols => 1;

        public static Vector4F operator -(Vector4F me) => (-me.X, -me.Y, -me.Z, -me.W);

        public static implicit operator Vector4F((float,float,float,float) input)
        => new Vector4F(new[] {input.Item1,input.Item2, input.Item3, input.Item4 });

        public Vector3F ToVector3F()
        => (X, Y, Z);
    }
}
