using SharpDX.Direct3D12;
using System;

namespace _3DNet.Rendering.D3D12.Buffer
{
    interface IBuffer : IDisposable
    {
        void Load(GraphicsCommandList commandList);
    }
}
