using Bogus;
using ToDoApp.Web.Shared.Fakers;
using FluentValidation.TestHelper;
using ToDoApp.Web.Features.Auth.Handlers;
using ToDoApp.Web.Features.Auth.Handlers.LoginUser;

namespace ToDoApp.UnitTests.Features.Auth.Validators;

public sealed class LoginUserCommandValidatorTests
{
    public LoginUserCommandValidator Validator { get; } = new();

    [Fact]
    public async ValueTask Validator_ShouldPass_WhenCommandIsValid()
    {
        //Arrange
        LoginUserCommand command = new Faker<LoginUserCommand>().ValidInstance().Generate();

        //Act
        TestValidationResult<LoginUserCommand> result = await Validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async ValueTask Validator_ShouldNotPass_WhenEmailIsInvalid()
    {
        //Arrange
        LoginUserCommand command = new Faker<LoginUserCommand>().ValidInstance().WithInvalidEmail().Generate();

        //Act
        TestValidationResult<LoginUserCommand> result = await Validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        result.ShouldHaveValidationErrorFor(c => c.Email);
    }

    [Fact]
    public async ValueTask Validator_ShouldNotPass_WhenPasswordTooShort()
    {
        //Arrange
        LoginUserCommand command = new Faker<LoginUserCommand>().ValidInstance().WithTooShortPassword().Generate();

        //Act
        TestValidationResult<LoginUserCommand> result = await Validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        result.ShouldHaveValidationErrorFor(c => c.Password);
    }

    [Fact]
    public async ValueTask Validator_ShouldNotPass_WhenPasswordDoNotContainUppercaseLetters()
    {
        //Arrange
        LoginUserCommand command = new Faker<LoginUserCommand>().ValidInstance().WithPasswordWithoutUppercaseLetters().Generate();

        //Act
        TestValidationResult<LoginUserCommand> result = await Validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        result.ShouldHaveValidationErrorFor(c => c.Password);
    }

    [Fact]
    public async ValueTask Validator_ShouldNotPass_WhenPasswordDoNotContainLowercaseLetters()
    {
        //Arrange
        LoginUserCommand command = new Faker<LoginUserCommand>().ValidInstance().WithPasswordWithoutLowercaseLetters().Generate();

        //Act
        TestValidationResult<LoginUserCommand> result = await Validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        result.ShouldHaveValidationErrorFor(c => c.Password);
    }

    [Fact]
    public async ValueTask Validator_ShouldNotPass_WhenPasswordDoNotContainDigits()
    {
        //Arrange
        LoginUserCommand command = new Faker<LoginUserCommand>().ValidInstance().WithPasswordWithoutDigits().Generate();

        //Act
        TestValidationResult<LoginUserCommand> result = await Validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        result.ShouldHaveValidationErrorFor(c => c.Password);
    }
}