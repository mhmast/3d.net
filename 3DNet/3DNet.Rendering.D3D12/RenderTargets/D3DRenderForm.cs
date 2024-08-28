using _3DNet.Engine.Rendering;
using _3DNet.Rendering.D3D12.RenderTargets;
using SharpDX.Direct3D12;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace _3DNet.Rendering.D3D12
{
    internal class D3DRenderForm : Form, IRenderWindow, ID3DRenderTarget
    {
        const int FrameCount = 2;
        private SwapChain3 _swapChain;
        private SwapChainDescription _swapChainDescription;
        private ResourceDescription _depthStencilBufferDesc;
        private readonly D3DRenderEngine _engine;
        //private SharpDX.Direct3D12.Resource _backBuffer1;
        //private SharpDX.Direct3D12.Resource _backBuffer2;
        //private SharpDX.Direct3D12.Resource _activeBackBuffer;
        private Action _onClosedAction;
        private DescriptorHeap _renderTargetViewHeap;
        private DescriptorHeap _depthStencilViewHeap;
        private DescriptorHeap _constantBufferViewHeap;
        private CpuDescriptorHandle _renderView;
        private SharpDX.Direct3D12.Resource _depthStencilBuffer;
        private readonly SharpDX.Direct3D12.Resource[] _backBuffers = new SharpDX.Direct3D12.Resource[FrameCount];
        private CpuDescriptorHandle _depthStencilView;
        private RawViewportF _viewport;
        private RawRectangle _scissorRect;
        private int _renderViewIncrementSize;
        private int _frameIndex;
        private bool _isResizingSwapChain = false;
        private Action _gotFocus;
        private Action _lostFocus;

        public Matrix4x4 Projection { get; private set; }

        public Format Format => _swapChainDescription.ModeDescription.Format;

        public bool FullScreen
        {
            get => _swapChain?.IsFullScreen ?? false;
            set
            {
                if (FullScreen == value) return;
                ResizeSwapchain(value);
            }
        }

        event Action IRenderWindow.OnClosed
        {
            add { _onClosedAction += value; }
            remove { _onClosedAction -= value; }
        }

        event Action IRenderWindow.GotFocus
        {
            add
            {
                _gotFocus += value;
            }

            remove
            {
                _gotFocus -= value;
            }
        }

        event Action IRenderWindow.LostFocus
        {
            add
            {
                _lostFocus += value;
            }

            remove
            {
                _lostFocus -= value;
            }
        }

        public D3DRenderForm(D3DRenderEngine engine, string name)
        {
            //InitializeComponent();
            Name = name;
            TopMost = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            _engine = engine;
            ReCreateSwapchainDescription(false);
            Cursor.Hide();
            GotFocus += (_, __) => _gotFocus?.DynamicInvoke();
            LostFocus += (_, __) => _lostFocus?.DynamicInvoke();
        }
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            OnGotFocus(e);
        }

        private void CreateSwapchainResources()
        {

            //Win32Native.SetWindowLong(Handle, Win32Native.WindowLongType.Style, (IntPtr)WindowStyles.WS_BORDER);
            _swapChain = _engine.CreateSwapChain(_swapChainDescription);
            _swapChain.DebugName = $"swapchain_{Name}";
            // Create descriptor heaps.
            // Describe and create a render target view (RTV) descriptor heap.
            var rtvHeapDesc = new DescriptorHeapDescription()
            {
                DescriptorCount = FrameCount,
                Flags = DescriptorHeapFlags.None,
                Type = DescriptorHeapType.RenderTargetView
            };

            _renderTargetViewHeap = _engine.CreateDescriptorHeap(rtvHeapDesc);
            _renderTargetViewHeap.Name = $"rtvheap_{Name}";

            var dsvHeapDesc = new DescriptorHeapDescription()
            {
                DescriptorCount = FrameCount,
                Flags = DescriptorHeapFlags.None,
                Type = DescriptorHeapType.DepthStencilView
            };

            _depthStencilViewHeap = _engine.CreateDescriptorHeap(dsvHeapDesc);
            _depthStencilViewHeap.Name = $"dsvheap_{Name}";

            // Describe and create a constant buffer view (CBV) descriptor heap.
            // Flags indicate that this descriptor heap can be bound to the pipeline 
            // and that descriptors contained in it can be referenced by a root table.

            var cbvHeapDesc = new DescriptorHeapDescription
            {
                DescriptorCount = FrameCount,
                Flags = DescriptorHeapFlags.ShaderVisible,
                Type = DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView
            };

            _constantBufferViewHeap = _engine.CreateDescriptorHeap(cbvHeapDesc);
            _constantBufferViewHeap.Name = $"cbvheap_{Name}";
            _renderView = _renderTargetViewHeap.CPUDescriptorHandleForHeapStart;
            _depthStencilView = _depthStencilViewHeap.CPUDescriptorHandleForHeapStart;

            _renderViewIncrementSize = _engine.GetRenderTargetDescriptorHandleIncrementSize();
            ReCreateBuffers();

        }

        private void ReCreateSwapchainDescription(bool fullScreen)
        {
            var screen = Screen.FromControl(this);
            var width = fullScreen ? screen.Bounds.Width : ClientSize.Width;
            var height = fullScreen ? screen.Bounds.Height : ClientSize.Height;
            _swapChainDescription = new SwapChainDescription
            {
                BufferCount = FrameCount,
                IsWindowed = !fullScreen,
                Flags = SwapChainFlags.None,
                SwapEffect = SwapEffect.FlipDiscard,
                Usage = Usage.RenderTargetOutput,
                SampleDescription = new SampleDescription(1, 0),
                OutputHandle = Handle,
                ModeDescription = new ModeDescription
                {
                    Height = height,
                    Width = width,
                    Format = Format.R8G8B8A8_UNorm,
                    RefreshRate = new Rational(60, 1)
                }
            };
            _depthStencilBufferDesc = ResourceDescription.Texture2D(Format.D32_Float, width, height, flags: ResourceFlags.AllowDepthStencil);
            _viewport = new RawViewportF { X = 0, Y = 0, Width = width, Height = height, MinDepth = 1, MaxDepth = 500 };
            _scissorRect = new RawRectangle { Left = 0, Top = 0, Right = width, Bottom = height };
            Projection = Matrix4x4.CreatePerspectiveFieldOfView((float)System.Math.PI / 4f, width / height, 1, 500);
        }



        protected override void OnClosing(CancelEventArgs e)
        {
            _onClosedAction?.DynamicInvoke();
        }

        protected override void Dispose(bool disposing)
        {

            if (disposing)
            {
                FullScreen = false;
                DisposeSwapchainAndBackBuffers();
                _depthStencilViewHeap?.Dispose();
                _renderTargetViewHeap?.Dispose();
            }
            base.Dispose(disposing);

        }


        public void Clear(IRenderContextInternal context, Color clearColor)
        {
            context.ClearDepthStencilView(_depthStencilView.Ptr, 1, 5);
            context.ClearRenderTargetView((_renderView + (_frameIndex * _renderViewIncrementSize)).Ptr, clearColor);
        }

        protected override void OnClientSizeChanged(EventArgs e)
        {
            if (_isResizingSwapChain)
            {
                return;
            }
            ResizeSwapchain(FullScreen);
        }

        private void ResizeSwapchain(bool fullScreen)
        {
            if (!IsHandleCreated)
            {
                return;
            }
            if (_isResizingSwapChain)
            {
                return;
            }
            _isResizingSwapChain = true;
            ReCreateSwapchainDescription(fullScreen);
            if (_swapChain == null)
            {
                CreateSwapchainResources();
            }
            else
            {
                ResizeSwapchainResources();
            }
            
            _isResizingSwapChain = false;
        }

        private void ResizeSwapchainResources()
        {
            //var dbg = _swapChain.GetDevice<SharpDX.Direct3D12.Device>().QueryInterface<DebugDevice>();
            //dbg.ReportLiveDeviceObjects(ReportingLevel.Detail);
            DisposeSwapchainAndBackBuffers();
            
            _swapChain = _engine.ReCreateSwapChain(_swapChainDescription);
            ReCreateBuffers();
        }

        private void DisposeSwapchainAndBackBuffers()
        {
            if (FullScreen)
            {
                _swapChain.SetFullscreenState(false, null);
            }
            for (var i = 0; i < FrameCount; i++)
            {
                _backBuffers[i]?.Dispose();
            }
            _depthStencilBuffer.Dispose();
            _swapChain.Dispose();
        }

        private void ReCreateBuffers()
        {
            //_disposeCollector.RemoveAndDispose(ref _backBuffer1);
            //_disposeCollector.RemoveAndDispose(ref _depthStencilBuffer);
            //_backBuffer1 = _swapChain.GetBackBuffer<SharpDX.Direct3D12.Resource>(0);
            //_backBuffer1.Name = $"bb1_{Name}";
            //_backBuffer2 = _swapChain.GetBackBuffer<SharpDX.Direct3D12.Resource>(1);
            //_backBuffer2.Name = $"bb2_{Name}";
            //_activeBackBuffer = _backBuffer1;
            //_buffer1Reset = true;
            //_buffer2Reset = true;

            for (var i = 0; i < FrameCount; i++)
            {
                var backBuffer = _swapChain.GetBackBuffer<SharpDX.Direct3D12.Resource>(i);
                backBuffer.Name = $"bb{i}_{Name}";
                _backBuffers[i] = backBuffer;
                _engine.CreateRenderTargetView(backBuffer, null, _renderView + (i * _renderViewIncrementSize));
            }

            var clearValue = new ClearValue()
            {
                Color = new RawVector4(0, 0, 0, 0),
                DepthStencil = new DepthStencilValue { Depth = 1, Stencil = 5 },
                Format = Format.D32_Float
            };
            _depthStencilBuffer = _engine.CreateCommittedResource(new HeapProperties(HeapType.Default), HeapFlags.None, _depthStencilBufferDesc, ResourceStates.DepthWrite, clearValue);
            _depthStencilBuffer.Name = $"dsb_{Name}";
            _engine.CreateDepthStencilView(_depthStencilBuffer, null, _depthStencilView);

        }

        public void Present()
        {
            _swapChain.Present(1, PresentFlags.None);
        }

        internal void ProcessWindowMessages()
        {
            if (!IsHandleCreated)
            {
                return;
            }
            while (IsHandleCreated && Win32Native.PeekMessage(out var lpMsg, Handle, 0, 0, 0) != 0)
            {
                if (Win32Native.GetMessage(out lpMsg, Handle, 0, 0) == -1)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "An error happened in rendering loop while processing windows messages. Error: {0}", new object[1] { Marshal.GetLastWin32Error() }));
                }

                var message = default(System.Windows.Forms.Message);
                message.HWnd = lpMsg.handle;
                message.LParam = lpMsg.lParam;
                message.Msg = (int)lpMsg.msg;
                message.WParam = lpMsg.wParam;
                var message2 = message;
                if (!Application.FilterMessage(ref message2))
                {
                    Win32Native.TranslateMessage(ref lpMsg);
                    Win32Native.DispatchMessage(ref lpMsg);
                }
            }
        }

        public void Begin(D3DRenderWindowContext context)
        {
            _frameIndex = _swapChain.CurrentBackBufferIndex;

            context.SetViewport(_viewport);
            context.SetScissorRect(_scissorRect);
            context.ResourceBarrierTransition(_backBuffers[_frameIndex], ResourceStates.Present, ResourceStates.RenderTarget);
            context.SetRenderTarget(_renderView + (_frameIndex * _renderViewIncrementSize), _depthStencilView);

            //context.SetProjection(Projection);
        }

        public void End(D3DRenderWindowContext context)
        {
            context.ResourceBarrierTransition(_backBuffers[_frameIndex], ResourceStates.RenderTarget, ResourceStates.Present);
        }

    }
}
