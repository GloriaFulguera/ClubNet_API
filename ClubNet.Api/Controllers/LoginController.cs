using ClubNet.Models;
using ClubNet.Models.DTO;
using ClubNet.Services.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ClubNet.Api.Controllers
{
    [Route("api/login")]
    [ApiController]
    public class LoginController:ControllerBase
    {
        private readonly ILoginRepository _loginService;
        public LoginController(ILoginRepository loginService)
        {
            _loginService = loginService;
        }
        [HttpPost("Register")]
        public async Task<ApiResponse> Registro(RegisterDTO usuario)
        {
            return await Task.Run(() => _loginService.Register(usuario));
        }
    }
}
