using Bogus;
using ToDoApp.Web.Shared.Fakers;
using FluentValidation.TestHelper;
using ToDoApp.Data.Features.Categories;
using ToDoApp.Web.Features.Categories.Dtos;
using ToDoApp.Web.Features.Categories.Handlers.CreateCategory;

namespace ToDoApp.UnitTests.Features.Categories.Validators;

public sealed class CreateCategoryCommandValidatorTests
{
    public CreateCategoryCommandValidator Validator { get; } = new();

    [Fact]
    public async ValueTask Validator_ShouldPass_WhenCommandIsValid()
    {
        //Arrange
        CategoryDto dto = new Faker<CategoryEntity>().ValidInstance(default).Generate().ToDto();
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
        CategoryDto dto = new Faker<CategoryEntity>().ValidInstance(default).WithEmptyName().Generate().ToDto();
        CreateCategoryCommand command = new(dto);

        //Act
        TestValidationResult<CreateCategoryCommand> result = Validator.TestValidate(command);

        //Assert
        result.ShouldHaveValidationErrors();
    }
}