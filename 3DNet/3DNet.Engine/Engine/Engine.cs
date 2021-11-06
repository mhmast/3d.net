﻿using _3DNet.Engine.Rendering;
using _3DNet.Engine.Scene;
using System.Collections.Generic;

namespace _3DNet.Engine.Engine
{
    internal class Engine : IEngine
    {
        readonly IRenderEngine _renderEngine;
        private bool _running = false;
        private IRenderWindowContext _context;
        private IScene _activeScene;
        private Dictionary<string, Scene.Scene> _scenes = new();
        public Engine(IRenderEngine renderEngine)
        {
            _renderEngine = renderEngine;
        }

        public void SetActiveContext(IRenderWindowContext context) => _context = context;

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
                if (!_context.IsDisposed)
                {
                    
                    _activeScene.Render(_context);
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
