﻿using _3DNet.Engine.Rendering;
using _3DNet.Engine.Rendering.Model;
using System.Drawing;

namespace _3DNet.Engine.Scene
{
    public interface IScene
    {
        void Update();

        ISceneObject CreateStandardObject(string name,IModel model);
        ICamera CreateStandardCamera(string name);
        void Render(IRenderWindowContext context);
        public Color BackgroundColor { get; set; }

        void SetActiveCamera(ICamera cam);
    }
}