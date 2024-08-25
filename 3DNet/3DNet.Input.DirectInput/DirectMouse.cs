using _3DNet.Engine.Input;
using DirectInputDevice = SharpDX.DirectInput.DirectInput;
using Mouse = SharpDX.DirectInput.Mouse;

namespace _3DNet.Input.DirectInput;
internal class DirectMouse : IMouse
{
    private readonly Mouse _mouse;
    private int _lastX = 0;
    private int _lastY = 0;
    private int _lastZ = 0;
    private SharpDX.DirectInput.MouseState _state;

    public DirectMouse()
    {
        var device = new DirectInputDevice();
        var mouse = device.GetDevices(SharpDX.DirectInput.DeviceClass.Pointer, SharpDX.DirectInput.DeviceEnumerationFlags.AttachedOnly).FirstOrDefault();
        if (mouse == null)
        {
            throw new ArgumentException("No Mouse found");
        }
        _mouse = new Mouse(device);
        _mouse.Acquire();
        _mouse.Poll();
        _state = _mouse.GetCurrentState();
    }
    public int X => _state.X;

    public int Y => _state.Y;
    public int Z => _state.Z;

    public float DeltaX => X - _lastX;

    public float DeltaY => Y - _lastY;

    public float DeltaZ => Z - _lastZ;

    public bool IsLeftButtonDown => _state.Buttons[0];

    public bool IsRightButtonDown => _state.Buttons.Last();

    public bool IsMiddleButtonDown => _state.Buttons.Length == 3 && _state.Buttons[1];

    public void Update()
    {
        _lastX = X;
        _lastY = Y;
        _lastZ = Z;
        _mouse.Poll();
        _state = _mouse.GetCurrentState();
    }
}
