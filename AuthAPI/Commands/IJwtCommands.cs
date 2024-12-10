using AuthAPI.Models;

namespace AuthAPI.Commands
{
    public interface IJwtCommands
    {
        public Task<IResult> Insert(Jwt jwt);
    }
}
