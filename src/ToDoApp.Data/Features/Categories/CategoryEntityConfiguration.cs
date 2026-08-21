using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Shared.KeyedEntities;
using ToDoApp.Data.Shared.CreationTimeEntities;
using ToDoApp.Data.Features.Auth.Users.ForeignKey;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ToDoApp.Data.Features.Categories;

public sealed class CategoryEntityConfiguration : IEntityTypeConfiguration<CategoryEntity>
{
    #region Interfaces
    public void Configure(EntityTypeBuilder<CategoryEntity> builder)
    {
        builder.ConfigureKeyedEntity();
        builder.ConfigureCreationTimeEntity();
        builder.Property(e => e.Name).IsRequired().HasMaxLength(CategoryEntityConstants.NameMaxLength);
        builder.ConfigureUserEntityForeignKey(e => e.Categories);
    }
    #endregion
}