using Bogus;
using FluentValidation.TestHelper;
using ToDoApp.Data.Features.Tasks;
using ToDoApp.Web.Features.Tasks.Dtos;

namespace ToDoApp.UnitTests.Features.Tasks.Dtos;

public sealed class TaskDtoValidatorTests
{
    public TaskDtoValidator Validator { get; } = new();

    [Fact]
    public async ValueTask Validator_ShouldPass_WhenDtoIsValid()
    {
        //Arrange
        TaskDto dto = new Faker<TaskEntity>().ValidInstance().Generate().ToDto();

        //Act
        TestValidationResult<TaskDto> result = Validator.TestValidate(dto);

        //Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async ValueTask Validator_ShouldNotPass_WhenTitleIsEmpty()
    {
        //Arrange
        TaskDto dto = new Faker<TaskEntity>().ValidInstance().WithEmptyTitle().Generate().ToDto();

        //Act
        TestValidationResult<TaskDto> result = Validator.TestValidate(dto);

        //Assert
        result.ShouldHaveValidationErrorFor(dto => dto.Title);
    }

    [Fact]
    public async ValueTask Validator_ShouldNotPass_WhenTitleIsTooLarge()
    {
        //Arrange
        TaskDto dto = new Faker<TaskEntity>().ValidInstance().WithTooLargeTitle().Generate().ToDto();

        //Act
        TestValidationResult<TaskDto> result = Validator.TestValidate(dto);

        //Assert
        result.ShouldHaveValidationErrorFor(dto => dto.Title);
    }

    [Fact]
    public async ValueTask Validator_ShouldNotPass_WhenDescriptionIsTooLarge()
    {
        //Arrange
        TaskDto dto = new Faker<TaskEntity>().ValidInstance().WithTooLargeDescription().Generate().ToDto();

        //Act
        TestValidationResult<TaskDto> result = Validator.TestValidate(dto);

        //Assert
        result.ShouldHaveValidationErrorFor(dto => dto.Description);
    }
}