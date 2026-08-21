using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Data.Shared.KeyedEntities;
using ToDoApp.Data.Features.Tasks_Categories;
using ToDoApp.Data.Shared.CreationTimeEntities;
using ToDoApp.Data.Features.Auth.Users.ForeignKey;

namespace ToDoApp.Data.Features.Tasks;

public sealed class TaskEntity : IKeyedEntity, ICreationTimeEntity, IUserEntityForeignKey
{
    //Value properties
    public Guid Id { get; set; } //Interfaces
    public DateTime CreatedAt { get; set; } //Interfaces
    public required string Title { get; set; }
    public string? Description { get; set; }
    public bool Done { get; set; }
    public Guid UserId { get; set; } //Interfaces

    //Navigation properties
    public UserEntity? User { get; set; } //Interfaces
    public List<Task_Category_JoinEntity> Categories { get; set; } = [];
}