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
    private static FrozenSet<FeatureProvider> Features { get; set; } = [];

    extension(WebApplicationBuilder thisBuilder)
    {
        public void AddCore()
        {
            thisBuilder.Services.AddDbContext<AppDbContext>(options =>
            {
                string? connectionStr = thisBuilder.Configuration.GetConnectionString(AppResources.Database);
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
            thisBuilder.AddCQRS();
            thisBuilder.AddFeatures();
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
            thisApp.UseFeatures();
        }
        public void MigrateDatabase()
        {
            using IServiceScope scope = thisApp.Services.CreateScope();
            AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.Migrate();
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