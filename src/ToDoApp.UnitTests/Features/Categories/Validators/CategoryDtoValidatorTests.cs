using Bogus;
using ToDoApp.Web.Shared.Fakers;
using FluentValidation.TestHelper;
using ToDoApp.Data.Features.Categories;
using ToDoApp.Web.Features.Categories.Dtos;

namespace ToDoApp.UnitTests.Features.Categories.Validators;

public sealed class CategoryDtoValidatorTests
{
    public CategoryDtoValidator Validator { get; } = new();

    [Fact]
    public async ValueTask Validator_ShouldPass_WhenDtoIsValid()
    {
        //Arrange
        CategoryDto dto = new Faker<CategoryEntity>().ValidInstance(default).Generate().ToDto();

        //Act
        TestValidationResult<CategoryDto> result = Validator.TestValidate(dto);

        //Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async ValueTask Validator_ShouldNotPass_WhenNameIsEmpty()
    {
        //Arrange
        CategoryDto dto = new Faker<CategoryEntity>().ValidInstance(default).WithEmptyName().Generate().ToDto();

        //Act
        TestValidationResult<CategoryDto> result = Validator.TestValidate(dto);

        //Assert
        result.ShouldHaveValidationErrorFor(dto => dto.Name);
    }

    [Fact]
    public async ValueTask Validator_ShouldNotPass_WhenNameIsTooLarge()
    {
        //Arrange
        CategoryDto dto = new Faker<CategoryEntity>().ValidInstance(default).WithTooLargeName().Generate().ToDto();

        //Act
        TestValidationResult<CategoryDto> result = Validator.TestValidate(dto);

        //Assert
        result.ShouldHaveValidationErrorFor(dto => dto.Name);
    }
}