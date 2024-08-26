using System;
using System.Numerics;

namespace _3DNet.Math.Extensions
{
    public static class NumericExtensions
    {
        public static Vector3 ToVector3(this Quaternion q) => new Vector3(q.X, q.Y, q.Z);
        public static Matrix4x4 ToMatrix4x4(this Quaternion q) => Matrix4x4.CreateFromQuaternion(q);
        public static Vector3 Normalize(this Vector3 v) => Vector3.Normalize(v);
        public static Quaternion Normalize(this Quaternion q) => Quaternion.Normalize(q);
        public static Vector3 Cross(this Vector3 v, Vector3 other) => Vector3.Cross(v, other);

        public static Vector3 ToEulerAngles(this Quaternion q)
        {
            // pitch (x-axis rotation)
            float sinr_cosp = 2 * (q.W * q.X + q.Y * q.Z);
            float cosr_cosp = 1 - 2 * (q.X * q.X + q.Y * q.Y);
            float pitch = MathF.Atan2(sinr_cosp, cosr_cosp);

            // Yaw (y-axis rotation)
            float sinp = 2 * (q.W * q.Y - q.Z * q.X);
            float yaw;
            if (MathF.Abs(sinp) >= 1)
                yaw = MathF.CopySign(MathF.PI / 2, sinp); // Use 90 degrees if out of range
            else
                yaw = MathF.Asin(sinp);

            // roll (z-axis rotation)
            float siny_cosp = 2 * (q.W * q.Z + q.X * q.Y);
            float cosy_cosp = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);
            float roll = MathF.Atan2(siny_cosp, cosy_cosp);

            // Convert radians to degrees
            return new Vector3(
                pitch * (180.0f / MathF.PI),
                yaw * (180.0f / MathF.PI),
                roll * (180.0f / MathF.PI)
            );
        }
        public static float[] ToFloatArrayRowMajor(this Matrix4x4 m) => new[] { m.M11, m.M12, m.M13, m.M14, m.M21, m.M22, m.M23, m.M24, m.M31, m.M32, m.M33, m.M34, m.M41, m.M42, m.M43, m.M44 };
    }
}
