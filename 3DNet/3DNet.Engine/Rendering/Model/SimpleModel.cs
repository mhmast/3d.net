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
        
        public void Render(IRenderContextInternal context)
        {
            Shader.WvpBuffer.Write(context.WvpBuffer);
            Shader.Load(context);
            context.Draw(_vertexBuffer,_indexBuffer);
        }

    }
}
