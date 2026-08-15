using Bogus;
using AwesomeAssertions;
using ToDoApp.Web.Shared.Fakers;
using Microsoft.EntityFrameworkCore;
using ToDoApp.IntegrationTests.Seeders;
using ToDoApp.Web.Features.Auth.Handlers;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Web.Features.Auth.Handlers.RegisterUser;

namespace ToDoApp.IntegrationTests.Features.Auth;

public sealed class RegisterUserHandlerTests(ToDoAppFixture thisApp)
{
    [Fact]
    public async ValueTask Handler_ShouldCreateUser()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        RegisterUserCommand command = new Faker<RegisterUserCommand>().ValidInstance().Generate();

        //Act
        using HttpResponseMessage response = await thisApp.HttpClient.SendRegisterUserAsync(command, TestContext.Current.CancellationToken);

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
    public async ValueTask Handler_ShouldFail_WhenUserExists()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        RegisterUserCommand command = await thisApp.AddUsersAndPickRandom();

        //Act
        using HttpResponseMessage response = await thisApp.HttpClient.SendRegisterUserAsync(command, TestContext.Current.CancellationToken);

        //Assert
        response.Should().Be409Conflict();
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenEmailIsInvalid()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        RegisterUserCommand command = new Faker<RegisterUserCommand>().ValidInstance().WithInvalidEmail().Generate();

        //Act
        using HttpResponseMessage response = await thisApp.HttpClient.SendRegisterUserAsync(command, TestContext.Current.CancellationToken);

        //Assert
        response.Should().Be422UnprocessableEntity();
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenPasswordTooShord()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        RegisterUserCommand command = new Faker<RegisterUserCommand>().ValidInstance().WithTooShortPassword().Generate();

        //Act
        using HttpResponseMessage response = await thisApp.HttpClient.SendRegisterUserAsync(command, TestContext.Current.CancellationToken);

        //Assert
        response.Should().Be422UnprocessableEntity();
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenPasswordDoNotContainUppercaseLetters()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        RegisterUserCommand command = new Faker<RegisterUserCommand>().ValidInstance().WithPasswordWithoutUppercaseLetters().Generate();

        //Act
        using HttpResponseMessage response = await thisApp.HttpClient.SendRegisterUserAsync(command, TestContext.Current.CancellationToken);

        //Assert
        response.Should().Be422UnprocessableEntity();
    }
    
    [Fact]
    public async ValueTask Handler_ShouldFail_WhenPasswordDoNotContainLowercaseLetters()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        RegisterUserCommand command = new Faker<RegisterUserCommand>().ValidInstance().WithPasswordWithoutLowercaseLetters().Generate();

        //Act
        using HttpResponseMessage response = await thisApp.HttpClient.SendRegisterUserAsync(command, TestContext.Current.CancellationToken);

        //Assert
        response.Should().Be422UnprocessableEntity();
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenPasswordDoNotContainDigits()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        RegisterUserCommand command = new Faker<RegisterUserCommand>().ValidInstance().WithPasswordWithoutDigits().Generate();

        //Act
        using HttpResponseMessage response = await thisApp.HttpClient.SendRegisterUserAsync(command, TestContext.Current.CancellationToken);

        //Assert
        response.Should().Be422UnprocessableEntity();
    }
}