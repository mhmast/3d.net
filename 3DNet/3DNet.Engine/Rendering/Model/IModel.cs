using _3DNet.Engine.Rendering.Shader;

namespace _3DNet.Engine.Rendering.Model
{
    public interface IModel
    {
        IShader Shader { get;  }
        void Render(IRenderContextInternal renderContext);
    }
}
