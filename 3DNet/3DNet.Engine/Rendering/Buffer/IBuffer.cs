using _3DNet.Engine.Rendering;
using System;

namespace _3DNet.Rendering.Buffer
{
    public interface IBuffer : IDisposable
    {
        int Count { get; }

        void Load(IRenderWindowContext context);
    }
}
