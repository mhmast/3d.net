using _3DNet.Engine.Engine;
using _3DNet.Engine.Input;
using _3DNet.Engine.Rendering;
using SharpDX.DirectInput;
using DirectInputDevice = SharpDX.DirectInput.DirectInput;


namespace _3DNet.Input.DirectInput;
internal class DirectKeyboard : IKeyBoard
{
    private readonly Keyboard _keyBoard;
    private KeyboardState _state;
    private IRenderContext? _context;
    private bool _captureKeyboard;

    public DirectKeyboard(IEngine engine)
    {
        var device = new DirectInputDevice();
        var keyBoard = device.GetDevices(deviceClass: DeviceClass.Keyboard, DeviceEnumerationFlags.AttachedOnly).FirstOrDefault();
        if (keyBoard == null)
        {
            throw new ArgumentException("No Keyboard found");
        }
        engine.ActiveContextChanged += (context) =>
        {
            if (_context != null)
            {
                _context.RenderWindow.GotFocus -= RenderWindowFocused;
                _context.RenderWindow.LostFocus -= RenderWindowFocusLost;
            }
            _context = context;
            _captureKeyboard = context.RenderWindow.FullScreen;
            _context.RenderWindow.GotFocus += RenderWindowFocused;
            _context.RenderWindow.LostFocus += RenderWindowFocusLost;
        };
        _keyBoard = new Keyboard(device);
        _keyBoard.Acquire();
        _keyBoard.Poll();
        _state = _keyBoard.GetCurrentState();
    }

    private void RenderWindowFocusLost() => _captureKeyboard = false;
    private void RenderWindowFocused() => _captureKeyboard = true;
    public bool IsButtonPressed(Engine.Input.Key key)
    => _state.IsPressed(GetKey(key));

    private static SharpDX.DirectInput.Key GetKey(Engine.Input.Key key) => (SharpDX.DirectInput.Key)key;

    public void Update()
    {
        if (!_captureKeyboard) return;
        _keyBoard.Poll();
        _state = _keyBoard.GetCurrentState();
    }
}
