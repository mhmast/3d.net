using _3DNet.Engine.Rendering;
using _3DNet.Engine.Scene;
using System.Collections.Generic;

namespace _3DNet.Engine.Engine
{
    internal class Engine : IEngine
    {
        readonly IRenderEngine _renderEngine;
        private bool _running = false;
        private IRenderContextInternal _context;
        private ISceneInternal _activeScene;
        private Dictionary<string, Scene.Scene> _scenes = new();
        public Engine(IRenderEngine renderEngine)
        {
            _renderEngine = renderEngine;
        }

        internal void SetActiveContext(IRenderContextInternal context) => _context = context;

        public IScene GetOrCreateScene(string name)
        {
            if (!_scenes.ContainsKey(name))
            {
                _scenes.Add(name, new Scene.Scene(name,this));
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
            long frame = 1;
            while (_running)
            {
                foreach (var scene in _scenes.Values)
                {
                    scene.Update();
                }
                if (!_context.IsDisposed)
                {
                    _context.SetProjection(_context.RenderWindow.Projection);
                    _activeScene.Render(_context, frame);
                }
                frame++;
            }
            _context.Dispose();
        }

        public void Stop()
        {
            _running = false;
        }

        public void SetActiveScene(ISceneInternal scene)
        {
            _activeScene = scene;
        }
    }
}
