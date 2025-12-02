using MAPI.Model;
using MAPI.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using MAPI.IServices;
using MAPI.Model.DTO;

namespace MAPI.Controllers
{
    //[Route("api/[UsersAuth]")]
    [Route("api/users")]
    [ApiController]
    
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepo;
        public UsersController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            var loginResponse = await _userRepo.Login(model);
            if (loginResponse.User == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                return BadRequest("Username or password is incorrect");
            }
            return Ok(loginResponse);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterationRequestDTO model)
        {
            bool ifUserNameUnique = _userRepo.IsUniqueUser(model.UserName);
            if (!ifUserNameUnique)
            {
                return BadRequest("Username already exists");
            }

            var user = await _userRepo.Register(model);
            if (user == null)
            {
                return BadRequest("Error while registering");
            }
            return Ok(user);
        }

    }
}
