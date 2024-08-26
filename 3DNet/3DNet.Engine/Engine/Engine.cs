using _3DNet.Engine.Rendering;
using _3DNet.Engine.Scene;
using System;
using System.Collections.Generic;

namespace _3DNet.Engine.Engine
{
    internal class Engine : IEngine
    {
        private bool _running = false;
        private IRenderContextInternal _context;
        private ISceneInternal _activeScene;
        private readonly Dictionary<string, Scene.Scene> _scenes = new();

        public event Action<IRenderContext> ActiveContextChanged;

        internal void SetActiveContext(IRenderContextInternal context)
        {
            _context = context;
            ActiveContextChanged?.DynamicInvoke(context);
        }

        public void CreateScene(string name, ISceneImpl impl)
        {
            if (!_scenes.ContainsKey(name))
            {
                _scenes.Add(name, new Scene.Scene(name, this, impl));
            }
            else
            {
                throw new ArgumentException($"A scene with name {name} already exists");
            }
        }

        public void Start()
        {
            if (_running)
            {
                return;
            }
            _running = true;
            long frame = 1;
            foreach (var scene in _scenes.Values)
            {
                scene.Init();
            }
            while (_running)
            {
                _activeScene.Update();
                if (!_context.IsDisposed)
                {
                    _context.SetProjection(_context.RenderWindow.Projection);
                    _activeScene.Render(_context, frame);
                }
                frame++;
            } 
        }

        public void Stop()
        {
            _running = false;
            _context.Dispose();
        }

        public void SetActiveScene(ISceneInternal scene)
        {
            _activeScene = scene;
        }
    }
}
