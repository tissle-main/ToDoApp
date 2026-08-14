using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Shared.KeyedEntities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ToDoApp.Data.Features.Tasks;

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