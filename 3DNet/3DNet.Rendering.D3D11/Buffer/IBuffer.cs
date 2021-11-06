using _3DNet.Engine.Rendering;
using System;

namespace _3DNet.Rendering.D3D12.Buffer
{
    interface IBuffer : IDisposable
    {
        void Load(IRenderWindowContext context);
    }
}
