using _3DNet.Engine.Rendering;
using _3DNet.Engine.Scene;

namespace _3DNet.Engine.Engine
{
    public interface IEngine
    {
        void Start();
        void Stop();
        void SetActiveContext(IRenderWindowContext context) ;
        IScene GetOrCreateScene(string name);
        void SetActiveScene(IScene scene);
    }
}