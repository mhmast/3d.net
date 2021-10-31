﻿using _3DNet.Engine.Engine;
using _3DNet.Engine.Rendering;
using _3DNet.Engine.Rendering.Model;
using _3DNet.Engine.Scene;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DNet.Console
{
    public class Game
    {
        private readonly IRenderTargetFactory _renderTargetFactory;
        private readonly IEngine _engine;
        private readonly IModelFactory _modelFactory;
        private IRenderWindow _renderWindow;
        private IScene _scene;

        public Game(IRenderTargetFactory renderTargetFactory, IEngine engine,IModelFactory modelFactory)
        {
            _renderTargetFactory = renderTargetFactory;
            _engine = engine;
            _modelFactory = modelFactory;
        }


        public void Init()
        {
            _renderWindow = _renderTargetFactory.CreateWindow(new Size(100, 100), "Main");
            _renderWindow.OnClosed += _engine.Stop;
            _scene = _engine.GetOrCreateScene("default");
            _scene.BackgroundColor = Color.Red;
            _engine.SetActiveRenderTarget(_renderWindow);
            _engine.SetActiveScene(_scene);
            _scene.CreateStandardObject(_modelFactory.CreateCube(10, 10, 10));
        }

        public void Start()
        {
            _engine.Start();
        }

        public void Stop()
        {
            _engine.Stop();
        }
    }
}
