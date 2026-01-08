using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using MonoRec.Models;
using System.Security.Claims;

namespace MonoRec.Services;

public class ProfileService : IProfileService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfileService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var user = await _userManager.GetUserAsync(context.Subject);

        if (user != null)
        {
            var claims = new List<Claim>();

            // Override the default 'name' claim with the user's actual Name property
            if (!string.IsNullOrEmpty(user.Name))
            {
                // Remove any existing 'name' claim
                var existingNameClaim = context.IssuedClaims.FirstOrDefault(c => c.Type == "name");
                if (existingNameClaim != null)
                {
                    context.IssuedClaims.Remove(existingNameClaim);
                }
                claims.Add(new Claim("name", user.Name));
            }

            // Add email claim explicitly (preferred_username is usually the email)
            if (!string.IsNullOrEmpty(user.Email))
            {
                claims.Add(new Claim("email", user.Email));
            }

            // Add role claims explicitly
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim("role", role));
            }

            context.IssuedClaims.AddRange(claims);
        }
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        var user = await _userManager.GetUserAsync(context.Subject);
        context.IsActive = user != null;
    }
}
