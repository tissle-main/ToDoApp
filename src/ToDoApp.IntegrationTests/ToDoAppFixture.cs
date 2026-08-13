using Respawn;
using Projects;
using ToDoApp.Data;
using Polly.Timeout;
using ToDoApp.AppHost;
using ToDoApp.IntegrationTests;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

[assembly: AssemblyFixture(typeof(ToDoAppFixture))]

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
    public async ValueTask<T> ExecuteDbContextAsync<T>(Func<AppDbContext, ValueTask<T>> func)
    {
        await using AppDbContext context = new(DbOptions);
        return await func(context);
    }
    public async ValueTask ResetDatabaseAsync()
    {
        TestContext.Current.CancellationToken.ThrowIfCancellationRequested();
        await using SqlConnection connection = new(ConnectionString);
        await connection.OpenAsync();
        await Respawner.ResetAsync(connection);
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
            catch(SqlException)
            {
                continue;
            }
            catch(TimeoutRejectedException)
            {
                continue;
            }
        }
    }
    public async ValueTask DisposeAsync()
    {
        await ResetDatabaseAsync();
        await Application.DisposeAsync();
    }
    #endregion
}
public sealed class ToDoAppApplication(params string[] args) : DistributedApplicationFactory(typeof(ToDoApp_AppHost), args);