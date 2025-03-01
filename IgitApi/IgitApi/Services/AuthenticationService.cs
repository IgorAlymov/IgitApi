using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IgitApi.Data.Entities;
using IgitApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace IgitApi.Services;

public class AuthenticationService(
    UserManager<User> userManager,
    RoleManager<Role> roleManager,
    IConfiguration configuration)
    : IAuthenticationService
{
    public async Task<AuthenticationResponse> RegisterAsync(RegisterRequest model)
    {
        var user = new User { UserName = model.Email, Email = model.Email };
        var result = await userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            throw new Exception(string.Join(", ", result.Errors));
        }

        if (await roleManager.FindByNameAsync("User") == null)
        {
            var role = new Role { Name = "User" };
            await roleManager.CreateAsync(role);
        }

        await userManager.AddToRoleAsync(user, "User");
        return await GenerateJwtToken(user);
    }

    public async Task<AuthenticationResponse> LoginAsync(LoginRequest model)
    {
        var user = await userManager.FindByEmailAsync(model.Email);
        if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
        {
            return await GenerateJwtToken(user);
        }

        throw new Exception("Invalid Credentials");
    }

    private async Task<AuthenticationResponse> GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        var roles = await userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var jwtSettings = configuration.GetSection("Jwt");
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
        var creds = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddDays(1);

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

public interface IAuthenticationService
{
    Task<AuthenticationResponse> RegisterAsync(RegisterRequest model);
    Task<AuthenticationResponse> LoginAsync(LoginRequest model);
}
