using _3DNet.Engine.Rendering;
using _3DNet.Math;

namespace _3DNet.Engine.Scene
{
    internal abstract class BaseSceneObject : ISceneObject
    {
        public BaseSceneObject(IScene scene, string name) : this(scene)
        {
            Name = name;
        }

        protected BaseSceneObject(IScene scene)
        {
            Scene = scene;
        }

        public IScene Scene { get; }
        public Matrix4x4 World { get; private set; }
        public string Name { get; }

        public abstract void Render(IRenderEngine engine);
    }
}