using QuizAPI.Commands;
using QuizAPI.DbContext;
using QuizAPI.Models;
using QuizAPI.Repository;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<ApplicationDbContext>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IQuestionCommands, QuestionCommands>();
builder.Services.AddScoped<IGameSessionRepository, GameSessionRepository>();
var app = builder.Build();

using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
await context.CreateModel();

var questionRepository = scope.ServiceProvider.GetRequiredService<IQuestionRepository>();
var questionCommands = scope.ServiceProvider.GetRequiredService<IQuestionCommands>();

var gameSessionRepository = scope.ServiceProvider.GetRequiredService<IGameSessionRepository>();


app.MapGet("/", () => "Hello World!");

#region Question Endpoints for Admin Operation
app.MapGet("/GetAllQuestions", async () => await questionRepository.GetAllQuestions());
app.MapPost("/PostQuestion", async (Question question) => await questionCommands.Insert(question));
app.MapPut("/UpdateQuestion/{id}", async (int id, Question question) => await questionCommands.Update(question));
app.MapDelete("/DeleteQuestion/{id}", async (int id) => await questionCommands.Delete(id));
#endregion

#region Question Endpoints for Game Participants
//User request a game session with a valid session string / id. Then the game is returned to the user. The rest is handled by a game manager which is going to keep track of 
//how many questions there are left / what is the score etc. in memory. At the end the result is saved to the database.
app.MapGet("/GetGameSession", async () => await gameSessionRepository.GetGameSession());
#endregion

app.Run();
