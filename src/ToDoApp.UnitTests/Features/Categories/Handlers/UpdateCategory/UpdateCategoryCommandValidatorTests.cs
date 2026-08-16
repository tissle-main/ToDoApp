using Bogus;
using FluentValidation.TestHelper;
using ToDoApp.Data.Features.Categories;
using ToDoApp.Web.Features.Categories.Dtos;
using ToDoApp.Web.Features.Categories.Handlers.UpdateCategory;

namespace ToDoApp.UnitTests.Features.Categories.Handlers.UpdateCategory;

public sealed class UpdateCategoryCommandValidatorTests
{
    public UpdateCategoryCommandValidator Validator { get; } = new();

    [Fact]
    public async ValueTask Validator_ShouldPass_WhenCommandIsValid()
    {
        //Arrange
        CategoryDto dto = new Faker<CategoryEntity>().ValidInstance().Generate().ToDto();
        UpdateCategoryCommand command = new(dto);

        //Act
        TestValidationResult<UpdateCategoryCommand> result = Validator.TestValidate(command);

        //Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async ValueTask Validator_ShouldNotPass_WhenDtoIsInvalid()
    {
        //Arrange
        CategoryDto dto = new Faker<CategoryEntity>().ValidInstance().WithEmptyName().Generate().ToDto();
        UpdateCategoryCommand command = new(dto);

        //Act
        TestValidationResult<UpdateCategoryCommand> result = Validator.TestValidate(command);

        //Assert
        result.ShouldHaveValidationErrors();
    }
}