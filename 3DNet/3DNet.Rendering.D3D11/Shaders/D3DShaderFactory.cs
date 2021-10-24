using _3DNet.Engine.Rendering.Shader;
using Device = SharpDX.Direct3D12.Device;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D12;
using SharpDX.DXGI;
using System.Security.Cryptography;
using System.Text;

namespace _3DNet.Rendering.D3D12.Shaders
{
    internal class D3DShaderFactory : IShaderFactory
    {
        private readonly D3DRenderEngine _d3DRenderEngine;

        public D3DShaderFactory(D3DRenderEngine d3DRenderEngine)
        {
            _d3DRenderEngine = d3DRenderEngine;
            DefaultShader = LoadShader("default.hlsl", "VSMain", "PSMain", "VS_5.0", "PS_5.0");
        }

        private HlslShader LoadShader(string shaderFile, string vertexShaderMethod, string pixelShaderMethod, string vertexShaderProfile, string pixelShaderProfile)
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
                    new InputElement("POSITION",0,Format.R32G32B32_Float,0,0),
            };
            GraphicsPipelineStateDescription gpsDesc = new()
            {
                InputLayout = new InputLayoutDescription(inputElementDescs),
                RootSignature = rootSignature,
                VertexShader = LoadShaderByteCode(shaderFile, vertexShaderMethod, vertexShaderProfile),
                PixelShader = LoadShaderByteCode(shaderFile, pixelShaderMethod, pixelShaderProfile),
                RasterizerState = RasterizerStateDescription.Default(),
                BlendState = BlendStateDescription.Default(),
                DepthStencilFormat = Format.D32_Float,
                DepthStencilState = new DepthStencilStateDescription() { IsDepthEnabled = false, IsStencilEnabled = false },
                SampleMask = int.MaxValue,
                PrimitiveTopologyType = PrimitiveTopologyType.Triangle,
                RenderTargetCount = 1,
                Flags = PipelineStateFlags.None,
                SampleDescription = new SampleDescription(1, 0),
                StreamOutput = new StreamOutputDescription()
            };
            var graphicsPipelineState = _d3DRenderEngine.CreateGraphicsPipelineState(gpsDesc);
            var shaderSignature = Encoding.UTF8.GetString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes($"{shaderFile}|{vertexShaderMethod}|{vertexShaderProfile}|{pixelShaderMethod}|{pixelShaderProfile}")));
            var commandList = _d3DRenderEngine.AddCommandList(graphicsPipelineState);
            return new HlslShader(shaderSignature,commandList);
        }

        private SharpDX.Direct3D12.ShaderBytecode LoadShaderByteCode(string shaderFile, string method, string profile)
        {
            return new SharpDX.Direct3D12.ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile(shaderFile, method, profile
#if DEBUG
                , ShaderFlags.Debug

#endif
                          ));

        }

        public HlslShader DefaultShader { get; }

        IShader IShaderFactory.DefaultShader => DefaultShader;
    }
}
