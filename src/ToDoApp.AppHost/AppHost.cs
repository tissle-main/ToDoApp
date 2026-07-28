var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.ToDoApp_WebAPI>("todoapp-webapi");

builder.Build().Run();
