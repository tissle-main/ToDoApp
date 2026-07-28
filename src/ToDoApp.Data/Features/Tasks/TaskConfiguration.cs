using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ToDoApp.Data.Features.Tasks;

public sealed class TaskConfiguration : IEntityTypeConfiguration<TaskEntity>
{
    #region Interfaces
    public void Configure(EntityTypeBuilder<TaskEntity> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.Title).IsRequired().HasMaxLength(TaskConstants.TitleMaxLength);
        builder.Property(e => e.Description).IsRequired().HasMaxLength(TaskConstants.DescriptionMaxLength);
        builder.Property(e => e.Done).IsRequired();
    }
    #endregion
}