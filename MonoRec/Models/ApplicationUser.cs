using Microsoft.AspNetCore.Identity;

namespace MonoRec.Models;

public class ApplicationUser : IdentityUser
{
    public string? Name { get; set; }
}


//test@test.com
//Welcome123!