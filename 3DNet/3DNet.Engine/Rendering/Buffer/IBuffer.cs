using _3DNet.Engine.Rendering;
using System;

namespace _3DNet.Rendering.Buffer
{
    public interface IBuffer : IDisposable
    {
        int Length { get; }

        void Load(IRenderContextInternal context);


    }

    public interface IBuffer<T> : IBuffer where T : struct
    {
        void Write(T[] values);
    }
}
