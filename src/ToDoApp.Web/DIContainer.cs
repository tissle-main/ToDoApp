using ToDoApp.Data;
using FluentValidation;
using ToDoApp.Web.Features;
using ToDoApp.ServiceDefaults;
using System.Collections.Frozen;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Web.Shared.Behaviors.Authorized;
using ToDoApp.Web.Shared.Behaviors.Validation;
using ToDoApp.Web.Shared.Behaviors.DbTransaction;

namespace ToDoApp.Web;

public static class DIContainer
{
    private const string UICorsPolicy = "UI_CORS";

    private static FrozenSet<FeatureProvider> Features { get; set; } = [];

    extension(WebApplicationBuilder thisBuilder)
    {
        public void AddCore()
        {
            thisBuilder.Services.AddDbContext<AppDbContext>(options =>
            {
                string? connectionStr = thisBuilder.Configuration.GetConnectionString(AppHostConstants.Database);
                options.UseSqlServer(connectionStr, builder =>
                {
                    builder.MigrationsAssembly(typeof(AppDbContext).Assembly.GetName().Name);
                });
            });
            thisBuilder.Services.AddProblemDetails(options =>
            {
                options.CustomizeProblemDetails = static void (ProblemDetailsContext ctx) =>
                {
                    ctx.ProblemDetails.Extensions.Add("instance", $"{ctx.HttpContext.Request.Method} {ctx.HttpContext.Request.Path}");
                };
            });
            thisBuilder.Configuration.AddUserSecrets<Program>();
            thisBuilder.AddUICors();
            thisBuilder.AddCQRS();
            thisBuilder.AddFeatures();
        }
        public void AddUICors()
        {
            thisBuilder.Services.AddCors(options =>
            {
                string? uiOrigin = thisBuilder.Configuration[AppHostConstants.UIOrigin];
                if(uiOrigin is not null)
                {
                    options.AddPolicy(UICorsPolicy, policy =>
                    {
                        policy.WithOrigins(uiOrigin).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                    });
                }
            });
        }
        public void AddCQRS()
        {
            thisBuilder.Services.AddMediator(options =>
            {
                options.ServiceLifetime = ServiceLifetime.Scoped;
                options.PipelineBehaviors = [
                    typeof(ValidationBehavior<,>),
                    typeof(AuthorizedBehavior<,>),
                    typeof(DbTransactionBehavior<,>)
                ];
            });
            thisBuilder.Services.AddValidatorsFromAssemblyContaining(typeof(DIContainer), ServiceLifetime.Singleton);
            ValidatorOptions.Global.LanguageManager.Enabled = false;
        }
        public void AddFeatures()
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
                provider.AddServices(thisBuilder);
            }
        }
    }
    extension(WebApplication thisApp)
    {
        public void UseCore()
        {
            thisApp.MigrateDatabase();
            thisApp.UseUICors();
            thisApp.UseFeatures();
        }
        public void MigrateDatabase()
        {
            using IServiceScope scope = thisApp.Services.CreateScope();
            AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.Migrate();
        }
        public void UseUICors()
        {
            thisApp.UseCors(UICorsPolicy);
        }
        public void UseFeatures()
        {
            foreach(FeatureProvider provider in Features)
            {
                provider.UseMiddleware(thisApp);
            }
        }
    }
}