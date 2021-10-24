﻿using _3DNet.Engine.Rendering;
using _3DNet.Engine.Rendering.Model;

namespace _3DNet.Engine.Scene
{
    public interface IScene
    {
        void Update();

        ISceneObject CreateStandardObject(IModel model);
        ICamera CreateStandardCamera(string name);
        void Render(IRenderEngine renderEngine);
    }
}