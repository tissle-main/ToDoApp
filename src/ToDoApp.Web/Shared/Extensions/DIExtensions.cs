using Serilog;
using ToDoApp.Data;
using FluentValidation;
using Scalar.AspNetCore;
using ToDoApp.Web.Features;
using ToDoApp.ServiceDefaults;
using System.Collections.Frozen;
using ToDoApp.Web.Shared.Scalar;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Web.Shared.Behaviors.Authorized;
using ToDoApp.Web.Shared.Behaviors.Validation;

namespace ToDoApp.Web.Shared.Extensions;

public static class DIExtensions
{
    private static FrozenSet<IFeatureProvider> Features { get; set; } = [];

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
        builder.Services.AddHttpContextAccessor();
        builder.AddCQRS();
        builder.AddFeatures();
        builder.AddSerilog();
        builder.AddScalar();
    }
    public static void AddCQRS(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediator(options =>
        {
            options.ServiceLifetime = ServiceLifetime.Scoped;
            options.PipelineBehaviors = [
                typeof(ValidationBehavior<,>),
                typeof(AuthorizedMessageBehavior<,>)
            ];
        });
        builder.AddValidation();
    }
    public static void AddValidation(this WebApplicationBuilder builder)
    {
        ValidatorOptions.Global.LanguageManager.Enabled = false;
        builder.Services.AddValidatorsFromAssemblyContaining(typeof(DIExtensions), ServiceLifetime.Singleton);
    }
    public static void AddFeatures(this WebApplicationBuilder builder)
    {
        Features = typeof(DIExtensions).Assembly.GetTypes().Where(type =>
        {
            return !type.IsAbstract && !type.IsInterface && type.IsAssignableTo(typeof(IFeatureProvider));
        }).Select(type =>
        {
            return (IFeatureProvider)Activator.CreateInstance(type)!;
        }).ToFrozenSet();
        foreach(IFeatureProvider provider in Features)
        {
            provider.AddFeature(builder);
        }
    }
    public static void AddSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog(static void (HostBuilderContext ctx, IServiceProvider provider, LoggerConfiguration cfg) =>
        {
            cfg.WriteTo.Console();
        });
    }
    public static void AddScalar(this WebApplicationBuilder builder)
    {
        if(builder.Environment.IsDevelopment())
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
                options.AddOperationTransformer<BearerSecurityOperationTransformer>();
            });
        }
    }

    public static void UseCore(this WebApplication app)
    {
        app.MigrateDatabase();
        app.UseFeatures();
        app.MapEndpoints();
        app.UseSerilog();
        app.UseScalar();
    }
    public static void MigrateDatabase(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();
    }
    public static void UseFeatures(this WebApplication app)
    {
        foreach(IFeatureProvider provider in Features)
        {
            provider.UseFeature(app);
        }
    }
    public static void MapEndpoints(this WebApplication app)
    {
        foreach(IFeatureProvider provider in Features)
        {
            provider.MapEndpoints(app);
        }
    }
    public static void UseSerilog(this WebApplication app)
    {
        app.UseSerilogRequestLogging();
    }
    public static void UseScalar(this WebApplication app)
    {
        if(app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
            });
        }
    }
}