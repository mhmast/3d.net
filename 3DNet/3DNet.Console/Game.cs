using _3DNet.Engine.Engine;
using _3DNet.Engine.Rendering;
using _3DNet.Engine.Rendering.Model;
using _3DNet.Engine.Scene;
using System.Drawing;
using System.Numerics;

namespace _3DNet.Console
{
    public class Game
    {
        private readonly IRenderContextFactory _renderContextFactory;
        private readonly IEngine _engine;
        private readonly IModelFactory _modelFactory;
        private IScene _scene;
        private IRenderContext _context;

        public Game(IRenderContextFactory renderContextFactory, IEngine engine,IModelFactory modelFactory)
        {
            _renderContextFactory = renderContextFactory;
            _engine = engine;
            _modelFactory = modelFactory;
        }


        public void Init()
        {
            _context = _renderContextFactory.CreateRenderContext("Main",new Size(1000, 1000),false );
            _context.RenderWindow.OnClosed += _engine.Stop;
            _scene = _engine.GetOrCreateScene("default");
            _scene.BackgroundColor = Color.Red;
            _scene.SetActiveScene();
            _context.SetActiveContext();
            var cubeModel = _modelFactory.CreateCube("cube", 10, 10, 10);
            var cube = _scene.CreateStandardObject("cubez+",cubeModel);
            cube.MoveTo(new Vector3(0, 0, 100));

            //_scene.CreateStandardObject("cubez-", cubeModel ).MoveTo(new Vector3(0,0,-100));
            _scene.CreateStandardObject("cubex+", cubeModel ).MoveTo(new Vector3(100,0,0));
            //_scene.CreateStandardObject("cubex-", cubeModel ).MoveTo(new Vector3(-100,0,0));
            //_scene.CreateStandardObject("cubey+", cubeModel ).MoveTo(new Vector3(0,100,0));
            //_scene.CreateStandardObject("cubey-", cubeModel ).MoveTo(new Vector3(0,-100,0));

            //var cam = _scene.CreateStandardCamera("defaultcam");
            //_scene.SetActiveCamera(cam);
            //cam.MoveTo(new Vector3(0, 0, 0));
            //cam.LookAt(cube);
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
