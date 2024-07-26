using _3DNet.Engine.Rendering;
using _3DNet.Engine.Rendering.Shader;
using _3DNet.Rendering.D3D12.Buffer;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D12;
using SharpDX.DXGI;
using System.Collections.Generic;

namespace _3DNet.Rendering.D3D12.Shaders
{
    internal class HlslShader : IShader, ID3DObject
    {
        private readonly D3DRenderEngine _d3DRenderEngine;
        private readonly DescriptorHeap _shaderHeap;
        private readonly Engine.Rendering.Shader.ShaderDescription _shaderDescription;
        private PipelineState _graphicsPipelineState;
        private D3DRenderWindowContext _context;
        private RootSignature _rootSignature;
        private readonly List<ShaderBuffer> _buffers = new();

        public HlslShader(string name, D3DRenderEngine d3DRenderEngine, Engine.Rendering.Shader.ShaderDescription shaderDescription)
        {
            Name = name;
            _d3DRenderEngine = d3DRenderEngine;
            _shaderDescription = shaderDescription;
            _d3DRenderEngine.RenderTargetCreated += RecreateShader;
            _d3DRenderEngine.RegisterD3DObject(this);
            _shaderHeap = _d3DRenderEngine.CreateDescriptorHeap(new DescriptorHeapDescription
            {
                DescriptorCount = 1,
                Flags = DescriptorHeapFlags.ShaderVisible,
                Type = DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView
            });

            for (var i = 0; i < _shaderDescription.Buffers.Count; i++)
            {
                ShaderBufferDescription bufferDesc = _shaderDescription.Buffers[i];
                _buffers.Add(new ShaderBuffer($"{name}_shdrbffr_{i}", _d3DRenderEngine, _shaderHeap, bufferDesc));
            }
        }

        private void RecreateShader()
        {
            var rootSignatureDesc = new RootSignatureDescription(RootSignatureFlags.AllowInputAssemblerInputLayout, new[]
            {
                new RootParameter(ShaderVisibility.All,new RootDescriptor(),RootParameterType.ConstantBufferView)
            });
            _rootSignature = _d3DRenderEngine.CreateRootSignature(rootSignatureDesc.Serialize());

            var inputElementDescs = new[]
            {
                    new InputElement("POSITION",0,Format.R32G32B32_Float,0,0,InputClassification.PerVertexData,0)
            };
            GraphicsPipelineStateDescription gpsDesc = new()
            {
                InputLayout = new InputLayoutDescription(inputElementDescs),
                RootSignature = _rootSignature,
                VertexShader = LoadShaderByteCode(_shaderDescription.ShaderFile, _shaderDescription.VertexShaderMethod, _shaderDescription.VertexShaderProfile),
                PixelShader = LoadShaderByteCode(_shaderDescription.ShaderFile, _shaderDescription.PixelShaderMethod, _shaderDescription.PixelShaderProfile),
                RasterizerState = new RasterizerStateDescription
                {
                    CullMode = CullMode.None,
                    FillMode = FillMode.Solid,
                    IsDepthClipEnabled = false,
                    ConservativeRaster = ConservativeRasterizationMode.On,
                    DepthBias = 0
                },
                BlendState = BlendStateDescription.Default(),
                DepthStencilFormat = Format.D32_Float,
                DepthStencilState = new DepthStencilStateDescription() { IsDepthEnabled = false, IsStencilEnabled = false },
                SampleMask = int.MaxValue,
                PrimitiveTopologyType = PrimitiveTopologyType.Triangle,
                RenderTargetCount = _d3DRenderEngine.NoOfCreatedTargets,
                Flags = PipelineStateFlags.ToolDebug,
                SampleDescription = new SampleDescription(1, 0),
                StreamOutput = new StreamOutputDescription()
            };
            for (var i = 0; i < _d3DRenderEngine.NoOfCreatedTargets; i++)
            {
                gpsDesc.RenderTargetFormats[i] = _d3DRenderEngine.RenderTargetFormats[i];
                gpsDesc.BlendState.RenderTarget[i] = new RenderTargetBlendDescription
                {
                    BlendOperation = BlendOperation.Add,
                    AlphaBlendOperation = BlendOperation.Add,
                    DestinationAlphaBlend = BlendOption.DestinationAlpha,
                    DestinationBlend = BlendOption.DestinationColor,
                    LogicOpEnable = false,
                    SourceAlphaBlend = BlendOption.SourceAlpha,
                    SourceBlend = BlendOption.SourceColor,
                    IsBlendEnabled = true,
                    RenderTargetWriteMask = ColorWriteMaskFlags.All
                };
            }

            _graphicsPipelineState?.Dispose();
            _graphicsPipelineState = _d3DRenderEngine.CreateGraphicsPipelineState(gpsDesc);
            _graphicsPipelineState.Name = $"gps_{Name}";


        }

        private static SharpDX.Direct3D12.ShaderBytecode LoadShaderByteCode(string shaderFile, string method, string profile)
        {
            return new SharpDX.Direct3D12.ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile(shaderFile, method, profile
#if DEBUG
                , ShaderFlags.WarningsAreErrors | ShaderFlags.Debug | ShaderFlags.PackMatrixRowMajor

#endif
                          ));

        }

        public string ShaderSignature => _shaderDescription.Signature;

        public string Name { get; }


        public void Begin(D3DRenderWindowContext context)
        {
            _context = context;
        }

        public void End(D3DRenderWindowContext context)
        {
        }

        public void Dispose()
        {
            _graphicsPipelineState?.Dispose();
            _shaderHeap?.Dispose();
            _d3DRenderEngine.UnregisterD3DObject(this);
        }

        public void Load(IRenderWindowContext context)
        {
            _context.SetPrimitiveTopology(PrimitiveTopology.TriangleList);
            _context.SetPipelineState(_graphicsPipelineState);
            _context.SetGraphicsRootSignature(_rootSignature);
            foreach (var buffer in _buffers)
            {
                buffer.Load(context);
            }
        }
    }
}