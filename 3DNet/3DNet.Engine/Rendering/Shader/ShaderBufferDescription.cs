using System;

namespace _3DNet.Engine.Rendering.Shader
{
    public class ShaderBufferDescription
    {
        private ShaderBufferDescription(string name, int slot, BufferType type, BufferUsage bufferUsage,Type dataType, IShaderBufferDataAdapter adapter)
        {
            Name = name;
            Slot = slot;
            Type = type;
            BufferUsage = bufferUsage;
            DataType = dataType;
            DataAdapter = adapter;
        }

        public string Name { get; }
        public int Slot { get; }
        public BufferType Type { get; }
        public BufferUsage BufferUsage { get;}
        public Type DataType { get; }
        public IShaderBufferDataAdapter DataAdapter { get; }

        public static ShaderBufferDescription Create<TBuffer>(string name, int slot, BufferType type, BufferUsage bufferUsage,IShaderBufferDataAdapter adapter) => new(name, slot, type, bufferUsage, typeof(TBuffer),adapter);

    }

}