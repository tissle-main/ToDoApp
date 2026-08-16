using Bogus;
using FluentValidation.TestHelper;
using ToDoApp.Data.Features.Tasks;
using ToDoApp.Web.Features.Tasks.Dtos;
using ToDoApp.Web.Features.Tasks.Handlers.CreateTask;

namespace ToDoApp.UnitTests.Features.Tasks.Handlers.CreateTask;

public sealed class CreateTaskCommandValidatorTests
{
    public CreateTaskCommandValidator Validator { get; } = new();

    [Fact]
    public async ValueTask Validator_ShouldPass_WhenCommandIsValid()
    {
        //Arrange
        TaskDto dto = new Faker<TaskEntity>().ValidInstance().Generate().ToDto();
        CreateTaskCommand command = new(dto);

        //Act
        TestValidationResult<CreateTaskCommand> result = Validator.TestValidate(command);

        //Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async ValueTask Validator_ShouldNotPass_WhenDtoIsInvalid()
    {
        //Arrange
        TaskDto dto = new Faker<TaskEntity>().ValidInstance().WithEmptyTitle().WithTooLargeTitle().WithTooLargeDescription().Generate().ToDto();
        CreateTaskCommand command = new(dto);

        //Act
        TestValidationResult<CreateTaskCommand> result = Validator.TestValidate(command);

        //Assert
        result.ShouldHaveValidationErrors();
    }
}