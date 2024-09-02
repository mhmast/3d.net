using System.Numerics;

namespace _3DNet.Engine.Rendering.Buffer;
public struct WorldBuffer
{
    public WorldBuffer()
    {
        world = Matrix4x4.Identity;

    }
    public Matrix4x4 world;
}
