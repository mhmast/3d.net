using _3DNet.Rendering.Buffer;
using System;
using System.Drawing;

namespace _3DNet.Engine.Rendering
{
    public interface IRenderContextInternal : IRenderContext
    {
        bool BeginScene(Color backgroundColor, long frame);
        void BeginObject(string name);
        void EndObject(string name);
        void EndScene(long frame);
        void ClearDepthStencilView(IntPtr ptr, float depth, byte stencil);
        void ClearRenderTargetView(IntPtr ptr, Color clearColor);
        void SetVertexBuffer(IntPtr bufferLocation, int sizeInBytes, int strideInBytes);
        void Draw(IBuffer vertexBuffer, IBuffer indexBuffer);
        void SetIndexBuffer(IntPtr bufferLocation, int sizeInBytes, int strideInBytes);
        void LoadShaderBuffer(int slot, IntPtr address);
        void QueueAction(Action a);
        void Update();
    }
}
