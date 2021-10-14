using _3DNet.Engine.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace _3DNet.Rendering.D3D11.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection UseDirectX11(this IServiceCollection services) =>
            services.AddTransient<IRenderEngine, D3DRenderEngine>(s => s.GetRequiredService<D3DRenderEngine>())
            .AddSingleton<D3DRenderEngine>(s =>
            {
                var engine = new D3DRenderEngine();
                engine.Initialize();
                return engine;
            })
                .AddTransient<IRenderTargetFactory, D3DRenderTargetFactory>();
    }
}
