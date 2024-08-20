using _3DNet.Engine.Rendering;
using System;

namespace _3DNet.Rendering.Buffer
{
    public interface IBuffer : IDisposable
    {
        int Length { get; }
        long SizeInBytes { get; }

        void Load(IRenderContextInternal context);
    }

    public interface IWritableBuffer : IBuffer
    {
        void Write<T>(T value) where T : struct;

    }

}
