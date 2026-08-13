//using Bogus;
//using ToDoApp.Web.Shared.Fakers;
//using FluentValidation.TestHelper;
//using ToDoApp.Web.Features.Tasks.Handlers;
//using ToDoApp.Web.Features.Tasks.Validators;

//namespace ToDoApp.UnitTests.Features.Tasks.Validators;

//public sealed class GetTasksByFilterQueryValidatorTests
//{
//    public GetTasksByFilterQueryValidator Validator { get; } = new();

//    [Fact]
//    public async ValueTask Validator_ShouldPass_WhenQueryIsValid()
//    {
//        //Arrange
//        GetTasksByFilterQuery query = new Faker<GetTasksByFilterQuery>().ValidInstance();

//        //Act
//        TestValidationResult<GetTasksByFilterQuery> result = Validator.TestValidate(query);

//        //Assert
//        result.ShouldNotHaveAnyValidationErrors();
//    }

//    [Fact]
//    public async ValueTask Validator_ShouldPass_WhenSkipIsNull()
//    {
//        //Arrange
//        GetTasksByFilterQuery query = new Faker<GetTasksByFilterQuery>().ValidInstance().WithNoSkip();

//        //Act
//        TestValidationResult<GetTasksByFilterQuery> result = Validator.TestValidate(query);

//        //Assert
//        result.ShouldNotHaveAnyValidationErrors();
//    }

//    [Fact]
//    public async ValueTask Validator_ShouldPass_WhenTakeIsNull()
//    {
//        //Arrange
//        GetTasksByFilterQuery query = new Faker<GetTasksByFilterQuery>().ValidInstance().WithNoTake();

//        //Act
//        TestValidationResult<GetTasksByFilterQuery> result = Validator.TestValidate(query);

//        //Assert
//        result.ShouldNotHaveAnyValidationErrors();
//    }

//    [Fact]
//    public async ValueTask Validator_ShouldNotPass_WhenSkipIsNegative()
//    {
//        //Arrange
//        GetTasksByFilterQuery query = new Faker<GetTasksByFilterQuery>().ValidInstance().WithNegativeSkip();

//        //Act
//        TestValidationResult<GetTasksByFilterQuery> result = Validator.TestValidate(query);

//        //Assert
//        result.ShouldHaveValidationErrorFor(e => e.Skip);
//    }

//    [Fact]
//    public async ValueTask Validator_ShouldNotPass_WhenTakeIsNegative()
//    {
//        //Arrange
//        GetTasksByFilterQuery query = new Faker<GetTasksByFilterQuery>().ValidInstance().WithNegativeTake();

//        //Act
//        TestValidationResult<GetTasksByFilterQuery> result = Validator.TestValidate(query);

//        //Assert
//        result.ShouldHaveValidationErrorFor(e => e.Take);
//    }

//    [Fact]
//    public async ValueTask Validator_ShouldNotPass_WhenSearchIsTooLarge()
//    {
//        //Arrange
//        GetTasksByFilterQuery query = new Faker<GetTasksByFilterQuery>().ValidInstance().WithTooLargeSearch();

//        //Act
//        TestValidationResult<GetTasksByFilterQuery> result = Validator.TestValidate(query);

//        //Assert
//        result.ShouldHaveValidationErrorFor(e => e.Search);
//    }

//    [Fact]
//    public async ValueTask Validator_ShouldNotPass_WhenCategoryIsTooLarge()
//    {
//        //Arrange
//        GetTasksByFilterQuery query = new Faker<GetTasksByFilterQuery>().ValidInstance().WithTooLargeCategory();

//        //Act
//        TestValidationResult<GetTasksByFilterQuery> result = Validator.TestValidate(query);

//        //Assert
//        result.ShouldHaveValidationErrorFor(e => e.Category);
//    }
//}