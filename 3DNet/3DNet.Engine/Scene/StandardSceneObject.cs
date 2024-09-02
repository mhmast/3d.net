using _3DNet.Engine.Rendering;
using _3DNet.Engine.Rendering.Model;
using System.Numerics;

namespace _3DNet.Engine.Scene
{
    internal class StandardSceneObject : BaseSceneObject<IStandardSceneObject>, IStandardSceneObject
    {

        private readonly IModel _model;
        private Matrix4x4 _scale = Matrix4x4.Identity;

        public StandardSceneObject(Scene scene,string name, IModel model) : base(scene,name)
        {
            _model = model;
        }

        protected override IStandardSceneObject Instance => this;

        public override void Render(IRenderContextInternal context)
        {
            context.SetWorld(World);
            _model.Render(context,Name);
        }
        public IStandardSceneObject Resize(Vector3 boundingBox)
        {
            _scale = Matrix4x4.CreateScale(boundingBox/_model.BoundingBox);
            ReCalculateWorld();
            return this;
        }

        protected override void OnWorldRecalculated()
        {
            base.OnWorldRecalculated();
            World = World * _scale;
        }
    }
}