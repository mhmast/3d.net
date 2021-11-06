using _3DNet.Engine.Rendering;
using _3DNet.Math;
using SharpDX.Direct3D12;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace _3DNet.Rendering.D3D12
{
    internal class D3DRenderWindowContext : IRenderWindowContext
    {
        private GraphicsCommandList _commandList;
        private Fence _fence;
        private readonly System.Threading.EventWaitHandle _renderHandle = new System.Threading.EventWaitHandle(false, System.Threading.EventResetMode.AutoReset);
        private readonly Device _device;
        private readonly CommandAllocator _commandAllocator;
        private readonly CommandQueue _commandQueue;
        private readonly IEnumerable<ID3DObject> _d3DObjects;
        private readonly D3DRenderForm _d3DRenderForm;
        private bool _disposing;

        public D3DRenderWindowContext(Device device, CommandAllocator commandAllocator, CommandQueue commandQueue, IEnumerable<ID3DObject> d3DObjects, D3DRenderForm d3DRenderForm)
        {
            _device = device;
            _commandAllocator = commandAllocator;
            _commandQueue = commandQueue;
            _d3DObjects = d3DObjects;
            _d3DRenderForm = d3DRenderForm;
            _d3DRenderForm.Disposed += (_, __) => Dispose();
            _d3DRenderForm.FormClosed += (_, __) => _d3DRenderForm.Dispose();
        }

        public IRenderWindow RenderWindow => _d3DRenderForm;

        public bool IsDisposed { get; private set; }
        public Matrix4x4 View { get; private set; }

        public bool BeginScene(Color backgroundColor)
        {
            Application.DoEvents();

            if (_d3DRenderForm.IsDisposed)
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
            _d3DRenderForm.Clear(this, backgroundColor);
            return true;
        }

        public void ClearDepthStencilView(IntPtr ptr, float depth, byte stencil) => _commandList.ClearDepthStencilView(new CpuDescriptorHandle { Ptr = ptr }, ClearFlags.FlagsDepth | ClearFlags.FlagsStencil, depth, stencil);


        public void ClearRenderTargetView(IntPtr ptr, Color clearColor)
        => _commandList.ClearRenderTargetView(new CpuDescriptorHandle { Ptr = ptr }, new SharpDX.Mathematics.Interop.RawColor4(clearColor.R, clearColor.G, clearColor.B, clearColor.A));

        public void Dispose()
        {
            if (IsDisposed || _disposing) { return; }
            _disposing = true;
            _commandList.Dispose();
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
                d3DObject.End(_commandList);
            }
            _commandList.Close();
            _commandQueue.ExecuteCommandList(_commandList);
            _d3DRenderForm.Present();
            _commandQueue.Signal(_fence, 1);
            _renderHandle.WaitOne();
            _fence.Dispose();
        }

        public void SetIndexBuffer(IntPtr bufferLocation, int sizeInBytes, int strideInBytes)
        => _commandList.SetIndexBuffer(new IndexBufferView{ BufferLocation = bufferLocation.ToInt64(), SizeInBytes = sizeInBytes, Format= GetFormat(strideInBytes) });

        private SharpDX.DXGI.Format GetFormat(int strideInBytes)
        => strideInBytes switch
        {
            4 => SharpDX.DXGI.Format.R32_UInt,
            _ => throw new NotImplementedException()
        };

        public void SetProjection(Matrix4x4 projection)
        {
            throw new NotImplementedException();
        }

        public void SetVertexBuffer(IntPtr bufferLocation, int sizeInBytes, int strideInBytes)
        => _commandList.SetVertexBuffer(0, new VertexBufferView { BufferLocation = bufferLocation.ToInt64(), SizeInBytes = sizeInBytes, StrideInBytes = strideInBytes });

        public void SetView(Matrix4x4 view)
        => View = view;
    }
}
