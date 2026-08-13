//using Bogus;
//using ToDoApp.Web.Shared.Fakers;
//using FluentValidation.TestHelper;
//using ToDoApp.Web.Features.Tasks.Dtos;
//using ToDoApp.Web.Features.Tasks.Handlers;
//using ToDoApp.Web.Features.Tasks.Validators;

//namespace ToDoApp.UnitTests.Features.Tasks.Validators;

//public sealed class UpdateTaskCommandValidatorTests
//{
//    public UpdateTaskCommandValidator Validator { get; } = new();

//    [Fact]
//    public async ValueTask Validator_ShouldPass_WhenCommandIsValid()
//    {
//        //Arrange
//        TaskDto dto = new Faker<TaskDto>().ValidInstance();
//        UpdateTaskCommand command = new(dto);

//        //Act
//        TestValidationResult<UpdateTaskCommand> result = Validator.TestValidate(command);

//        //Assert
//        result.ShouldNotHaveAnyValidationErrors();
//    }

//    [Fact]
//    public async ValueTask Validator_ShouldNotPass_WhenDtoIsInvalid()
//    {
//        //Arrange
//        TaskDto dto = new Faker<TaskDto>().ValidInstance().WithEmptyTitle().WithTooLargeTitle().WithTooLargeDescription();
//        UpdateTaskCommand command = new(dto);

//        //Act
//        TestValidationResult<UpdateTaskCommand> result = Validator.TestValidate(command);

//        //Assert
//        result.ShouldHaveValidationErrors();
//    }
//}