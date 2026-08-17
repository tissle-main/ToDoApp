using Bogus;
using AwesomeAssertions;
using Microsoft.EntityFrameworkCore;
using ToDoApp.IntegrationTests.Seeders;
using ToDoApp.Data.Features.Auth.RefreshTokens;
using ToDoApp.Web.Features.Auth.Dtos.RefreshTokens;
using ToDoApp.Web.Features.Auth.Handlers.RemoveExpiredRefreshTokens;

namespace ToDoApp.IntegrationTests.Features.Auth.Handlers.RemoveExpiredRefreshTokens;

public sealed class RemoveExpiredRefreshTokensHandlerTests(ToDoAppFixture thisApp)
{
    [Fact]
    public async ValueTask Handler_ShouldRemoveAllExpiredRefreshTokens()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        await thisApp.AddUsersAsync();
        int expectedTokenCount = await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<RefreshTokenEntity>().ValidInstance().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken);
            int expectedTokenCount = await db.RefreshTokens.CountAsync(TestContext.Current.CancellationToken);
            await new Faker<RefreshTokenEntity>().ValidInstance().MakeExpired().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken);
            return expectedTokenCount;
        });

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendRemoveExpiredRefreshTokensAsync(TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be204NoContent();
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            bool expiredTokensExist = await db.RefreshTokens.AnyAsync(e => DateTime.UtcNow > e.ExpiresAt, TestContext.Current.CancellationToken);
            expiredTokensExist.Should().BeFalse();

            int actualTokenCount = await db.RefreshTokens.CountAsync(TestContext.Current.CancellationToken);
            actualTokenCount.Should().Be(expectedTokenCount);
        });
    }
}