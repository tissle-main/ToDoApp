using Bogus;
using AwesomeAssertions;
using System.Net.Http.Json;
using ToDoApp.Web.Shared.Fakers;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Web.Features.Auth.Dtos;
using ToDoApp.IntegrationTests.Seeders;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Data.Features.Auth.RefreshTokens;
using ToDoApp.Web.Features.Auth.Handlers.RefreshAccessToken;

namespace ToDoApp.IntegrationTests.Features.Auth;

public sealed class RefreshAccessTokenHandlerTests(ToDoAppFixture thisApp)
{
    private Faker Faker { get; } = new();

    [Fact]
    public async ValueTask Handler_ShouldReplaceRefreshToken()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _) = await thisApp.AddUsers2AndPickRandom();
        string refreshToken = await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<RefreshTokenEntity>().ValidInstance(default).SeedDatabaseForAllUsers(db, TestContext.Current.CancellationToken);
            List<RefreshTokenEntity> list = await new Faker<RefreshTokenEntity>()
                .ValidInstance(user.Id)
                .SeedDatabase(db, TestContext.Current.CancellationToken, min: 1, max: 1);
            return list[0].Value;
        });
        RefreshAccessTokenCommand command = new(refreshToken);

        //Act
        using HttpResponseMessage response = await thisApp.HttpClient.SendRefreshAccessTokenAsync(command, TestContext.Current.CancellationToken);

        //Assert
        response.Should().Be200Ok();
        RefreshAccessTokenResponse? result = await response.Content.ReadFromJsonAsync<RefreshAccessTokenResponse>(TestContext.Current.CancellationToken);
        result.Should().NotBeNull();
        result.Email.Should().Be(user.Email);
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            RefreshTokenEntity? newToken = await db.RefreshTokens.AsNoTracking().SingleOrDefaultAsync(
                e => e.UserId == user.Id && e.Value == result.RefreshToken.Value,
                TestContext.Current.CancellationToken
            );
            newToken.Should().NotBeNull();
            newToken.Should().NotBe(command.RefreshToken);
            newToken.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
            result.RefreshToken.Should().BeEquivalentTo(newToken.ToDto());

            bool oldTokenExists = await db.RefreshTokens.AsNoTracking().AnyAsync(
                e => e.UserId == user.Id && e.Value == refreshToken,
                TestContext.Current.CancellationToken
            );
            oldTokenExists.Should().BeFalse();
        });
    }

    [Fact]
    public async ValueTask Handler_ShouldRemoveExpiredRefreshTokens()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _) = await thisApp.AddUsers2AndPickRandom();
        string refreshToken = await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<RefreshTokenEntity>().ValidInstance(default).MakeExpired().SeedDatabaseForAllUsers(db, TestContext.Current.CancellationToken);
            await new Faker<RefreshTokenEntity>().ValidInstance(default).SeedDatabaseForAllUsers(db, TestContext.Current.CancellationToken);
            List<RefreshTokenEntity> tokens = await new Faker<RefreshTokenEntity>()
                .ValidInstance(user.Id)
                .SeedDatabase(db, TestContext.Current.CancellationToken, min: 1, max: 1);
            return tokens[0].Value;
        });
        RefreshAccessTokenCommand command = new(refreshToken);

        //Act
        using HttpResponseMessage response = await thisApp.HttpClient.SendRefreshAccessTokenAsync(command, TestContext.Current.CancellationToken);

        //Assert
        response.Should().Be200Ok();
        RefreshAccessTokenResponse? result = await response.Content.ReadFromJsonAsync<RefreshAccessTokenResponse>(TestContext.Current.CancellationToken);
        result.Should().NotBeNull();
        result.Email.Should().Be(user.Email);
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            RefreshTokenEntity? newToken = await db.RefreshTokens.AsNoTracking().SingleOrDefaultAsync(
                e => e.UserId == user.Id && e.Value == result.RefreshToken.Value,
                TestContext.Current.CancellationToken
            );
            newToken.Should().NotBeNull();
            newToken.Should().NotBe(command.RefreshToken);
            newToken.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
            result.RefreshToken.Should().BeEquivalentTo(newToken.ToDto());

            bool oldTokenExists = await db.RefreshTokens.AsNoTracking().AnyAsync(
                e => e.UserId == user.Id && e.Value == refreshToken,
                TestContext.Current.CancellationToken
            );
            oldTokenExists.Should().BeFalse();

            int expiredCount = await db.RefreshTokens.AsNoTracking().CountAsync(e => DateTime.UtcNow > e.ExpiresAt, TestContext.Current.CancellationToken);
            expiredCount.Should().Be(0);
        });
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenRefreshTokenNotFound()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _) = await thisApp.AddUsers2AndPickRandom();
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<RefreshTokenEntity>().ValidInstance(default).SeedDatabaseForAllUsers(db, TestContext.Current.CancellationToken);
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
        (UserEntity user, _) = await thisApp.AddUsers2AndPickRandom();
        string refreshToken = await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<RefreshTokenEntity>().ValidInstance(default).SeedDatabaseForAllUsers(db, TestContext.Current.CancellationToken);
            List<RefreshTokenEntity> tokens = await new Faker<RefreshTokenEntity>()
                .ValidInstance(user.Id)
                .MakeExpired()
                .SeedDatabase(db, TestContext.Current.CancellationToken, min: 1, max: 1);
            return tokens[0].Value;
        });
        RefreshAccessTokenCommand command = new(refreshToken);

        //Act
        using HttpResponseMessage response = await thisApp.HttpClient.SendRefreshAccessTokenAsync(command, TestContext.Current.CancellationToken);

        //Assert
        response.Should().Be400BadRequest();
    }
}