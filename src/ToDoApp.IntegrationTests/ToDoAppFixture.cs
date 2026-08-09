using Respawn;
using ToDoApp.Data;
using ToDoApp.AppHost;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ToDoApp.IntegrationTests;

public sealed class ToDoAppFixture : IAsyncLifetime
{
    #region Instance
    private ToDoAppApplication Application { get; } = new();
    private string ConnectionString { get; set; } = null!; //Init after InitializedAsync
    private DbContextOptions<AppDbContext> DbOptions { get; set; } = null!; //Init after InitializedAsync
    private Respawner Respawner { get; set; } = null!; //Init after InitializedAsync
    public HttpClient HttpClient { get; private set; } = null!; //Init after InitializedAsync

    public async ValueTask ExecuteDbContextAsync(Func<AppDbContext, ValueTask> func)
    {
        await using AppDbContext context = new(DbOptions);
        await func(context);
    }
    public async ValueTask ResetDatabaseAsync()
    {
        while(true)
        {
            try
            {
                TestContext.Current.CancellationToken.ThrowIfCancellationRequested();
                await using SqlConnection connection = new(ConnectionString);
                await connection.OpenAsync();
                await Respawner.ResetAsync(connection);
                return;
            }
            catch { }
        }
    }
    #endregion

    #region Interfaces
    public async ValueTask InitializeAsync()
    {
        while(true)
        {
            try
            {
                TestContext.Current.CancellationToken.ThrowIfCancellationRequested();
                await Application.StartAsync(TestContext.Current.CancellationToken);
                HttpClient = Application.CreateHttpClient(AppResources.Web);
                ConnectionString = await Application.GetConnectionString(AppResources.Database) ?? throw new NullReferenceException("ConnectionString is null");
                DbOptions = new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(ConnectionString).Options;
                await ExecuteDbContextAsync(async db =>
                {
                    await db.Database.MigrateAsync(TestContext.Current.CancellationToken);
                });

                await using SqlConnection connection = new(ConnectionString);
                await connection.OpenAsync();
                Respawner = await Respawner.CreateAsync(connection, new RespawnerOptions()
                {
                    DbAdapter = DbAdapter.SqlServer,
                    TablesToIgnore = ["__EFMigrationsHistory"]
                });
                return;
            }
            catch { }
        }
    }
    public async ValueTask DisposeAsync()
    {
        await ResetDatabaseAsync();
        await Application.DisposeAsync();
    }
    #endregion
}