using Microsoft.AspNetCore.Authorization;
using QuizAPI.Commands;
using QuizAPI.Dto;
using QuizAPI.Models;
using QuizAPI.Repository;
using QuizAPI.Services;
using QuizAPI.UserAccessor;
using System.Text.Json;

namespace QuizAPI.AdminManager
{
    public class AdminManager : IAdminManager
    {
        private readonly IQuestionCommands _questionCommands;
        private readonly IQuestionRepository _questionRepository;
        private readonly IUserAccessor _userAccessor;
        private readonly IAuthenticationService _authenticationService;
        public AdminManager(IQuestionCommands questionCommands, IQuestionRepository questionRepository, 
            IAuthenticationService authenticationService, IUserAccessor userAccessor)
        {
            _questionCommands = questionCommands;
            _questionRepository = questionRepository;
            _userAccessor = userAccessor;
            _authenticationService = authenticationService;

        }

        public async Task<IResult> GetAllQuestions() 
        {
            try
            {
                var isAuthorized = await CheckIfUserIsGameMaster();
                if (isAuthorized)
                {
                    var restuls = await _questionRepository.GetAllQuestionsLight();
                    return Results.Ok(restuls);
                }
                else
                {
                    return Results.Unauthorized();
                }
            }
            catch (Exception ex)
            {
                return Results.Problem($"{ex.Message}");
            }

        }

        public async Task<IResult> GetQuestionDetails(int questionId)
        {
            try
            {
                var isAuthorized = await CheckIfUserIsGameMaster();
                if (isAuthorized)
                {
                    var questionDetails = await _questionRepository.GetQuestion(questionId);
                    if (questionDetails == null)
                    {
                        return Results.Problem("Question does not exist!");
                    }
                    return Results.Ok(questionDetails);
                }
                else
                {
                    return Results.Unauthorized();
                }
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }

        }

        public async Task<IResult> UpdateQuestion(int questionId, QuestionExtendedDto question)
        {
            try
            {
                var isAuthorized = await CheckIfUserIsGameMaster();
                if (isAuthorized)
                {
                    await _questionCommands.Update(questionId, question);
                    return Results.Ok("Record Updated");
                }
                else
                {
                    return Results.Unauthorized();
                }

            }
            catch(Exception ex)
            {
                return Results.Problem(ex.Message.ToString());
            }

        }

        public async Task<IResult> PostNewQuestion(QuestionExtendedDto question) 
        {
            try
            {
                var isAuthorized = await CheckIfUserIsGameMaster();
                if (isAuthorized)
                {
                    await _questionCommands.Insert(question);
                    return Results.Ok("Question added!");
                }
                else
                {
                    return Results.Unauthorized();
                }
            }
            catch (Exception ex)
            {
                return Results.Problem($"{ex.Message}");
            }
        }

        public async Task<IResult> DeleteQuestion(int questionId)
        {
            try
            {
                var isAuthorized = await CheckIfUserIsGameMaster();
                if (isAuthorized)
                {
                    await _questionCommands.Delete(questionId);
                    return Results.Ok("Record Deleted!");
                }
                else
                {
                    return Results.Unauthorized();
                }
            }
            catch(Exception ex)
            {
                return Results.Problem(ex.Message.ToString());
            }
        }

        private async Task<bool> CheckIfUserIsGameMaster()
        {
            var user = _userAccessor.UserName;
            var userData = await _authenticationService.GetUser(user);
            UserToDisplayDto userToDisplayDto = JsonSerializer.Deserialize<UserToDisplayDto>(userData)!;

            if (userToDisplayDto.isGameMaster == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
