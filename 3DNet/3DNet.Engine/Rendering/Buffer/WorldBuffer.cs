using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace _3DNet.Engine.Rendering.Buffer;
public struct WorldBuffer
{
    public WorldBuffer()
    {
        world = Matrix4x4.Identity;

    }
    public Matrix4x4 world;

    public override readonly bool Equals([NotNullWhen(true)] object obj)
    {
        if(obj is WorldBuffer buf)
        {
            return buf.world == world;
        }
        return false;
    }

    public override readonly int GetHashCode() => world.GetHashCode();

    public static bool operator ==(WorldBuffer left, WorldBuffer right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(WorldBuffer left, WorldBuffer right)
    {
        return !(left == right);
    }
}
