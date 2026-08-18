using Bogus;
using AwesomeAssertions;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Web.Shared.Extensions;
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
        (UserEntity user, _, _) = await thisApp.AddUsers2AndLoginRandomAsync();
        RefreshTokenEntity refreshToken = await thisApp.ExecuteDbContextAsync(async db =>
        {
            return await db.RefreshTokens.SingleAsync(TestContext.Current.CancellationToken);
        });

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendRefreshAccessTokenAsync(TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be200Ok();
        string? refreshTokenValue = message.GetRefreshToken();
        refreshTokenValue.Should().NotBeNull();
        refreshTokenValue.Should().NotBe(refreshToken.Value);
        RefreshAccessTokenResponse? response = await message.Content.ReadFromJsonAsync<RefreshAccessTokenResponse>(TestContext.Current.CancellationToken);
        response.Should().NotBeNull();
        response.Email.Should().Be(user.Email);
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            RefreshTokenEntity? newRefreshToken = await db.RefreshTokens.SingleOrDefaultAsync(TestContext.Current.CancellationToken);
            newRefreshToken.Should().NotBeNull();
            newRefreshToken.Value.Should().Be(refreshTokenValue);
        });
    }

    [Fact]
    public async ValueTask Handler_ShouldRemoveAllExpiredRefreshTokens()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _, _) = await thisApp.AddUsers2AndLoginRandomAsync();
        RefreshTokenEntity refreshToken = await thisApp.ExecuteDbContextAsync(async db =>
        {
            return await db.RefreshTokens.SingleAsync(TestContext.Current.CancellationToken);
        });
        int expectedTokenCount = await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<RefreshTokenEntity>().ValidInstance().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken);
            int expectedTokenCount = await db.RefreshTokens.CountAsync(TestContext.Current.CancellationToken);
            await new Faker<RefreshTokenEntity>().ValidInstance().MakeExpired().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken);
            return expectedTokenCount;
        });

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendRefreshAccessTokenAsync(TestContext.Current.CancellationToken);

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
        (UserEntity user, _, _) = await thisApp.AddUsers2AndLoginRandomAsync();
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            RefreshTokenEntity refreshToken = await db.RefreshTokens.SingleAsync(TestContext.Current.CancellationToken);
            db.RefreshTokens.Remove(refreshToken);
            await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        });

        //Act
        using HttpResponseMessage response = await thisApp.HttpClient.SendRefreshAccessTokenAsync(TestContext.Current.CancellationToken);

        //Assert
        response.Should().Be404NotFound();
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenRefreshTokenExpired()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _, _) = await thisApp.AddUsers2AndLoginRandomAsync();
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            RefreshTokenEntity refreshToken = await db.RefreshTokens.SingleAsync(TestContext.Current.CancellationToken);
            refreshToken.ExpiresAt = DateTime.UtcNow.AddDays(-1);
            db.RefreshTokens.Update(refreshToken);
            await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        });

        //Act
        using HttpResponseMessage response = await thisApp.HttpClient.SendRefreshAccessTokenAsync(TestContext.Current.CancellationToken);

        //Assert
        response.Should().Be400BadRequest();
    }
}