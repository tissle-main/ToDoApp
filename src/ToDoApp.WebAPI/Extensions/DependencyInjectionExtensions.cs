using System.Reflection;
using ToDoApp.WebAPI.Features;
using System.Diagnostics.CodeAnalysis;

namespace ToDoApp.WebAPI.Extensions;

[ExcludeFromCodeCoverage]
public static class DependencyInjectionExtensions
{
    public static void MapEndpointsFromAssembly(this IEndpointRouteBuilder builder, Assembly? assembly = null)
    {
        assembly ??= typeof(IEndpointsProvider).Assembly;
        IEnumerable<Type> types = assembly.GetTypes().Where(type => typeof(IEndpointsProvider).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);
        foreach(Type type in types)
        {
            var endpoints = (IEndpointsProvider)Activator.CreateInstance(type)!;
            endpoints.MapEndpoints(builder);
        }
    }
    public static void AddJoinHandlersFromAssembly(this IServiceCollection services, Assembly? assembly = null)
    {
        assembly ??= typeof(IJoinHandler<>).Assembly;
        IEnumerable<Type> handlerTypes = assembly.GetTypes().Where(type =>
        {
            bool notAbstract = !type.IsAbstract;
            bool notInterface = !type.IsInterface;
            bool implementsInterface = type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IJoinHandler<>));
            return notAbstract && notInterface && implementsInterface;
        });
        foreach(Type type in handlerTypes)
        {
            Type interfaceType = type.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IJoinHandler<>));
            services.AddScoped(interfaceType, type);
        }
    }
}