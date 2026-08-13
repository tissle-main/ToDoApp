namespace ToDoApp.Web.Features;

public abstract class FeatureProvider
{
    public virtual void AddServices(WebApplicationBuilder builder)
    {

    }
    public virtual void UseMiddleware(WebApplication app)
    {

    }
}