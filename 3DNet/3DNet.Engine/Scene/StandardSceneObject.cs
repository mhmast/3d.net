using _3DNet.Engine.Rendering;
using _3DNet.Math;

namespace _3DNet.Engine.Scene
{
    class StandardSceneObject : BaseSceneObject
    {

        private readonly IModel _model;
        public StandardSceneObject(IScene scene, IModel model) : base(scene)
        {
            
            _model = model;
        }

        protected override void RenderInternal(IRenderEngine engine)
        { 
            _model.Draw(engine);
        }
    }
}