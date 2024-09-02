using _3DNet.Rendering.Buffer;
using System;

namespace _3DNet.Engine.Rendering.Shader
{
    public interface IShader : IDisposable
    {
        string Name { get; }
        string ShaderSignature { get; }
        IWritableBuffer ViewProjectionBuffer { get; }
        IWritableBuffer GetOrCreateWorldBufferForObject(string name);
        void Load(IRenderContextInternal context);
    }
}