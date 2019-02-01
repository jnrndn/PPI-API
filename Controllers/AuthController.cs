using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PPI.API.Data;
using PPI.API.Dtos;
using PPI.API.Models;

namespace PPI.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _auth;
        private readonly IConfiguration _config;

        public AuthController(IAuthRepository auth, IConfiguration config)
        {
            _auth = auth;
            _config = config;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto userToRegister)
        {
            userToRegister.Username = userToRegister.Username.ToLower();
            if (await _auth.UserExist(userToRegister.Username))
                return BadRequest("Username already exist");

            var userToCreate = new User
            {
                Username = userToRegister.Username
            };

            var createdUser = await _auth.Register(userToCreate, userToRegister.Password);

            return StatusCode(201);
        }
        
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto userToLogin)
        {
            var user = await _auth.Login(userToLogin.Username.ToLower(), userToLogin.Password);

            if (user == null)
                return Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username), 
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var secTokenDesc = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now).AddMinutes(20),
                SigningCredentials = cred,
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(secTokenDesc);
            
            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
            });
        }

        [AllowAnonymous]
        [HttpPost("recover")]
        public async Task<IActionResult> Recover(UserRecoverDto userRecover)
        {

            if (!await _auth.Recover(userRecover.Email.ToLower()))
                return BadRequest("Email doesn't exist");
            
            return Ok();
        }
    }
}
