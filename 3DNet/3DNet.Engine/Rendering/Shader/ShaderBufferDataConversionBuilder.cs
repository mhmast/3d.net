using System;
using System.Collections.Generic;

namespace _3DNet.Engine.Rendering.Shader;

internal class ShaderBufferDataConversionBuilder : IShaderBufferDataAdapterBuilder
{
    public IShaderBufferDataAdapterBuilder<TOUT> AddConverter<TIN, TOUT>(Func<TIN, TOUT> converter) where TOUT : struct
    {
        var builder = new ShaderBufferDataConversionBuilder<TOUT>();
        builder.AddConverter(converter);
        return builder;
    }
}

internal class ShaderBufferDataConversionBuilder<T> : IShaderBufferDataAdapterBuilder<T> where T : struct
{
    private readonly IDictionary<Type, Func<object, T>> _converters = new Dictionary<Type, Func<object, T>>();
    public IShaderBufferDataAdapterBuilder<T> AddConverter<TIN>(Func<TIN, T> converter)
    {
        _converters.Add(typeof(TIN), inObj =>converter((TIN)inObj));
        return this;
    }
    public IShaderBufferDataAdapter Build() => new ShaderBufferDataAdapter<T>(_converters);
}
