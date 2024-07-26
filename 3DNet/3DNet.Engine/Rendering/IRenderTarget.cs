using System.Drawing;
using System.Numerics;

namespace _3DNet.Engine.Rendering
{
    public interface IRenderTarget
    {
        Matrix4x4 Projection { get; }
        bool IsDisposed { get; }

        void Clear(IRenderContextInternal context, Color clearColor);
        void Present();
    }
}