using _3DNet.Engine.Rendering;
using _3DNet.Math.Extensions;
using _3DNet.Rendering.Buffer;
using SharpDX.Direct3D;
using SharpDX.Direct3D12;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Threading;
using System.Windows.Forms;

namespace _3DNet.Rendering.D3D12
{
    internal class D3DRenderWindowContext : IRenderContextInternal
    {
        private readonly Queue<Action<GraphicsCommandList>> _commandQueue = new();
        private readonly EventWaitHandle _renderHandle = new(false, EventResetMode.AutoReset);
        private readonly Device _d3DDevice;
        private readonly CommandAllocator _d3DCommandAllocator;
        private readonly CommandQueue _d3DCommandQueue;
        private readonly IEnumerable<ID3DObject> _d3DObjects;
        private readonly D3DRenderForm _d3DRenderForm;
        private readonly Action<IRenderContextInternal> _setActive;
        private bool _disposing;
        private Matrix4x4 _world = Matrix4x4.Identity;
        private Matrix4x4 _worldViewProjection;
        private Matrix4x4 _projection = Matrix4x4.Identity;
        private Matrix4x4 _view = Matrix4x4.Identity;
        private readonly byte[] _worldViewProjectionBytes = new byte[sizeof(float) * 16];
        private readonly Fence _fence;

        public D3DRenderWindowContext(Device device, CommandAllocator commandAllocator, CommandQueue commandQueue, IEnumerable<ID3DObject> d3DObjects, D3DRenderForm d3DRenderForm, Action<IRenderContextInternal> setActive)
        {
            _d3DDevice = device;
            _d3DCommandAllocator = commandAllocator;
            _d3DCommandQueue = commandQueue;
            _d3DObjects = d3DObjects;
            _d3DRenderForm = d3DRenderForm;
            _setActive = setActive;
            _d3DRenderForm.Disposed += (_, __) => Dispose();
            _d3DRenderForm.FormClosed += (_, __) => _d3DRenderForm.Dispose();
            _fence = _d3DDevice.CreateFence(0, FenceFlags.None);
        }

        public IRenderWindow RenderWindow => _d3DRenderForm;

        public bool IsDisposed { get; private set; }
        public Matrix4x4 View
        {
            get => _view; private set
            {
                if (_view != value)
                {
                    _view = value;
                    _worldViewProjection = _world * _view * _projection;
                }
            }
        }
        public Matrix4x4 Projection
        {
            get => _projection; private set
            {
                if (_projection != value)
                {
                    _projection = value;
                    _worldViewProjection = _world * _view * _projection;
                }
            }
        }
        public Matrix4x4 World
        {
            get => _world; private set
            {
                if (_world != value)
                {
                    _world = value;
                    _worldViewProjection = _world * _view * _projection;
                }
            }
        }
        public Matrix4x4 WorldViewProjection => _worldViewProjection;

        public bool BeginScene(Color backgroundColor, long ms)
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
        => _commandQueue.Enqueue(c => c.ClearRenderTargetView(new CpuDescriptorHandle { Ptr = ptr }, new RawColor4(clearColor.R, clearColor.G, clearColor.B, clearColor.A)));

        public void Dispose()
        {
            if (IsDisposed || _disposing) { return; }
            _disposing = true;
            if (!_d3DRenderForm.IsDisposed)
            {
                _d3DRenderForm.Dispose();
            }
            _fence.Dispose();
            _disposing = false;
            IsDisposed = true;
        }

        public void EndScene(long ms)
        {
            foreach (var d3DObject in _d3DObjects)
            {
                d3DObject.End(this);
            }
            var commandList = _d3DDevice.CreateCommandList(CommandListType.Direct, _d3DCommandAllocator, null);
            while (_commandQueue.Count > 0)
            {
                _commandQueue.Dequeue()(commandList);
            }
            commandList.Close();
            _d3DCommandQueue.ExecuteCommandList(commandList);
            _d3DRenderForm.Present();
            _d3DCommandQueue.Signal(_fence, ms);
            if (_fence.CompletedValue < ms)
            {
                _fence.SetEventOnCompletion(ms, _renderHandle.SafeWaitHandle.DangerousGetHandle());
                _renderHandle.WaitOne();
            }

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

        internal void SetPrimitiveTopology(PrimitiveTopology topology)
        => _commandQueue.Enqueue(c => c.PrimitiveTopology = topology);

        internal void SetPipelineState(PipelineState graphicsPipelineState)
        => _commandQueue.Enqueue(c => c.PipelineState = graphicsPipelineState);

        public void LoadShaderBuffer(int slot, IntPtr address)
        => _commandQueue.Enqueue(c => c.SetGraphicsRootConstantBufferView(slot, address.ToInt64()));

        internal void SetGraphicsRootSignature(RootSignature rootSignature)
        => _commandQueue.Enqueue(c => c.SetGraphicsRootSignature(rootSignature));

        public void QueueAction(Action a)
        => _commandQueue.Enqueue(c => a());

        public void Draw(IBuffer vertexBuffer, IBuffer indexBuffer)
        {
            vertexBuffer.Load(this);
            indexBuffer.Load(this);
            _commandQueue.Enqueue(c => c.DrawIndexedInstanced(indexBuffer.Length, 1, 0, 0, 0));
        }

        internal void SetScissorRect(RawRectangle scissorRect) => _commandQueue.Enqueue(c => c.SetScissorRectangles(scissorRect));
        internal void SetViewport(RawViewportF viewport) => _commandQueue.Enqueue(c => c.SetViewport(viewport));
        public void SetActiveContext() => _setActive(this);
    }
}
