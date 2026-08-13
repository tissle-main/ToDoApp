//using Bogus;
//using ToDoApp.Web.Shared.Fakers;
//using FluentValidation.TestHelper;
//using ToDoApp.Web.Features.Categories.Dtos;
//using ToDoApp.Web.Features.Categories.Validators;

//namespace ToDoApp.UnitTests.Features.Categories.Validators;

//public sealed class CategoryDtoValidatorTests
//{
//    public CategoryDtoValidator Validator { get; } = new();

//    [Fact]
//    public async ValueTask Validator_ShouldPass_WhenDtoIsValid()
//    {
//        //Arrange
//        CategoryDto dto = new Faker<CategoryDto>().ValidInstance();

//        //Act
//        TestValidationResult<CategoryDto> result = Validator.TestValidate(dto);

//        //Assert
//        result.ShouldNotHaveAnyValidationErrors();
//    }

//    [Fact]
//    public async ValueTask Validator_ShouldNotPass_WhenNameIsEmpty()
//    {
//        //Arrange
//        CategoryDto dto = new Faker<CategoryDto>().ValidInstance().WithEmptyName();

//        //Act
//        TestValidationResult<CategoryDto> result = Validator.TestValidate(dto);

//        //Assert
//        result.ShouldHaveValidationErrorFor(dto => dto.Name);
//    }

//    [Fact]
//    public async ValueTask Validator_ShouldNotPass_WhenNameIsTooLarge()
//    {
//        //Arrange
//        CategoryDto dto = new Faker<CategoryDto>().ValidInstance().WithTooLargeName();

//        //Act
//        TestValidationResult<CategoryDto> result = Validator.TestValidate(dto);

//        //Assert
//        result.ShouldHaveValidationErrorFor(dto => dto.Name);
//    }
//}