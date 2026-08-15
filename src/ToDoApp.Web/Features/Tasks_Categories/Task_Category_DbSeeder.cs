using ToDoApp.Data;
using ToDoApp.Data.Features.Tasks;
using ToDoApp.Web.Shared.JoinEntities;
using ToDoApp.Data.Features.Categories;
using ToDoApp.Data.Features.Tasks_Categories;

namespace ToDoApp.Web.Features.Tasks_Categories;

public static class Task_Category_DbSeeder
{
    extension(AppDbContext thisDbContext)
    {
        public async ValueTask Seed_Task_Category_JoinEntitiesAsync(Guid userId, CancellationToken cancellationToken)
        {
            await thisDbContext.SeedJoinEntitiesAsync<Task_Category_JoinEntity, TaskEntity, CategoryEntity>(userId, cancellationToken);
        }
        public async ValueTask Seed_Task_Category_JoinEntitiesForAllUsersAsync(CancellationToken cancellationToken, Guid[]? exceptUserIds = null)
        {
            await thisDbContext.SeedJoinEntitiesForAllUsersAsync<Task_Category_JoinEntity, TaskEntity, CategoryEntity>(cancellationToken, exceptUserIds);
        }
    }
}