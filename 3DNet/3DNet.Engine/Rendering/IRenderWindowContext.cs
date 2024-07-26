using _3DNet.Rendering.Buffer;
using System;
using System.Drawing;
using System.Numerics;

namespace _3DNet.Engine.Rendering
{
    public interface IRenderWindowContext : IDisposable
    {
        IRenderWindow RenderWindow { get; }
        bool IsDisposed { get; }
        Matrix4x4 World { get; }
        Matrix4x4 View { get; }
        Matrix4x4 Projection { get; }
        void SetWorld(Matrix4x4 world);
        void SetView(Matrix4x4 view);
        void SetProjection(Matrix4x4 projection);
        bool BeginScene(Color backgroundColor,long ms);
        void EndScene(long ms);
        void ClearDepthStencilView(IntPtr ptr, float depth, byte stencil);
        void ClearRenderTargetView(IntPtr ptr, Color clearColor);
        void SetVertexBuffer(IntPtr bufferLocation, int sizeInBytes, int strideInBytes);
        void Draw(IBuffer vertexBuffer, IBuffer indexBuffer);
        void SetIndexBuffer(IntPtr bufferLocation, int sizeInBytes, int strideInBytes);
        void LoadShaderBuffer(int slot, IntPtr address);
        void QueueAction(Action a);
    }
}
