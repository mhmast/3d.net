using _3DNet.Engine.Rendering;
using System.Collections.Generic;

namespace _3DNet.Engine.Scene
{
    internal class Scene : IScene
    {
        private readonly List<ISceneObject> _createdObjects = new();

        public ISceneObject CreateStandardObject(IModel model)
        {
            var obj = new StandardSceneObject(this,model);
            _createdObjects.Add(obj);
            return obj;
        }

        public void Render(IRenderEngine renderEngine)
        {
            foreach(var obj in _createdObjects)
            {
                obj.Render(renderEngine);
            }
        }

        public void Update()
        {
            
        }
    }
}
