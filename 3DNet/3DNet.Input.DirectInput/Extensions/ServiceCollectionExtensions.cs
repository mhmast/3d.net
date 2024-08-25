using _3DNet.Engine.Input;
using Microsoft.Extensions.DependencyInjection;

namespace _3DNet.Input.DirectInput.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection UseDirectInput(this IServiceCollection services) => services.AddSingleton<IInputFactory, DirectInputFactory>();
}
