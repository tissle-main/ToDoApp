using ToDoApp.AppHost;
using Aspire.Hosting.JavaScript;
using Arshid.Aspire.ApiDocs.Extensions;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);
IResourceBuilder<SqlServerServerResource> sql = builder.AddSqlServer(AppResources.Sql);
IResourceBuilder<SqlServerDatabaseResource> database = sql.AddDatabase(AppResources.Database);
IResourceBuilder<ProjectResource> web = builder.AddProject<Projects.ToDoApp_Web>(AppResources.Web);
IResourceBuilder<ViteAppResource> ui = builder.AddViteApp(AppResources.UI, "../ToDoApp.UI", "start");

web.WithReference(database).WaitFor(database).WithScalar(IsHttps: true).WithOpenApi(IsHttps: true);
ui.WithReference(web).WaitFor(web).WithHttpEndpoint(env: "PORT").WithExternalHttpEndpoints();
await builder.Build().RunAsync();