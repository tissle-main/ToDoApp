using Bogus;
using AwesomeAssertions;
using System.Net.Http.Json;
using ToDoApp.Web.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using ToDoApp.IntegrationTests.Seeders;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Web.Features.Auth.Dtos.Users;
using ToDoApp.Data.Features.Auth.RefreshTokens;
using ToDoApp.Web.Features.Auth.Handlers.LoginUser;
using ToDoApp.Web.Features.Auth.Dtos.RefreshTokens;

namespace ToDoApp.IntegrationTests.Features.Auth.Handlers.LoginUser;

public sealed class LoginUserHandlerTests(ToDoAppFixture thisApp)
{
    [Fact]
    public async ValueTask Handler_ShouldCreateRefreshToken()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, string password) = await thisApp.AddUsers2AndPickRandomAsync();
        LoginUserCommand command = new(user.Email!, password);

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendLoginUserAsync(command, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be200Ok();
        string? refreshTokenValue = message.GetRefreshToken();
        refreshTokenValue.Should().NotBeNull();
        LoginUserResponse? response = await message.Content.ReadFromJsonAsync<LoginUserResponse>(TestContext.Current.CancellationToken);
        response.Should().NotBeNull();
        response.User.Should().BeEquivalentTo(user.ToDto());
        response.User.Email.Should().Be(command.Email);
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            RefreshTokenEntity? refreshToken = await db.RefreshTokens.SingleOrDefaultAsync(TestContext.Current.CancellationToken);
            refreshToken.Should().NotBeNull();
            refreshToken.UserId.Should().Be(user.Id);
            refreshToken.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
            refreshToken.Value.Should().Be(refreshTokenValue);
        });
    }

    [Fact]
    public async ValueTask Handler_ShouldRemoveAllExpiredRefreshTokens()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, string password) = await thisApp.AddUsers2AndPickRandomAsync();
        int expectedTokenCount = await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<RefreshTokenEntity>().ValidInstance().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken);
            int expectedTokenCount = await db.RefreshTokens.CountAsync(TestContext.Current.CancellationToken);
            await new Faker<RefreshTokenEntity>().ValidInstance().MakeExpired().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken);
            return expectedTokenCount;
        }) + 1;
        LoginUserCommand command = new(user.Email!, password);

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendLoginUserAsync(command, TestContext.Current.CancellationToken);

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
    public async ValueTask Handler_ShouldFail_WhenUserNotFound()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (_, string password) = await thisApp.AddUsers2AndPickRandomAsync();
        LoginUserCommand command = new Faker<LoginUserCommand>().ValidInstance().WithPassword(password).Generate();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendLoginUserAsync(command, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be404NotFound();
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenPasswordInvalid()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _) = await thisApp.AddUsers2AndPickRandomAsync();
        LoginUserCommand command = new Faker<LoginUserCommand>().ValidInstance().WithEmail(user.Email!).Generate();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendLoginUserAsync(command, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be400BadRequest();
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenEmailIsInvalid()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        LoginUserCommand command = new Faker<LoginUserCommand>().ValidInstance().WithInvalidEmail();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendLoginUserAsync(command, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be422UnprocessableEntity();
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenPasswordTooShord()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        LoginUserCommand command = new Faker<LoginUserCommand>().ValidInstance().WithTooShortPassword();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendLoginUserAsync(command, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be422UnprocessableEntity();
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenPasswordDoNotContainUppercaseLetters()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        LoginUserCommand command = new Faker<LoginUserCommand>().ValidInstance().WithPasswordWithoutUppercaseLetters();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendLoginUserAsync(command, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be422UnprocessableEntity();
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenPasswordDoNotContainLowercaseLetters()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        LoginUserCommand command = new Faker<LoginUserCommand>().ValidInstance().WithPasswordWithoutLowercaseLetters();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendLoginUserAsync(command, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be422UnprocessableEntity();
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenPasswordDoNotContainDigits()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        LoginUserCommand command = new Faker<LoginUserCommand>().ValidInstance().WithPasswordWithoutDigits();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendLoginUserAsync(command, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be422UnprocessableEntity();
    }
}