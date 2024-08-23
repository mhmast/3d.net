using _3DNet.Engine.Rendering;
using _3DNet.Engine.Rendering.Buffer;
using _3DNet.Math.Extensions;
using _3DNet.Rendering.Buffer;
using SharpDX.Direct3D;
using SharpDX.Direct3D12;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly GraphicsCommandList _d3DCommandList;
        private readonly IEnumerable<ID3DObject> _d3DObjects;
        private readonly D3DRenderForm _d3DRenderForm;
        private readonly Action<IRenderContextInternal> _setActive;
        private bool _disposing;
        private WvpBuffer _worldViewProjectionBuffer = new();
        private long _currentFrame;
        private PipelineState _lastKnownPipelineState;
        private readonly Fence _d3DFence;

        public D3DRenderWindowContext(Device device, CommandAllocator commandAllocator, CommandQueue commandQueue, IEnumerable<ID3DObject> d3DObjects, D3DRenderForm d3DRenderForm, Action<IRenderContextInternal> setActive)
        {
            _d3DDevice = device;
            _d3DCommandAllocator = commandAllocator;
            _d3DCommandQueue = commandQueue;
            _d3DCommandList = _d3DDevice.CreateCommandList(CommandListType.Direct, _d3DCommandAllocator, null);
            _d3DCommandList.Close();
            _d3DObjects = d3DObjects;
            _d3DRenderForm = d3DRenderForm;
            _setActive = setActive;
            _d3DFence = _d3DDevice.CreateFence(0, FenceFlags.None);
        }

        public IRenderWindow RenderWindow => _d3DRenderForm;

        public bool IsDisposed { get; private set; }

        public WvpBuffer WvpBuffer => _worldViewProjectionBuffer;

        public bool BeginScene(Color backgroundColor, long frame)
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
            _d3DCommandAllocator.Reset();
            _d3DCommandList.Reset(_d3DCommandAllocator, _lastKnownPipelineState);
            _d3DRenderForm.Begin(this);
            _d3DRenderForm.Clear(this, backgroundColor);
            return true;
        }
        public void EndScene(long frame)
        {
           
            foreach (var d3DObject in _d3DObjects)
            {
                d3DObject.End(this);
            }
            _d3DRenderForm.End(this);
            while (_commandQueue.Count > 0)
            {
                _commandQueue.Dequeue()(_d3DCommandList);
            }
            _d3DCommandList.Close();
            _d3DCommandQueue.ExecuteCommandList(_d3DCommandList);
            _d3DRenderForm.Present();

            _currentFrame = frame;
            WaitForFrameToComplete(_currentFrame);
            Debug.WriteLine("-------END SCENE");
        }

        private void WaitForFrameToComplete(long frame)
        {
            _d3DCommandQueue.Signal(_d3DFence, frame);
            if (_d3DFence.CompletedValue < frame)
            {
                _d3DFence.SetEventOnCompletion(frame, _renderHandle.SafeWaitHandle.DangerousGetHandle());
                _renderHandle.WaitOne();
            }
        }
        public void ClearDepthStencilView(IntPtr ptr, float depth, byte stencil) => _commandQueue.Enqueue(c => c.ClearDepthStencilView(new CpuDescriptorHandle { Ptr = ptr }, ClearFlags.FlagsDepth | ClearFlags.FlagsStencil, depth, stencil));

        public void ClearRenderTargetView(IntPtr ptr, Color clearColor)
        => _commandQueue.Enqueue(c =>
        {
            System.Diagnostics.Debug.WriteLine($"-------{nameof(ClearRenderTargetView)} {ptr}");
            c.ClearRenderTargetView(new CpuDescriptorHandle { Ptr = ptr }, new RawColor4(clearColor.R, clearColor.G, clearColor.B, clearColor.A));
        });

        public void Dispose()
        {
            if (IsDisposed || _disposing) { return; }
            _disposing = true;
            WaitForFrameToComplete(_currentFrame);
            if (!_d3DRenderForm.IsDisposed)
            {
                _d3DRenderForm.Dispose();
            }
            _d3DCommandAllocator.Dispose();
            _d3DCommandList.Dispose();
            _d3DCommandQueue.Dispose();
            _d3DDevice.Dispose();
            _d3DFence.Dispose();
            _disposing = false;
            IsDisposed = true;
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
        => _commandQueue.Enqueue(c => _worldViewProjectionBuffer.projection = projection);

        public void SetVertexBuffer(IntPtr bufferLocation, int sizeInBytes, int strideInBytes)
        => _commandQueue.Enqueue(c => c.SetVertexBuffer(0, new VertexBufferView { BufferLocation = bufferLocation.ToInt64(), SizeInBytes = sizeInBytes, StrideInBytes = strideInBytes }));

        public void SetView(Matrix4x4 view)
        => _commandQueue.Enqueue(c => _worldViewProjectionBuffer.view = view);
        public void SetWorld(Matrix4x4 world)
        => _commandQueue.Enqueue(c => _worldViewProjectionBuffer.world = world);

        internal void ResourceBarrierTransition(Resource buffer, ResourceStates oldState, ResourceStates newState)
       => _commandQueue.Enqueue(c =>
       {
           System.Diagnostics.Debug.WriteLine($"-------{nameof(ResourceBarrierTransition)} {buffer.NativePointer}");
           c.ResourceBarrierTransition(buffer, oldState, newState);
       });

        internal void SetPrimitiveTopology(PrimitiveTopology topology)
        => _commandQueue.Enqueue(c => c.PrimitiveTopology = topology);

        internal void SetPipelineState(PipelineState graphicsPipelineState)
        => _commandQueue.Enqueue(c =>
        {
            c.PipelineState = graphicsPipelineState;
            _lastKnownPipelineState= graphicsPipelineState;
        });

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
        internal void SetRenderTarget(CpuDescriptorHandle renderTarget, CpuDescriptorHandle depthStencil) => _commandQueue.Enqueue(c =>
        {
            System.Diagnostics.Debug.WriteLine($"-------{nameof(SetRenderTarget)} {renderTarget.Ptr} {depthStencil.Ptr}");
            c.SetRenderTargets(renderTarget, depthStencil);
        });
        public void SetActiveContext() => _setActive(this);
    }
}
