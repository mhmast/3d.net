using _3DNet.Engine.Rendering.Shader;
using _3DNet.Rendering.Buffer;
using System.Numerics;

namespace _3DNet.Engine.Rendering.Model
{
    internal class SimpleModel : IModel
    {
        private readonly IBuffer _vertexBuffer;
        private readonly IBuffer _indexBuffer;
        
        public SimpleModel(IBuffer buffer, IBuffer indexBuffer, IShader shader,Vector3 boundingBox)
        {
            _vertexBuffer = buffer;
            _indexBuffer = indexBuffer;
            Shader = shader;
            BoundingBox = boundingBox;
        }

        public IShader Shader { get ; }

        public Vector3 BoundingBox { get; }

        public void Render(IRenderContextInternal context)
        {
            Shader.WvpBuffer.Write(context.WvpBuffer);
            Shader.Load(context);
            context.Draw(_vertexBuffer,_indexBuffer);
        }

    }
}
