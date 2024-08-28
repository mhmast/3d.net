using System;

namespace _3DNet.Engine.Rendering
{
    public interface IRenderWindow : IRenderTarget
    {
        void Show();
        bool FullScreen { get; set; }
        event Action OnClosed;
        event Action GotFocus;
        event Action LostFocus;
    }
}