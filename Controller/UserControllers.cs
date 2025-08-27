
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using MyApiProject.Data;
using MyApiProject.Dtos;
using MyApiProject.Models;
using MyApiProject.Repository.Interfaces;
using BCrypt.Net;

namespace MyApiProject.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserControllers : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly IConfiguration _config;

        public UserControllers(IUserRepository userRepo, IConfiguration config)
        {
            _userRepo = userRepo;
            _config = config;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDTO dto)
        {
            var existingUser = await _userRepo.GetByUsernameAsync(dto.UserName);
            if (existingUser != null) return BadRequest("UserName already exist!");

            var user = new User
            {
                UserName = dto.UserName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };
            var addedUser = await _userRepo.AddAsync(user);
            return Ok(new UserResponseDTO { Id = addedUser.Id, UserName = addedUser.UserName });
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDTO dto)
        {
            var user = await _userRepo.GetByUsernameAsync(dto.UserName);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid credentials");
            }
            var tokens = GenerateJwtToken(user);
            return Ok(new { tokens });
        }
        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentail = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var Claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };
            var tokens = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: Claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: credentail
            );

            return new JwtSecurityTokenHandler().WriteToken(tokens);
        }

    }
}