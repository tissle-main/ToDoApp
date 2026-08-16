using Bogus;
using FluentValidation.TestHelper;
using ToDoApp.Data.Features.Categories;
using ToDoApp.Web.Features.Categories.Dtos;
using ToDoApp.Web.Features.Categories.Handlers.CreateCategory;

namespace ToDoApp.UnitTests.Features.Categories.Handlers.CreateCategory;

public sealed class CreateCategoryCommandValidatorTests
{
    public CreateCategoryCommandValidator Validator { get; } = new();

    [Fact]
    public async ValueTask Validator_ShouldPass_WhenCommandIsValid()
    {
        //Arrange
        CategoryDto dto = new Faker<CategoryEntity>().ValidInstance().Generate().ToDto();
        CreateCategoryCommand command = new(dto);

        //Act
        TestValidationResult<CreateCategoryCommand> result = Validator.TestValidate(command);

        //Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async ValueTask Validator_ShouldNotPass_WhenDtoIsInvalid()
    {
        //Arrange
        CategoryDto dto = new Faker<CategoryEntity>().ValidInstance().WithEmptyName().Generate().ToDto();
        CreateCategoryCommand command = new(dto);

        //Act
        TestValidationResult<CreateCategoryCommand> result = Validator.TestValidate(command);

        //Assert
        result.ShouldHaveValidationErrors();
    }
}