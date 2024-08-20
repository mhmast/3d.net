using _3DNet.Rendering.Buffer;
using System;
using System.Collections.Generic;

namespace _3DNet.Engine.Rendering.Shader
{
    public interface IShader : IDisposable
    {
        string Name { get; }
        string ShaderSignature { get; }
        IWritableBuffer WvpBuffer { get; }
        IDictionary<string, IWritableBuffer> Buffers { get; }
        void Load(IRenderContextInternal context);
    }
}