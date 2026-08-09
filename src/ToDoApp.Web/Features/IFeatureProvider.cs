namespace ToDoApp.Web.Features;

public interface IFeatureProvider
{
    public abstract void AddFeature(IHostApplicationBuilder builder);
    public abstract void UseFeature(IApplicationBuilder builder);
    public abstract void MapEndpoints(IEndpointRouteBuilder builder);
}