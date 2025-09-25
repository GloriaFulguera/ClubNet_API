using ClubNet.Models;
using ClubNet.Models.DTO;

namespace ClubNet.Services.Repositories
{
    public interface ILoginRepository
    {
        //public Task<LoginResultDTO> Login(LoginDTO login);
        public Task<ApiResponse> Register(RegisterDTO register);
    }
}
