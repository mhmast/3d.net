using _3DNet.Engine.Rendering;
using _3DNet.Engine.Rendering.Model;

namespace _3DNet.Engine.Scene
{
    class StandardSceneObject : BaseSceneObject 
    {

        private readonly IModel _model;
        public StandardSceneObject(Scene scene,string name, IModel model) : base(scene,name)
        {
            _model = model;
        }

        public override void Render(IRenderContextInternal context)
        {
            context.SetWorld(World);
            _model.Render(context);
        }
    }
}