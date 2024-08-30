﻿using _3DNet.Engine.Rendering;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D12.Device;
using System.Drawing;
using SharpDX.Direct3D;
using SharpDX.Direct3D12;
using System.Collections.Generic;
using _3DNet.Rendering.D3D12.Buffer;
using NativeWvpBuffer = _3DNet.Rendering.D3D12.Buffer.WvpBuffer;
using System;
using _3DNet.Rendering.D3D12.RenderTargets;
using System.Linq;
using _3DNet.Rendering.D3D12.Shaders;
using _3DNet.Engine.Rendering.Shader;
using System.IO;
using System.Diagnostics;
using WvpBuffer = _3DNet.Engine.Rendering.Buffer.WvpBuffer;
using System.Threading;

namespace _3DNet.Rendering.D3D12
{
    internal class D3DRenderEngine : IRenderEngine, IShaderFactory, ICommandBundleExecutor
    {
#if DEBUG
        private readonly DriverType _driverType = DriverType.Hardware;
        private SharpDX.Direct3D12.InfoQueue _infoQueue;
#else
        private readonly DriverType _driverType = DriverType.Hardware;
#endif
        private Device _device;
        private CommandQueue _commandQueue;
        private CommandAllocator _commandAllocator;
        private CommandAllocator _bundleAllocator;
        private GraphicsCommandList _commandList;
        private readonly IDictionary<string,GraphicsCommandList> _commandLists = new Dictionary<string,GraphicsCommandList>();
        private Fence _fence;
        private long _currentFrame;
        private bool _isDisposed;
        private readonly EventWaitHandle _renderHandle = new(false, EventResetMode.AutoReset);
        private readonly IDictionary<string, ID3DRenderTarget> _activeTargets = new Dictionary<string, ID3DRenderTarget>();
        private readonly string _basePath = new FileInfo(typeof(D3DRenderEngine).Assembly.Location).DirectoryName;
        private readonly IDictionary<string, HlslShader> _shaders = new Dictionary<string, HlslShader>();

        public D3DRenderEngine(IShaderBufferDataAdapterBuilder shaderBufferDataAdapterBuilder)
        {
            _shaderBufferDataConverterBuilder = shaderBufferDataAdapterBuilder;
        }

        internal void CreateConstantBufferView(ConstantBufferViewDescription cbvDesc, CpuDescriptorHandle cPUDescriptorHandleForHeapStart)
        => _device.CreateConstantBufferView(cbvDesc, cPUDescriptorHandleForHeapStart);

        internal int NoOfCreatedTargets => _activeTargets.Count;

        internal Format[] RenderTargetFormats => _activeTargets.Values.Select(t => t.Format).ToArray();

        private readonly List<ID3DObject> _d3DObjects = new();
        private readonly IShaderBufferDataAdapterBuilder _shaderBufferDataConverterBuilder;

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
            var form = new D3DRenderForm(this, name) { Size = size, FullScreen = fullScreen };
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

        internal SwapChain3 CreateSwapChain(SwapChainDescription swapChainDescription)
        {
            using var factory = new Factory4();
            var tempSwapChain = new SwapChain(factory, _commandQueue, swapChainDescription);
            var swapChain = tempSwapChain.QueryInterface<SwapChain3>();
            tempSwapChain.Dispose();
            return swapChain;
        }

