using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using QuizAPI.Commands;
using QuizAPI.DbContext;
using QuizAPI.Dto;
using QuizAPI.GameManager;
using QuizAPI.Models;
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
        ValidateIssuer = true,
        ValidateAudience = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JwtSettings:TokenString")!))
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Request.Cookies.TryGetValue("Token", out var accessToken);
            if (!string.IsNullOrEmpty(accessToken))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();
builder.Services.AddHttpClient("Auth", x => x.BaseAddress = new Uri("https://localhost:7501"));
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

var app = builder.Build();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
await context.CreateModel();


var questionRepository = scope.ServiceProvider.GetRequiredService<IQuestionRepository>();
var questionCommands = scope.ServiceProvider.GetRequiredService<IQuestionCommands>();
var gameManager = scope.ServiceProvider.GetRequiredService<IGameManager>();
var activeGameSession = scope.ServiceProvider.GetRequiredService<IActiveGameSessionsCommands>();
var tempGameSessionCommands = scope.ServiceProvider.GetRequiredService<ITempGameSessionCommands>();
var statisticsManager = scope.ServiceProvider.GetRequiredService<IStatisticManager>();


//Clear all temp game tables
await tempGameSessionCommands.DropTempTables();
//Clear all active game sessions
await activeGameSession.TruncateActiveGameSession();
app.MapGet("/", () => "This is Quiz API!");

#region Question Endpoints for Admin Operation
app.MapGet("/GetAllQuestions", async () => await questionRepository.GetAllQuestions());
app.MapPost("/PostQuestion", async (Question question) => await questionCommands.Insert(question));
app.MapPut("/UpdateQuestion/{id}", async (int id, Question question) => await questionCommands.Update(question));
app.MapDelete("/DeleteQuestion/{id}", async (int id) => await questionCommands.Delete(id));
#endregion

#region Question Endpoints for Game Participants
//User request a game session with a valid session string / id. Then the game is returned to the user. The rest is handled by a game manager which is going to keep track of 
//how many questions there are left / what is the score etc. in memory. At the end the result is saved to the database.
app.MapGet("/GetNextQuestion",  async () => await gameManager.GetNextQuestion());
app.MapGet("/GetGameSession", async (int numberOfQuestions) => await gameManager.GetGameSession(numberOfQuestions));
app.MapGet("/RestartGameSession", async () => await gameManager.GetGameSession());

app.MapGet("CheckForActiveGameSession", async () => await gameManager.CheckForActiveGameSession());
app.MapPost("/CloseGameSession", async () => await gameManager.CloseGameSession());
app.MapPost("/CheckCorrectAnswer", async(AnswerDto answerDto) =>  await gameManager.CheckCorrectAnswer(answerDto));
#endregion

#region Endpoints for Statistics Queries
app.MapGet("/GetLastPlayedGame", async () => await statisticsManager.GetLastPlayedGame());
app.MapGet("/GetAllPlayedGames", async () => await statisticsManager.GetAllPlayedGames());
#endregion

app.Run();
