using Projects;
using Aspire.Hosting;

namespace ToDoApp.IntegrationTests;

public sealed class ToDoAppApplication(params string[] args) : DistributedApplicationFactory(typeof(ToDoApp_AppHost), args);