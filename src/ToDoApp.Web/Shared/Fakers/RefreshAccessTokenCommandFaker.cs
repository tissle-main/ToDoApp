using Bogus;
using ToDoApp.Web.Features.Auth.Handlers;
using ToDoApp.Data.Features.Auth.RefreshTokens;

namespace ToDoApp.Web.Shared.Fakers;

public static class RefreshAccessTokenCommandFaker
{
    public static Faker<RefreshAccessTokenCommand> ValidInstance(this Faker<RefreshAccessTokenCommand> faker)
    {
        return faker.CustomInstantiator(g =>
        {
            string refreshToken = g.Random.String(RefreshTokenEntityConstants.RefreshTokenMaxLength);
            return new RefreshAccessTokenCommand(refreshToken);
        });
    }
}