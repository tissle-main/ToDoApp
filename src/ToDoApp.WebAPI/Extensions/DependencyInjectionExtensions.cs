using System.Text;
using System.Reflection;
using ToDoApp.WebAPI.Features;
using ToDoApp.WebAPI.Services.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authentication.JwtBearer;

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
    public static void AddJwtAuth(this WebApplicationBuilder builder)
    {
        IConfigurationSection jwt = builder.Configuration.GetSection(JwtOptions.SectionName);
        builder.Services.Configure<JwtOptions>(jwt);
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwt["Issuer"],
                ValidateAudience = true,
                ValidAudience = jwt["Audience"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });
        builder.Services.AddAuthorization();
        builder.Services.AddScoped<IJwtService, JwtService>();
    }
}