using Bogus;
using AwesomeAssertions;
using ToDoApp.Data.Features.Tasks;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Web.Features.Tasks.Dtos;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Data.Features.Categories;
using ToDoApp.IntegrationTests.Seeders;
using ToDoApp.Web.Features.Categories.Dtos;
using ToDoApp.Data.Features.Auth.RefreshTokens;
using ToDoApp.Web.Features.Auth.Dtos.RefreshTokens;
using ToDoApp.Web.Features.Auth.Handlers.DeleteUser;

namespace ToDoApp.IntegrationTests.Features.Auth.Handlers.DeleteUser;

public sealed class DeleteUserHandlerTests(ToDoAppFixture thisApp)
{
    [Fact]
    public async ValueTask Handler_ShouldDeleteUserCascading()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _, string accessToken) = await thisApp.AddUsers2AndLoginRandomAsync();
        (
            int userCount,
            int refreshTokenCount,
            int userRefreshTokenCount,
            int categoryCount,
            int userCategoryCount,
            int taskCount,
            int userTaskCount
        ) = await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<RefreshTokenEntity>().ValidInstance().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken);
            await new Faker<CategoryEntity>().ValidInstance().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken);
            await new Faker<TaskEntity>().ValidInstance().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken);
            int userCount = await db.Users.CountAsync(TestContext.Current.CancellationToken);
            int refreshTokenCount = await db.RefreshTokens.CountAsync(TestContext.Current.CancellationToken);
            int userRefreshTokenCount = await db.RefreshTokens.Where(e => e.UserId == user.Id).CountAsync(TestContext.Current.CancellationToken);
            int categoryCount = await db.Categories.CountAsync(TestContext.Current.CancellationToken);
            int userCategoryCount = await db.Categories.Where(e => e.UserId == user.Id).CountAsync(TestContext.Current.CancellationToken);
            int taskCount = await db.Tasks.CountAsync(TestContext.Current.CancellationToken);
            int userTaskCount = await db.Tasks.Where(e => e.UserId == user.Id).CountAsync(TestContext.Current.CancellationToken);
            return (userCount, refreshTokenCount, userRefreshTokenCount, categoryCount, userCategoryCount, taskCount, userTaskCount);
        });

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendDeleteUserAsync(accessToken, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be204NoContent();
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            bool userExists = await db.Users.AnyAsync(e => e.Id == user.Id, TestContext.Current.CancellationToken);
            userExists.Should().BeFalse();
            int newUserCount = await db.Users.CountAsync(TestContext.Current.CancellationToken);
            newUserCount.Should().Be(userCount - 1);

            bool userRefreshTokensExist = await db.RefreshTokens.AnyAsync(e => e.UserId == user.Id, TestContext.Current.CancellationToken);
            userRefreshTokensExist.Should().BeFalse();
            int newRefreshTokenCount = await db.RefreshTokens.CountAsync(TestContext.Current.CancellationToken);
            newRefreshTokenCount.Should().Be(refreshTokenCount - userRefreshTokenCount);

            bool userCategoriesExist = await db.Categories.AnyAsync(e => e.UserId == user.Id, TestContext.Current.CancellationToken);
            userCategoriesExist.Should().BeFalse();
            int newCategoryCount = await db.Categories.CountAsync(TestContext.Current.CancellationToken);
            newCategoryCount.Should().Be(categoryCount - userCategoryCount);

            bool userTasksExist = await db.Tasks.AnyAsync(e => e.UserId == user.Id, TestContext.Current.CancellationToken);
            userTasksExist.Should().BeFalse();
            int newTaskCount = await db.Tasks.CountAsync(TestContext.Current.CancellationToken);
            newTaskCount.Should().Be(taskCount - userTaskCount);
        });
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenUnauthorized()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendDeleteUserAsync("", TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be401Unauthorized();
    }
}