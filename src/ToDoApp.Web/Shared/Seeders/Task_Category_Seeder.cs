using Bogus;
using ToDoApp.Data;
using ToDoApp.Data.Features.Tasks;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Features.Categories;
using ToDoApp.Data.Features.Tasks_Categories;

namespace ToDoApp.Web.Shared.Seeders;

public static class Task_Category_Seeder
{
    private static Faker Faker { get; } = new();

    public static async ValueTask SeedTasks(this AppDbContext dbContext, CancellationToken cancellationToken)
    {
        Guid[] userIds = await dbContext.Users.AsNoTracking().Select(e => e.Id).ToArrayAsync(cancellationToken);
        foreach(Guid userId in userIds)
        {
            TaskEntity[] tasks = await dbContext.Tasks.AsNoTracking().Where(e => e.UserId == userId).ToArrayAsync(cancellationToken);
            CategoryEntity[] categories = await dbContext.Categories.AsNoTracking().Where(e => e.UserId == userId).ToArrayAsync(cancellationToken);
            foreach(TaskEntity task in tasks)
            {
                foreach(CategoryEntity category in categories)
                {
                    if(Faker.Random.Bool())
                    {
                        Task_Category_JoinEntity task_category = new()
                        {
                            LeftId = task.Id,
                            RightId = category.Id
                        };
                        await dbContext.Tasks_Categories.AddAsync(task_category, cancellationToken);
                    }
                }
            }
        }
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}