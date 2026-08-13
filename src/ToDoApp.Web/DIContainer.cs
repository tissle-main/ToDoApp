using ToDoApp.Data;
using FluentValidation;
using ToDoApp.Web.Features;
using ToDoApp.ServiceDefaults;
using System.Collections.Frozen;
using ToDoApp.Web.Shared.Behaviors;
using Microsoft.EntityFrameworkCore;

namespace ToDoApp.Web;

public static class DIContainer
{
    private static FrozenSet<FeatureProvider> Features { get; set; } = [];

    public static void AddCore(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            string? connectionStr = builder.Configuration.GetConnectionString(AppResources.Database);
            options.UseSqlServer(connectionStr, builder =>
            {
                builder.MigrationsAssembly(typeof(AppDbContext).Assembly.GetName().Name);
            });
        });
        builder.Services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = static void(ProblemDetailsContext ctx) =>
            {
                ctx.ProblemDetails.Extensions.Add("instance", $"{ctx.HttpContext.Request.Method} {ctx.HttpContext.Request.Path}");
            };
        });
        builder.AddCQRS();
        builder.AddFeatures();
    }
    public static void AddCQRS(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediator(options =>
        {
            options.ServiceLifetime = ServiceLifetime.Scoped;
            options.PipelineBehaviors = [
                typeof(ValidationBehavior<,>),
                typeof(AuthorizedBehavior<,>),
                typeof(DbSaveBehavior<,>)
            ];
        });
        builder.Services.AddValidatorsFromAssemblyContaining(typeof(DIContainer), ServiceLifetime.Singleton);
        ValidatorOptions.Global.LanguageManager.Enabled = false;
    }
    public static void AddFeatures(this WebApplicationBuilder builder)
    {
        Features = typeof(DIContainer).Assembly.GetTypes().Where(type =>
        {
            return !type.IsAbstract && type.IsAssignableTo(typeof(FeatureProvider));
        }).Select(type =>
        {
            return (FeatureProvider)Activator.CreateInstance(type)!;
        }).ToFrozenSet();
        foreach(FeatureProvider provider in Features)
        {
            provider.AddServices(builder);
        }
    }

    public static void UseCore(this WebApplication app)
    {
        app.MigrateDatabase();
        app.UseFeatures();
    }
    public static void MigrateDatabase(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();
    }
    public static void UseFeatures(this WebApplication app)
    {
        foreach(FeatureProvider provider in Features)
        {
            provider.UseMiddleware(app);
        }
    }
}