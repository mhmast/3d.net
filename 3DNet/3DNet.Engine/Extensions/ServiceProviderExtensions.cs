
using Microsoft.Extensions.DependencyInjection;

namespace _3DNet.Engine.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection Use3DNet(this IServiceCollection services)
        {
            return services;
        }
    }
}
