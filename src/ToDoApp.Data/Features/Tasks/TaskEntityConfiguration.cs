using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Shared.KeyedEntities;
using ToDoApp.Data.Features.Auth.Users.ForeignKey;
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
        builder.ConfigureUserEntityForeignKey(e => e.Tasks);
    }
    #endregion
}