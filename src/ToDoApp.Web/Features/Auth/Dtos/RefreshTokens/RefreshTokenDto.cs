namespace ToDoApp.Web.Features.Auth.Dtos.RefreshTokens;

public sealed class RefreshTokenDto
{
    public required string Value { get; set; }
    public DateTime ExpiresAt { get; set; }
}