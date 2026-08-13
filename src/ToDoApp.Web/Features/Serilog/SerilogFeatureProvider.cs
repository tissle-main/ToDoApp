using Serilog;

namespace ToDoApp.Web.Features.Serilog;

public sealed class SerilogFeatureProvider : FeatureProvider
{
    #region Base
    public override void AddServices(WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog(static void (HostBuilderContext ctx, IServiceProvider provider, LoggerConfiguration cfg) =>
        {
            cfg.WriteTo.Console();
        });
    }
    public override void UseMiddleware(WebApplication app)
    {
        app.UseSerilogRequestLogging();
    }
    #endregion
}