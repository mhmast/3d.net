using System.Numerics;

namespace _3DNet.Engine.Rendering.Buffer;
public struct WvpBuffer
{
    public WvpBuffer()
    {
        world = Matrix4x4.Identity;
        view = Matrix4x4.Identity;
        projection = Matrix4x4.Identity;
    }
    public Matrix4x4 world;
    public Matrix4x4 view;
    public Matrix4x4 projection;
}
