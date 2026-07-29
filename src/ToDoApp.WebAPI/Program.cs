//using ToDoApp.WebAPI.Extensions;
using Serilog;
using ToDoApp.Data;
using FluentValidation;
using Microsoft.OpenApi;
using ToDoApp.WebAPI.Behaviors;
using ToDoApp.WebAPI.Extensions;
using ToDoApp.WebAPI.Middleware;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Features.Auth.Roles;
using ToDoApp.Data.Features.Auth.Users;

ValidatorOptions.Global.LanguageManager.Enabled = false;
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Host.UseSerilog(static void (HostBuilderContext ctx, IServiceProvider provider, LoggerConfiguration cfg) =>
{
    cfg.WriteTo.Console();
});

//builder.Services.AddMvcCore();
//builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Task Manager API",
        Version = "v1"
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Введіть JWT токен."
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});
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
builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
}).AddRoles<ApplicationRole>().AddSignInManager().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
builder.AddJwtAuth();

WebApplication app = builder.Build();
app.MapDefaultEndpoints();
if(app.Environment.IsDevelopment())
{
    app.MapSwagger();
    app.MapSwaggerUI();
}
using(IServiceScope scope = app.Services.CreateScope())
{
    Console.WriteLine("Applying migrations...");
    AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    //await dbContext.Database.EnsureDeletedAsync();
    await dbContext.Database.MigrateAsync();
    Console.WriteLine("Migrations applied");
}
app.UseSerilogRequestLogging();
app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
//app.MapEndpointsFromAssembly();
app.MapControllers();
await app.RunAsync();