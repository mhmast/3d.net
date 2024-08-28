using _3DNet.Engine.Engine;
using _3DNet.Engine.Input;

namespace _3DNet.Input.DirectInput;
internal class DirectInputFactory : IInputFactory
{
    private readonly DirectMouse _mouse;
    private readonly DirectKeyboard _keyboard;

    public DirectInputFactory(IEngine engine)
    {
        _mouse = new DirectMouse(engine);
        _keyboard = new DirectKeyboard(engine);
    }
    public IKeyBoard GetKeyBoard() => _keyboard;
    public IMouse GetMouse() => _mouse;
}
