using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Shared.KeyedEntities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ToDoApp.Data.Features.Categories;

public sealed class CategoryEntityConfiguration : IEntityTypeConfiguration<CategoryEntity>
{
    #region Interfaces
    public void Configure(EntityTypeBuilder<CategoryEntity> builder)
    {
        builder.ConfigureKeyedEntity();
        builder.Property(e => e.Name).IsRequired().HasMaxLength(CategoryEntityConstants.NameMaxLength);
        builder.HasOne(e => e.User).WithMany(u => u.Categories).HasForeignKey(e => e.UserId).IsRequired().OnDelete(DeleteBehavior.Restrict);
    }
    #endregion
}