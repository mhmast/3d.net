using _3DNet.Engine.Rendering;
using _3DNet.Engine.Scene;

namespace _3DNet.Engine.Engine
{
    public interface IEngine
    {
        void Start(IScene scene);
        void Stop();

        IScene CreateScene(IRenderTarget sceneTarget);
    }
}