using _3DNet.Engine.Rendering.Shader;
using _3DNet.Rendering.D3D12.Buffer;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D12;
using SharpDX.DXGI;

namespace _3DNet.Rendering.D3D12.Shaders
{
    internal class HlslShader : IShader, ID3DObject
    {
        private GraphicsCommandList _commandList;
        private readonly D3DRenderEngine _d3DRenderEngine;
        private readonly Engine.Rendering.Shader.ShaderDescription _shaderDescription;
        private readonly D3DRenderTargetFactory _renderTargetFactory;
        private PipelineState _graphicsPipelineState;

        public HlslShader(string name, D3DRenderEngine d3DRenderEngine, D3DRenderTargetFactory renderTargetFactory, Engine.Rendering.Shader.ShaderDescription shaderDescription)
        {
            Name = name;
            _d3DRenderEngine = d3DRenderEngine;
            _renderTargetFactory = renderTargetFactory;
            _shaderDescription = shaderDescription;
            _renderTargetFactory.RenderTargetCreated += RecreateShader;
            _d3DRenderEngine.RegisterD3DObject(this);
        }

        private void RecreateShader()
        {
            var rootSignatureDesc = new RootSignatureDescription(RootSignatureFlags.AllowInputAssemblerInputLayout,
               new[]
               {
                                new RootParameter(ShaderVisibility.Vertex,
                                    new DescriptorRange()
                                    {
                                        RangeType = DescriptorRangeType.ConstantBufferView,
                                        BaseShaderRegister = 0,
                                        OffsetInDescriptorsFromTableStart = int.MinValue,
                                        DescriptorCount = 1
                                    })
            });
            var rootSignature = _d3DRenderEngine.CreateRootSignature(rootSignatureDesc.Serialize());

            var inputElementDescs = new[]
            {
                    new InputElement("SV_POSITION",0,Format.R32G32B32_Float,0,0),
            };
            GraphicsPipelineStateDescription gpsDesc = new()
            {
                InputLayout = new InputLayoutDescription(inputElementDescs),
                RootSignature = rootSignature,
                VertexShader = LoadShaderByteCode(_shaderDescription.ShaderFile, _shaderDescription.VertexShaderMethod, _shaderDescription.VertexShaderProfile),
                PixelShader = LoadShaderByteCode(_shaderDescription.ShaderFile, _shaderDescription.PixelShaderMethod, _shaderDescription.PixelShaderProfile),
                RasterizerState = new RasterizerStateDescription { CullMode = CullMode.None,FillMode = FillMode.Solid},
                BlendState = BlendStateDescription.Default(),
                DepthStencilFormat = Format.D32_Float,
                DepthStencilState = new DepthStencilStateDescription() { IsDepthEnabled = false, IsStencilEnabled = false },
                SampleMask = int.MaxValue,
                PrimitiveTopologyType = PrimitiveTopologyType.Triangle,
                RenderTargetCount = _renderTargetFactory.NoOfCreatedTargets,
                Flags = PipelineStateFlags.None,
                SampleDescription = new SampleDescription(1, 0),
                StreamOutput = new StreamOutputDescription()
            };
            for (var i = 0; i < _renderTargetFactory.NoOfCreatedTargets; i++)
            {
                gpsDesc.RenderTargetFormats[i] = _renderTargetFactory.RenderTargetFormats[i];
            }

            _graphicsPipelineState = _d3DRenderEngine.CreateGraphicsPipelineState(gpsDesc);
            _graphicsPipelineState.Name = $"gps_{Name}";
        }

        private SharpDX.Direct3D12.ShaderBytecode LoadShaderByteCode(string shaderFile, string method, string profile)
        {
            return new SharpDX.Direct3D12.ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile(shaderFile, method, profile
#if DEBUG
                , ShaderFlags.Debug

#endif
                          ));

        }

        public string ShaderSignature => _shaderDescription.Signature;

        public string Name { get; }

        internal void LoadBuffer(IBuffer buffer)
        {
            buffer.Load(_commandList);
        }

        public void Begin(GraphicsCommandList commandList)
        {
            _commandList = commandList;
            _commandList.PipelineState = _graphicsPipelineState;
        }

        public void End(GraphicsCommandList commandList)
        {
        }

        public void Dispose()
        {
            _graphicsPipelineState?.Dispose();
            _d3DRenderEngine.UnregisterD3DObject(this);
        }
    }
}