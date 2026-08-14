using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ToDoApp.Data.Shared.KeyedEntities;

public static class KeyedEntityConfiguration
{
    public static void ConfigureKeyedEntity<TEntity>(this EntityTypeBuilder<TEntity> builder) where TEntity : class, IKeyedEntity
    {
        builder.HasKey(e => e.Id);
    }
}