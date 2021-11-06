using _3DNet.Engine.Rendering;
using System;

namespace _3DNet.Rendering.Buffer
{
    public interface IBuffer : IDisposable
    {
        void Load(IRenderWindowContext context);
    }
}
