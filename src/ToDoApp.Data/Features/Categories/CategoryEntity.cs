using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Data.Shared.KeyedEntities;
using ToDoApp.Data.Features.Tasks_Categories;
using ToDoApp.Data.Features.Auth.Users.ForeignKey;

namespace ToDoApp.Data.Features.Categories;

public sealed class CategoryEntity : IKeyedEntity, IUserEntityForeignKey
{
    //Value properties
    public Guid Id { get; set; } //Interfaces
    public required string Name { get; set; }
    public Guid UserId { get; set; } //Interfaces

    //Navigation properties
    public UserEntity? User { get; set; } //Interfaces
    public List<Task_Category_JoinEntity> Tasks { get; set; } = [];
}