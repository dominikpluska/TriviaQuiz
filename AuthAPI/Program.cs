using AuthAPI.Commands;
using AuthAPI.DbContext;
using AuthAPI.Dto;
using AuthAPI.JwtGenerator;
using AuthAPI.Repository;
using AuthAPI.UserManager;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<ApplicationDbContext>();
builder.Services.AddScoped<ICreateJwtToken, CreateJwtToken>();
builder.Services.AddScoped<IAccountsRepository, AccountsRepository>();
builder.Services.AddScoped<IAccountsCommands, AccountsCommands>();
builder.Services.AddScoped<IJwtCommands, JwtCommands>();
builder.Services.AddScoped<IUserManager, UserManager>();

var app = builder.Build();

using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
await context.CreateModel();


var userManager = scope.ServiceProvider.GetRequiredService<IUserManager>();

app.MapGet("/", () => "This is Auth API!");

#region UserEndpoints
app.MapPost("/Register", async (UserDto userDto) => await userManager.RegisterNewUser(userDto));
app.MapPost("/Login", async (UserLoginDto userLoginDto) => await userManager.Login(userLoginDto));
#endregion

app.Run();
