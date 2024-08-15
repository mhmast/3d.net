using _3DNet.Engine.Rendering;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D12.Device;
using System.Drawing;
using SharpDX.Direct3D;
using SharpDX.Direct3D12;
using System.Collections.Generic;
using _3DNet.Rendering.D3D12.Buffer;
using System;
using _3DNet.Rendering.D3D12.RenderTargets;
using System.Linq;
using _3DNet.Rendering.D3D12.Shaders;
using _3DNet.Engine.Rendering.Shader;
using System.IO;
using SharpDX;
using System.Numerics;
using _3DNet.Rendering.Buffer;
using System.Diagnostics;

namespace _3DNet.Rendering.D3D12
{
    internal class D3DRenderEngine : IRenderEngine, IShaderFactory
    {
#if DEBUG
        private readonly DriverType _driverType = DriverType.Warp;
        private SharpDX.Direct3D12.InfoQueue _infoQueue;
#else
        private readonly DriverType _driverType = DriverType.Hardware;
#endif
        private Device _device;
        private CommandQueue _commandQueue;
        private CommandAllocator _commandAllocator;
        private IBuffer<Matrix4x4> _wvpBuffer;
        private readonly IDictionary<string, ID3DRenderTarget> _activeTargets = new Dictionary<string, ID3DRenderTarget>();
        private readonly string _basePath = new FileInfo(typeof(D3DRenderEngine).Assembly.Location).DirectoryName;
        private readonly IDictionary<string, HlslShader> _shaders = new Dictionary<string, HlslShader>();

        internal void CreateConstantBufferView(ConstantBufferViewDescription cbvDesc, CpuDescriptorHandle cPUDescriptorHandleForHeapStart)
        => _device.CreateConstantBufferView(cbvDesc, cPUDescriptorHandleForHeapStart);

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
            form.Load += (_, __) => RenderTargetCreated?.DynamicInvoke();
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

            DebugInterface.Get().EnableDebugLayer();

#endif
            using var factory = new Factory4();
            var adapter = _driverType == DriverType.Hardware ? null : factory.GetWarpAdapter();
            _device = new Device(adapter, FeatureLevel.Level_12_1);

#if DEBUG
            // Get the InfoQueue from the device's debug interface
            _infoQueue = _device.QueryInterface<SharpDX.Direct3D12.InfoQueue>();

            // Optionally filter messages to focus on certain types of issues
            _infoQueue.SetBreakOnSeverity(MessageSeverity.Corruption, true);
            _infoQueue.SetBreakOnSeverity(MessageSeverity.Error, true);

            // Set the maximum number of messages to be stored in the queue
            _infoQueue.MessageCountLimit = 1024;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
#endif

            _commandQueue = _device.CreateCommandQueue(new CommandQueueDescription(CommandListType.Direct));
            _commandAllocator = _device.CreateCommandAllocator(CommandListType.Direct);
            var defaultShaderDescription = new ShaderDescription(Path.Combine(_basePath, "Shaders", "default.hlsl"), "vs_5_0", "VSMain", "ps_5_0", "PSMain");
            DefaultShader = LoadShader("Default", defaultShaderDescription);
            _wvpBuffer = DefaultShader.CreateBuffer<Matrix4x4>(new ShaderBufferDescription("Default_wvp_buffer", 0, BufferType.GPUInput, BufferUsage.VertexShader), 1);
        }

#if DEBUG
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(((Exception)e.ExceptionObject).Message);
            for (int i = 0; i < _infoQueue.NumStoredMessagesAllowedByRetrievalFilter; i++)
            {
                var message = _infoQueue.GetMessage(i);
                Debug.WriteLine($"DXERROR: {message.Description}");
            }
        }

#endif

        internal RootSignature CreateRootSignature(Blob byteCode) => _device.CreateRootSignature(byteCode);

        public IndexBuffer CreateIndexBuffer(string name, uint[] data) => new(_device, name, data);

        public VertexBuffer<T> CreateVertexBuffer<T>(string name, IEnumerable<T> data) where T : struct => new(_device, name, data.ToArray());


        internal PipelineState CreateGraphicsPipelineState(GraphicsPipelineStateDescription gpsDesc) => _device.CreateGraphicsPipelineState(gpsDesc);

        internal DescriptorHeap CreateDescriptorHeap(DescriptorHeapDescription desc) => _device.CreateDescriptorHeap(desc);

        internal void CreateRenderTargetView(SharpDX.Direct3D12.Resource backBuffer, RenderTargetViewDescription? desc, CpuDescriptorHandle renderView) => _device.CreateRenderTargetView(backBuffer, desc, renderView);

        internal SharpDX.Direct3D12.Resource CreateCommittedResource(HeapProperties heapProperties, HeapFlags heapFlags, ResourceDescription desc, ResourceStates copyDestination, ClearValue? clearValue = null) => _device.CreateCommittedResource(heapProperties, heapFlags, desc, copyDestination, clearValue);

        internal void CreateDepthStencilView(SharpDX.Direct3D12.Resource depthStencilBuffer, DepthStencilViewDescription? descRef, CpuDescriptorHandle depthStencilView) => _device.CreateDepthStencilView(depthStencilBuffer, descRef, depthStencilView);

        public IRenderContext CreateRenderContext(string name, Size size, bool fullScreen, Action<IRenderContextInternal> setActive)
        => new D3DRenderWindowContext(_device, _commandAllocator, _commandQueue, _d3DObjects, CreateWindow(size, name, fullScreen), setActive);
    }
}
