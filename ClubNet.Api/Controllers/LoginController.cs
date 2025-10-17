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
        public IActionResult Registro(RegisterDTO usuario)
        {
            var result = _loginService.Register(usuario);
            if(result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpPost("Login")]
        public IActionResult Login(LoginDTO usuario)
        {
            var result= _loginService.Login(usuario);
            if(result.Success)
                return Ok(result);
            else
                return Unauthorized(result);
        }
    }
}
