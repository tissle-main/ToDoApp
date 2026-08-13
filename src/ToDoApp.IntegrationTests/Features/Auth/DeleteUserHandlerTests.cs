using Bogus;
using AwesomeAssertions;
using ToDoApp.Data.Features.Auth;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Web.Features.Auth.Dtos;
using ToDoApp.IntegrationTests.Seeders;
using ToDoApp.Web.Features.Auth.Handlers;

namespace ToDoApp.IntegrationTests.Features.Auth;

public sealed class DeleteUserHandlerTests(ToDoAppFixture thisApp)
{
    [Fact]
    public async ValueTask Handler_ShouldDeleteUserCascading()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, string password, string accessToken) = await thisApp.AddUsers2AndLoginRandom();
        (int userCount, int refreshTokenCount, int userRefreshTokenCount) = await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<RefreshTokenEntity>().ValidInstance(default).SeedDatabaseForAllUsers(db, TestContext.Current.CancellationToken);
            int userCount = await db.Users.AsNoTracking().CountAsync(TestContext.Current.CancellationToken);
            int refreshTokenCount = await db.RefreshTokens.AsNoTracking().CountAsync(TestContext.Current.CancellationToken);
            int userRefreshTokenCount = await db.RefreshTokens.AsNoTracking().Where(e => e.UserId == user.Id).CountAsync(TestContext.Current.CancellationToken);
            return (userCount, refreshTokenCount, userRefreshTokenCount);
        });

        //Act
        using HttpResponseMessage response = await thisApp.HttpClient.SendDeleteUserAsync(accessToken, TestContext.Current.CancellationToken);

        //Assert
        response.Should().Be204NoContent();
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            bool userExists = await db.Users.AsNoTracking().AnyAsync(e => e.Id == user.Id, TestContext.Current.CancellationToken);
            userExists.Should().BeFalse();
            int newUserCount = await db.Users.AsNoTracking().CountAsync(TestContext.Current.CancellationToken);
            newUserCount.Should().Be(userCount - 1);

            bool userRefreshTokensExist = await db.RefreshTokens.AsNoTracking().AnyAsync(e => e.UserId == user.Id, TestContext.Current.CancellationToken);
            userRefreshTokensExist.Should().BeFalse();
            int newRefreshTokenCount = await db.RefreshTokens.AsNoTracking().CountAsync(TestContext.Current.CancellationToken);
            newRefreshTokenCount.Should().Be(refreshTokenCount - userRefreshTokenCount);
        });
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenUnauthorized()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();

        //Act
        using HttpResponseMessage response = await thisApp.HttpClient.SendDeleteUserAsync("", TestContext.Current.CancellationToken);

        //Assert
        response.Should().Be401Unauthorized();
    }
}