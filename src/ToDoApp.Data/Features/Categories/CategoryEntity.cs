using ToDoApp.Data.Features.Auth;
using ToDoApp.Data.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Features.Tasks_Categories;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ToDoApp.Data.Features.Categories;

public sealed class CategoryEntity : IKeyedEntity
{
    //Value properties
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public Guid UserId { get; set; }

    //Navigation properties
    public UserEntity? User { get; set; }
    public List<Task_Category_JoinEntity> Tasks { get; set; } = [];
}
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
public static class CategoryEntityConstants
{
    public const int NameMaxLength = 50;
}