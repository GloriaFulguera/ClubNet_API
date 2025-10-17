using ClubNet.Models;
using ClubNet.Models.DTO;

namespace ClubNet.Services.Repositories
{
    public interface ILoginRepository
    {
        public ApiResponse<string> Login(LoginDTO login);
        public ApiResponse Register(RegisterDTO register);
    }
}
