namespace ToDoApp.Web.Features.Auth.Services;

public interface IRefreshTokenGenerator
{
    public abstract ValueTask<string> GenerateTokenAsync(int length, CancellationToken cancellationToken);
}