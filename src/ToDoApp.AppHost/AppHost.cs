using ToDoApp.AppHost;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);
IResourceBuilder<SqlServerServerResource> sqlserver = builder.AddSqlServer(ToDoAppResources.SqlServer).WithDataVolume();
IResourceBuilder<SqlServerDatabaseResource> database = sqlserver.AddDatabase(ToDoAppResources.Database);
IResourceBuilder<ProjectResource> webapi = builder.AddProject<Projects.ToDoApp_WebAPI>(ToDoAppResources.WebAPI)
    .WithReference(database).WaitFor(database)
    .WithExternalHttpEndpoints();
builder.AddViteApp(ToDoAppResources.UI, "../ToDoApp.UI", "start")
    .WithReference(webapi).WaitFor(webapi)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints();
await builder.Build().RunAsync();