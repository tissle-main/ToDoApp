//using AwesomeAssertions;
//using ToDoApp.Data.Features.Auth.Users;
//using ToDoApp.Web.Features.Auth.Handlers;
//using ToDoApp.IntegrationTests.Features.Auth;

//namespace ToDoApp.IntegrationTests.Features.Categories;

//public sealed class GetCategoriesHandlerTests(ToDoAppFixture thisApp)
//{
//    #region Static
//    public const string Url = "/category";
//    #endregion

//    #region Instance
//    [Fact]
//    public async ValueTask Handler_ShouldReturnAllCategories_WhenIdsIsNotProvided()
//    {
//        //Arrange
//        await thisApp.ResetDatabaseAsync();
//        (UserEntity user, string password) = await RegisterUserHandlerTests.RegisterUserAsync(thisApp);
//        LoginUserResponse loginResponse = await LoginUserHandlerTests.LoginUserAsync(thisApp, user.Email!, password);

//    }

//    [Fact]
//    public async ValueTask Handler_ShouldFail_WhenUnauthorized()
//    {
//        //Arrange
//        await thisApp.ResetDatabaseAsync();

//        //Act
//        using HttpResponseMessage response = await thisApp.HttpClient.GetAsync(Url, TestContext.Current.CancellationToken);

//        //Assert
//        response.Should().Be401Unauthorized();
//    }
//    #endregion
//}