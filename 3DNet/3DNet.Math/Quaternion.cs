namespace _3DNet.Math
{
    public class Quaternion : Vector4F
    {
        public static Quaternion Identity=> new Quaternion(new[] { 0f, 0, 0, 1 });

        private Quaternion(Vector4F data) : base(data.Data) { }
        public Quaternion(float value) : base(value)
        {
        }

        public Quaternion(float[] data) : base(data)
        {
        }

        public static Quaternion CreateFromAxisAngle(Vector3F axis, float angle)
        {
            var halfAngle = angle * 0.5f;
            var s = (float)System.Math.Sin(halfAngle);
            var c = (float)System.Math.Cos(halfAngle);
            return new Quaternion((axis.X * s, axis.Y * s, axis.Y * s,c));
        }

        public Quaternion Concatenate(Quaternion other)
        {
           
            // Concatenate rotation is actually q2 * q1 instead of q1 * q2.
            // So that's why value2 goes q1 and value1 goes q2.
            var q1x = other.X;
            var q1y = other.Y;
            var q1z = other.Z;
            var q1w = other.W;

            var q2x = X;
            var q2y = Y;
            var q2z = Z;
            var q2w = W;

            // cross(av, bv)
            var cx = q1y * q2z - q1z * q2y;
            var cy = q1z * q2x - q1x * q2z;
            var cz = q1x * q2y - q1y * q2x;
            var dot = q1x * q2x + q1y * q2y + q1z * q2z;

            return new Quaternion((q1x * q2w + q2x * q1w + cx, q1y * q2w + q2y * q1w + cy, q1z * q2w + q2z * q1w + cz, q1w * q2w - dot));
        }

        public Matrix4x4 ToMatrix4x4()
        {

            var xx = X * X;
            var yy = Y * Y;
            var zz = Z * Z;
            var xy = X * Y;
            var wz = Z * W;
            var xz = Z * X;
            var wy = Y * W;
            var yz = Y * Z;
            var wx = X * W;

            return new Matrix4x4(
                (1.0f - 2.0f * (yy + zz), 2.0f * (xy + wz), 2.0f * (xz - wy), 0.0f),
                (2.0f * (xy - wz), 1.0f - 2.0f * (zz + xx), 2.0f * (yz + wx), 0.0f),
                (2.0f * (xz + wy), 2.0f * (yz - wx), 1.0f - 2.0f * (yy + xx), 0.0f),
                (0.0f, 0.0f, 0.0f, 1.0f)
                );
        }

        public static Quaternion operator +(Quaternion left,Quaternion right)=>left.Concatenate(right);
        public static implicit operator Quaternion((float,float,float,float) data)=>new Quaternion(data);
    }
}
