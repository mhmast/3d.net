using System.Numerics;

namespace _3DNet.Engine.Extensions
{
    internal static class NumericExtensions
    {
        public static Vector3 ToVector3(this Quaternion q)=> new Vector3(q.X,q.Y, q.Z); 
        public static Matrix4x4 ToMatrix4x4(this Quaternion q)=> Matrix4x4.CreateFromQuaternion(q); 
        public static Vector3 Normalize(this Vector3 v)=> Vector3.Normalize(v); 
        public static Vector3 Cross(this Vector3 v,Vector3 other)=> Vector3.Cross(v,other); 
    }
}
