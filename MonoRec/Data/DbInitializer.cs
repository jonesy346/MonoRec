using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MonoRec.Models;

namespace MonoRec.Data;

public static class DbInitializer
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        // Apply migrations for both contexts to create all tables
        var appDbContext = services.GetRequiredService<ApplicationDbContext>();
        await appDbContext.Database.MigrateAsync();

        var monoRecContext = services.GetRequiredService<MonoRecDbContext>();
        await monoRecContext.Database.MigrateAsync();

        // Seed Identity data first (roles and users)
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await SeedIdentityData(userManager, roleManager);

        // Seed MonoRec data linked to Identity users
        await SeedMonoRecData(monoRecContext, userManager);
    }

    private static async Task SeedMonoRecData(MonoRecDbContext context, UserManager<ApplicationUser> userManager)
    {
        // Check if data already exists
        if (await context.Doctors.AnyAsync())
        {
            return; // Database has been seeded
        }

        // NEW APPROACH: Create Doctor/Patient entities that correspond to Identity users
        // This links the authentication system to the business data by storing the ApplicationUser.Id

        // Get the doctor@example.com user and create a linked Doctor entity
        var doctorUser = await userManager.FindByEmailAsync("doctor@example.com");
        if (doctorUser != null)
        {
            var doctorEntity = new Doctor("Dr. Example")
            {
                UserId = doctorUser.Id // Store the Identity user ID to create the link
            };
            await context.Doctors.AddAsync(doctorEntity);
            await context.SaveChangesAsync();
            Console.WriteLine($"Doctor entity 'Dr. Example' created and linked to user ID: {doctorUser.Id}");
        }

        // Get the patient@example.com user and create a linked Patient entity
        var patientUser = await userManager.FindByEmailAsync("patient@example.com");
        if (patientUser != null)
        {
            var patientEntity = new Patient("Patient Example")
            {
                UserId = patientUser.Id // Store the Identity user ID to create the link
            };
            await context.Patients.AddAsync(patientEntity);
            await context.SaveChangesAsync();
            Console.WriteLine($"Patient entity 'Patient Example' created and linked to user ID: {patientUser.Id}");
        }

        // OLD APPROACH: Create sample data not linked to any user accounts
        // Keeping these for testing purposes - these are standalone records
        // var doctors = new Doctor[]
        // {
        //     new Doctor("Dr. Smith"),
        //     new Doctor("Dr. Johnson"),
        //     new Doctor("Dr. Williams")
        // };
        // await context.Doctors.AddRangeAsync(doctors);
        // await context.SaveChangesAsync();

        // var patients = new Patient[]
        // {
        //     new Patient("John Doe"),
        //     new Patient("Jane Smith"),
        //     new Patient("Bob Wilson")
        // };
        // await context.Patients.AddRangeAsync(patients);
        // await context.SaveChangesAsync();

        Console.WriteLine("MonoRec database seeded successfully.");
    }

    private static async Task SeedIdentityData(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Create roles if they don't exist
        string[] roleNames = { "Doctor", "Patient" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
                Console.WriteLine($"Role '{roleName}' created successfully.");
            }
        }

        // Create Doctor user
        await CreateUserWithRole(userManager, "doctor@example.com", "Password123!", "Doctor");

        // Create Patient user
        await CreateUserWithRole(userManager, "patient@example.com", "Password123!", "Patient");

        // Keep the original test user for backward compatibility
        await CreateUserWithRole(userManager, "test@test.com", "Welcome123!", null);
    }

    private static async Task CreateUserWithRole(UserManager<ApplicationUser> userManager, string email, string password, string roleName)
    {
        var existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            return; // User already exists
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true // Bypass email confirmation for seeded users
        };

        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            if (roleName != null)
            {
                await userManager.AddToRoleAsync(user, roleName);
                Console.WriteLine($"User '{email}' created with role '{roleName}'. Password: {password}");
            }
            else
            {
                Console.WriteLine($"User '{email}' created successfully. Password: {password}");
            }
        }
        else
        {
            Console.WriteLine($"Failed to create user '{email}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }
}
