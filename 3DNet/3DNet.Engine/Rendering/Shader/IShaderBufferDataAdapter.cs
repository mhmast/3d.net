namespace _3DNet.Engine.Rendering.Shader;
public interface IShaderBufferDataAdapter
{
    void ConvertAndWrite(object input,IBufferWriter bufferWriter);
}
