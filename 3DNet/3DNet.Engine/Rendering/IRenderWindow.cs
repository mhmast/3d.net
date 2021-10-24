using System;

namespace _3DNet.Engine.Rendering
{
    public interface IRenderWindow : IRenderTarget
    {
        void Show();
        event Action OnClosed;
    }
}