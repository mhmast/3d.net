using _3DNet.Engine.Rendering;
using _3DNet.Engine.Rendering.Model;
using System.Collections.Generic;
using System.Drawing;

namespace _3DNet.Engine.Scene
{
    internal class Scene : IScene
    {
        private readonly List<ISceneObject> _createdObjects = new();
        private readonly IRenderTarget _target;

        public ICamera ActiveCamera { get; set; }

        public Scene(IRenderTarget target)
        {
            _target = target;
        }

        public ISceneObject CreateStandardObject(IModel model)
        {
            var obj = new StandardSceneObject(this,model);
            _createdObjects.Add(obj);
            return obj;
        }

        public void Render(IRenderEngine renderEngine)
        {
            _target.Clear(Color.Red);
            renderEngine.SetProjection(_target.Projection);
            foreach(var obj in _createdObjects)
            {
                obj.Render(renderEngine);
            }
            _target.Present();
        }

        public void Update()
        {
            
        }

        public ICamera CreateStandardCamera(string name)
        {
            var obj = new StandardCamera(this,name);
            _createdObjects.Add(obj);
            ActiveCamera = ActiveCamera ?? obj;
            return obj;
        }
    }
}
