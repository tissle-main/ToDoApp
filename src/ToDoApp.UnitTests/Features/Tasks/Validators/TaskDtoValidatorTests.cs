//using Bogus;
//using ToDoApp.Web.Shared.Fakers;
//using FluentValidation.TestHelper;
//using ToDoApp.Web.Features.Tasks.Dtos;
//using ToDoApp.Web.Features.Tasks.Validators;

//namespace ToDoApp.UnitTests.Features.Tasks.Validators;

//public sealed class TaskDtoValidatorTests
//{
//    public TaskDtoValidator Validator { get; } = new();

//    [Fact]
//    public async ValueTask Validator_ShouldPass_WhenDtoIsValid()
//    {
//        //Arrange
//        TaskDto dto = new Faker<TaskDto>().ValidInstance();

//        //Act
//        TestValidationResult<TaskDto> result = Validator.TestValidate(dto);

//        //Assert
//        result.ShouldNotHaveAnyValidationErrors();
//    }

//    [Fact]
//    public async ValueTask Validator_ShouldNotPass_WhenTitleIsEmpty()
//    {
//        //Arrange
//        TaskDto dto = new Faker<TaskDto>().ValidInstance().WithEmptyTitle();

//        //Act
//        TestValidationResult<TaskDto> result = Validator.TestValidate(dto);

//        //Assert
//        result.ShouldHaveValidationErrorFor(dto => dto.Title);
//    }

//    [Fact]
//    public async ValueTask Validator_ShouldNotPass_WhenTitleIsTooLarge()
//    {
//        //Arrange
//        TaskDto dto = new Faker<TaskDto>().ValidInstance().WithTooLargeTitle();

//        //Act
//        TestValidationResult<TaskDto> result = Validator.TestValidate(dto);

//        //Assert
//        result.ShouldHaveValidationErrorFor(dto => dto.Title);
//    }

//    [Fact]
//    public async ValueTask Validator_ShouldNotPass_WhenDescriptionIsTooLarge()
//    {
//        //Arrange
//        TaskDto dto = new Faker<TaskDto>().ValidInstance().WithTooLargeDescription();

//        //Act
//        TestValidationResult<TaskDto> result = Validator.TestValidate(dto);

//        //Assert
//        result.ShouldHaveValidationErrorFor(dto => dto.Description);
//    }
//}