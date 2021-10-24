using _3DNet.Engine.Rendering.Model;
using _3DNet.Rendering.D3D12.Buffer;
using _3DNet.Rendering.D3D12.Shaders;

namespace _3DNet.Rendering.D3D12.Model
{
    internal class D3DModelFactory : ModelFactory
    {
        private readonly D3DRenderEngine _renderEngine;
        private readonly D3DShaderFactory _shaderFactory;

        public D3DModelFactory(D3DRenderEngine renderEngine,D3DShaderFactory shaderFactory)
        {
            _renderEngine = renderEngine;
            _shaderFactory = shaderFactory;
        }

        public override IModel CreateCube(float width, float height, float depth)
        => new SimpleModel(_renderEngine.CreateVertexBuffer(CreateCubeVertices(width, height, depth)), _renderEngine.CreateIndexBuffer(CreateCubeIndices()),_shaderFactory.DefaultShader);
    }
}
