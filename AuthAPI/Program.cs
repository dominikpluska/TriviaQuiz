using AuthAPI.AdminManger;
using AuthAPI.Commands;
using AuthAPI.CookieGenerator;
using AuthAPI.DbContext;
using AuthAPI.Dto;
using AuthAPI.JwtGenerator;
using AuthAPI.Repository;
using AuthAPI.UserManager;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AuthAPI.UserAccessor;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:4200", "https://localhost:7500")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration.GetValue<string>("JwtSettings:Issuer")!,
        ValidateIssuer = true,
        ValidateLifetime = true,
        ValidAudience = builder.Configuration.GetValue<string>("JwtSettings:Audience")!,
        ValidateAudience = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JwtSettings:TokenString")!)),
        ValidateIssuerSigningKey = true
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Request.Cookies.TryGetValue("TriviaQuiz", out var accessToken);
            if(!string.IsNullOrEmpty(accessToken))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine($"Token validated for user: {context.Principal?.Identity?.Name}");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();
builder.Services.AddScoped<ApplicationDbContext>();
builder.Services.AddScoped<ICreateJwtToken, CreateJwtToken>();
builder.Services.AddScoped<IAccountsRepository, AccountsRepository>();
builder.Services.AddScoped<IAccountsCommands, AccountsCommands>();
builder.Services.AddScoped<IUserManager, UserManager>();
builder.Services.AddScoped<IAdminManager, AdminManager>();
builder.Services.AddScoped<ICookieGenerator, CookieGenerator>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IUserAccessor, HttpUserAccessor>();

var app = builder.Build();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
await context.CreateModel();

var userManager = scope.ServiceProvider.GetRequiredService<IUserManager>();
var adminManager = scope.ServiceProvider.GetRequiredService<IAdminManager>();

app.MapGet("/", () => "This is Auth API!");

#region AdminEndpoints
app.MapGet(("/admin/GetAllUsers"), async () =>  await adminManager.GetAllUsers()).RequireAuthorization();
app.MapGet(("/admin/GetUser/{id}"), async (int id) => await adminManager.GetUserById(id)).RequireAuthorization();
app.MapPost(("/admin/AddNewUser"), async (UserDto userDto) => await adminManager.AddNewUser(userDto)).RequireAuthorization();
//This needs to be changed!
app.MapPost(("/admin/UpdateUser"), async (UserDto user) => await adminManager.UpdateUser(user)).RequireAuthorization();
app.MapPost(("/admin/ChangeUserPassword"), async (SetNewPasswordDto setNewPasswordDto) => await adminManager.ChangeUserPassword(setNewPasswordDto.UserId, setNewPasswordDto.Password)).RequireAuthorization();
app.MapDelete(("/admin/DeleteUser/{id}"), async (int id) => await adminManager.DeleteUser(id)).RequireAuthorization();
app.MapDelete(("/admin/DeactivateUse/{id}"), async (int id) => await adminManager.DeactivateUser(id)).RequireAuthorization();
#endregion

#region UserEndpoints
app.MapPost("/Register", async (UserDto userDto) => await userManager.RegisterNewUser(userDto));
app.MapPost("/Login", async (UserLoginDto userLoginDto) => await userManager.Login(userLoginDto));
app.MapPost("/ChangeUserPassword", async (ChangePasswordDto changePasswordDto) => await userManager.ChangePassword(changePasswordDto)).RequireAuthorization();
app.MapPost("/ChangeUserNameAndEmail", async (UserNameAndMailDto userNameAndMailDto) => await userManager.ChangeUserNameAndEmail(userNameAndMailDto)).RequireAuthorization();
app.MapGet("/AuthCheck", () => userManager.CheckAuthentication()).RequireAuthorization();
app.MapGet("/LogOut", () => userManager.Logout()).RequireAuthorization();
app.MapGet("/GetUser", (string userName) => userManager.GetUser(userName)).RequireAuthorization();
app.MapGet("/GetUserNameAndMail", () => userManager.GetUserNameAndMail()).RequireAuthorization();
#endregion

app.Run();
