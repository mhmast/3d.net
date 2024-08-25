using _3DNet.Engine.Input;

namespace _3DNet.Input.DirectInput;
internal class DirectInputFactory : IInputFactory
{
    private readonly DirectMouse _mouse;

    public DirectInputFactory()
    {
        _mouse = new DirectMouse();
    }
    public IKeyBoard GetKeyBoard() => throw new NotImplementedException();
    public IMouse GetMouse() => _mouse;
}
