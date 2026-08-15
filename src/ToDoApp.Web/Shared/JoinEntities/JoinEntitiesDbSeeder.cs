using Bogus;
using ToDoApp.Data;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Shared.JoinEntities;
using ToDoApp.Data.Shared.KeyedEntities;
using ToDoApp.Data.Features.Auth.Users.ForeignKey;

namespace ToDoApp.Web.Shared.JoinEntities;

public static class JoinEntitiesDbSeeder
{
    private static Faker Faker { get; } = new();

    extension<TJoinEntity, TLeftEntity, TRightEntity>(AppDbContext thisDbContext)
        where TJoinEntity : class, IJoinEntity<TJoinEntity, TLeftEntity, TRightEntity>, new()
        where TLeftEntity : class, IKeyedEntity, IUserEntityForeignKey
        where TRightEntity : class, IKeyedEntity, IUserEntityForeignKey
    {
        public async ValueTask SeedJoinEntitiesAsync(Guid userId, CancellationToken cancellationToken)
        {
            TLeftEntity[] lefts = await thisDbContext.Set<TLeftEntity>().AsNoTracking().Where(e => e.UserId == userId).ToArrayAsync(cancellationToken);
            TRightEntity[] rights = await thisDbContext.Set<TRightEntity>().AsNoTracking().Where(e => e.UserId == userId).ToArrayAsync(cancellationToken);
            foreach(TLeftEntity left in lefts)
            {
                foreach(TRightEntity right in rights)
                {
                    if(Faker.Random.Bool())
                    {
                        TJoinEntity join = new()
                        {
                            LeftId = left.Id,
                            RightId = right.Id
                        };
                        await thisDbContext.Set<TJoinEntity>().AddAsync(join, cancellationToken);
                    }
                }
            }
        }
        public async ValueTask SeedJoinEntitiesForAllUsersAsync(CancellationToken cancellationToken, Guid[]? exceptUserIds = null)
        {
            Guid[] userIds = await thisDbContext.Users.AsNoTracking().Select(e => e.Id).Except(exceptUserIds ?? []).ToArrayAsync(cancellationToken);
            foreach(Guid userId in userIds)
            {
                await thisDbContext.SeedJoinEntitiesAsync<TJoinEntity, TLeftEntity, TRightEntity>(userId, cancellationToken);
            }
        }
    }
}