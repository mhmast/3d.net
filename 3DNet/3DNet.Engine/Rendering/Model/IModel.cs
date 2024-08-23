using _3DNet.Engine.Rendering.Shader;
using System.Numerics;

namespace _3DNet.Engine.Rendering.Model
{
    public interface IModel
    {
        Vector3 BoundingBox { get; }
        IShader Shader { get;  }
        void Render(IRenderContextInternal renderContext);
    }
}
