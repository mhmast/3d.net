using _3DNet.Engine.Rendering;

namespace _3DNet.Engine.Scene
{
    public interface ISceneObject
    {
        IScene Scene { get; }
        void Render(IRenderEngine engine);
    }
}
