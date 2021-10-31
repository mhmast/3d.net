using _3DNet.Engine.Rendering;
using _3DNet.Engine.Rendering.Model;
using System.Collections.Generic;
using System.Drawing;

namespace _3DNet.Engine.Scene
{
    internal class Scene : IScene
    {
        private readonly List<ISceneObject> _createdObjects = new();
        private string name;

        public Scene(string name)
        {
            this.name = name;
        }

        public Scene()
        {
        }

        public ICamera ActiveCamera { get; set; }
        public Color BackgroundColor { get; set; }



        public ISceneObject CreateStandardObject(IModel model)
        {
            var obj = new StandardSceneObject(this, model);
            _createdObjects.Add(obj);
            return obj;
        }

        public void RenderOn(IRenderTarget renderTarget, IRenderEngine renderEngine)
        {
            renderEngine.SetProjection(renderTarget.Projection);
            bool success = renderEngine.BeginScene(renderTarget, BackgroundColor);
            if(!success)
            {
                return;
            }
            foreach (var obj in _createdObjects)
            {
                obj.Render(renderEngine);
            }
            renderEngine.EndScene(renderTarget);
        }

        public void Update()
        {

        }

        public ICamera CreateStandardCamera(string name)
        {
            var obj = new StandardCamera(this, name);
            _createdObjects.Add(obj);
            ActiveCamera = ActiveCamera ?? obj;
            return obj;
        }


    }
}
