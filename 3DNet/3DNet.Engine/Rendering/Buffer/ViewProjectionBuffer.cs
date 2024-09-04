using System.Diagnostics.CodeAnalysis;
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

    public override readonly bool Equals([NotNullWhen(true)] object obj)
    {
        if (obj is ViewProjectionBuffer buf)
        {
            return view.Equals(buf.view) && projection.Equals(buf.projection);
        }
        return false;
    }

    public override readonly int GetHashCode() => view.GetHashCode() ^ projection.GetHashCode();

    public static bool operator ==(ViewProjectionBuffer left, ViewProjectionBuffer right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ViewProjectionBuffer left, ViewProjectionBuffer right)
    {
        return !(left == right);
    }
}
