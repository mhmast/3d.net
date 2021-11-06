using _3DNet.Engine.Rendering;
using _3DNet.Math;
using _3DNet.Rendering.D3D12.RenderTargets;
using SharpDX.Direct3D12;
using SharpDX.DXGI;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace _3DNet.Rendering.D3D12
{
    internal partial class D3DRenderForm : Form, IRenderWindow, ID3DRenderTarget, ID3DObject
    {
        const int FrameCount = 2;
        private SwapChain _swapChain;
        private SwapChainDescription _swapChainDescription;

        private readonly D3DRenderEngine _engine;
        private readonly bool _fullScreen;
        private SharpDX.Direct3D12.Resource _backBuffer1;
        private SharpDX.Direct3D12.Resource _backBuffer2;
        private SharpDX.Direct3D12.Resource _activeBackBuffer;
        private Action _a;
        private DescriptorHeap _renderTargetViewHeap;
        private DescriptorHeap _depthStencilViewHeap;
        private DescriptorHeap _constantBufferViewHeap;
        private CpuDescriptorHandle _renderView;
        private SharpDX.Direct3D12.Resource _depthStencilBuffer;
        private CpuDescriptorHandle _depthStencilView;
        private bool _buffer1Reset = true;
        private bool _buffer2Reset = true;
        
        public Matrix4x4 Projection { get; private set; }

        public Format Format => _swapChainDescription.ModeDescription.Format;

        event Action IRenderWindow.OnClosed
        {
            add { _a += value; }
            remove { _a -= value; }
        }

        public D3DRenderForm(D3DRenderEngine engine, string name, bool fullScreen)
        {
            //InitializeComponent();
            Name = name;
            _engine = engine;
            _fullScreen = fullScreen;
            ReCreateSwapchainDescription();
            engine.RegisterD3DObject(this);
        }

        private void CreateSwapchainResources()
        {
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

            _backBuffer1 = _swapChain.GetBackBuffer<SharpDX.Direct3D12.Resource>(0);
            _backBuffer1.Name = $"bb1_{Name}";
            _backBuffer2 = _swapChain.GetBackBuffer<SharpDX.Direct3D12.Resource>(1);
            _backBuffer2.Name = $"bb2_{Name}";
            _activeBackBuffer = _backBuffer1;
            ReCreateBuffers();
        }

        private void ReCreateSwapchainDescription()
        {
            _swapChainDescription = new SwapChainDescription
            {
                BufferCount = FrameCount,
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
        }



        protected override void OnClosed(EventArgs e)
        {
            Dispose();
            _a?.DynamicInvoke();
        }

        protected override void Dispose(bool disposing)
        {

            if (disposing)
            {

                components?.Dispose();
                _depthStencilBuffer?.Dispose();
                _depthStencilViewHeap?.Dispose();
                _backBuffer1?.Dispose();
                _swapChain?.Dispose();
                _renderTargetViewHeap?.Dispose();
                _engine.UnregisterD3DObject(this);
            }
            base.Dispose(disposing);

        }


        public void Clear(IRenderWindowContext context, Color clearColor)
        {
            context.ClearDepthStencilView(_depthStencilView.Ptr, 1, 5);
            context.ClearRenderTargetView(_renderView.Ptr, clearColor);
        }

        protected override void OnClientSizeChanged(EventArgs e)
        {
            if (!IsHandleCreated)
            {
                return;
            }

            ReCreateSwapchainDescription();
            if (_swapChain == null)
            {
                CreateSwapchainResources();
            }
            else
            {
                ResizeSwapchainResources();
            }
            Projection = Matrix4x4.PerspectiveFovLH((float)System.Math.PI / 4f, Width / Height, 5, 500);
        }

        private void ResizeSwapchainResources()
        {
            //var dbg = _swapChain.GetDevice<SharpDX.Direct3D12.Device>().QueryInterface<DebugDevice>();
            //dbg.ReportLiveDeviceObjects(ReportingLevel.Detail);
            _swapChain.ResizeTarget(ref _swapChainDescription.ModeDescription);
            ReCreateBuffers();
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

            var depthStencilBufferDesc = ResourceDescription.Texture2D(Format.D32_Float, ClientSize.Width, ClientSize.Height, flags: ResourceFlags.AllowDepthStencil);
            ClearValue? clearValue = new()
            {
                Color = new SharpDX.Mathematics.Interop.RawVector4(0, 0, 0, 0),
                DepthStencil = new DepthStencilValue { Depth = 1, Stencil = 5 },
                Format = Format.D32_Float
            };
            _depthStencilBuffer = _engine.CreateCommittedResource(new HeapProperties(HeapType.Default), HeapFlags.None, depthStencilBufferDesc, ResourceStates.DepthWrite, clearValue);
            _depthStencilBuffer.Name = $"dsb_{Name}";
            _engine.CreateDepthStencilView(_depthStencilBuffer, null, _depthStencilView);
        }

        public void Present()
        {
            _swapChain.Present(0, PresentFlags.None);
        }


        public void Begin(D3DRenderWindowContext context)
        {
            
            if(_buffer1Reset && _activeBackBuffer == _backBuffer1)
            {
                context.ResourceBarrierTransition(_backBuffer1, ResourceStates.Common, ResourceStates.RenderTarget);
                _buffer1Reset = false;
            }
            else if(_buffer2Reset && _activeBackBuffer == _backBuffer2)
            {
                context.ResourceBarrierTransition(_backBuffer2, ResourceStates.Common, ResourceStates.RenderTarget);
                _buffer2Reset = false;
            }
            else
            {
                context.ResourceBarrierTransition(_activeBackBuffer, ResourceStates.Present, ResourceStates.RenderTarget);
            }
            _engine.CreateRenderTargetView(_activeBackBuffer, null, _renderView);
            context.SetProjection(Projection);
        }

        public void End(D3DRenderWindowContext context)
        {
            context.ResourceBarrierTransition(_activeBackBuffer, ResourceStates.RenderTarget, ResourceStates.Present);
            _activeBackBuffer = _activeBackBuffer == _backBuffer1 ? _backBuffer2 : _backBuffer1;
        }
    }
}
