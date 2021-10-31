using System;

namespace _3DNet.Engine.Rendering.Shader
{
    public interface IShader: IDisposable
    {
        string Name { get; }
        string ShaderSignature { get; }
    }
}