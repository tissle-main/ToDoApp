using ToDoApp.AppHost;
using Aspire.Hosting.JavaScript;
using Arshid.Aspire.ApiDocs.Extensions;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);
IResourceBuilder<SqlServerServerResource> sqlserver = builder.AddSqlServer(AppHostConstants.SqlServer).WithDataVolume();
IResourceBuilder<SqlServerDatabaseResource> database = sqlserver.AddDatabase(AppHostConstants.Database);
IResourceBuilder<ProjectResource> web = builder.AddProject<Projects.ToDoApp_Web>(AppHostConstants.Web);
IResourceBuilder<ViteAppResource> ui = builder.AddViteApp(AppHostConstants.UI, "../ToDoApp.UI", "start");

web.WithReference(database).WaitFor(database).WithScalar(true).WithOpenApi(true).WithEnvironment(AppHostConstants.UIOrigin, ui.GetEndpoint("http"));
ui.WithReference(web).WaitFor(web).WithHttpEndpoint(env: "PORT").WithExternalHttpEndpoints();
await builder.Build().RunAsync();