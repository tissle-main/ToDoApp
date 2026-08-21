using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ToDoApp.Data.Shared.CreationTimeEntities;

public static class CreationTimeEntityConfiguration
{
    extension<TEntity>(EntityTypeBuilder<TEntity> thisBuilder) where TEntity : class, ICreationTimeEntity
    {
        public void ConfigureCreationTimeEntity()
        {
            thisBuilder.Property(e => e.CreatedAt).IsRequired();
        }
    }
}