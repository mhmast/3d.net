using System.Numerics;

namespace _3DNet.Engine.Rendering.Buffer;
public struct ViewProjectionBuffer
{
    public ViewProjectionBuffer()
    {
        view = Matrix4x4.Identity;
        projection = Matrix4x4.Identity;
    }
    public Matrix4x4 view;
    public Matrix4x4 projection;
}
