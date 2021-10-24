using _3DNet.Engine.Rendering;
using _3DNet.Math;
using SharpDX.Direct3D12;
using SharpDX.DXGI;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace _3DNet.Rendering.D3D12
{
    public partial class D3DRenderForm : Form, IRenderWindow
    {
        private SwapChain _swapChain;
        private SwapChainDescription _swapChainDescription;
        private readonly SharpDX.Direct3D12.Device _device;
        private readonly GraphicsCommandList _commandList;
        private readonly bool _fullScreen;
        private SharpDX.Direct3D12.Resource _backBuffer;
        private Action _a;
        private DescriptorHeap _renderTargetViewHeap;
        private DescriptorHeap _depthStencilViewHeap;
        private object _constantBufferViewHeap;
        private CpuDescriptorHandle _renderView;
        private CpuDescriptorHandle _depthStencilView;

        public Matrix4x4 Projection { get; private set; }

        event Action IRenderWindow.OnClosed
        {
            add { _a += value; }
            remove { _a -= value; }
        }

        public D3DRenderForm(SharpDX.Direct3D12.Device device, GraphicsCommandList commandList, string name, bool fullScreen)
        {
            //InitializeComponent();
            Name = name;
            _device = device;
            _commandList = commandList;
            _fullScreen = fullScreen;
        }

        protected override void OnClosed(EventArgs e)
        {
            Dispose();
            _a?.DynamicInvoke();
        }

        protected override void OnActivated(EventArgs e)
        {
            _swapChainDescription = new SwapChainDescription
            {
                BufferCount = 2,
                IsWindowed = !_fullScreen,
                Flags = SwapChainFlags.None,
                SwapEffect = SwapEffect.FlipDiscard,
                Usage = Usage.RenderTargetOutput,
                SampleDescription = new SampleDescription(1, 0),
                OutputHandle = Handle,
                ModeDescription = new ModeDescription
                {
                    Height = ClientSize.Height,
                    Width = ClientSize.Width,
                    Format = Format.R8G8B8A8_UNorm,
                    RefreshRate = new Rational(60, 1)
                }
            };

            using (var factory = new Factory1())
            {
                var tempSwapChain = new SwapChain(factory, _commandList, _swapChainDescription);
                _swapChain = tempSwapChain.QueryInterface<SwapChain3>();
                tempSwapChain.Dispose();
            }

            // Create descriptor heaps.
            // Describe and create a render target view (RTV) descriptor heap.
            var rtvHeapDesc = new DescriptorHeapDescription()
            {
                DescriptorCount = 2,
                Flags = DescriptorHeapFlags.None,
                Type = DescriptorHeapType.RenderTargetView
            };

            _renderTargetViewHeap = _device.CreateDescriptorHeap(rtvHeapDesc);


            var dsvHeapDesc = new DescriptorHeapDescription()
            {
                DescriptorCount = 2,
                Flags = DescriptorHeapFlags.None,
                Type = DescriptorHeapType.DepthStencilView
            };

            _depthStencilViewHeap = _device.CreateDescriptorHeap(dsvHeapDesc);


            // Describe and create a constant buffer view (CBV) descriptor heap.
            // Flags indicate that this descriptor heap can be bound to the pipeline 
            // and that descriptors contained in it can be referenced by a root table.

            var cbvHeapDesc = new DescriptorHeapDescription
            {
                DescriptorCount = 1,
                Flags = DescriptorHeapFlags.ShaderVisible,
                Type = DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView
            };

            _constantBufferViewHeap = _device.CreateDescriptorHeap(cbvHeapDesc);

            // Create frame resources.
            _renderView = _renderTargetViewHeap.CPUDescriptorHandleForHeapStart;
            _depthStencilView = _depthStencilViewHeap.CPUDescriptorHandleForHeapStart;
            _device.CreateRenderTargetView(_swapChain.GetBackBuffer<SharpDX.Direct3D12.Resource>(0), null, _renderTargetViewHeap.CPUDescriptorHandleForHeapStart);
            _device.CreateDepthStencilView(_swapChain.GetBackBuffer<SharpDX.Direct3D12.Resource>(0), null, _depthStencilViewHeap.CPUDescriptorHandleForHeapStart);
            OnClientSizeChanged(null);
        }

        public void Clear(Color clearColor)
        {
            _commandList.ClearRenderTargetView(_renderView, new SharpDX.Mathematics.Interop.RawColor4(clearColor.R, clearColor.G, clearColor.B, clearColor.A));
            _commandList.ClearDepthStencilView(_depthStencilView, ClearFlags.None, 0, 0);
        }

        protected override void OnClientSizeChanged(EventArgs e)
        {
            if (!IsHandleCreated)
            {
                return;
            }
            _backBuffer?.Dispose();

            _swapChain.ResizeBuffers(_swapChainDescription.BufferCount, ClientSize.Width, ClientSize.Height, Format.Unknown, SwapChainFlags.None);
            _backBuffer = _swapChain.GetBackBuffer<SharpDX.Direct3D12.Resource>(0);


            Projection = Matrix4x4.PerspectiveFovLH((float)System.Math.PI / 4f, Width / Height, 5, 500);
        }
        public void Present()
        {
            _swapChain.Present(0, PresentFlags.None);
            Application.DoEvents();
        }

        public void SetAsTarget()
        {
            throw new NotImplementedException();
        }
    }
}
