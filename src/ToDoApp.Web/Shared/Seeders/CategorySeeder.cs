using Bogus;
using ToDoApp.Data;
using ToDoApp.Web.Shared.Fakers;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Features.Categories;

namespace ToDoApp.Web.Shared.Seeders;

public static class CategorySeeder
{
    public static async ValueTask SeedCategories(this AppDbContext dbContext, CancellationToken cancellationToken)
    {
        Guid[] userIds = await dbContext.Users.AsNoTracking().Select(e => e.Id).ToArrayAsync(cancellationToken);
        foreach(Guid userId in userIds)
        {
            IEnumerable<CategoryEntity> categories = new Faker<CategoryEntity>().ValidInstance(userId).GenerateBetween(2, 10);
            await dbContext.Categories.AddRangeAsync(categories, cancellationToken);
        }
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}