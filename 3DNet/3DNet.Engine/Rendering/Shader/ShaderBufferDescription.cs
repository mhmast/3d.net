using _3DNet.Rendering.Buffer;

namespace _3DNet.Engine.Rendering.Shader
{
    public class ShaderBufferDescription
    {
        public ShaderBufferDescription(string name, int slot, BufferType type, BufferUsage bufferUsage)
        {
            Name = name;
            Slot = slot;
            Type = type;
            BufferUsage = bufferUsage;
        }

        public string Name { get; }
        public int Slot { get; }
        public BufferType Type { get; }
        public BufferUsage BufferUsage { get;}
    }
}