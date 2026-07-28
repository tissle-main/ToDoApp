namespace ToDoApp.WebAPI.Features;

public interface IEndpointsProvider
{
    public abstract IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder builder);
}