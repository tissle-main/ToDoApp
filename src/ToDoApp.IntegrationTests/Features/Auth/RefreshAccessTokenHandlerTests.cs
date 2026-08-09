using Bogus;
using AwesomeAssertions;
using System.Net.Http.Json;
using ToDoApp.Web.Shared.Fakers;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Web.Features.Auth.Handlers;
using ToDoApp.Data.Features.Auth.RefreshTokens;

namespace ToDoApp.IntegrationTests.Features.Auth;

public sealed class RefreshAccessTokenHandlerTests(ToDoAppFixture thisApp)
{
    #region Static
    public const string Path = "/auth/refresh";
    #endregion

    #region Instance
    [Fact]
    public async Task Handle_ShouldReplaceRefreshToken()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _) = await RegisterUserHandlerTests.RegisterUserAsync(thisApp);
        RefreshTokenEntity token = null!;
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            token = new Faker<RefreshTokenEntity>().ValidInstance(user.Id).Generate();
            await db.RefreshTokens.AddAsync(token, TestContext.Current.CancellationToken);
            await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        });
        RefreshAccessTokenCommand refreshCommand = new(token.RefreshToken);

        //Act
        using HttpResponseMessage response = await HttpClientJsonExtensions.PutAsJsonAsync(
            thisApp.HttpClient,
            Path,
            refreshCommand,
            TestContext.Current.CancellationToken
        );

        //Assert
        response.Should().Be200Ok();
        RefreshAccessTokenResponse? result = await response.Content.ReadFromJsonAsync<RefreshAccessTokenResponse>(TestContext.Current.CancellationToken);
        result.Should().NotBeNull();
        result.Email.Should().Be(user.Email);
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            RefreshTokenEntity? newToken = await db.RefreshTokens.AsNoTracking().Include(e => e.User).SingleOrDefaultAsync(TestContext.Current.CancellationToken);
            newToken.Should().NotBeNull();
            newToken.Should().NotBe(refreshCommand.RefreshToken);
            newToken.UserId.Should().Be(user.Id);
            newToken.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
        });
    }

    [Fact]
    public async Task Handler_ShouldRemoveExpiredRefreshTokens()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _) = await RegisterUserHandlerTests.RegisterUserAsync(thisApp);
        RefreshTokenEntity token = null!;
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            token = new Faker<RefreshTokenEntity>().ValidInstance(user.Id).Generate();
            await db.RefreshTokens.AddAsync(token, TestContext.Current.CancellationToken);
            await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        });
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            List<RefreshTokenEntity> tokens = new Faker<RefreshTokenEntity>().ValidInstance(user.Id).MakeExpired().Generate(5);
            await db.RefreshTokens.AddRangeAsync(tokens, TestContext.Current.CancellationToken);
            await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        });
        RefreshAccessTokenCommand refreshCommand = new(token.RefreshToken);

        //Act
        using HttpResponseMessage response = await HttpClientJsonExtensions.PutAsJsonAsync(
            thisApp.HttpClient,
            Path,
            refreshCommand,
            TestContext.Current.CancellationToken
        );

        //Assert
        response.Should().Be200Ok();
        RefreshAccessTokenResponse? result = await response.Content.ReadFromJsonAsync<RefreshAccessTokenResponse>(TestContext.Current.CancellationToken);
        result.Should().NotBeNull();
        result.Email.Should().Be(user.Email);
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            RefreshTokenEntity? newToken = await db.RefreshTokens.AsNoTracking().Include(e => e.User).SingleOrDefaultAsync(TestContext.Current.CancellationToken);
            newToken.Should().NotBeNull();
            newToken.Should().NotBe(refreshCommand.RefreshToken);
            newToken.UserId.Should().Be(user.Id);
            newToken.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
        });
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenRefreshTokenNotFound()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _) = await RegisterUserHandlerTests.RegisterUserAsync(thisApp);
        RefreshTokenEntity token = null!;
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            token = new Faker<RefreshTokenEntity>().ValidInstance(user.Id).Generate();
            await db.RefreshTokens.AddAsync(token, TestContext.Current.CancellationToken);
            await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        });
        RefreshAccessTokenCommand refreshCommand = new Faker<RefreshAccessTokenCommand>().ValidInstance().Generate();

        //Act
        using HttpResponseMessage response = await HttpClientJsonExtensions.PutAsJsonAsync(
            thisApp.HttpClient,
            Path,
            refreshCommand,
            TestContext.Current.CancellationToken
        );

        //Assert
        response.Should().Be404NotFound();
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenRefreshTokenExpired()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _) = await RegisterUserHandlerTests.RegisterUserAsync(thisApp);
        RefreshTokenEntity token = null!;
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            token = new Faker<RefreshTokenEntity>().ValidInstance(user.Id).MakeExpired().Generate();
            await db.RefreshTokens.AddAsync(token, TestContext.Current.CancellationToken);
            await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        });
        RefreshAccessTokenCommand refreshCommand = new(token.RefreshToken);

        //Act
        using HttpResponseMessage response = await HttpClientJsonExtensions.PutAsJsonAsync(
            thisApp.HttpClient,
            Path,
            refreshCommand,
            TestContext.Current.CancellationToken
        );

        //Assert
        response.Should().Be400BadRequest();
    }
    #endregion
}