using _3DNet.Engine.Rendering;
using _3DNet.Math;

namespace _3DNet.Engine.Scene
{
    internal abstract class BaseSceneObject : ISceneObject
    {
        
        protected BaseSceneObject(IScene scene)
        {
            Scene = scene;
        }

        public IScene Scene { get; }
        public Matrix4x4 World { get; private set; }

        public void Render(IRenderEngine engine)
        {
            engine.SetWorld(World);
            RenderInternal(engine);
        }

        protected abstract void RenderInternal(IRenderEngine engine);

    }
}