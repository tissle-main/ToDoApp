using Bogus;
using ToDoApp.Data.Features.Auth.RefreshTokens;

namespace ToDoApp.Web.Features.Auth.Dtos.RefreshTokens;

public static class RefreshTokenEntityFaker
{
    extension(Faker<RefreshTokenEntity> thisFaker)
    {
        public Faker<RefreshTokenEntity> ValidInstance()
        {
            return thisFaker.CustomInstantiator(g =>
            {
                return new RefreshTokenEntity()
                {
                    Value = g.Random.String2(RefreshTokenEntityConstants.RefreshTokenMaxLength),
                    ExpiresAt = g.Date.Soon(refDate: DateTime.UtcNow.AddDays(1))
                };
            });
        }
        public Faker<RefreshTokenEntity> WithId(Guid id)
        {
            return thisFaker.RuleFor(e => e.Id, g => id);
        }
        public Faker<RefreshTokenEntity> WithUserId(Guid userId)
        {
            return thisFaker.RuleFor(e => e.UserId, g => userId);
        }
        public Faker<RefreshTokenEntity> MakeExpired()
        {
            return thisFaker.RuleFor(
                e => e.ExpiresAt,
                g => g.Date.Recent(refDate: DateTime.UtcNow.AddDays(-1))
            );
        }
    }
}