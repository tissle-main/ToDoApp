using ToDoApp.Data.Features.Tasks;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Features.Categories;
using ToDoApp.Data.Shared.JoinEntities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ToDoApp.Data.Features.Tasks_Categories;

public sealed class Task_Category_JoinEntityConfiguration : IEntityTypeConfiguration<Task_Category_JoinEntity>
{
    #region Interfaces
    public void Configure(EntityTypeBuilder<Task_Category_JoinEntity> builder)
    {
        builder.ConfigureJoinEntity<Task_Category_JoinEntity, TaskEntity, CategoryEntity>(l => l.Categories, r => r.Tasks);
    }
    #endregion
}