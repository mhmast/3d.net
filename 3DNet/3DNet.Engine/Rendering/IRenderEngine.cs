using _3DNet.Engine.Rendering.Buffer;
using _3DNet.Math;
using System.Drawing;

namespace _3DNet.Engine.Rendering
{
    public interface IRenderEngine
    {
        void Initialize();
        void SetWorld(Matrix4x4 world);
        void SetVertexBuffer(IVertexBuffer buffer);
        void BeginScene(IRenderTarget target,Color clearColor);
        void EndScene(IRenderTarget target);
        void SetIndexBuffer(IIndexBuffer indexBuffer);
    }
}
