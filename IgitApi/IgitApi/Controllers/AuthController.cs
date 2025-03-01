using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IgitApi.Data.Entities;
using IgitApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using LoginRequest = Microsoft.AspNetCore.Identity.Data.LoginRequest;
using RegisterRequest = Microsoft.AspNetCore.Identity.Data.RegisterRequest;

namespace IgitApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    IConfiguration configuration)
    : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest model)
    {
        var user = new User { UserName = model.Email, Email = model.Email };
        var result = await userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            if (await userManager.FindByEmailAsync(user.Email) == null)
            {
                await userManager.AddToRoleAsync(user, "User");
            }

            await signInManager.SignInAsync(user, isPersistent: false);
            return Ok(await GenerateJwtToken(user));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return BadRequest(ModelState);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest model)
    {
        var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false,
            lockoutOnFailure: false);

        if (result.Succeeded)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            return Ok(await GenerateJwtToken(user));
        }

        ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
        return BadRequest(ModelState);
    }

    private async Task<AuthenticationResponse> GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var roles = await userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var jwtSettings = configuration.GetSection("Jwt");
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
        var creds = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddDays(1); // Token expiration time

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new AuthenticationResponse
        {
            Id = user.Id.ToString(), Email = user.Email, Token = new JwtSecurityTokenHandler().WriteToken(token)
        };
    }
}
