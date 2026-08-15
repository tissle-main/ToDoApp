using Bogus;
using ToDoApp.Data.Features.Auth.RefreshTokens;
using ToDoApp.Web.Features.Auth.Dtos.RefreshTokens;

namespace ToDoApp.Web.Features.Auth.Handlers.RefreshAccessToken;

public static class RefreshAccessTokenCommandFaker
{
    extension(Faker<RefreshAccessTokenCommand> thisFaker)
    {
        public Faker<RefreshAccessTokenCommand> ValidInstance()
        {
            return thisFaker.CustomInstantiator(g =>
            {
                string refreshToken = new Faker<RefreshTokenEntity>().ValidInstance().Generate().Value;
                return new RefreshAccessTokenCommand(refreshToken);
            });
        }
    }  
}