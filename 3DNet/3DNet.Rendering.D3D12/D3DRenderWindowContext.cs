using _3DNet.Engine.Rendering;
using _3DNet.Math;
using SharpDX.Direct3D12;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace _3DNet.Rendering.D3D12
{
    internal class D3DRenderWindowContext : IRenderWindowContext
    {
        private readonly Queue<Action<GraphicsCommandList>> _commandQueue = new();
        private readonly EventWaitHandle _renderHandle = new(false, EventResetMode.AutoReset);
        private readonly Device _d3DDevice;
        private readonly CommandAllocator _d3DCommandAllocator;
        private readonly CommandQueue _d3DCommandQueue;
        private readonly IEnumerable<ID3DObject> _d3DObjects;
        private readonly D3DRenderForm _d3DRenderForm;
        private bool _disposing;

        public D3DRenderWindowContext(Device device, CommandAllocator commandAllocator, CommandQueue commandQueue, IEnumerable<ID3DObject> d3DObjects, D3DRenderForm d3DRenderForm)
        {
            _d3DDevice = device;
            _d3DCommandAllocator = commandAllocator;
            _d3DCommandQueue = commandQueue;
            _d3DObjects = d3DObjects;
            _d3DRenderForm = d3DRenderForm;
            _d3DRenderForm.Disposed += (_, __) => Dispose();
            _d3DRenderForm.FormClosed += (_, __) => _d3DRenderForm.Dispose();
        }

        public IRenderWindow RenderWindow => _d3DRenderForm;

        public bool IsDisposed { get; private set; }
        public Matrix4x4 View { get; private set; } = Matrix4x4.Identity;
        public Matrix4x4 Projection { get; private set; } = Matrix4x4.Identity;
        public Matrix4x4 World { get; private set; } = Matrix4x4.Identity;

        public bool BeginScene(Color backgroundColor)
        {
            Application.DoEvents();

            if (_d3DRenderForm.IsDisposed)
            {
                return false;
            }
            _commandQueue.Clear();
            foreach (var d3DObject in _d3DObjects)
            {
                d3DObject.Begin(this);
            }
            _d3DRenderForm.Clear(this, backgroundColor);
            return true;
        }

        public void ClearDepthStencilView(IntPtr ptr, float depth, byte stencil) => _commandQueue.Enqueue(c => c.ClearDepthStencilView(new CpuDescriptorHandle { Ptr = ptr }, ClearFlags.FlagsDepth | ClearFlags.FlagsStencil, depth, stencil));

        public void ClearRenderTargetView(IntPtr ptr, Color clearColor)
        => _commandQueue.Enqueue(c => c.ClearRenderTargetView(new CpuDescriptorHandle { Ptr = ptr }, new SharpDX.Mathematics.Interop.RawColor4(clearColor.R, clearColor.G, clearColor.B, clearColor.A)));

        public void Dispose()
        {
            if (IsDisposed || _disposing) { return; }
            _disposing = true;
            if (!_d3DRenderForm.IsDisposed)
            {
                _d3DRenderForm.Dispose();
            }
            _disposing = false;
            IsDisposed = true;
        }

        public void EndScene()
        {
            foreach (var d3DObject in _d3DObjects)
            {
                d3DObject.End(this);
            }
            var fence = _d3DDevice.CreateFence(0, FenceFlags.None);
            fence.SetEventOnCompletion(1, _renderHandle.SafeWaitHandle.DangerousGetHandle());
            var commandList = _d3DDevice.CreateCommandList(CommandListType.Direct, _d3DCommandAllocator, null);
            while (_commandQueue.Count > 0)
            {
                _commandQueue.Dequeue()(commandList);
            }
            commandList.Close();
            _d3DCommandQueue.ExecuteCommandList(commandList);
            _d3DRenderForm.Present();
            _d3DCommandQueue.Signal(fence, 1);
            _renderHandle.WaitOne();
            fence.Dispose();
        }

        public void SetIndexBuffer(IntPtr bufferLocation, int sizeInBytes, int strideInBytes)
        => _commandQueue.Enqueue(c => c.SetIndexBuffer(new IndexBufferView { BufferLocation = bufferLocation.ToInt64(), SizeInBytes = sizeInBytes, Format = GetFormat(strideInBytes) }));

        private static SharpDX.DXGI.Format GetFormat(int strideInBytes)
        => strideInBytes switch
        {
            4 => SharpDX.DXGI.Format.R32_UInt,
            _ => throw new NotImplementedException()
        };

        public void SetProjection(Matrix4x4 projection)
        => _commandQueue.Enqueue(c => Projection = projection);

        public void SetVertexBuffer(IntPtr bufferLocation, int sizeInBytes, int strideInBytes)
        => _commandQueue.Enqueue(c => c.SetVertexBuffer(0, new VertexBufferView { BufferLocation = bufferLocation.ToInt64(), SizeInBytes = sizeInBytes, StrideInBytes = strideInBytes }));

        public void SetView(Matrix4x4 view)
        => _commandQueue.Enqueue(c => View = view);
        public void SetWorld(Matrix4x4 world)
        => _commandQueue.Enqueue(c => World = world);

        internal void ResourceBarrierTransition(Resource buffer, ResourceStates oldState, ResourceStates newState)
       => _commandQueue.Enqueue(c => c.ResourceBarrierTransition(buffer, oldState, newState));

        internal void SetPipelineState(PipelineState graphicsPipelineState)
        => _commandQueue.Enqueue(c => c.PipelineState = graphicsPipelineState);

        public void LoadShaderBuffer(int slot, IntPtr address)
        => _commandQueue.Enqueue(c => c.SetGraphicsRootConstantBufferView(slot, address.ToInt64()));

        internal void SetGraphicsRootSignature(RootSignature rootSignature)
        => _commandQueue.Enqueue(c=>c.SetGraphicsRootSignature(rootSignature));

        public void QueueAction(Action a)
        => _commandQueue.Enqueue(c=>a());
    }
}
