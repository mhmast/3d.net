using _3DNet.Engine.Rendering.Buffer;
using _3DNet.Engine.Rendering.Model;
using _3DNet.Rendering.Buffer;

namespace _3DNet.Rendering.D3D12.Model
{
    internal class D3DModelFactory : ModelFactory
    {
        private readonly D3DRenderEngine _renderEngine;
        
        public D3DModelFactory(D3DRenderEngine renderEngine):base(renderEngine)
        {
            _renderEngine = renderEngine;
        }

        protected override IBuffer CreateIndexBuffer(int[] indices)
        =>_renderEngine.CreateIndexBuffer(indices);

        protected override IBuffer CreateVertexBuffer(IVertex[] vertices)
        => _renderEngine.CreateVertexBuffer(vertices);
    }
}
