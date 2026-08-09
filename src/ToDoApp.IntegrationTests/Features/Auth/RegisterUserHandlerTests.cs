using Bogus;
using AwesomeAssertions;
using System.Net.Http.Json;
using ToDoApp.Web.Shared.Fakers;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Web.Features.Auth.Handlers;

namespace ToDoApp.IntegrationTests.Features.Auth;

public sealed class RegisterUserHandlerTests(ToDoAppFixture thisApp)
{
    #region Static
    public const string Path = "/auth/register";

    public static async ValueTask<(UserEntity User, string Password)> RegisterUserAsync(ToDoAppFixture app)
    {
        RegisterUserCommand registerCommand = new Faker<RegisterUserCommand>().ValidInstance().Generate();
        using HttpResponseMessage registerResponse = await HttpClientJsonExtensions.PostAsJsonAsync(
            app.HttpClient,
            Path,
            registerCommand,
            TestContext.Current.CancellationToken
        );
        registerResponse.Should().Be204NoContent();
        UserEntity? user = null!;
        await app.ExecuteDbContextAsync(async db =>
        {
            user = await db.Users.AsNoTracking().SingleOrDefaultAsync(e => e.Email == registerCommand.Email, TestContext.Current.CancellationToken);
            user.Should().NotBeNull();
        });
        return (user, registerCommand.Password);
    }
    #endregion

    #region Instance
    [Fact]
    public async Task Handler_ShouldCreateUser()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        RegisterUserCommand command = new Faker<RegisterUserCommand>().ValidInstance().Generate();

        //Act
        using HttpResponseMessage response = await HttpClientJsonExtensions.PostAsJsonAsync(
            thisApp.HttpClient,
            Path,
            command,
            TestContext.Current.CancellationToken
        );

        //Assert
        response.Should().Be204NoContent();
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            UserEntity? user = await db.Users.AsNoTracking().SingleOrDefaultAsync(TestContext.Current.CancellationToken);
            user.Should().NotBeNull();
            user.Should().Match<UserEntity>(user => user.Email == command.Email);
        });
    }

    [Fact]
    public async Task Handler_ShouldFail_WhenUserExists()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, string password) = await RegisterUserAsync(thisApp);
        RegisterUserCommand command = new(user.Email!, password);

        //Act
        using HttpResponseMessage response = await HttpClientJsonExtensions.PostAsJsonAsync(
            thisApp.HttpClient,
            Path,
            command,
            TestContext.Current.CancellationToken
        );

        //Assert
        response.Should().Be409Conflict();
    }

    [Fact]
    public async Task Handler_ShouldFail_WhenEmailIsInvalid()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        RegisterUserCommand command = new Faker<RegisterUserCommand>().ValidInstance().WithInvalidEmail().Generate();

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
        RegisterUserCommand command = new Faker<RegisterUserCommand>().ValidInstance().WithTooShortPassword().Generate();

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
        RegisterUserCommand command = new Faker<RegisterUserCommand>().ValidInstance().WithPasswordWithoutUppercaseLetters().Generate();

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
        RegisterUserCommand command = new Faker<RegisterUserCommand>().ValidInstance().WithPasswordWithoutLowercaseLetters().Generate();

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
        RegisterUserCommand command = new Faker<RegisterUserCommand>().ValidInstance().WithPasswordWithoutDigits().Generate();

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