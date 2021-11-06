using _3DNet.Engine.Rendering.Model;

namespace _3DNet.Rendering.D3D12.Model
{
    internal class D3DModelFactory : ModelFactory
    {
        private readonly D3DRenderEngine _renderEngine;
        
        public D3DModelFactory(D3DRenderEngine renderEngine)
        {
            _renderEngine = renderEngine;
        }

        public override IModel CreateCube(float width, float height, float depth)
        => new SimpleModel(_renderEngine.CreateVertexBuffer(CreateCubeVertices(width, height, depth)), _renderEngine.CreateIndexBuffer(CreateCubeIndices()),_renderEngine.DefaultShader);
    }
}
