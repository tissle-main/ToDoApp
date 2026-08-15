using ToDoApp.Data.Features.Auth.Users;

namespace ToDoApp.Web.Features.Auth.Services;

public interface IAccessTokenGenerator
{
    public abstract ValueTask<string> GenerateTokenAsync(UserEntity user, CancellationToken cancellationToken);
}