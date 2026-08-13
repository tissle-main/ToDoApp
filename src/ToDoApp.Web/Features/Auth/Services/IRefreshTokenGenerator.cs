using System.Security.Cryptography;

namespace ToDoApp.Web.Features.Auth.Services;

public interface IRefreshTokenGenerator
{
    public abstract ValueTask<string> GenerateTokenAsync(int length, CancellationToken cancellationToken);
}
public sealed class RefreshTokenGenerator : IRefreshTokenGenerator
{
    #region Interfaces
    public async ValueTask<string> GenerateTokenAsync(int length, CancellationToken cancellationToken)
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(length))[..length];
    }
    #endregion
}