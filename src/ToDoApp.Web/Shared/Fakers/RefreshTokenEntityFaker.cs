using Bogus;
using ToDoApp.Data.Features.Auth.RefreshTokens;

namespace ToDoApp.Web.Shared.Fakers;

public static class RefreshTokenEntityFaker
{
    public static Faker<RefreshTokenEntity> ValidInstance(this Faker<RefreshTokenEntity> faker, Guid userId)
    {
        return faker.CustomInstantiator(g =>
        {
            return new RefreshTokenEntity()
            {
                RefreshToken = g.Random.String(RefreshTokenEntityConstants.RefreshTokenMaxLength),
                ExpiresAt = g.Date.Soon(refDate: DateTime.UtcNow.AddDays(1)),
                UserId = userId
            };
        });
    }
    public static Faker<RefreshTokenEntity> MakeExpired(this Faker<RefreshTokenEntity> faker)
    {
        return faker.RuleFor(
            e => e.ExpiresAt,
            g => g.Date.Recent(refDate: DateTime.UtcNow)
        );
    }
}