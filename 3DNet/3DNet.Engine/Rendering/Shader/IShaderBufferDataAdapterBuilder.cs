using System;

namespace _3DNet.Engine.Rendering.Shader;
public interface IShaderBufferDataAdapterBuilder
{
    IShaderBufferDataAdapterBuilder<TOUT> AddConverter<TIN, TOUT>(Func<TIN, TOUT> converter) where TOUT : struct;
}

public interface IShaderBufferDataAdapterBuilder<TOUT> where TOUT : struct
{
    IShaderBufferDataAdapterBuilder<TOUT> AddConverter<TIN>(Func<TIN, TOUT> converter);
    IShaderBufferDataAdapter Build();
}
