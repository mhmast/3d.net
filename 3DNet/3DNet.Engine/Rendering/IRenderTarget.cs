using _3DNet.Math;
using System.Drawing;

namespace _3DNet.Engine.Rendering
{
    public interface IRenderTarget
    {
        Matrix4x4 Projection { get; }
        bool IsDisposed { get; }

        void Clear(Color clearColor);
        void Present();
    }
}