using System.Drawing;

namespace _3DNet.Engine.Rendering
{
    internal class RenderContextFactory : IRenderContextFactory
    {
        private readonly Engine.Engine _engine;
        private readonly IRenderEngine _renderEngine;

        public RenderContextFactory(Engine.Engine engine,IRenderEngine renderEngine)
        {
            _engine = engine;
            _renderEngine = renderEngine;
        }
        public IRenderContext CreateRenderContext(string name, Size size, bool fullScreen) => _renderEngine.CreateRenderContext(name, size, fullScreen, _engine.SetActiveContext);
    }
}
