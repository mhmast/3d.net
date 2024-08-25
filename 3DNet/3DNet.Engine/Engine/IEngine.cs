using _3DNet.Engine.Scene;

namespace _3DNet.Engine.Engine
{
    public interface IEngine
    {
        void Start();
        void Stop();
        void CreateScene(string name,ISceneImpl impl);
    }
}