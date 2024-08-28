using System;
using System.Drawing;

namespace _3DNet.Engine.Rendering
{
    public interface IRenderEngine : IDisposable
    {
        void Initialize();
        IRenderContext CreateRenderContext(string name, Size size, bool fullScreen, Action<IRenderContextInternal> setActive);
    }
}
