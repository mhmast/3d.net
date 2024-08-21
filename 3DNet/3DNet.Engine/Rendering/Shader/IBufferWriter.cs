namespace _3DNet.Engine.Rendering.Shader;
public interface IBufferWriter
{
    void Write<T>(T o) where T : struct;
}
