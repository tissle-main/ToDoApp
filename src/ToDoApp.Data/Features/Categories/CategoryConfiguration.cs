using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ToDoApp.Data.Features.Categories;

public sealed class CategoryConfiguration : IEntityTypeConfiguration<CategoryEntity>
{
    #region Interfaces
    public void Configure(EntityTypeBuilder<CategoryEntity> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.Name).IsRequired().HasMaxLength(CategoryConstants.NameMaxLength);
    }
    #endregion
}