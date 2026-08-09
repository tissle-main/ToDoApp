namespace ToDoApp.Data.Shared.JoinEntities;

public interface IJoinEntity<TLeftEntity, TRightEntity>
{
    //Value properties
    public abstract Guid LeftId { get; set; }
    public abstract Guid RightId { get; set; }

    //Navigation properties
    public abstract TLeftEntity? Left { get; set; }
    public abstract TRightEntity? Right { get; set; }
}