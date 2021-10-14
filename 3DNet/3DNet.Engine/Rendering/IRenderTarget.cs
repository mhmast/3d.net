using System;
using System.Drawing;

namespace _3DNet.Engine.Rendering
{
    public interface IRenderTarget
    {
        int Height { get; }
        int Width { get; }
        void Clear(Color clearColor);
        void Present();
    }

    public interface IRenderWindow : IRenderTarget
    {
        void Show();
    }
}