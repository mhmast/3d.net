using System;

namespace _3DNet.Math
{
    public class Vector3F : MatrixBase<Vector3F, Vector3F>
    {
        public Vector3F(float value) : base(value)
        {
        }

        public Vector3F(float[] data) : base(data)
        {
        }

        public static Vector3F Up => new Vector3F(new[] { 0f, 1f, 0f });
        public float X => this[1, 1];
        public float Y => this[2, 1];
        public float Z => this[3, 1];

        public Vector4F ToVerctor4F()
        => new Vector4F(new[] { X, Y, Z, 0f });

        public override int Rows => 3;

        public override int Cols => 1;

        public static implicit operator Vector3F((float, float, float) vals)
        {
            return new Vector3F(new[] { vals.Item1,vals.Item2,vals.Item3});
        }

    }
}
