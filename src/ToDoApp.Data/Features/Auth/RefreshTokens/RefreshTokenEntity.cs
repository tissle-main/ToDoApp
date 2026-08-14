using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Data.Shared.KeyedEntities;

namespace ToDoApp.Data.Features.Auth.RefreshTokens;

public sealed class RefreshTokenEntity : IKeyedEntity
{
    //Value properties
    public Guid Id { get; set; } //Interfaces
    public required string Value { get; set; }
    public required DateTime ExpiresAt { get; set; }
    public Guid UserId { get; set; }

    //Navigation properties
    public UserEntity? User { get; set; }
}