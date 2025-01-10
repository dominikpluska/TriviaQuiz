using Microsoft.AspNetCore.Authorization;
using QuizAPI.Commands;
using QuizAPI.Dto;
using QuizAPI.Models;
using QuizAPI.Repository;
using QuizAPI.Services;
using QuizAPI.UserAccessor;

namespace QuizAPI.AdminManager
{
    public class AdminManager : IAdminManager
    {
        private readonly IQuestionCommands _questionCommands;
        private readonly IQuestionRepository _questionRepository;
 
        public AdminManager(IQuestionCommands questionCommands, IQuestionRepository questionRepository)
        {
            _questionCommands = questionCommands;
            _questionRepository = questionRepository;
        }

        public async Task<IResult> GetAllQuestions() 
        {
            try
            {
                var restuls = await _questionRepository.GetAllQuestionsLight();
                return Results.Ok(restuls);
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
                var questionDetails = await _questionRepository.GetQuestion(questionId);
                if (questionDetails == null)
                {
                    return Results.Problem("Question does not exist!");
                }
                return Results.Ok(questionDetails);
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
                await _questionCommands.Update(questionId, question);
                return Results.Ok("Record Updated");
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
                await _questionCommands.Insert(question);
                return Results.Ok("Question added!");
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
                await _questionCommands.Delete(questionId);
                return Results.Ok("Record Deleted!");
            }
            catch(Exception ex)
            {
                return Results.Problem(ex.Message.ToString());
            }
        }
    }
}
