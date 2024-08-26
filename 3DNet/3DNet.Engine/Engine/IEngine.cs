using _3DNet.Engine.Rendering;
using _3DNet.Engine.Scene;
using System;

namespace _3DNet.Engine.Engine
{
    public interface IEngine
    {
        void Start();
        void Stop();
        void CreateScene(string name,ISceneImpl impl);
        event Action<IRenderContext> ActiveContextChanged;

    }
}