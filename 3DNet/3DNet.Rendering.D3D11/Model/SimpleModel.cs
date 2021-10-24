using _3DNet.Engine.Rendering.Shader;
using _3DNet.Rendering.D3D12.Buffer;
using _3DNet.Rendering.D3D12.Shaders;

namespace _3DNet.Engine.Rendering.Model
{
    internal class SimpleModel : IModel
    {
        private readonly VertexBuffer _vertexBuffer;
        private readonly IndexBuffer _indexBuffer;

        public SimpleModel(VertexBuffer buffer, IndexBuffer indexBuffer, HlslShader shader)
        {
            _vertexBuffer = buffer;
            _indexBuffer = indexBuffer;
            Shader = shader;
        }

        public HlslShader Shader { get ; }
        IShader IModel.Shader { get =>Shader;  }

        public void Render(IRenderEngine engine)
        {
            Shader.LoadBuffer(_vertexBuffer);
            Shader.LoadBuffer(_indexBuffer);
        }
    }
}
