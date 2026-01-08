using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MonoRec.Models;

namespace MonoRec.Controllers;

[Route("api/users")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        // Try to get user from current context (works with both Cookie and JWT auth)
        var user = await _userManager.GetUserAsync(User);

        // Check if user is authenticated
        if (user == null)
        {
            return Ok(new
            {
                authenticated = false,
                message = "User not authenticated"
            });
        }

        var roles = await _userManager.GetRolesAsync(user);

        return Ok(new
        {
            authenticated = true,
            userId = user.Id,
            email = user.Email,
            name = user.Name,
            roles = roles
        });
    }
}
