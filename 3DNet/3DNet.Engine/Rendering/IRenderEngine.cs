using _3DNet.Engine.Rendering.Shader;
using System.Drawing;

namespace _3DNet.Engine.Rendering
{
    public interface IRenderEngine
    {
        void Initialize();
        IRenderWindowContext CreateRenderWindowContext(string name, Size size, bool fullScreen);
    }
}
