using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Data.Shared.KeyedEntities;

namespace ToDoApp.Data.Features.Auth.RefreshTokens;

public sealed class RefreshTokenEntity : IKeyedEntity
{
    //Value properties
    public Guid Id { get; set; }
    public required string RefreshToken { get; set; }
    public required DateTime ExpiresAt { get; set; }
    public required Guid UserId { get; set; }

    //Navigation properties
    public UserEntity? User { get; set; }
}