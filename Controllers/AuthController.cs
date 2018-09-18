using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PPI.API.Data;
using PPI.API.Dtos;
using PPI.API.Models;

namespace PPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRrepository _auth;

        public AuthController(IAuthRrepository auth)
        {
            _auth = auth;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegister)
        {
            userForRegister.Username = userForRegister.Username.ToLower();
            if (await _auth.UserExist(userForRegister.Username))
                return BadRequest("Username already exist");

            var userToCreate = new User
            {
                Username = userForRegister.Username
            };

            var createdUser = await _auth.Register(userToCreate, userForRegister.Password);

            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLogin userLogin)
        {
            userLogin.Username = userLogin.Username.ToLower();
            var user = await _auth.Login(userLogin.Username, userLogin.Password);

            return Ok(user);

        }
        
    }
}
