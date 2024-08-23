using _3DNet.Engine.Rendering;
using _3DNet.Engine.Rendering.Model;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace _3DNet.Engine.Scene
{
    internal class Scene : ISceneInternal
    {
        private readonly Dictionary<string, BaseSceneObject> _createdObjects = new();
        private readonly Engine.Engine _engine;
        private IRenderable _activeCamera;

        private string Name { get; }

        public Scene(string name,Engine.Engine engine)
        {
            Name = name;
            _engine = engine;
        }


        public Color BackgroundColor { get; set; }

        private void CheckSceneObjectOrThrow(string name)
        {
            if (_createdObjects.ContainsKey(name))
            {
                throw new Exception($"Scene object with name {name} already exists");
            }
        }

        public IStandardSceneObject CreateStandardObject(string name, IModel model)
        {
            CheckSceneObjectOrThrow(name);
            var obj = new StandardSceneObject(this, name, model);
            _createdObjects.Add(name, obj);
            return obj;
        }

        public void Render(IRenderContextInternal context, long frame)
        {
            bool success = context.BeginScene(BackgroundColor, frame);
            if (!success)
            {
                return;
            }
            _activeCamera?.Render(context);
            foreach (var obj in _createdObjects.Values)
            {
                obj.Render(context);
            }
            context.EndScene(frame);
        }

        public void Update()
        {

        }

        public ICamera CreateStandardCamera(string name)
        {
            var obj = new StandardCamera(this, name);
            //_createdObjects.Add(name, obj);
            _activeCamera ??= obj;
            return obj;
        }

        public void SetActiveScene() => _engine.SetActiveScene(this);
        internal void SetActiveCamera(IRenderable activeCamera) => _activeCamera = activeCamera;
    }
}
