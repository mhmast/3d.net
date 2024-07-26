using System;
using System.Numerics;

namespace _3DNet.Engine.Rendering
{
    public interface IRenderContext : IDisposable
    {
        IRenderWindow RenderWindow { get; }
        bool IsDisposed { get; }
        Matrix4x4 World { get; }
        Matrix4x4 View { get; }
        Matrix4x4 Projection { get; }
        void SetWorld(Matrix4x4 world);
        void SetView(Matrix4x4 view);
        void SetProjection(Matrix4x4 projection);
        void SetActiveContext();
    }
}
