using System;

namespace _3DNet.Engine.Rendering.Shader
{
    public abstract class ShaderBufferDescription
    {
        protected ShaderBufferDescription(string name, int slot, BufferType type, BufferUsage bufferUsage,Type dataType)
        {
            Name = name;
            Slot = slot;
            Type = type;
            BufferUsage = bufferUsage;
            DataType = dataType;
        }

        public string Name { get; }
        public int Slot { get; }
        public BufferType Type { get; }
        public BufferUsage BufferUsage { get;}
        public Type DataType { get; }

        public static ShaderBufferDescription Create<TIn,TOut>

    }

}