using ToDoApp.Data.Features.Auth;
using ToDoApp.Data.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Features.Tasks_Categories;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ToDoApp.Data.Features.Tasks;

public sealed class TaskEntity : IKeyedEntity
{
    //Value properties
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public bool Done { get; set; }
    public Guid UserId { get; set; }

    //Navigation properties
    public UserEntity? User { get; set; }
    public List<Task_Category_JoinEntity> Categories { get; set; } = [];
}
public sealed class TaskEntityConfiguration : IEntityTypeConfiguration<TaskEntity>
{
    #region Interfaces
    public void Configure(EntityTypeBuilder<TaskEntity> builder)
    {
        builder.ConfigureKeyedEntity();
        builder.Property(e => e.Title).IsRequired().HasMaxLength(TaskEntityConstants.TitleMaxLength);
        builder.Property(e => e.Description).IsRequired(false).HasMaxLength(TaskEntityConstants.DescriptionMaxLength);
        builder.Property(e => e.Done).IsRequired();
        builder.HasOne(e => e.User).WithMany(u => u.Tasks).HasForeignKey(e => e.UserId).IsRequired().OnDelete(DeleteBehavior.Restrict);
    }
    #endregion
}
public static class TaskEntityConstants
{
    public const int TitleMaxLength = 50;
    public const int DescriptionMaxLength = 400;
}