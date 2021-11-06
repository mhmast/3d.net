using _3DNet.Math;
using System;
using System.Drawing;

namespace _3DNet.Engine.Rendering
{
    public interface IRenderWindowContext : IDisposable
    {
        IRenderWindow RenderWindow { get; }
        bool IsDisposed { get; }

        void SetView(Matrix4x4 view);
        void SetProjection(Matrix4x4 projection);
        bool BeginScene(Color backgroundColor);
        void EndScene();
        void ClearDepthStencilView(IntPtr ptr, float depth, byte stencil);
        void ClearRenderTargetView(IntPtr ptr, Color clearColor);
        void SetVertexBuffer(IntPtr bufferLocation, int sizeInBytes, int strideInBytes);
        void SetIndexBuffer(IntPtr bufferLocation, int sizeInBytes, int strideInBytes);
    }
}
