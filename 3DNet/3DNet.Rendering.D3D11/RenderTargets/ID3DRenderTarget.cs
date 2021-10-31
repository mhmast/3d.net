using _3DNet.Engine.Rendering;
using SharpDX.DXGI;

namespace _3DNet.Rendering.D3D12.RenderTargets
{
    internal interface ID3DRenderTarget : IRenderTarget
    {
        Format Format { get; }
    }
}
