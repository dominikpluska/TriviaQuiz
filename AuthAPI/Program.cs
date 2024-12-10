using AuthAPI.DbContext;
using AuthAPI.JwtGenerator;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<ApplicationDbContext>();
builder.Services.AddScoped<ICreateJwtToken, CreateJwtToken>();
var app = builder.Build();

using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
await context.CreateModel();

app.MapGet("/", () => "This is Auth API!");

app.Run();
