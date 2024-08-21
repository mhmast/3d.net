using _3DNet.Engine.Rendering;
using _3DNet.Engine.Scene;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;

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
            var sw = new Stopwatch();
            sw.Start();
            while (_running)
            {
                foreach (var scene in _scenes.Values)
                {
                    scene.Update();
                }
                if (!_context.IsDisposed)
                {
                    _context.SetProjection(_context.RenderWindow.Projection);
                    _activeScene.Render(_context,sw.ElapsedMilliseconds);
                }
            }
            sw.Stop();
        }

        public void Stop() => _running = false;

        public void SetActiveScene(ISceneInternal scene)
        {
            _activeScene = scene;
        }
    }
}
