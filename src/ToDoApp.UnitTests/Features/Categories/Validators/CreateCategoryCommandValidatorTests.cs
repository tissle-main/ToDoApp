//using Bogus;
//using ToDoApp.Web.Shared.Fakers;
//using FluentValidation.TestHelper;
//using ToDoApp.Web.Features.Categories.Dtos;
//using ToDoApp.Web.Features.Categories.Handlers;
//using ToDoApp.Web.Features.Categories.Validators;

//namespace ToDoApp.UnitTests.Features.Categories.Validators;

//public sealed class CreateCategoryCommandValidatorTests
//{
//    public CreateCategoryCommandValidator Validator { get; } = new();

//    [Fact]
//    public async ValueTask Validator_ShouldPass_WhenCommandIsValid()
//    {
//        //Arrange
//        CategoryDto dto = new Faker<CategoryDto>().ValidInstance();
//        CreateCategoryCommand command = new(dto);

//        //Act
//        TestValidationResult<CreateCategoryCommand> result = Validator.TestValidate(command);

//        //Assert
//        result.ShouldNotHaveAnyValidationErrors();
//    }

//    [Fact]
//    public async ValueTask Validator_ShouldNotPass_WhenDtoIsInvalid()
//    {
//        //Arrange
//        CategoryDto dto = new Faker<CategoryDto>().ValidInstance().WithEmptyName().WithTooLargeName();
//        CreateCategoryCommand command = new(dto);

//        //Act
//        TestValidationResult<CreateCategoryCommand> result = Validator.TestValidate(command);

//        //Assert
//        result.ShouldHaveValidationErrors();
//    }
//}