using System.Drawing;

namespace _3DNet.Engine.Rendering
{
    public interface IRenderTargetFactory
    {
        IRenderWindow CreateWindow(Size size, string name, bool fullScreen = false);
    }
}
