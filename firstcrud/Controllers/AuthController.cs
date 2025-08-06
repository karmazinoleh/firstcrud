using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using firstcrud.Controllers.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace firstcrud.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(UserDbContext context, ILogger<ProductController> logger, IConfiguration configuration) : ControllerBase
{
    private readonly UserDbContext _context = context;

    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(UserDto request) // todo: remove returning of User
    {
        User user = new User();
        
        var hashPassword = new PasswordHasher<User>()
            .HashPassword(user, request.Password);
        
        user.Username = request.Username;
        user.PasswordHash = hashPassword;
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return Ok(user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(UserDto request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == request.Username);
        if (user == null 
            && new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
        {
            return BadRequest("Username or password is incorrect");
        }

        string token = CreateToken(user);
        return Ok(token);
    }

    private string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username)
        };
        
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")));
        
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
        
        var tokenDesciptor = new JwtSecurityToken(
            issuer: configuration.GetValue<string>("AppSettings:Issuer"),
            audience: configuration.GetValue<string>("AppSettings:Audience"),
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds
        );
        
        return new JwtSecurityTokenHandler().WriteToken(tokenDesciptor);
    }
    
}