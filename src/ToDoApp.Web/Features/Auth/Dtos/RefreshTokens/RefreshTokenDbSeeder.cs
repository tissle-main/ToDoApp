using Bogus;
using ToDoApp.Data;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Features.Auth.RefreshTokens;

namespace ToDoApp.Web.Features.Auth.Dtos.RefreshTokens;

public static class RefreshTokenDbSeeder
{
    extension(Faker<RefreshTokenEntity> thisFaker)
    {
        public async ValueTask<List<RefreshTokenEntity>> SeedDatabaseAsync(
            AppDbContext dbContext,
            CancellationToken cancellationToken,
            int min = 2,
            int max = 10
        )
        {
            List<RefreshTokenEntity> categories = thisFaker.GenerateBetween(min, max);
            await dbContext.RefreshTokens.AddRangeAsync(categories, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return categories;
        }
        public async ValueTask<Dictionary<Guid, List<RefreshTokenEntity>>> SeedDatabaseForAllUsersAsync(
            AppDbContext dbContext,
            CancellationToken cancellationToken,
            Guid[]? exceptUserIds = null,
            int min = 2,
            int max = 10
        )
        {
            Dictionary<Guid, List<RefreshTokenEntity>> dict = [];
            Guid[] userIds = await dbContext.Users.Select(e => e.Id).Except(exceptUserIds ?? []).ToArrayAsync(cancellationToken);
            foreach(Guid userId in userIds)
            {
                List<RefreshTokenEntity> list = await thisFaker.Clone().WithUserId(userId).SeedDatabaseAsync(dbContext, cancellationToken, min, max);
                dict.Add(userId, list);
            }
            return dict;
        }
    }
}