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
}