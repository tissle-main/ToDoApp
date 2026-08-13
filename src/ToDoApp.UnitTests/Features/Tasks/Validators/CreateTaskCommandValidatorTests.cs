//using Bogus;
//using ToDoApp.Web.Shared.Fakers;
//using FluentValidation.TestHelper;
//using ToDoApp.Web.Features.Tasks.Dtos;
//using ToDoApp.Web.Features.Tasks.Handlers;
//using ToDoApp.Web.Features.Tasks.Validators;

//namespace ToDoApp.UnitTests.Features.Tasks.Validators;

//public sealed class CreateTaskCommandValidatorTests
//{
//    public CreateTaskCommandValidator Validator { get; } = new();

//    [Fact]
//    public async ValueTask Validator_ShouldPass_WhenCommandIsValid()
//    {
//        //Arrange
//        TaskDto dto = new Faker<TaskDto>().ValidInstance();
//        CreateTaskCommand command = new(dto);

//        //Act
//        TestValidationResult<CreateTaskCommand> result = Validator.TestValidate(command);

//        //Assert
//        result.ShouldNotHaveAnyValidationErrors();
//    }

//    [Fact]
//    public async ValueTask Validator_ShouldNotPass_WhenDtoIsInvalid()
//    {
//        //Arrange
//        TaskDto dto = new Faker<TaskDto>().ValidInstance().WithEmptyTitle().WithTooLargeTitle().WithTooLargeDescription();
//        CreateTaskCommand command = new(dto);

//        //Act
//        TestValidationResult<CreateTaskCommand> result = Validator.TestValidate(command);

//        //Assert
//        result.ShouldHaveValidationErrors();
//    }
//}