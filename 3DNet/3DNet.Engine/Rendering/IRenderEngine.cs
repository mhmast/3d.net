using _3DNet.Math;
using System.Drawing;

namespace _3DNet.Engine.Rendering
{
    public interface IRenderEngine
    {
        void Initialize();
        void SetWorld(Matrix4x4 world);
        bool BeginScene(IRenderTarget target,Color clearColor);
        void EndScene(IRenderTarget target);
        void SetView(Matrix4x4 view);
        void SetProjection(Matrix4x4 projection);
    }
}
