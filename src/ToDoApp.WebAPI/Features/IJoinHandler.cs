using FluentResults;

namespace ToDoApp.WebAPI.Features;

public interface IJoinHandler<TEntity> where TEntity : IEquatable<TEntity>
{
    public abstract ValueTask<Result> Handle(List<TEntity> oldList, List<TEntity> newList, CancellationToken cancellationToken);
}