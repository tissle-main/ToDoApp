using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ToDoApp.Data.Shared.Entities;

public interface IKeyedEntity
{
    //Value properties
    public abstract Guid Id { get; set; }
}
public static class KeyedEntityConfiguration
{
    public static void ConfigureKeyedEntity<TEntity>(this EntityTypeBuilder<TEntity> builder) where TEntity : class, IKeyedEntity
    {
        builder.HasKey(e => e.Id);
    }
}
public static class KeyedEntityDbContextExtensions
{
    public static void GenerateIdForKeyedEntities(this AppDbContext dbContext)
    {
        foreach(EntityEntry<IKeyedEntity> entry in dbContext.ChangeTracker.Entries<IKeyedEntity>())
        {
            if(entry.State is EntityState.Added)
            {
                entry.Entity.Id = Guid.CreateVersion7();
            }
        }
    }
}