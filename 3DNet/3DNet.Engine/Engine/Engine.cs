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

        public IScene CreateScene()
        => new Scene.Scene();

        public void Start(IScene scene)
        {
            if(_running)
            {
                return;
            }
            while(_running)
            {
                scene.Update();
                scene.Render(_renderEngine);
            }
        }

        public void Stop() => _running = false;
    }
}
