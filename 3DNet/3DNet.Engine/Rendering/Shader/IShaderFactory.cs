using _3DNet.Rendering.Buffer;

namespace _3DNet.Engine.Rendering.Shader
{
    public interface IShaderFactory
    {
        IShader DefaultShader { get; }
        IShader LoadShader(string name,ShaderDescription description);

    }
}