        internal SwapChain3 ReCreateSwapChain(SwapChainDescription swapChainDescription)
        {
            _commandQueue.Dispose();
            _commandQueue = _device.CreateCommandQueue(CommandListType.Direct);
            return CreateSwapChain(swapChainDescription);
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
            _infoQueue.SetBreakOnSeverity(MessageSeverity.Message, true);
            _infoQueue.SetBreakOnSeverity(MessageSeverity.Information, true);
            _infoQueue.SetBreakOnSeverity(MessageSeverity.Corruption, true);
            _infoQueue.SetBreakOnSeverity(MessageSeverity.Error, true);

            // Set the maximum number of messages to be stored in the queue
            _infoQueue.MessageCountLimit = 1024;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
#endif

            _commandQueue = _device.CreateCommandQueue(new CommandQueueDescription(CommandListType.Direct));
            _commandAllocator = _device.CreateCommandAllocator(CommandListType.Direct);
            _bundleAllocator = _device.CreateCommandAllocator(CommandListType.Bundle);
            _commandList = _device.CreateCommandList(CommandListType.Direct, _commandAllocator, null);
            _commandList.Close();
            _fence = _device.CreateFence(0, FenceFlags.None);
            var bufferAdapter = _shaderBufferDataConverterBuilder.AddConverter<WvpBuffer, NativeWvpBuffer>(m => (NativeWvpBuffer)m).Build();
            var buffers = new[] { ShaderBufferDescription.Create<NativeWvpBuffer>("globals", 0, BufferType.GPUInput, BufferUsage.VertexShader, bufferAdapter) };
            var defaultShaderDescription = new ShaderDescription(Path.Combine(_basePath, "Shaders", "default.hlsl"), "vs_5_0", "VSMain", "ps_5_0", "PSMain", buffers, "globals");
            DefaultShader = LoadShader("Default", defaultShaderDescription);
        }

#if DEBUG
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(((Exception)e.ExceptionObject).Message);
            FlushInfoBuffer();
        }

        private void FlushInfoBuffer()
        {
            _device.QueryInterfaceOrNull<DebugDevice>().ReportLiveDeviceObjects(ReportingLevel.Summary);
            for (int i = 0; i < _infoQueue.NumStoredMessagesAllowedByRetrievalFilter; i++)
            {
                var message = _infoQueue.GetMessage(i);
                if (message.Severity == MessageSeverity.Information)
                {
                    Debug.WriteLine($"DXMESSAGE: {message.Description}");
                }
                else
                {
                    Debug.WriteLine($"DXERROR: {message.Description}");
                }
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
        => new D3DRenderWindowContext(_d3DObjects, CreateWindow(size, name, fullScreen), setActive, this);

        public void ExecuteCommandBundle(string name, Queue<Action<GraphicsCommandList>> actions)
        {
            var commandList = GetOrCreateCommandBundle(name);
            commandList.Reset(_bundleAllocator,null);
            while (actions.Count > 0)
            {
                actions.Dequeue()(commandList);
            }
            commandList.Close();
            _commandList.ExecuteBundle(commandList);
        }

        private GraphicsCommandList GetOrCreateCommandBundle(string name)
        {
            if(!_commandLists.ContainsKey(name))
            {
                var list = _device.CreateCommandList(CommandListType.Bundle, _bundleAllocator, null);
                list.Close();
                _commandLists.Add(name, list);
            }
            return _commandLists[name];
        }

        internal ICommandBundleExecutor BeginExecuteCommandBundle(Queue<Action<GraphicsCommandList>> actions)
        {
            _commandAllocator.Reset();
            _bundleAllocator.Reset();
            _commandList.Reset(_commandAllocator, null);
            while (actions.Count > 0)
            {
                actions.Dequeue()(_commandList);
            }
            return this;
        }

        internal void EndExecuteCommandBundle(long frame,IRenderTarget renderTarget)
        {
            _commandList.Close();
            _commandQueue.ExecuteCommandList(_commandList);
            renderTarget.Present();

            _currentFrame = frame;
            if (!_isDisposed)
            {
                WaitForFrameToComplete(_currentFrame);
            }
        }
        private void WaitForFrameToComplete(long frame)
        {
            _commandQueue.Signal(_fence, frame);
            if (_fence.CompletedValue < frame)
            {
                _fence.SetEventOnCompletion(frame, _renderHandle.SafeWaitHandle.DangerousGetHandle());
                _renderHandle.WaitOne();
            }
        }

        internal int GetRenderTargetDescriptorHandleIncrementSize() => _device.GetDescriptorHandleIncrementSize(DescriptorHeapType.RenderTargetView);

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    WaitForFrameToComplete(_currentFrame);

                    _commandAllocator.Dispose();
                    foreach(var commandList in _commandLists.Values)
                    {
                        commandList.Dispose();
                    }
                    _commandList.Dispose();
                    _commandQueue.Dispose();
                    _fence.Dispose();
                    _device.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _isDisposed = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~D3DRenderEngine()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }


    }
}
