using Bogus;
using ToDoApp.Data;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Features.Categories;

namespace ToDoApp.Web.Features.Categories.Dtos;

public static class CategoryDbSeeder
{
    extension(Faker<CategoryEntity> thisFaker)
    {
        public async ValueTask<List<CategoryEntity>> SeedDatabaseAsync(
            AppDbContext dbContext,
            CancellationToken cancellationToken,
            int min = 2,
            int max = 10
        )
        {
            List<CategoryEntity> categories = thisFaker.GenerateBetween(min, max);
            await dbContext.Categories.AddRangeAsync(categories, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return categories;
        }
        public async ValueTask<Dictionary<Guid, List<CategoryEntity>>> SeedDatabaseForAllUsersAsync(
            AppDbContext dbContext,
            CancellationToken cancellationToken,
            Guid[]? exceptUserIds = null,
            int min = 2,
            int max = 10
        )
        {
            Dictionary<Guid, List<CategoryEntity>> dict = [];
            Guid[] userIds = await dbContext.Users.Select(e => e.Id).Except(exceptUserIds ?? []).ToArrayAsync(cancellationToken);
            foreach(Guid userId in userIds)
            {
                List<CategoryEntity> list = await thisFaker.Clone().WithUserId(userId).SeedDatabaseAsync(dbContext, cancellationToken, min, max);
                dict.Add(userId, list);
            }
            return dict;
        }
    }
}