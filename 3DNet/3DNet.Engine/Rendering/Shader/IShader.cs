using _3DNet.Rendering.Buffer;
using System;
using System.Numerics;

namespace _3DNet.Engine.Rendering.Shader
{
    public interface IShader : IDisposable
    {
        string Name { get; }
        string ShaderSignature { get; }
        IBuffer<Matrix4x4> WvpBuffer { get; }

        void Load(IRenderContextInternal context);
        IBuffer<T> CreateBuffer<T>(ShaderBufferDescription shaderBufferDescription, T[] data) where T : struct;
        IBuffer<T> CreateBuffer<T>(ShaderBufferDescription shaderBufferDescription, int length) where T : struct;
    }
}