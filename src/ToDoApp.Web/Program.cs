using ToDoApp.Web;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.AddCore();

WebApplication app = builder.Build();
app.MapDefaultEndpoints();
app.UseHttpsRedirection();
app.UseCore();
await app.RunAsync();