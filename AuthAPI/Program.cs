using AuthAPI.AdminManger;
using AuthAPI.Commands;
using AuthAPI.DbContext;
using AuthAPI.Dto;
using AuthAPI.JwtGenerator;
using AuthAPI.Models;
using AuthAPI.Repository;
using AuthAPI.UserManager;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<ApplicationDbContext>();
builder.Services.AddScoped<ICreateJwtToken, CreateJwtToken>();
builder.Services.AddScoped<IAccountsRepository, AccountsRepository>();
builder.Services.AddScoped<IAccountsCommands, AccountsCommands>();
builder.Services.AddScoped<IJwtCommands, JwtCommands>();
builder.Services.AddScoped<IUserManager, UserManager>();
builder.Services.AddScoped<IAdminManager, AdminManager>();

var app = builder.Build();

using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
await context.CreateModel();


var userManager = scope.ServiceProvider.GetRequiredService<IUserManager>();
var adminManager = scope.ServiceProvider.GetRequiredService<IAdminManager>();

app.MapGet("/", () => "This is Auth API!");

#region AdminEndpoints
app.MapGet(("/GetAllUsers"), async () =>  await adminManager.GetAllUsers());
app.MapGet(("/GetUser/{id}"), async (int id) => await adminManager.GetUserById(id));
app.MapPost(("/AddNewUser"), async (UserDto userDto) => await adminManager.AddNewUser(userDto));
//This needs to be changed!
app.MapPost(("/UpdateUser"), async (User user) => await adminManager.UpdateUser(user));
app.MapDelete(("/DeleteUser/{id}"), async (int id) => await adminManager.DeleteUser(id));
app.MapDelete(("/DeactivateUse/{id}"), async (int id) => await adminManager.DeactivateUser(id));
#endregion

#region UserEndpoints
app.MapPost("/Register", async (UserDto userDto) => await userManager.RegisterNewUser(userDto));
app.MapPost("/Login", async (UserLoginDto userLoginDto) => await userManager.Login(userLoginDto));
#endregion

app.Run();
