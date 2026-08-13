using Bogus;
using ToDoApp.Data;
using Riok.Mapperly.Abstractions;
using ToDoApp.Data.Features.Auth;
using Microsoft.EntityFrameworkCore;

namespace ToDoApp.Web.Features.Auth.Dtos;

public sealed class RefreshTokenDto
{
    public required string Value { get; set; }
    public DateTime ExpiresAt { get; set; }
}
[Mapper] public static partial class RefreshTokenEntityDtoMapper
{
    [MapperIgnoreSource(nameof(RefreshTokenEntity.Id))]
    [MapperIgnoreSource(nameof(RefreshTokenEntity.User))]
    [MapperIgnoreSource(nameof(RefreshTokenEntity.UserId))]
    public static partial RefreshTokenDto ToDto(this RefreshTokenEntity entity);
    public static partial IEnumerable<RefreshTokenDto> ToDtos(this IEnumerable<RefreshTokenEntity> entities);

    [MapperIgnoreSource(nameof(RefreshTokenEntity.Id))]
    [MapperIgnoreSource(nameof(RefreshTokenEntity.User))]
    [MapperIgnoreSource(nameof(RefreshTokenEntity.UserId))]
    public static partial void MapToDto(this RefreshTokenEntity source, RefreshTokenDto destination);
    public static partial void MapToDto(this RefreshTokenDto source, RefreshTokenDto destination);

    [MapperIgnoreTarget(nameof(RefreshTokenEntity.Id))]
    [MapperIgnoreTarget(nameof(RefreshTokenEntity.User))]
    [MapperIgnoreTarget(nameof(RefreshTokenEntity.UserId))]
    public static partial RefreshTokenEntity ToEntity(this RefreshTokenDto dto);
    public static partial IEnumerable<RefreshTokenDto> ToEntities(this IEnumerable<RefreshTokenEntity> dtos);

    [MapperIgnoreTarget(nameof(RefreshTokenEntity.Id))]
    [MapperIgnoreTarget(nameof(RefreshTokenEntity.User))]
    [MapperIgnoreTarget(nameof(RefreshTokenEntity.UserId))]
    public static partial void MapToEntity(this RefreshTokenDto source, RefreshTokenEntity destination);
    public static partial void MapToEntity(this RefreshTokenEntity source, RefreshTokenEntity destination);
    public static partial IQueryable<RefreshTokenDto> ProjectToDto(this IQueryable<RefreshTokenEntity> query);
}
public static class RefreshTokenEntityFaker
{
    public static Faker<RefreshTokenEntity> ValidInstance(this Faker<RefreshTokenEntity> faker, Guid userId)
    {
        return faker.CustomInstantiator(g =>
        {
            return new RefreshTokenEntity()
            {
                Value = g.Random.String(RefreshTokenEntityConstants.RefreshTokenMaxLength),
                ExpiresAt = g.Date.Soon(refDate: DateTime.UtcNow.AddDays(1)),
                UserId = userId
            };
        });
    }
    public static Faker<RefreshTokenEntity> WithUserId(this Faker<RefreshTokenEntity> faker, Guid userId)
    {
        return faker.RuleFor(e => e.UserId, g => userId);
    }
    public static Faker<RefreshTokenEntity> MakeExpired(this Faker<RefreshTokenEntity> faker)
    {
        return faker.RuleFor(
            e => e.ExpiresAt,
            g => g.Date.Recent(refDate: DateTime.UtcNow)
        );
    }
}
public static class RefreshTokenDbSeeder
{
    public static async ValueTask<List<RefreshTokenEntity>> SeedDatabase(
        this Faker<RefreshTokenEntity> faker,
        AppDbContext dbContext,
        CancellationToken cancellationToken,
        int min = 2,
        int max = 10
    )
    {
        List<RefreshTokenEntity> categories = faker.GenerateBetween(min, max);
        await dbContext.RefreshTokens.AddRangeAsync(categories, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return categories;
    }
    public static async ValueTask<Dictionary<Guid, List<RefreshTokenEntity>>> SeedDatabaseForAllUsers(
        this Faker<RefreshTokenEntity> faker,
        AppDbContext dbContext,
        CancellationToken cancellationToken,
        Guid[]? exceptUserIds = null,
        int min = 2,
        int max = 10
    )
    {
        Dictionary<Guid, List<RefreshTokenEntity>> dict = [];
        Guid[] userIds = await dbContext.Users.AsNoTracking().Select(e => e.Id).Except(exceptUserIds ?? []).ToArrayAsync(cancellationToken);
        foreach(Guid userId in userIds)
        {
            List<RefreshTokenEntity> list = await faker.Clone().WithUserId(userId).SeedDatabase(dbContext, cancellationToken, min, max);
            dict.Add(userId, list);
        }
        return dict;
    }
}