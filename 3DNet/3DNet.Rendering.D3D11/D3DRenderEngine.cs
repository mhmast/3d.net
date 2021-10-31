using _3DNet.Engine.Rendering;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D12.Device;
using System.Drawing;
using SharpDX.Direct3D;
using _3DNet.Math;
using SharpDX.Direct3D12;
using System.Collections.Generic;
using System.Linq;
using _3DNet.Rendering.D3D12.Buffer;
using _3DNet.Engine.Rendering.Buffer;
using System;
using System.Windows.Forms;
using _3DNet.Rendering.D3D12.Shaders;

namespace _3DNet.Rendering.D3D12
{
    internal class D3DRenderEngine : IRenderEngine
    {
        private readonly DriverType _driverType = DriverType.Warp;
        private Device _device;
        private CommandQueue _commandQueue;
        private CommandAllocator _commandAllocator;
        private GraphicsCommandList _commandList;

        //private List<CommandListHolder> _commandLists = new();
        private Fence _fence;
        private readonly System.Threading.EventWaitHandle _renderHandle = new System.Threading.EventWaitHandle(false, System.Threading.EventResetMode.AutoReset);
        private readonly List<ID3DObject> _d3DObjects = new();

     
        public void RegisterD3DObject(ID3DObject d3DObject) => _d3DObjects.Add(d3DObject);
        public void UnregisterD3DObject(ID3DObject d3DObject) => _d3DObjects.Remove(d3DObject);

        public bool BeginScene(IRenderTarget target, Color clearColor)
        {
            Application.DoEvents();

            if (target.IsDisposed)
            {
                return false;
            }
            _fence = _device.CreateFence(0, FenceFlags.None);
            _fence.SetEventOnCompletion(1, _renderHandle.SafeWaitHandle.DangerousGetHandle());
            _commandList = _device.CreateCommandList(CommandListType.Direct, _commandAllocator, null);
            foreach (var d3DObject in _d3DObjects)
            {
                d3DObject.Begin(_commandList);
            }
            target.Clear(clearColor);
            return true;
        }

        public void EndScene(IRenderTarget target)
        {
            foreach (var d3DObject in _d3DObjects)
            {
                d3DObject.End(_commandList);
            }
            _commandList.Close();
            _commandQueue.ExecuteCommandList(_commandList);
            target.Present();
            _commandQueue.Signal(_fence, 1);
            _renderHandle.WaitOne();
            _fence.Dispose();
        }

        internal SwapChain CreateSwapChain(SwapChainDescription swapChainDescription)
        {
            using var factory = new Factory4();
            var tempSwapChain = new SwapChain(factory, _commandQueue, swapChainDescription);
            var swapChain = tempSwapChain.QueryInterface<SwapChain3>(); ;
            tempSwapChain.Dispose();
            return swapChain;
        }

        internal D3DRenderForm CreateRenderForm(string name, bool fullScreen, Size size) => new(this, name, fullScreen) { Size = size };

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
            
        }

        internal RootSignature CreateRootSignature(Blob byteCode) => _device.CreateRootSignature(byteCode);

        public void SetProjection(Matrix4x4 projection)
        {

        }


        public IndexBuffer CreateIndexBuffer(IEnumerable<int> data) => new(_device, data);

        public VertexBuffer<T> CreateVertexBuffer<T>(IEnumerable<T> data) where T : IVertex => new(_device, data);


        public void SetView(Matrix4x4 view)
        {

        }

        public void SetWorld(Matrix4x4 world)
        {
        }

        //internal GraphicsCommandList AddCommandList(PipelineState graphicsPipelineState)
        //{
        //    var commandList = _device.CreateCommandList(CommandListType.Direct, _commandAllocator, graphicsPipelineState);
        //    _commandLists.Add(new CommandListHolder(commandList,_commandAllocator,graphicsPipelineState));
        //    return commandList;
        //}

        //class CommandListHolder
        //{
        //    private readonly CommandAllocator allocator;
        //    private readonly PipelineState pipeline;

        //    public CommandListHolder(GraphicsCommandList  list, CommandAllocator allocator,PipelineState pipeline)
        //    {
        //        List = list;
        //        this.allocator = allocator;
        //        this.pipeline = pipeline;
        //    }

        //    public void Reset() => List.Reset(allocator, pipeline);

        //    public GraphicsCommandList List { get; }
        //}

        internal PipelineState CreateGraphicsPipelineState(GraphicsPipelineStateDescription gpsDesc) => _device.CreateGraphicsPipelineState(gpsDesc);

        internal DescriptorHeap CreateDescriptorHeap(DescriptorHeapDescription desc) => _device.CreateDescriptorHeap(desc);

        internal void CreateRenderTargetView(SharpDX.Direct3D12.Resource backBuffer, RenderTargetViewDescription? desc, CpuDescriptorHandle renderView) => _device.CreateRenderTargetView(backBuffer, desc, renderView);

        internal SharpDX.Direct3D12.Resource CreateCommittedResource(HeapProperties heapProperties, HeapFlags heapFlags, ResourceDescription desc, ResourceStates copyDestination, ClearValue? clearValue) => _device.CreateCommittedResource(heapProperties, heapFlags, desc, copyDestination, clearValue);

        internal void CreateDepthStencilView(SharpDX.Direct3D12.Resource depthStencilBuffer, DepthStencilViewDescription? descRef, CpuDescriptorHandle depthStencilView) => _device.CreateDepthStencilView(depthStencilBuffer, descRef, depthStencilView);


    }
}
