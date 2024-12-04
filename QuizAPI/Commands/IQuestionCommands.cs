using Microsoft.AspNetCore.Mvc;
using QuizAPI.Models;

namespace QuizAPI.Commands
{
    public interface IQuestionCommands
    {
        public Task<IResult> Insert(Question question);
        public Task<IResult> Update(Question question);
        public Task<IResult> Delete(int questionId);

    }
}
