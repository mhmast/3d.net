using _3DNet.Engine.Rendering;
using _3DNet.Engine.Rendering.Model;
using System.Numerics;

namespace _3DNet.Engine.Scene
{
    class StandardSceneObject : BaseSceneObject, IStandardSceneObject
    {

        private readonly IModel _model;
        private Matrix4x4 _scale;

        public StandardSceneObject(Scene scene,string name, IModel model) : base(scene,name)
        {
            _model = model;
        }

        public override void Render(IRenderContextInternal context)
        {
            context.SetWorld(World);
            _model.Render(context);
        }
        public void Resize(Vector3 boundingBox)
        {
            _scale = Matrix4x4.CreateScale(boundingBox/_model.BoundingBox);
            ReCalculateWorld();
        }

        protected override void OnWorldRecalculated()
        {
            base.OnWorldRecalculated();
            World = _scale * World;
        }
    }
}