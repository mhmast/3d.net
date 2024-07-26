using System.Drawing;

namespace _3DNet.Engine.Rendering
{
    public interface IRenderContextFactory
    {
        IRenderContext CreateRenderContext(string name, Size size, bool fullScreen);
    }
}