using _3DNet.Engine.Rendering;
using _3DNet.Engine.Rendering.Buffer;
using _3DNet.Rendering.Buffer;
using SharpDX.Direct3D;
using SharpDX.Direct3D12;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace _3DNet.Rendering.D3D12
{
    internal class D3DRenderWindowContext : IRenderContextInternal
    {
        private const string DefaultCommandQueue = "Default";
        private string _currentCommandQueue = DefaultCommandQueue;
        private readonly IDictionary<string, Queue<Action<GraphicsCommandList>>> _commandQueues = new Dictionary<string, Queue<Action<GraphicsCommandList>>>();
        private readonly IEnumerable<ID3DObject> _d3DObjects;
        private readonly D3DRenderForm _d3DRenderForm;
        private readonly Action<IRenderContextInternal> _setActive;
        private readonly D3DRenderEngine _d3DRenderEngine;
        private bool _disposing;
        private WvpBuffer _worldViewProjectionBuffer = new();

        private Queue<Action<GraphicsCommandList>> CommandQueue { get => _commandQueues[_currentCommandQueue]; }

        public event Action GotFocus;
        public event Action LostFocus;

        public D3DRenderWindowContext(IEnumerable<ID3DObject> d3DObjects, D3DRenderForm d3DRenderForm, Action<IRenderContextInternal> setActive, D3DRenderEngine d3DRenderEngine)
        {
            _d3DObjects = d3DObjects;
            _d3DRenderForm = d3DRenderForm;
            _setActive = setActive;
            _d3DRenderEngine = d3DRenderEngine;
            d3DRenderForm.GotFocus += (_, __) => GotFocus?.DynamicInvoke();
            d3DRenderForm.LostFocus += (_, __) => LostFocus?.DynamicInvoke();
            EnsureCommandQueue(DefaultCommandQueue);
        }

        public IRenderWindow RenderWindow => _d3DRenderForm;

        public bool IsDisposed { get; private set; }

        public WvpBuffer WvpBuffer => _worldViewProjectionBuffer;

        public bool FullScreen => _d3DRenderForm.FullScreen;

        public void Update()
        {
            _d3DRenderForm.ProcessWindowMessages();
        }

        public bool BeginScene(Color backgroundColor, long frame)
        {
            if (_d3DRenderForm.IsDisposed)
            {
                return false;
            }
            ClearQueues();
            _currentCommandQueue = DefaultCommandQueue;
            EnsureCommandQueue(_currentCommandQueue);
            foreach (var d3DObject in _d3DObjects)
            {
                d3DObject.Begin(this);
            }

            _d3DRenderForm.Begin(this);
            _d3DRenderForm.Clear(this, backgroundColor);
            return true;
        }

        private void EnsureCommandQueue(string currentCommandQueue)
        {
            if (!_commandQueues.ContainsKey(currentCommandQueue))
            {
                _commandQueues.Add(currentCommandQueue, new Queue<Action<GraphicsCommandList>>());
            }
        }

        private void ClearQueues()
        {
            foreach (var queue in _commandQueues.Values)
            {
                queue.Clear();
            }
        }

        public void EndScene(long frame)
        {
            foreach (var d3DObject in _d3DObjects)
            {
                d3DObject.End(this);
            }
            _d3DRenderForm.End(this);
            var executor = _d3DRenderEngine.BeginExecuteCommandBundle(_commandQueues[DefaultCommandQueue]);
            foreach (var queue in _commandQueues.Where(k=>k.Key != DefaultCommandQueue))
            {
                executor.ExecuteCommandBundle(queue.Key, queue.Value);
            }
            _d3DRenderEngine.EndExecuteCommandBundle(frame, _d3DRenderForm);
        }


        public void ClearDepthStencilView(IntPtr ptr, float depth, byte stencil) => CommandQueue.Enqueue(c => c.ClearDepthStencilView(new CpuDescriptorHandle { Ptr = ptr }, ClearFlags.FlagsDepth | ClearFlags.FlagsStencil, depth, stencil));

        public void ClearRenderTargetView(IntPtr ptr, Color clearColor)
        => CommandQueue.Enqueue(c =>
        {
            c.ClearRenderTargetView(new CpuDescriptorHandle { Ptr = ptr }, new RawColor4(clearColor.R, clearColor.G, clearColor.B, clearColor.A));
        });

        public void Dispose()
        {
            if (IsDisposed || _disposing) { return; }
            _disposing = true;

            if (!_d3DRenderForm.IsDisposed)
            {
                _d3DRenderForm.Dispose();
            };
            _disposing = false;
            IsDisposed = true;
        }


        public void SetIndexBuffer(IntPtr bufferLocation, int sizeInBytes, int strideInBytes)
        => CommandQueue.Enqueue(c => c.SetIndexBuffer(new IndexBufferView { BufferLocation = bufferLocation.ToInt64(), SizeInBytes = sizeInBytes, Format = GetFormat(strideInBytes) }));

        private static SharpDX.DXGI.Format GetFormat(int strideInBytes)
        => strideInBytes switch
        {
            4 => SharpDX.DXGI.Format.R32_UInt,
            _ => throw new NotImplementedException()
        };

        public void SetProjection(Matrix4x4 projection)
        => CommandQueue.Enqueue(c => _worldViewProjectionBuffer.projection = projection);

        public void SetVertexBuffer(IntPtr bufferLocation, int sizeInBytes, int strideInBytes)
        => CommandQueue.Enqueue(c => c.SetVertexBuffer(0, new VertexBufferView { BufferLocation = bufferLocation.ToInt64(), SizeInBytes = sizeInBytes, StrideInBytes = strideInBytes }));

        public void SetView(Matrix4x4 view)
        => CommandQueue.Enqueue(c => _worldViewProjectionBuffer.view = view);
        public void SetWorld(Matrix4x4 world)
        => CommandQueue.Enqueue(c => _worldViewProjectionBuffer.world = world);

        internal void ResourceBarrierTransition(Resource buffer, ResourceStates oldState, ResourceStates newState)
       => CommandQueue.Enqueue(c =>
       {
           c.ResourceBarrierTransition(buffer, oldState, newState);
       });

        internal void SetPrimitiveTopology(PrimitiveTopology topology)
        => CommandQueue.Enqueue(c => c.PrimitiveTopology = topology);

        internal void SetPipelineState(PipelineState graphicsPipelineState)
        => CommandQueue.Enqueue(c => c.PipelineState = graphicsPipelineState);

        public void LoadShaderBuffer(int slot, IntPtr address)
        => CommandQueue.Enqueue(c => c.SetGraphicsRootConstantBufferView(slot, address.ToInt64()));

        internal void SetGraphicsRootSignature(RootSignature rootSignature)
        => CommandQueue.Enqueue(c => c.SetGraphicsRootSignature(rootSignature));

        public void QueueAction(Action a)
        => CommandQueue.Enqueue(c => a());

        public void Draw(IBuffer vertexBuffer, IBuffer indexBuffer)
        {
            vertexBuffer.Load(this);
            indexBuffer.Load(this);
            CommandQueue.Enqueue(c => c.DrawIndexedInstanced(indexBuffer.Length, 1, 0, 0, 0));
        }

        internal void SetScissorRect(RawRectangle scissorRect) => CommandQueue.Enqueue(c => c.SetScissorRectangles(scissorRect));
        internal void SetViewport(RawViewportF viewport) => CommandQueue.Enqueue(c => c.SetViewport(viewport));
        internal void SetRenderTarget(CpuDescriptorHandle renderTarget, CpuDescriptorHandle depthStencil) => CommandQueue.Enqueue(c =>
        {
            c.SetRenderTargets(renderTarget, depthStencil);
        });
        public void SetActiveContext() => _setActive(this);
        public void BeginObject(string name)
        {
            _currentCommandQueue = name;
            EnsureCommandQueue(name);
        }
        public void EndObject(string name)
        {
            _currentCommandQueue = DefaultCommandQueue;
        }
    }
}
