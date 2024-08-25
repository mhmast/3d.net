﻿using Microsoft.Extensions.DependencyInjection;
using _3DNet.Engine.Extensions;
using _3DNet.Rendering.D3D12.Extensions;
using _3DNet.Input.DirectInput.Extensions;

namespace _3DNet.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection()
                .Use3DNet()
                .UseDirectX12()
                .UseDirectInput()
                .AddTransient<Game>()
                .BuildServiceProvider();
            var game = services.GetRequiredService<Game>();
            game.Init();
            game.Start();

        }
    }
}
