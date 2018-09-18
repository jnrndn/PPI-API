using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
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
        private readonly IConfiguration _config;

        public AuthController(IAuthRrepository auth, IConfiguration config)
        {
            _auth = auth;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto userRegister)
        {
            userRegister.Username = userRegister.Username.ToLower();
            if (await _auth.UserExist(userRegister.Username))
                return BadRequest("Username already exist");

            var userToCreate = new User
            {
                Username = userRegister.Username
            };

            var createdUser = await _auth.Register(userToCreate, userRegister.Password);

            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto userLoginDto)
        {
            var user = await _auth.Login(userLoginDto.Username.ToLower(), userLoginDto.Password);

            if (user == null)
                return Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username), 
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

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
        
    }
}
