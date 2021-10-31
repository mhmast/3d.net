using _3DNet.Engine.Engine;
using _3DNet.Engine.Rendering;
using _3DNet.Engine.Rendering.Model;
using _3DNet.Engine.Scene;
using System.Drawing;

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
            _renderWindow = _renderTargetFactory.CreateWindow(new Size(1000, 1000), "Main");
            _renderWindow.OnClosed += _engine.Stop;
            _scene = _engine.GetOrCreateScene("default");
            _scene.BackgroundColor = Color.Red;
            _engine.SetActiveRenderTarget(_renderWindow);
            _engine.SetActiveScene(_scene);
            var cube = _scene.CreateStandardObject("cube",_modelFactory.CreateCube(10, 10, 10));
            cube.MoveTo((0, 0, 20));
            var cam = _scene.CreateStandardCamera("defaultcam");
            _scene.SetActiveCamera(cam);
            cam.MoveTo((0, 0, 0));
            cam.LookAt(cube);
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
