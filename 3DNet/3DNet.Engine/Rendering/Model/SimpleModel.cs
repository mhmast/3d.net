using _3DNet.Engine.Rendering.Shader;
using _3DNet.Rendering.Buffer;

namespace _3DNet.Engine.Rendering.Model
{
    internal class SimpleModel : IModel
    {
        private readonly IBuffer _vertexBuffer;
        private readonly IBuffer _indexBuffer;

        public SimpleModel(IBuffer buffer, IBuffer indexBuffer, IShader shader)
        {
            _vertexBuffer = buffer;
            _indexBuffer = indexBuffer;
            Shader = shader;
        }

        public IShader Shader { get ; }
        
        public void Render(IRenderWindowContext context)
        {
            _vertexBuffer.Load(context);
            _indexBuffer.Load(context);
            Shader.Load(context);
        }

    }
}
