using ToDoApp.Data.Features.Tasks;
using ToDoApp.Data.Features.Categories;

namespace ToDoApp.Data.Features.Tasks_Categories;

public sealed class Task_Category_JoinEntity : IEquatable<Task_Category_JoinEntity>
{
    #region Instance
    //Value properties
    public Guid TaskId { get; set; }
    public Guid CategoryId { get; set; }

    //Navigation properties
    public TaskEntity? Task { get; set; }
    public CategoryEntity? Category { get; set; }
    #endregion

    #region Base
    public override bool Equals(object? obj)
    {
        return Equals(obj as Task_Category_JoinEntity);
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(TaskId, CategoryId);
    }
    #endregion

    #region Interfaces
    public bool Equals(Task_Category_JoinEntity? other)
    {
        if(other is null)
        {
            return false;
        }
        return this.TaskId == other.TaskId && this.CategoryId == other.CategoryId;
    }
    #endregion
}