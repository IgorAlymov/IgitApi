using IgitApi.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IgitApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController(RoleManager<Role> roleManager) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> CreateRole(string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
        {
            return BadRequest("Role name cannot be empty.");
        }

        if (await roleManager.RoleExistsAsync(roleName))
        {
            return BadRequest($"Role '{roleName}' already exists.");
        }

        var role = new Role { Name = roleName };
        var result = await roleManager.CreateAsync(role);

        if (result.Succeeded)
        {
            return Ok($"Role '{roleName}' created successfully.");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return BadRequest(ModelState);
    }
}
