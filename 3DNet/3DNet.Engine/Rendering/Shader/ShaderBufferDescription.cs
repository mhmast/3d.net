using System;

namespace _3DNet.Engine.Rendering.Shader
{
    public class ShaderBufferDescription
    {
        public ShaderBufferDescription(int slot, BufferType type, int sizeInBytes,BufferUsage bufferUsage,Func<IRenderWindowContext,byte[]> data)
        {
            Slot = slot;
            Type = type;
            SizeInBytes = sizeInBytes;
            BufferUsage = bufferUsage;
            Data = data;
        }
        public int Slot { get; }
        public BufferType Type { get; }
        public int SizeInBytes { get; }
        public BufferUsage BufferUsage { get;}
        public Func<IRenderWindowContext, byte[]> Data { get; }
    }
}