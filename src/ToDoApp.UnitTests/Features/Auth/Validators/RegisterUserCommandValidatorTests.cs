using Bogus;
using ToDoApp.Web.Shared.Fakers;
using FluentValidation.TestHelper;
using ToDoApp.Web.Features.Auth.Handlers;
using ToDoApp.Web.Features.Auth.Validators;

namespace ToDoApp.UnitTests.Features.Auth.Validators;

public sealed class RegisterUserCommandValidatorTests
{
    public RegisterUserCommandValidator Validator { get; } = new();

    [Fact]
    public async Task Validator_ShouldPass_WhenCommandIsValid()
    {
        //Arrange
        RegisterUserCommand command = new Faker<RegisterUserCommand>().ValidInstance().Generate();

        //Act
        TestValidationResult<RegisterUserCommand> result = await Validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validator_ShouldNotPass_WhenEmailIsInvalid()
    {
        //Arrange
        RegisterUserCommand command = new Faker<RegisterUserCommand>().ValidInstance().WithInvalidEmail().Generate();

        //Act
        TestValidationResult<RegisterUserCommand> result = await Validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        result.ShouldHaveValidationErrorFor(c => c.Email);
    }

    [Fact]
    public async Task Validator_ShouldNotPass_WhenPasswordTooShort()
    {
        //Arrange
        RegisterUserCommand command = new Faker<RegisterUserCommand>().ValidInstance().WithTooShortPassword().Generate();

        //Act
        TestValidationResult<RegisterUserCommand> result = await Validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        result.ShouldHaveValidationErrorFor(c => c.Password);
    }

    [Fact]
    public async Task Validator_ShouldNotPass_WhenPasswordDoNotContainUppercaseLetters()
    {
        //Arrange
        RegisterUserCommand command = new Faker<RegisterUserCommand>().ValidInstance().WithPasswordWithoutUppercaseLetters().Generate();

        //Act
        TestValidationResult<RegisterUserCommand> result = await Validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        result.ShouldHaveValidationErrorFor(c => c.Password);
    }

    [Fact]
    public async Task Validator_ShouldNotPass_WhenPasswordDoNotContainLowercaseLetters()
    {
        //Arrange
        RegisterUserCommand command = new Faker<RegisterUserCommand>().ValidInstance().WithPasswordWithoutLowercaseLetters().Generate();

        //Act
        TestValidationResult<RegisterUserCommand> result = await Validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        result.ShouldHaveValidationErrorFor(c => c.Password);
    }

    [Fact]
    public async Task Validator_ShouldNotPass_WhenPasswordDoNotContainDigits()
    {
        //Arrange
        RegisterUserCommand command = new Faker<RegisterUserCommand>().ValidInstance().WithPasswordWithoutDigits().Generate();

        //Act
        TestValidationResult<RegisterUserCommand> result = await Validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        result.ShouldHaveValidationErrorFor(c => c.Password);
    }
}