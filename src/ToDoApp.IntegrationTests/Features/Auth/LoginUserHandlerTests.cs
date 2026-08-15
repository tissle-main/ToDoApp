using Bogus;
using AwesomeAssertions;
using System.Net.Http.Json;
using ToDoApp.Web.Shared.Fakers;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Web.Features.Auth.Dtos;
using ToDoApp.IntegrationTests.Seeders;
using ToDoApp.Web.Features.Auth.Handlers;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Data.Features.Auth.RefreshTokens;
using ToDoApp.Web.Features.Auth.Handlers.LoginUser;

namespace ToDoApp.IntegrationTests.Features.Auth;

public sealed class LoginUserHandlerTests(ToDoAppFixture thisApp)
{
    [Fact]
    public async ValueTask Handler_ShouldCreateRefreshToken()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, string password) = await thisApp.AddUsers2AndPickRandom();
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<RefreshTokenEntity>().ValidInstance(default).SeedDatabaseForAllUsers(db, TestContext.Current.CancellationToken);
        });
        LoginUserCommand command = new(user.Email!, password);

        //Act
        using HttpResponseMessage response = await thisApp.HttpClient.SendLoginUserAsync(command, TestContext.Current.CancellationToken);

        //Assert
        response.Should().Be200Ok();
        LoginUserResponse? result = await response.Content.ReadFromJsonAsync<LoginUserResponse>(TestContext.Current.CancellationToken);
        result.Should().NotBeNull();
        result.Email.Should().Be(command.Email).And.Be(user.Email);
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            RefreshTokenEntity? token = await db.RefreshTokens.AsNoTracking().SingleOrDefaultAsync(
                e => e.UserId == user.Id && e.Value == result.RefreshToken.Value,
                TestContext.Current.CancellationToken
            );
            token.Should().NotBeNull();
            token.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
            result.RefreshToken.Should().BeEquivalentTo(token.ToDto());
        });
    }

    [Fact]
    public async ValueTask Handler_ShouldRemoveExpiredRefreshTokens()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, string password) = await thisApp.AddUsers2AndPickRandom();
        LoginUserCommand command = new(user.Email!, password);
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<RefreshTokenEntity>().ValidInstance(default).MakeExpired().SeedDatabaseForAllUsers(db, TestContext.Current.CancellationToken);
            await new Faker<RefreshTokenEntity>().ValidInstance(default).SeedDatabaseForAllUsers(db, TestContext.Current.CancellationToken);
        });

        //Act
        using HttpResponseMessage response = await thisApp.HttpClient.SendLoginUserAsync(command, TestContext.Current.CancellationToken);

        //Assert
        response.Should().Be200Ok();
        LoginUserResponse? result = await response.Content.ReadFromJsonAsync<LoginUserResponse>(TestContext.Current.CancellationToken);
        result.Should().NotBeNull();
        result.Email.Should().Be(command.Email).And.Be(user.Email);
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            RefreshTokenEntity? token = await db.RefreshTokens.AsNoTracking().SingleOrDefaultAsync(
                e => e.UserId == user.Id && e.Value == result.RefreshToken.Value,
                TestContext.Current.CancellationToken
            );
            token.Should().NotBeNull();
            token.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
            result.RefreshToken.Should().BeEquivalentTo(token.ToDto());
            
            int expiredCount = await db.RefreshTokens.AsNoTracking().CountAsync(e => DateTime.UtcNow > e.ExpiresAt, TestContext.Current.CancellationToken);
            expiredCount.Should().Be(0);
        });
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenUserNotFound()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (_, string password) = await thisApp.AddUsers2AndPickRandom();
        LoginUserCommand command = new Faker<LoginUserCommand>().ValidInstance().WithPassword(password).Generate();

        //Act
        using HttpResponseMessage response = await thisApp.HttpClient.SendLoginUserAsync(command, TestContext.Current.CancellationToken);

        //Assert
        response.Should().Be404NotFound();
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenPasswordInvalid()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _) = await thisApp.AddUsers2AndPickRandom();
        LoginUserCommand command = new Faker<LoginUserCommand>().ValidInstance().WithEmail(user.Email!).Generate();

        //Act
        using HttpResponseMessage response = await thisApp.HttpClient.SendLoginUserAsync(command, TestContext.Current.CancellationToken);

        //Assert
        response.Should().Be400BadRequest();
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenEmailIsInvalid()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        LoginUserCommand command = new Faker<LoginUserCommand>().ValidInstance().WithInvalidEmail();

        //Act
        using HttpResponseMessage response = await thisApp.HttpClient.SendLoginUserAsync(command, TestContext.Current.CancellationToken);

        //Assert
        response.Should().Be422UnprocessableEntity();
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenPasswordTooShord()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        LoginUserCommand command = new Faker<LoginUserCommand>().ValidInstance().WithTooShortPassword();

        //Act
        using HttpResponseMessage response = await thisApp.HttpClient.SendLoginUserAsync(command, TestContext.Current.CancellationToken);

        //Assert
        response.Should().Be422UnprocessableEntity();
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenPasswordDoNotContainUppercaseLetters()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        LoginUserCommand command = new Faker<LoginUserCommand>().ValidInstance().WithPasswordWithoutUppercaseLetters();

        //Act
        using HttpResponseMessage response = await thisApp.HttpClient.SendLoginUserAsync(command, TestContext.Current.CancellationToken);

        //Assert
        response.Should().Be422UnprocessableEntity();
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenPasswordDoNotContainLowercaseLetters()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        LoginUserCommand command = new Faker<LoginUserCommand>().ValidInstance().WithPasswordWithoutLowercaseLetters();

        //Act
        using HttpResponseMessage response = await thisApp.HttpClient.SendLoginUserAsync(command, TestContext.Current.CancellationToken);

        //Assert
        response.Should().Be422UnprocessableEntity();
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenPasswordDoNotContainDigits()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        LoginUserCommand command = new Faker<LoginUserCommand>().ValidInstance().WithPasswordWithoutDigits();

        //Act
        using HttpResponseMessage response = await thisApp.HttpClient.SendLoginUserAsync(command, TestContext.Current.CancellationToken);

        //Assert
        response.Should().Be422UnprocessableEntity();
    }
}