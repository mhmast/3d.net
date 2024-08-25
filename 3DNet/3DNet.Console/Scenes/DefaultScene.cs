using _3DNet.Engine.Input;
using _3DNet.Engine.Rendering.Model;
using _3DNet.Engine.Scene;
using System.Drawing;
using System.Numerics;

namespace _3DNet.Console.Scenes;
internal class DefaultScene : ISceneImpl
{
    private readonly IModelFactory _modelFactory;
    private readonly IInputFactory _inputFactory;
    private ICamera _cam;
    private IMouse _mouse;
    private float _sensitivityFactor = 0.5f;

    public DefaultScene(IModelFactory modelFactory, IInputFactory inputFactory)
    {
        _modelFactory = modelFactory;
        _inputFactory = inputFactory;
    }

    public void Initialize(IScene scene)
    {
        scene.BackgroundColor = Color.Red;
        scene.SetActiveScene();
        ;
        var cubeModel = _modelFactory.CreateCube("cube", 10, 10, 10);
        var cube = scene.CreateStandardObject("cubez+", cubeModel);
        cube.MoveTo(new Vector3(0, 0, 100));
        cube.Resize(new Vector3(1, 1, 1));
        //_scene.CreateStandardObject("cubez-", cubeModel ).MoveTo(new Vector3(0,0,-100));
        //_scene.CreateStandardObject("cubex+", cubeModel ).MoveTo(new Vector3(100,0,0));
        //_scene.CreateStandardObject("cubex-", cubeModel).MoveTo(new Vector3(-100, 0, 0));
        //_scene.CreateStandardObject("cubey+", cubeModel).MoveTo(new Vector3(0, 100, 0));
        //_scene.CreateStandardObject("cubey-", cubeModel).MoveTo(new Vector3(0, -100, 0));

        _cam = scene.CreateStandardCamera("defaultcam");
        _cam.SetActiveCamera();
        _cam.MoveTo(new Vector3(0, 0, 0));
        _cam.LookAt(cube);
        _mouse = _inputFactory.GetMouse();
    }
    public void Update(IScene scene)
    {
        _mouse.Update();
        _cam.Rotate(_cam.Right, _mouse.DeltaY * _sensitivityFactor);
        _cam.Rotate(_cam.Up, 360/ _mouse.DeltaX * _sensitivityFactor);
    }
}
