using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ToDoApp.Data.Features.Tasks_Categories;

public sealed class Task_Category_JoinConfiguration : IEntityTypeConfiguration<Task_Category_JoinEntity>
{
    #region Interfaces
    public void Configure(EntityTypeBuilder<Task_Category_JoinEntity> builder)
    {
        builder.HasKey(e => new { e.TaskId, e.CategoryId });
        builder.HasOne(e => e.Task).WithMany(t => t.Categories).HasForeignKey(e => e.TaskId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Category).WithMany(c => c.Tasks).HasForeignKey(e => e.CategoryId).OnDelete(DeleteBehavior.Restrict);
    }
    #endregion
}