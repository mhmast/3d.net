
using _3DNet.Engine.Engine;
using _3DNet.Engine.Rendering;
using _3DNet.Engine.Rendering.Shader;
using Microsoft.Extensions.DependencyInjection;

namespace _3DNet.Engine.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection Use3DNet(this IServiceCollection services) => 
            
            services.AddSingleton<Engine.Engine, Engine.Engine>()
            .AddSingleton<IEngine>(s=>s.GetRequiredService<Engine.Engine>())
            .AddTransient<IRenderContextFactory,RenderContextFactory>()
            .AddTransient<IShaderBufferDataAdapterBuilder,ShaderBufferDataConversionBuilder>();
              
    }
}
