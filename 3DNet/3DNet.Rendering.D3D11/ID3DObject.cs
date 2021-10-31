using SharpDX.Direct3D12;

namespace _3DNet.Rendering.D3D12
{
    internal interface ID3DObject
    {
        void Begin(GraphicsCommandList commandList);
        void End(GraphicsCommandList commandList);
    }
}
