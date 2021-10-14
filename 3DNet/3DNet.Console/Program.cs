using Microsoft.Extensions.DependencyInjection;
using _3DNet.Engine.Extensions;
using _3DNet.Rendering.D3D11.Extensions;

namespace _3DNet.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection()
                .Use3DNet()
                .UseDirectX11()
                .AddTransient<Game>()
                .BuildServiceProvider();
            var game = services.GetRequiredService<Game>();
            game.Init();
            game.Start();

        }
    }
}
