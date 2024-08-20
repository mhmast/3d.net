using System.Numerics;

namespace _3DNet.Math.Extensions
{
    public static class NumericExtensions
    {
        public static Vector3 ToVector3(this Quaternion q) => new Vector3(q.X, q.Y, q.Z);
        public static Matrix4x4 ToMatrix4x4(this Quaternion q) => Matrix4x4.CreateFromQuaternion(q);
        public static Vector3 Normalize(this Vector3 v) => Vector3.Normalize(v);
        public static Vector3 Cross(this Vector3 v, Vector3 other) => Vector3.Cross(v, other);
        public static float[] ToFloatArrayRowMajor(this Matrix4x4 m) => new[] { m.M11, m.M12, m.M13, m.M14, m.M21, m.M22, m.M23, m.M24, m.M31, m.M32, m.M33, m.M34, m.M41, m.M42, m.M43, m.M44 };
    }
}
