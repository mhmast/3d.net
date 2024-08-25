using _3DNet.Console.Scenes;
using _3DNet.Engine.Engine;
using _3DNet.Engine.Input;
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
        private readonly IInputFactory _inputFactory;
        private IScene _scene;
        private IRenderContext _context;

        public Game(IRenderContextFactory renderContextFactory, IEngine engine, IModelFactory modelFactory,IInputFactory inputFactory)
        {
            _renderContextFactory = renderContextFactory;
            _engine = engine;
            _modelFactory = modelFactory;
            _inputFactory = inputFactory;
        }


        public void Init()
        {
            _context = _renderContextFactory.CreateRenderContext("Main", new Size(1000, 1000), false);
            _context.RenderWindow.OnClosed += _engine.Stop;
            _context.SetActiveContext();
            _engine.CreateScene("default", new DefaultScene(_modelFactory,_inputFactory));
            
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
