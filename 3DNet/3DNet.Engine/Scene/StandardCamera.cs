using _3DNet.Engine.Rendering;

namespace _3DNet.Engine.Scene
{
    internal class StandardCamera : BaseSceneObject, ICamera
    {
        public StandardCamera(Scene scene, string name) : base(scene,name)
        {
        }

        public override void Render(IRenderEngine engine)
        {
            engine.SetView(World);
        }
    }
}