using ToDoApp.Data.Features.Tasks;
using System.Diagnostics.CodeAnalysis;
using ToDoApp.Data.Features.Categories;
using ToDoApp.Data.Shared.JoinEntities;

namespace ToDoApp.Data.Features.Tasks_Categories;

public sealed class Task_Category_JoinEntity : IJoinEntity<Task_Category_JoinEntity, TaskEntity, CategoryEntity>
{
    #region Instance
    //Value properties
    public Guid LeftId { get; set; } //Interfaces
    public Guid RightId { get; set; } //Interfaces

    //Navigation properties
    public TaskEntity? Left { get; set; } //Interfaces
    public CategoryEntity? Right { get; set; } //Interfaces
    #endregion

    #region Base
    public override bool Equals(object? obj)
    {
        return Equals(obj as Task_Category_JoinEntity);
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(LeftId, RightId);
    }
    #endregion

    #region Interfaces
    public bool Equals([NotNullWhen(true)] Task_Category_JoinEntity? obj)
    {
        if(obj is null)
        {
            return false;
        }
        return this.LeftId == obj.LeftId && this.RightId == obj.RightId;
    }
    #endregion
}