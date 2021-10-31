using _3DNet.Engine.Rendering.Shader;
using Device = SharpDX.Direct3D12.Device;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D12;
using SharpDX.DXGI;
using System.Security.Cryptography;
using System.Text;
using System.Reflection;
using System.IO;
using ShaderDescription = _3DNet.Engine.Rendering.Shader.ShaderDescription;
using System.Collections.Generic;

namespace _3DNet.Rendering.D3D12.Shaders
{
    internal class D3DShaderFactory : IShaderFactory
    {
        private readonly D3DRenderEngine _d3DRenderEngine;
        private readonly D3DRenderTargetFactory _renderTargetFactory;
        private readonly string _basePath = new FileInfo(typeof(D3DShaderFactory).Assembly.Location).DirectoryName;
        private readonly IDictionary<string,HlslShader> _shaders = new Dictionary<string,HlslShader>();

        public D3DShaderFactory(D3DRenderEngine d3DRenderEngine,D3DRenderTargetFactory renderTargetFactory)
        {
            _d3DRenderEngine = d3DRenderEngine;
            _renderTargetFactory = renderTargetFactory;
            DefaultShader = LoadShader("Default",new ShaderDescription { ShaderFile = Path.Combine(_basePath, "Shaders", "default.hlsl"), VertexShaderMethod = "VSMain", PixelShaderMethod = "PSMain", VertexShaderProfile = "vs_5_0", PixelShaderProfile = "ps_5_0" });
        }

        private HlslShader LoadShader(string name,ShaderDescription description)
        {
            var shader =  new HlslShader(name,_d3DRenderEngine,_renderTargetFactory, description);
            _shaders.Add(shader.ShaderSignature, shader);
            return shader;
        }

       

        IShader IShaderFactory.LoadShader(string name,ShaderDescription description)
       => LoadShader(name,description);

        public HlslShader DefaultShader { get; }

        IShader IShaderFactory.DefaultShader => DefaultShader;
    }
}
