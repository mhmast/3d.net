using _3DNet.Engine.Rendering;

namespace _3DNet.Engine.Scene
{
    public interface IScene
    {
        void Update();

        ISceneObject CreateStandardObject(IModel model);
        void Render(IRenderEngine renderEngine);
    }
}