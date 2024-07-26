using _3DNet.Engine.Rendering;

namespace _3DNet.Engine.Scene
{
    internal interface ISceneInternal : IScene
    {
        void Render(IRenderContextInternal context, long ms);
    }
}