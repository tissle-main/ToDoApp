using AwesomeAssertions;
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Web.Features.Auth.Handlers;

namespace ToDoApp.IntegrationTests.Features.Auth;

public sealed class DeleteUserHandlerTests(ToDoAppFixture thisApp)
{
    #region Static
    public const string Path = "/auth/delete";

    public static async ValueTask<HttpResponseMessage> AuthorizedDeleteAsync(ToDoAppFixture app, string accessToken)
    {
        HttpRequestMessage deleteRequest = new(HttpMethod.Delete, Path);
        deleteRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return await app.HttpClient.SendAsync(deleteRequest, TestContext.Current.CancellationToken);
    }
    #endregion

    #region Instance
    [Fact]
    public async Task Handle_ShouldDeleteUserCascading()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, string password) = await RegisterUserHandlerTests.RegisterUserAsync(thisApp);
        LoginUserResponse loginResponse = await LoginUserHandlerTests.LoginUserAsync(thisApp, user.Email!, password);

        //Act
        using HttpResponseMessage response = await AuthorizedDeleteAsync(thisApp, loginResponse.AccessToken);

        //Assert
        response.Should().Be204NoContent();
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            bool anyUsers = await db.Users.AsNoTracking().AnyAsync(TestContext.Current.CancellationToken);
            anyUsers.Should().BeFalse();
            bool anyRefreshTokens = await db.RefreshTokens.AsNoTracking().AnyAsync(TestContext.Current.CancellationToken);
            anyRefreshTokens.Should().BeFalse();
        });
    }
    #endregion
}