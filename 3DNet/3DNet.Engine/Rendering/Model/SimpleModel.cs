using _3DNet.Engine.Rendering.Buffer;

namespace _3DNet.Engine.Rendering.Model
{
    class SimpleModel : IModel
    {
        private readonly IVertexBuffer _vertexBuffer;
        private readonly IIndexBuffer _indexBuffer;

        public SimpleModel(IVertexBuffer buffer,IIndexBuffer indexBuffer)
        {
            _vertexBuffer = buffer;
            _indexBuffer = indexBuffer;
        }

        public void Draw(IRenderEngine engine)
        {
            engine.SetVertexBuffer(_vertexBuffer);
            engine.SetIndexBuffer(_indexBuffer);
        }
    }
}
