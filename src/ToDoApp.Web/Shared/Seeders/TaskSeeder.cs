using Bogus;
using ToDoApp.Data;
using ToDoApp.Web.Shared.Fakers;
using ToDoApp.Data.Features.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ToDoApp.Web.Shared.Seeders;

public static class TaskSeeder
{
    public static async ValueTask SeedTasks(this AppDbContext dbContext, CancellationToken cancellationToken)
    {
        Guid[] userIds = await dbContext.Users.AsNoTracking().Select(e => e.Id).ToArrayAsync(cancellationToken);
        foreach(Guid userId in userIds)
        {
            IEnumerable<TaskEntity> tasks = new Faker<TaskEntity>().ValidInstance(userId).GenerateBetween(2, 10);
            await dbContext.Tasks.AddRangeAsync(tasks, cancellationToken);
        }
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}