using _3DNet.Engine.Rendering;
using _3DNet.Engine.Rendering.Model;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace _3DNet.Engine.Scene
{
    internal class Scene : IScene
    {
        private readonly Dictionary<string,ISceneObject> _createdObjects = new();
        private StandardCamera _activeCamera;

        private string Name { get; }

        public Scene(string name)
        {
            Name = name;
        }

      
        public Color BackgroundColor { get; set; }

        private void CheckSceneObjectOrThrow(string name)
        {
            if (_createdObjects.ContainsKey(name))
            {
                throw new Exception($"Scene object with name {name} already exists");
            }
        }

        public ISceneObject CreateStandardObject(string name,IModel model)
        {
            CheckSceneObjectOrThrow(name);
            var obj = new StandardSceneObject(this,name, model);
            _createdObjects.Add(name,obj);
            return obj;
        }

        public void Render(IRenderWindowContext context)
        {
            bool success = context.BeginScene(BackgroundColor);
            if(!success)
            {
                return;
            }
            foreach (var obj in _createdObjects.Values)
            {
                obj.Render(context);
            }
            context.EndScene();
        }

        public void Update()
        {

        }

        public ICamera CreateStandardCamera(string name)
        {
            var obj = new StandardCamera(this, name);
            _createdObjects.Add(name,obj);
            _activeCamera = _activeCamera ?? obj;
            return obj;
        }

        public void SetActiveCamera(ICamera cam)
        {
            _activeCamera = _createdObjects[cam.Name] as StandardCamera;
        }
    }
}
