using Bogus;
using FluentValidation.TestHelper;
using ToDoApp.Web.Features.Tasks.Handlers.GetTasksByFilter;

namespace ToDoApp.UnitTests.Features.Tasks.Handlers.GetTasksByFilter;

public sealed class GetTasksByFilterQueryValidatorTests
{
    public GetTasksByFilterQueryValidator Validator { get; } = new();

    [Fact]
    public async ValueTask Validator_ShouldPass_WhenQueryIsValid()
    {
        //Arrange
        GetTasksByFilterQuery query = new Faker<GetTasksByFilterQuery>().ValidInstance().WithSkip(0).WithTake(0);

        //Act
        TestValidationResult<GetTasksByFilterQuery> result = Validator.TestValidate(query);

        //Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async ValueTask Validator_ShouldPass_WhenSkipIsNull()
    {
        //Arrange
        GetTasksByFilterQuery query = new Faker<GetTasksByFilterQuery>().ValidInstance().WithTake(0);

        //Act
        TestValidationResult<GetTasksByFilterQuery> result = Validator.TestValidate(query);

        //Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async ValueTask Validator_ShouldPass_WhenTakeIsNull()
    {
        //Arrange
        GetTasksByFilterQuery query = new Faker<GetTasksByFilterQuery>().ValidInstance().WithSkip(0);

        //Act
        TestValidationResult<GetTasksByFilterQuery> result = Validator.TestValidate(query);

        //Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async ValueTask Validator_ShouldNotPass_WhenSkipIsNegative()
    {
        //Arrange
        GetTasksByFilterQuery query = new Faker<GetTasksByFilterQuery>().ValidInstance().WithNegativeSkip().WithTake(0);

        //Act
        TestValidationResult<GetTasksByFilterQuery> result = Validator.TestValidate(query);

        //Assert
        result.ShouldHaveValidationErrorFor(e => e.Skip);
    }

    [Fact]
    public async ValueTask Validator_ShouldNotPass_WhenTakeIsNegative()
    {
        //Arrange
        GetTasksByFilterQuery query = new Faker<GetTasksByFilterQuery>().ValidInstance().WithSkip(0).WithNegativeTake();

        //Act
        TestValidationResult<GetTasksByFilterQuery> result = Validator.TestValidate(query);

        //Assert
        result.ShouldHaveValidationErrorFor(e => e.Take);
    }
}