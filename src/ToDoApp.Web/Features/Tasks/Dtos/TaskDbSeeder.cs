using Bogus;
using ToDoApp.Data;
using ToDoApp.Data.Features.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ToDoApp.Web.Features.Tasks.Dtos;

public static class TaskDbSeeder
{
    extension(Faker<TaskEntity> thisFaker)
    {
        public async ValueTask<List<TaskEntity>> SeedDatabaseAsync(
            AppDbContext dbContext,
            CancellationToken cancellationToken,
            int min = 2,
            int max = 10
        )
        {
            List<TaskEntity> tasks = thisFaker.GenerateBetween(min, max);
            await dbContext.Tasks.AddRangeAsync(tasks, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return tasks;
        }
        public async ValueTask<Dictionary<Guid, List<TaskEntity>>> SeedDatabaseForAllUsersAsync(
            AppDbContext dbContext,
            CancellationToken cancellationToken,
            Guid[]? exceptUserIds = null,
            int min = 2,
            int max = 10
        )
        {
            Dictionary<Guid, List<TaskEntity>> dict = [];
            Guid[] userIds = await dbContext.Users.Select(e => e.Id).Except(exceptUserIds ?? []).ToArrayAsync(cancellationToken);
            foreach(Guid userId in userIds)
            {
                List<TaskEntity> list = await thisFaker.Clone().WithUserId(userId).SeedDatabaseAsync(dbContext, cancellationToken, min, max);
                dict.Add(userId, list);
            }
            return dict;
        }
    }
}