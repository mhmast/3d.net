using _3DNet.Engine.Rendering;
using _3DNet.Engine.Scene;
using System.Collections.Generic;

namespace _3DNet.Engine.Engine
{
    internal class Engine : IEngine
    {
        readonly IRenderEngine _renderEngine;
        private bool _running = false;
        private IRenderTarget _renderTarget;
        private IScene _activeScene;
        private Dictionary<string, Scene.Scene> _scenes = new();
        public Engine(IRenderEngine renderEngine)
        {
            _renderEngine = renderEngine;
        }

        public void SetActiveRenderTarget(IRenderTarget renderTarget) => _renderTarget = renderTarget;

        public IScene GetOrCreateScene(string name)
        {
            if (!_scenes.ContainsKey(name))
            {
                _scenes.Add(name, new Scene.Scene(name));
            }
            return _scenes[name];
        }

        public void Start()
        {
            if (_running)
            {
                return;
            }
            _running = true;
            while (_running)
            {
                foreach (var scene in _scenes.Values)
                {
                    scene.Update();
                }
                if (!_renderTarget.IsDisposed)
                {
                    _activeScene.RenderOn(_renderTarget, _renderEngine);
                }
            }
        }

        public void Stop() => _running = false;

        public void SetActiveScene(IScene scene)
        {
            _activeScene = scene;
        }
    }
}
