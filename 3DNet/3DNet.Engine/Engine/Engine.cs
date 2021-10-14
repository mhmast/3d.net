using _3DNet.Engine.Rendering;
using _3DNet.Engine.Scene;

namespace _3DNet.Engine.Engine
{
    internal class Engine : IEngine
    {
        readonly IRenderEngine _renderEngine;
        private bool _running = false;

        public Engine(IRenderEngine renderEngine)
        {
            _renderEngine = renderEngine;
        }

        public IScene CreateScene(IRenderTarget target)
        => new Scene.Scene(target);

        public void Start(IScene scene)
        {
            if(_running)
            {
                return;
            }
            _running = true;
            while(_running)
            {
                scene.Update();
                scene.Render(_renderEngine);
            }
        }

        public void Stop() => _running = false;
    }
}
