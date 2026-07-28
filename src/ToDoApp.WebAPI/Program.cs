using Serilog;
using ToDoApp.Data;
using FluentValidation;
using ToDoApp.WebAPI.Behaviors;
//using ToDoApp.WebAPI.Extensions;
using ToDoApp.WebAPI.Middleware;
using ToDoApp.WebAPI.Extensions;
using Microsoft.EntityFrameworkCore;

ValidatorOptions.Global.LanguageManager.Enabled = false;
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Host.UseSerilog(static void(HostBuilderContext ctx, IServiceProvider provider, LoggerConfiguration cfg) =>
{
    cfg.WriteTo.Console();
});
//builder.Services.AddMvcCore();
//builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    string? connection_str = builder.Configuration.GetConnectionString("todoapp-database");
    options.UseSqlServer(connection_str, builder =>
    {
        builder.MigrationsAssembly(typeof(AppDbContext).Assembly.GetName().Name);
    });
});
builder.Services.AddMediator(options =>
{
    options.ServiceLifetime = ServiceLifetime.Scoped;
    options.PipelineBehaviors = [typeof(ValidationBehavior<,>)];
});
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddJoinHandlersFromAssembly();

WebApplication app = builder.Build();
app.MapDefaultEndpoints();
if(app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapSwagger();
    app.MapSwaggerUI();
}
using(IServiceScope scope = app.Services.CreateScope())
{
    Console.WriteLine("Applying migrations...");
    await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.MigrateAsync();
    Console.WriteLine("Migrations applied");
}
app.UseSerilogRequestLogging();
app.UseExceptionHandler();
app.UseHttpsRedirection();
//app.MapEndpointsFromAssembly();
app.MapControllers();
await app.RunAsync();