using Bogus;
using AwesomeAssertions;
using System.Net.Http.Json;
using ToDoApp.Web.Shared.Fakers;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Web.Features.Auth.Handlers;
using ToDoApp.Data.Features.Auth.RefreshTokens;

namespace ToDoApp.IntegrationTests.Features.Auth;

public sealed class LoginUserHandlerTests(ToDoAppFixture thisApp)
{
    #region Static
    public const string Path = "/auth/login";

    public static async ValueTask<LoginUserResponse> LoginUserAsync(ToDoAppFixture app, string email, string password)
    {
        LoginUserCommand loginCommand = new(email, password);
        using HttpResponseMessage response = await HttpClientJsonExtensions.PostAsJsonAsync(
            app.HttpClient,
            Path,
            loginCommand,
            TestContext.Current.CancellationToken
        );
        response.Should().Be200Ok();
        LoginUserResponse? loginResponse = await response.Content.ReadFromJsonAsync<LoginUserResponse>(TestContext.Current.CancellationToken);
        loginResponse.Should().NotBeNull();
        return loginResponse;
    }
    #endregion

    #region Instance
    [Fact]
    public async Task Handler_ShouldGenerateTokens()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, string password) = await RegisterUserHandlerTests.RegisterUserAsync(thisApp);
        LoginUserCommand loginCommand = new(user.Email!, password);

        //Act
        using HttpResponseMessage response = await HttpClientJsonExtensions.PostAsJsonAsync(
            thisApp.HttpClient,
            Path,
            loginCommand,
            TestContext.Current.CancellationToken
        );

        //Assert
        response.Should().Be200Ok();
        LoginUserResponse? loginResponse = await response.Content.ReadFromJsonAsync<LoginUserResponse>(TestContext.Current.CancellationToken);
        loginResponse.Should().NotBeNull();
        loginResponse.Email.Should().Be(loginCommand.Email).And.Be(user.Email);
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            RefreshTokenEntity? token = await db.RefreshTokens.AsNoTracking().SingleOrDefaultAsync(TestContext.Current.CancellationToken);
            token.Should().NotBeNull();
            token.RefreshToken.Should().Be(loginResponse.RefreshToken);
            token.UserId.Should().Be(user.Id);
            token.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
        });
    }

    [Fact]
    public async Task Handler_ShouldRemoveExpiredRefreshTokens()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, string password) = await RegisterUserHandlerTests.RegisterUserAsync(thisApp);
        LoginUserCommand loginCommand = new(user.Email!, password);
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            List<RefreshTokenEntity> tokens = new Faker<RefreshTokenEntity>().ValidInstance(user.Id).MakeExpired().Generate(5);
            await db.RefreshTokens.AddRangeAsync(tokens, TestContext.Current.CancellationToken);
            await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        });

        //Act
        using HttpResponseMessage response = await HttpClientJsonExtensions.PostAsJsonAsync(
            thisApp.HttpClient,
            Path,
            loginCommand,
            TestContext.Current.CancellationToken
        );

        //Assert
        response.Should().Be200Ok();
        LoginUserResponse? loginResponse = await response.Content.ReadFromJsonAsync<LoginUserResponse>(TestContext.Current.CancellationToken);
        loginResponse.Should().NotBeNull();
        loginResponse.Email.Should().Be(loginCommand.Email).And.Be(user.Email);
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            RefreshTokenEntity? token = await db.RefreshTokens.AsNoTracking().SingleOrDefaultAsync(TestContext.Current.CancellationToken);
            token.Should().NotBeNull();
            token.RefreshToken.Should().Be(loginResponse.RefreshToken);
            token.UserId.Should().Be(user.Id);
            token.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
        });
    }

    [Fact]
    public async Task Handler_ShouldFail_WhenUserNotFound()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (_, string password) = await RegisterUserHandlerTests.RegisterUserAsync(thisApp);
        LoginUserCommand command = new Faker<LoginUserCommand>().ValidInstance().WithPassword(password).Generate();

        //Act
        using HttpResponseMessage response = await HttpClientJsonExtensions.PostAsJsonAsync(
            thisApp.HttpClient,
            Path,
            command,
            TestContext.Current.CancellationToken
        );

        //Assert
        response.Should().Be404NotFound();
    }

    [Fact]
    public async Task Handler_ShouldFail_WhenPasswordInvalid()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _) = await RegisterUserHandlerTests.RegisterUserAsync(thisApp);
        LoginUserCommand command = new Faker<LoginUserCommand>().ValidInstance().WithEmail(user.Email!).Generate();

        //Act
        using HttpResponseMessage response = await HttpClientJsonExtensions.PostAsJsonAsync(
            thisApp.HttpClient,
            Path,
            command,
            TestContext.Current.CancellationToken
        );

        //Assert
        response.Should().Be400BadRequest();
    }

    [Fact]
    public async Task Handler_ShouldFail_WhenEmailIsInvalid()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        LoginUserCommand command = new Faker<LoginUserCommand>().ValidInstance().WithInvalidEmail();

        //Act
        using HttpResponseMessage response = await HttpClientJsonExtensions.PostAsJsonAsync(
            thisApp.HttpClient,
            Path,
            command,
            TestContext.Current.CancellationToken
        );

        //Assert
        response.Should().Be422UnprocessableEntity();
    }

    [Fact]
    public async Task Handler_ShouldFail_WhenPasswordTooShord()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        LoginUserCommand command = new Faker<LoginUserCommand>().ValidInstance().WithTooShortPassword();

        //Act
        using HttpResponseMessage response = await HttpClientJsonExtensions.PostAsJsonAsync(
            thisApp.HttpClient,
            Path,
            command,
            TestContext.Current.CancellationToken
        );

        //Assert
        response.Should().Be422UnprocessableEntity();
    }

    [Fact]
    public async Task Handler_ShouldFail_WhenPasswordDoNotContainUppercaseLetters()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        LoginUserCommand command = new Faker<LoginUserCommand>().ValidInstance().WithPasswordWithoutUppercaseLetters();

        //Act
        using HttpResponseMessage response = await HttpClientJsonExtensions.PostAsJsonAsync(
            thisApp.HttpClient,
            Path,
            command,
            TestContext.Current.CancellationToken
        );

        //Assert
        response.Should().Be422UnprocessableEntity();
    }

    [Fact]
    public async Task Handler_ShouldFail_WhenPasswordDoNotContainLowercaseLetters()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        LoginUserCommand command = new Faker<LoginUserCommand>().ValidInstance().WithPasswordWithoutLowercaseLetters();

        //Act
        using HttpResponseMessage response = await HttpClientJsonExtensions.PostAsJsonAsync(
            thisApp.HttpClient,
            Path,
            command,
            TestContext.Current.CancellationToken
        );

        //Assert
        response.Should().Be422UnprocessableEntity();
    }

    [Fact]
    public async Task Handler_ShouldFail_WhenPasswordDoNotContainDigits()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        LoginUserCommand command = new Faker<LoginUserCommand>().ValidInstance().WithPasswordWithoutDigits();

        //Act
        using HttpResponseMessage response = await HttpClientJsonExtensions.PostAsJsonAsync(
            thisApp.HttpClient,
            Path,
            command,
            TestContext.Current.CancellationToken
        );

        //Assert
        response.Should().Be422UnprocessableEntity();
    }
    #endregion
}