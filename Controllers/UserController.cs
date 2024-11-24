using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using TodoApp_Backend.DTOs;
using TodoApp_Backend.Models;

namespace TodoApp_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public readonly todoappContext context;

        public UserController(todoappContext context)
        {
            this.context = context;
        }
        [HttpPost("Signup")]

        public async Task<ActionResult<string>> Signup([FromBody] SignupDTO signupDTO)  
        {
            var alreadyExist = await context.Users.AnyAsync(user => user.Email ==
            signupDTO.Email);
            if (alreadyExist) 
            {
                return BadRequest("Email aready Exist");
            }
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(signupDTO.Password);    

            var newUser = new User
            {
                Username = signupDTO.Username,
                Email = signupDTO.Email,
                Password = hashedPassword
            };
            context.Users.Add(newUser);
            await context.SaveChangesAsync(); 
             return Ok("User created sucessfuly!");
        }
        [HttpPost("login")]
        public async Task<ActionResult<object>> Login([FromBody] LoginDTO request)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                return BadRequest("Invalid email or password");

            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.UTF8.GetBytes("TopSecretKey11223344556677889900");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, user.Email), new Claim("UserId", user.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Audience = "MyApp",
                Issuer = "MyApp"
            };


            // Create the token
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Return the token to the client
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { token = tokenString, username = user.Username });
        }

    }
}
