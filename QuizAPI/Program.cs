using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using QuizAPI.AdminManager;
using QuizAPI.Commands;
using QuizAPI.DbContext;
using QuizAPI.Dto;
using QuizAPI.GameManager;
using QuizAPI.Repository;
using QuizAPI.Services;
using QuizAPI.StatisticManager;
using QuizAPI.UserAccessor;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:4200", "https://localhost:7501")
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
            if (!string.IsNullOrEmpty(accessToken))
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
builder.Services.AddHttpClient("Auth", x => x.BaseAddress = new Uri(builder.Configuration.GetValue<string>("AuthAPI")!));
builder.Services.AddScoped<ApplicationDbContext>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IQuestionCommands, QuestionCommands>();
builder.Services.AddScoped<IActiveGameSessionsRepository, ActiveGameSessionsRepository>();
builder.Services.AddScoped<IActiveGameSessionsCommands, ActiveGameSessionsCommands>();
builder.Services.AddScoped<ITempGameSessionCommands, TempGameSessionCommands>();
builder.Services.AddScoped<ITempGameSessionRepository, TempGameSessionRepository>();
builder.Services.AddScoped<ICashedGameSessions, CashedGameSessions>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<ICahedGameSessionRepository, CahedGameSessionRepository>();
builder.Services.AddScoped<IStatisticManager, StatisticManager>();
builder.Services.AddScoped<IGameManager, GameManager>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IUserAccessor, HttpUserAccessor>();
builder.Services.AddScoped<IAdminManager, AdminManager>();

var app = builder.Build();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
await context.CreateModel();

var gameManager = scope.ServiceProvider.GetRequiredService<IGameManager>();
var activeGameSession = scope.ServiceProvider.GetRequiredService<IActiveGameSessionsCommands>();
var tempGameSessionCommands = scope.ServiceProvider.GetRequiredService<ITempGameSessionCommands>();
var statisticsManager = scope.ServiceProvider.GetRequiredService<IStatisticManager>();
var adminManager = scope.ServiceProvider.GetRequiredService<IAdminManager>();


//Clear all temp game tables
await tempGameSessionCommands.DropTempTables();
//Clear all active game sessions
await activeGameSession.TruncateActiveGameSession();
app.MapGet("/", () => "This is Quiz API!");

#region Question Endpoints for Admin Operation
app.MapGet("/admin/GetAllQuestions", async () => await adminManager.GetAllQuestions()).RequireAuthorization();
app.MapGet("/admin/GetQuestionDetails/{id}", async (int id) => await adminManager.GetQuestionDetails(id)).RequireAuthorization();
app.MapPost("/admin/PostQuestion", async (QuestionExtendedDto question) => await adminManager.PostNewQuestion(question)).RequireAuthorization();
app.MapPut("/admin/UpdateQuestion/{id}", async (QuestionExtendedDto question, int id) => await adminManager.UpdateQuestion(id, question)).RequireAuthorization();
app.MapDelete("/admin/DeleteQuestion/{id}", async (int id) => await adminManager.DeleteQuestion(id));
#endregion

#region Question Endpoints for Game Participants
app.MapGet("/GetNextQuestion",  async () => await gameManager.GetNextQuestion()).RequireAuthorization();
app.MapGet("/GetGameSession", async (int numberOfQuestions) => await gameManager.GetGameSession(numberOfQuestions)).RequireAuthorization();
app.MapGet("/RestartGameSession", async () => await gameManager.GetGameSession()).RequireAuthorization();
app.MapGet("CheckForActiveGameSession", async () => await gameManager.CheckForActiveGameSession()).RequireAuthorization();
app.MapPost("/CloseGameSession", async () => await gameManager.CloseGameSession()).RequireAuthorization();
app.MapPost("/CheckCorrectAnswer", async(AnswerDto answerDto) =>  await gameManager.CheckCorrectAnswer(answerDto)).RequireAuthorization();
#endregion

#region Endpoints for Statistics Queries
app.MapGet("/GetLastPlayedGame", async () => await statisticsManager.GetLastPlayedGame()).RequireAuthorization();
app.MapGet("/GetAllPlayedGames", async () => await statisticsManager.GetAllPlayedGames()).RequireAuthorization();
app.MapGet("/GetGameSessionStats", async (string gameSessionId) => await statisticsManager.GetGameSessionStats(gameSessionId)).RequireAuthorization();
#endregion

app.Run();
