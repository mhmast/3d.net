﻿using _3DNet.Engine.Rendering.Buffer;
using System;
using System.Numerics;

namespace _3DNet.Engine.Rendering
{
    public interface IRenderContext : IDisposable
    {
        IRenderWindow RenderWindow { get; }
        bool IsDisposed { get; }
        WvpBuffer WvpBuffer{ get; }
        bool FullScreen { get; }

        event Action Focus;
        event Action FocusLost;

        void SetWorld(Matrix4x4 world);
        void SetView(Matrix4x4 view);
        void SetProjection(Matrix4x4 projection);
        void SetActiveContext();
    }
}
