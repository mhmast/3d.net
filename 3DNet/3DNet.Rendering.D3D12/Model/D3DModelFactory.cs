using _3DNet.Engine.Rendering.Buffer;
using _3DNet.Engine.Rendering.Model;
using _3DNet.Rendering.Buffer;
using System.Collections.Generic;

namespace _3DNet.Rendering.D3D12.Model
{
    internal class D3DModelFactory : ModelFactory
    {
        private readonly D3DRenderEngine _renderEngine;

        public D3DModelFactory(D3DRenderEngine renderEngine) : base(renderEngine)
        {
            _renderEngine = renderEngine;
        }

        protected override IBuffer CreateIndexBuffer(string name,uint[] indices)
        => _renderEngine.CreateIndexBuffer(name,indices);

        protected override IBuffer CreateVertexBuffer<T>(string name, IEnumerable<T> vertices) where T : struct
        => _renderEngine.CreateVertexBuffer(name, vertices);
    }
}
