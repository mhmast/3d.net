﻿using _3DNet.Engine.Rendering;
using _3DNet.Engine.Rendering.Model;
using _3DNet.Engine.Rendering.Shader;
using _3DNet.Rendering.D3D12.Model;
using _3DNet.Rendering.D3D12.Shaders;
using Microsoft.Extensions.DependencyInjection;

namespace _3DNet.Rendering.D3D12.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection UseDirectX12(this IServiceCollection services) =>
            services.AddTransient<IRenderEngine, D3DRenderEngine>(s => s.GetRequiredService<D3DRenderEngine>())
            .AddSingleton(s =>
            {
                var engine = new D3DRenderEngine();
                engine.Initialize();
                return engine;
            })
            .AddSingleton<D3DRenderTargetFactory>()
            .AddTransient<IRenderTargetFactory>(s=>s.GetRequiredService<D3DRenderTargetFactory>())
            .AddTransient<IModelFactory, D3DModelFactory>()
            .AddSingleton<Shaders.D3DShaderFactory>()
            .AddTransient<IShaderFactory>(s=>s.GetRequiredService<D3DShaderFactory>());
    }
}
