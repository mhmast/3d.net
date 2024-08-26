using _3DNet.Engine.Engine;
using _3DNet.Engine.Input;

namespace _3DNet.Input.DirectInput;
internal class DirectInputFactory : IInputFactory
{
    private readonly DirectMouse _mouse;

    public DirectInputFactory(IEngine engine)
    {
        _mouse = new DirectMouse(engine);
    }
    public IKeyBoard GetKeyBoard() => throw new NotImplementedException();
    public IMouse GetMouse() => _mouse;
}
