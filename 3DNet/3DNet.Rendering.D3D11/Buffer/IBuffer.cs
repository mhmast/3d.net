using SharpDX.Direct3D12;

namespace _3DNet.Rendering.D3D12.Buffer
{
    interface IBuffer
    {
        void Load(GraphicsCommandList commandList);
    }
}
