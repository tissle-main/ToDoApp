using Bogus;
using FluentValidation.TestHelper;
using ToDoApp.Data.Features.Tasks;
using ToDoApp.Web.Features.Tasks.Dtos;
using ToDoApp.Web.Features.Tasks.Handlers.UpdateTask;

namespace ToDoApp.UnitTests.Features.Tasks.Handlers.UpdateTask;

public sealed class UpdateTaskCommandValidatorTests
{
    public UpdateTaskCommandValidator Validator { get; } = new();

    [Fact]
    public async ValueTask Validator_ShouldPass_WhenCommandIsValid()
    {
        //Arrange
        TaskDto dto = new Faker<TaskEntity>().ValidInstance().Generate().ToDto();
        UpdateTaskCommand command = new(dto);

        //Act
        TestValidationResult<UpdateTaskCommand> result = Validator.TestValidate(command);

        //Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async ValueTask Validator_ShouldNotPass_WhenDtoIsInvalid()
    {
        //Arrange
        TaskDto dto = new Faker<TaskEntity>().ValidInstance().WithEmptyTitle().WithTooLargeTitle().WithTooLargeDescription().Generate().ToDto();
        UpdateTaskCommand command = new(dto);

        //Act
        TestValidationResult<UpdateTaskCommand> result = Validator.TestValidate(command);

        //Assert
        result.ShouldHaveValidationErrors();
    }
}