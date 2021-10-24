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
using System;

namespace _3DNet.Rendering.D3D12
{
    internal class D3DRenderEngine : IRenderEngine
    {
        private readonly DriverType _driverType;
        private Device _device;
        private CommandAllocator _commandAllocator;
        private List<GraphicsCommandList> _commandLists = new();

        public void BeginScene(IRenderTarget target, Color clearColor)
        {
            target.Clear(clearColor);
        }

        public void EndScene(IRenderTarget target)
        {
            target.Present();
        }

        internal D3DRenderForm CreateRenderForm(string name, bool fullScreen, Size size) => new (_device, _commandLists.FirstOrDefault(), name, fullScreen) { Size = size };

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
            //_commandQueue = _device.CreateCommandQueue(new CommandQueueDescription(CommandListType.Direct));
            _commandAllocator = _device.CreateCommandAllocator(CommandListType.Direct);
        }

        internal RootSignature CreateRootSignature(Blob byteCode) => _device.CreateRootSignature(byteCode);

        public void SetProjection(Matrix4x4 projection)
        {

        }


        public IndexBuffer CreateIndexBuffer(IEnumerable<int> data) => new(_device, data);

        public VertexBuffer<T> CreateVertexBuffer<T>(IEnumerable<T> data) where T : struct => new(_device, data);


        public void SetView(Matrix4x4 view)
        {

        }

        public void SetWorld(Matrix4x4 world)
        {
        }

        internal GraphicsCommandList AddCommandList(PipelineState graphicsPipelineState)
        {
            var commandList = _device.CreateCommandList(CommandListType.Direct, _commandAllocator, graphicsPipelineState);
            _commandLists.Add(commandList);
            return commandList;
        }

        internal PipelineState CreateGraphicsPipelineState(GraphicsPipelineStateDescription gpsDesc) => _device.CreateGraphicsPipelineState(gpsDesc);
    }
}
