namespace ToDoApp.Data.Shared.KeyedEntities;

public interface IKeyedEntity
{
    //Value properties
    public abstract Guid Id { get; set; }
}