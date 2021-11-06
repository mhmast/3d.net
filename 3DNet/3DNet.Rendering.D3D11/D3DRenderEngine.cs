using _3DNet.Engine.Rendering;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D12.Device;
using System.Drawing;
using SharpDX.Direct3D;
using SharpDX.Direct3D12;
using System.Collections.Generic;
using _3DNet.Rendering.D3D12.Buffer;
using _3DNet.Engine.Rendering.Buffer;
using System;
using _3DNet.Rendering.D3D12.RenderTargets;
using System.Linq;
using _3DNet.Rendering.D3D12.Shaders;
using _3DNet.Engine.Rendering.Shader;
using System.IO;

namespace _3DNet.Rendering.D3D12
{
    internal class D3DRenderEngine : IRenderEngine ,IShaderFactory
    {
        private readonly DriverType _driverType = DriverType.Warp;
        private Device _device;
        private CommandQueue _commandQueue;
        private CommandAllocator _commandAllocator;
        private readonly IDictionary<string, ID3DRenderTarget> _activeTargets = new Dictionary<string, ID3DRenderTarget>();
        private readonly string _basePath = new FileInfo(typeof(D3DRenderEngine).Assembly.Location).DirectoryName;
        private readonly IDictionary<string, HlslShader> _shaders = new Dictionary<string, HlslShader>();

        internal int NoOfCreatedTargets => _activeTargets.Count;

        internal Format[] RenderTargetFormats => _activeTargets.Values.Select(t => t.Format).ToArray();

        private readonly List<ID3DObject> _d3DObjects = new();

        public event Action RenderTargetCreated;
        public event Action RenderTargetDropped;

        public void RegisterD3DObject(ID3DObject d3DObject) => _d3DObjects.Add(d3DObject);
        public void UnregisterD3DObject(ID3DObject d3DObject) => _d3DObjects.Remove(d3DObject);


        private D3DRenderForm CreateWindow(Size size, string name, bool fullScreen = false)
        {
            if (_activeTargets.ContainsKey(name))
            {
                throw new ArgumentException($"There is already a rendertarget with the name {name}");
            }
            var form = new D3DRenderForm(this, name, fullScreen) { Size = size };
            _activeTargets.Add(name, form);
            form.FormClosed += (_, __) =>
            {
                _activeTargets.Remove(name);
                RenderTargetDropped?.DynamicInvoke();
            };
            form.Activated += (_, __) => RenderTargetCreated?.DynamicInvoke();
            form.Show();

            return form;
        }

        internal SwapChain CreateSwapChain(SwapChainDescription swapChainDescription)
        {
            using var factory = new Factory4();
            var tempSwapChain = new SwapChain(factory, _commandQueue, swapChainDescription);
            var swapChain = tempSwapChain.QueryInterface<SwapChain3>(); ;
            tempSwapChain.Dispose();
            return swapChain;
        }


        private HlslShader LoadShader(string name, ShaderDescription description)
        {
            var shader = new HlslShader(name, this, description);
            _shaders.Add(shader.ShaderSignature, shader);
            return shader;
        }

        IShader IShaderFactory.LoadShader(string name, ShaderDescription description)
       => LoadShader(name, description);

        internal HlslShader DefaultShader { get; set; }


        IShader IShaderFactory.DefaultShader => DefaultShader;

        public void Initialize()
        {
#if DEBUG
            // Enable the D3D12 debug layer.
            {
                DebugInterface.Get().EnableDebugLayer();

            }
#endif
            using var factory = new Factory4();
            var adapter = _driverType == DriverType.Hardware ? null : factory.GetWarpAdapter();
            _device = new Device(adapter, FeatureLevel.Level_12_1);
            _commandQueue = _device.CreateCommandQueue(new CommandQueueDescription(CommandListType.Direct));
            _commandAllocator = _device.CreateCommandAllocator(CommandListType.Direct);
            DefaultShader = LoadShader("Default", new ShaderDescription { ShaderFile = Path.Combine(_basePath, "Shaders", "default.hlsl"), VertexShaderMethod = "VSMain", PixelShaderMethod = "PSMain", VertexShaderProfile = "vs_5_0", PixelShaderProfile = "ps_5_0" });

        }

        internal RootSignature CreateRootSignature(Blob byteCode) => _device.CreateRootSignature(byteCode);

        public IndexBuffer CreateIndexBuffer(IEnumerable<int> data) => new(_device, data);

        public VertexBuffer<T> CreateVertexBuffer<T>(IEnumerable<T> data) where T : IVertex => new(_device, data);


        internal PipelineState CreateGraphicsPipelineState(GraphicsPipelineStateDescription gpsDesc) => _device.CreateGraphicsPipelineState(gpsDesc);

        internal DescriptorHeap CreateDescriptorHeap(DescriptorHeapDescription desc) => _device.CreateDescriptorHeap(desc);

        internal void CreateRenderTargetView(SharpDX.Direct3D12.Resource backBuffer, RenderTargetViewDescription? desc, CpuDescriptorHandle renderView) => _device.CreateRenderTargetView(backBuffer, desc, renderView);

        internal SharpDX.Direct3D12.Resource CreateCommittedResource(HeapProperties heapProperties, HeapFlags heapFlags, ResourceDescription desc, ResourceStates copyDestination, ClearValue? clearValue) => _device.CreateCommittedResource(heapProperties, heapFlags, desc, copyDestination, clearValue);

        internal void CreateDepthStencilView(SharpDX.Direct3D12.Resource depthStencilBuffer, DepthStencilViewDescription? descRef, CpuDescriptorHandle depthStencilView) => _device.CreateDepthStencilView(depthStencilBuffer, descRef, depthStencilView);

        public IRenderWindowContext CreateRenderWindowContext(string name, Size size, bool fullScreen)
        => new D3DRenderWindowContext(_device, _commandAllocator, _commandQueue, _d3DObjects, CreateWindow(size, name, fullScreen));

    }
}
