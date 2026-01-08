using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MonoRec.Models;

namespace MonoRec.Controllers;

// upon application start, this controller is called once so that the guest role and user is established

[ApiController]
[Route("[controller]")]
public class UsersController : Controller
{
    // roleManager, userManager are APIs that work with domain models (IdentityUser, Role)
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    // Study and uncomment this lines later with Jason

    [HttpGet]
    public async Task Index()
    {
        bool isRoleCreated = await _roleManager.RoleExistsAsync("Admin");
        if (!isRoleCreated)
        {
            // first we create Admin role   
            var role = new IdentityRole();
            role.Name = "Admin";
            await _roleManager.CreateAsync(role);
        }
        //Then we create a user 
        var user = new ApplicationUser();
        //user.UserName = "fake@cognizant.com";
        //user.Email = "fake@cognizant.com";
        //string userPWD = "MonkeyGadget123!";

        user.UserName = "test@test.com";
        user.Email = "test@test.com";
        string userPWD = "test123!";

        IdentityResult chkUser = await _userManager.CreateAsync(user, userPWD);

        //Add default User to Role Admin    
        if (chkUser.Succeeded)
        {
            var result = await _userManager.AddToRoleAsync(user, "Admin");
        }
    }

    // A user can have more than one role  
    // Good software developer practice: The different role name should have the different permissions 


}
