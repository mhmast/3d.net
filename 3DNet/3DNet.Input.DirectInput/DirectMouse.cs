﻿using _3DNet.Engine.Engine;
using _3DNet.Engine.Input;
using _3DNet.Engine.Rendering;
using System.Diagnostics;
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
    private IRenderContext? _context;
    private bool _captureMouse = true;

    public DirectMouse(IEngine engine)
    {
        var device = new DirectInputDevice();
        var mouse = device.GetDevices(SharpDX.DirectInput.DeviceClass.Pointer, SharpDX.DirectInput.DeviceEnumerationFlags.AttachedOnly).FirstOrDefault();
        if (mouse == null)
        {
            throw new ArgumentException("No Mouse found");
        }
        engine.ActiveContextChanged += (context) =>
        {
            if (_context != null)
            {
                _context.Focus -= ContextFocused;
                _context.FocusLost -= ContextFocusLost;
            }
            _context = context;
            _captureMouse = context.FullScreen;
            _context.Focus += ContextFocused;
            _context.FocusLost += ContextFocusLost;
        };
        _mouse = new Mouse(device);
        _mouse.Acquire();
        _mouse.Poll();
        _state = _mouse.GetCurrentState();
    }

    private void ContextFocusLost() => _captureMouse = false;
    private void ContextFocused() => _captureMouse = true;

    public int X => _state.X;

    public int Y => _state.Y;
    public int Z => _state.Z;

    public float DeltaX => _state.X;

    public float DeltaY => _state.Y;

    public float DeltaZ => _state.Z;

    public bool IsLeftButtonDown => _state.Buttons[0];

    public bool IsRightButtonDown => _state.Buttons.Last();

    public bool IsMiddleButtonDown => _state.Buttons.Length == 3 && _state.Buttons[1];

    public void Update()
    {
        if (!_captureMouse) return;
        _mouse.Poll();
        _state = _mouse.GetCurrentState();
    }
}
