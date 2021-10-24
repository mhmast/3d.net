using _3DNet.Engine.Rendering;
using _3DNet.Engine.Rendering.Model;

namespace _3DNet.Engine.Scene
{
    class StandardSceneObject : BaseSceneObject 
    {

        private readonly IModel _model;
        public StandardSceneObject(IScene scene, IModel model) : base(scene)
        {
            
            _model = model;
        }

        public override void Render(IRenderEngine engine)
        {
            _model.Render(engine);
        }
    }
}