using Projects;

namespace ToDoApp.IntegrationTests;

public sealed class ToDoAppFactory(params string[] args) : DistributedApplicationFactory(typeof(ToDoApp_AppHost), args);