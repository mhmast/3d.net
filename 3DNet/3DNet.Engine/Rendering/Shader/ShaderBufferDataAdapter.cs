using System;
using System.Collections.Generic;

namespace _3DNet.Engine.Rendering.Shader;

internal class ShaderBufferDataAdapter<T> : IShaderBufferDataAdapter where T : struct
{
    private IDictionary<Type, Func<object, T>> _converters;

    public ShaderBufferDataAdapter(IDictionary<Type, Func<object,T>> converters) => _converters = converters;

    public void ConvertAndWrite(object input, IBufferWriter bufferWriter)
    {
        var value = _converters[input.GetType()](input);
        bufferWriter.Write(value);
    }
}