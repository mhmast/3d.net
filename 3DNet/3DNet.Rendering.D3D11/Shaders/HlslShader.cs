using _3DNet.Engine.Rendering.Shader;
using _3DNet.Rendering.D3D12.Buffer;
using SharpDX.Direct3D12;

namespace _3DNet.Rendering.D3D12.Shaders
{
    internal class HlslShader : IShader
    {
        private GraphicsCommandList _commandList;

       
        public HlslShader(string shaderSignature, GraphicsCommandList commandList)
        {
            ShaderSignature = shaderSignature;
            _commandList = commandList;
        }

        public string ShaderSignature { get; }

        internal void LoadBuffer(IBuffer buffer)
        {
            buffer.Load(_commandList);
        }
    }
}