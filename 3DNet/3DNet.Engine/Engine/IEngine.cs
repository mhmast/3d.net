using _3DNet.Engine.Scene;

namespace _3DNet.Engine.Engine
{
    internal interface IEngine
    {
        void Start(IScene scene);
        void Stop();

        IScene CreateScene();
    }
}