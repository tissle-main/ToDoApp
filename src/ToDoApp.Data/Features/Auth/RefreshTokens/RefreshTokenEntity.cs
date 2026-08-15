using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Data.Features.Auth.Users.ForeignKey;
using ToDoApp.Data.Shared.KeyedEntities;

namespace ToDoApp.Data.Features.Auth.RefreshTokens;

public sealed class RefreshTokenEntity : IKeyedEntity, IUserEntityForeignKey
{
    //Value properties
    public Guid Id { get; set; } //Interfaces
    public required string Value { get; set; }
    public required DateTime ExpiresAt { get; set; }
    public Guid UserId { get; set; } //Interfaces

    //Navigation properties
    public UserEntity? User { get; set; } //Interfaces
}