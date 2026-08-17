using Bogus;
using AwesomeAssertions;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using ToDoApp.IntegrationTests.Seeders;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Data.Features.Auth.RefreshTokens;
using ToDoApp.Web.Features.Auth.Dtos.RefreshTokens;
using ToDoApp.Web.Features.Auth.Handlers.RefreshAccessToken;

namespace ToDoApp.IntegrationTests.Features.Auth.Handlers.RefreshAccessToken;

public sealed class RefreshAccessTokenHandlerTests(ToDoAppFixture thisApp)
{
    [Fact]
    public async ValueTask Handler_ShouldReplaceRefreshToken()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _) = await thisApp.AddUsers2AndPickRandomAsync();
        RefreshTokenEntity refreshToken = await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<RefreshTokenEntity>().ValidInstance().WithUserId(user.Id).SeedDatabaseAsync(db, TestContext.Current.CancellationToken, min: 1, max: 1);
            return await db.RefreshTokens.SingleAsync(TestContext.Current.CancellationToken);
        });
        RefreshAccessTokenCommand command = new(refreshToken.Value);

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendRefreshAccessTokenAsync(command, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be200Ok();
        RefreshAccessTokenResponse? response = await message.Content.ReadFromJsonAsync<RefreshAccessTokenResponse>(TestContext.Current.CancellationToken);
        response.Should().NotBeNull();
        response.Email.Should().Be(user.Email);
        response.RefreshToken.Value.Should().NotBe(command.RefreshToken);
        response.RefreshToken.Should().NotBeEquivalentTo(refreshToken.ToDto());
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            RefreshTokenEntity? newRefreshToken = await db.RefreshTokens.SingleOrDefaultAsync(TestContext.Current.CancellationToken);
            newRefreshToken.Should().NotBeNull();
            newRefreshToken.ToDto().Should().BeEquivalentTo(response.RefreshToken);
        });
    }

    [Fact]
    public async ValueTask Handler_ShouldRemoveAllExpiredRefreshTokens()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _) = await thisApp.AddUsers2AndPickRandomAsync();
        RefreshTokenEntity refreshToken = await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<RefreshTokenEntity>().ValidInstance().WithUserId(user.Id).SeedDatabaseAsync(db, TestContext.Current.CancellationToken, min: 1, max: 1);
            return await db.RefreshTokens.SingleAsync(TestContext.Current.CancellationToken);
        });
        int expectedTokenCount = await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<RefreshTokenEntity>().ValidInstance().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken);
            int expectedTokenCount = await db.RefreshTokens.CountAsync(TestContext.Current.CancellationToken);
            await new Faker<RefreshTokenEntity>().ValidInstance().MakeExpired().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken);
            return expectedTokenCount;
        });
        RefreshAccessTokenCommand command = new(refreshToken.Value);

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendRefreshAccessTokenAsync(command, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be200Ok();
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            bool expiredTokensExist = await db.RefreshTokens.AnyAsync(e => DateTime.UtcNow > e.ExpiresAt, TestContext.Current.CancellationToken);
            expiredTokensExist.Should().BeFalse();

            int actualTokenCount = await db.RefreshTokens.CountAsync(TestContext.Current.CancellationToken);
            actualTokenCount.Should().Be(expectedTokenCount);
        });
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenRefreshTokenNotFound()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _) = await thisApp.AddUsers2AndPickRandomAsync();
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<RefreshTokenEntity>().ValidInstance().WithUserId(user.Id).SeedDatabaseAsync(
                db,
                TestContext.Current.CancellationToken,
                min: 1,
                max: 1
            );
        });
        RefreshAccessTokenCommand command = new Faker<RefreshAccessTokenCommand>().ValidInstance().Generate();

        //Act
        using HttpResponseMessage response = await thisApp.HttpClient.SendRefreshAccessTokenAsync(command, TestContext.Current.CancellationToken);

        //Assert
        response.Should().Be404NotFound();
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenRefreshTokenExpired()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _) = await thisApp.AddUsers2AndPickRandomAsync();
        RefreshTokenEntity refreshToken = await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<RefreshTokenEntity>().ValidInstance().WithUserId(user.Id).MakeExpired().SeedDatabaseAsync(
                db,
                TestContext.Current.CancellationToken,
                min: 1,
                max: 1
            );
            return await db.RefreshTokens.SingleAsync(TestContext.Current.CancellationToken);
        });
        RefreshAccessTokenCommand command = new(refreshToken.Value);

        //Act
        using HttpResponseMessage response = await thisApp.HttpClient.SendRefreshAccessTokenAsync(command, TestContext.Current.CancellationToken);

        //Assert
        response.Should().Be400BadRequest();
    }
}