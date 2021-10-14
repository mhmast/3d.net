using _3DNet.Engine.Rendering;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace _3DNet.Rendering.D3D11
{
    public partial class D3DRenderForm : Form, IRenderWindow
    {
        private readonly DeviceContext _deviceContext;
        private SwapChain _swapChain;
        private SwapChainDescription _swapChainDescription;
        private readonly SharpDX.Direct3D11.Device _device;
        private readonly bool _fullScreen;
        private Texture2D _backBuffer;
        private RenderTargetView _renderView;
        private Texture2D _depthBuffer;
        private DepthStencilView _depthView;

        public D3DRenderForm(SharpDX.Direct3D11.Device device, string name, bool fullScreen)
        {
            //InitializeComponent();
            Name = name;
            _device = device;
            _deviceContext = device.ImmediateContext;
            _fullScreen = fullScreen;
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
                _swapChain = new SwapChain(factory, _device, _swapChainDescription);

                factory.MakeWindowAssociation(Handle, WindowAssociationFlags.IgnoreAll);
            }
            OnClientSizeChanged(null);
        }

        public void Clear(Color clearColor)
        {
            _deviceContext.ClearRenderTargetView(_renderView, new SharpDX.Mathematics.Interop.RawColor4(clearColor.R, clearColor.G, clearColor.B, clearColor.A));
        }

        protected override void OnClientSizeChanged(EventArgs e)
        {
            if(!IsHandleCreated)
            {
                return;
            }
            _backBuffer?.Dispose();
            _renderView?.Dispose();
            _depthBuffer?.Dispose();
            _depthView?.Dispose();

            _swapChain.ResizeBuffers(_swapChainDescription.BufferCount, ClientSize.Width, ClientSize.Height, Format.Unknown, SwapChainFlags.None);
            _backBuffer = SharpDX.Direct3D11.Resource.FromSwapChain<Texture2D>(_swapChain, 0);
            _renderView = new RenderTargetView(_deviceContext.Device, _backBuffer);
            _depthBuffer = new Texture2D(_deviceContext.Device, new Texture2DDescription()
            {
                Format = Format.D32_Float_S8X24_UInt,
                ArraySize = 1,
                MipLevels = 1,
                Width = ClientSize.Width,
                Height = ClientSize.Height,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            });

            _depthView = new DepthStencilView(_deviceContext.Device, _depthBuffer);

            // Setup targets and viewport for rendering
            _deviceContext.Rasterizer.SetViewport(0, 0, ClientSize.Width, ClientSize.Height, 0.0f, 1.0f);
            _deviceContext.OutputMerger.SetTargets(_depthView, _renderView);

        }
        public void Present()
        {
            _swapChain.Present(0, PresentFlags.None);
            Application.DoEvents();
        }
    }
}
