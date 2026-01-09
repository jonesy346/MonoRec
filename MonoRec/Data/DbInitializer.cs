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

        // Seed MonoRec data
        await SeedMonoRecData(monoRecContext, userManager);
    }

    private static async Task SeedMonoRecData(MonoRecDbContext context, UserManager<ApplicationUser> userManager)
    {
        // Check if data already exists
        if (await context.Doctors.AnyAsync())
        {
            return; // Database has been seeded
        }

        // Seed 10 sample doctors (not linked to user accounts)
        var doctorNames = new[] { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez" };
        var doctors = new List<Doctor>();

        foreach (var name in doctorNames)
        {
            var doctor = new Doctor($"Dr. {name}");
            doctors.Add(doctor);
        }
        await context.Doctors.AddRangeAsync(doctors);
        await context.SaveChangesAsync();
        Console.WriteLine("Seeded 10 sample doctors.");

        // Seed 10 sample patients (not linked to user accounts)
        var patientFirstNames = new[] { "Alice", "Bob", "Charlie", "Diana", "Edward", "Fiona", "George", "Hannah", "Isaac", "Julia" };
        var patients = new List<Patient>();

        foreach (var name in patientFirstNames)
        {
            var patient = new Patient($"{name} Patient");
            patients.Add(patient);
        }
        await context.Patients.AddRangeAsync(patients);
        await context.SaveChangesAsync();
        Console.WriteLine("Seeded 10 sample patients.");

        Console.WriteLine("MonoRec database seeded successfully.");
    }

    private static async Task SeedIdentityData(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        Console.WriteLine("=== Starting SeedIdentityData ===");

        // Create roles if they don't exist
        string[] roleNames = { "Doctor", "Patient" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
                Console.WriteLine($"Role '{roleName}' created successfully.");
            }
            else
            {
                Console.WriteLine($"Role '{roleName}' already exists.");
            }
        }

        Console.WriteLine("=== Creating doctor accounts ===");
        // Seed 10 doctor user accounts with full names
        var doctorData = new[]
        {
            ("smith@monorec.com", "John Smith"),
            ("johnson@monorec.com", "Emily Johnson"),
            ("williams@monorec.com", "Michael Williams"),
            ("brown@monorec.com", "Sarah Brown"),
            ("jones@monorec.com", "David Jones"),
            ("garcia@monorec.com", "Maria Garcia"),
            ("miller@monorec.com", "James Miller"),
            ("davis@monorec.com", "Linda Davis"),
            ("rodriguez@monorec.com", "Carlos Rodriguez"),
            ("martinez@monorec.com", "Ana Martinez")
        };
        foreach (var (email, fullName) in doctorData)
        {
            await CreateUserWithRole(userManager, email, "Doctor123!", "Doctor", fullName);
        }

        // Seed 10 patient user accounts with full names
        var patientData = new[]
        {
            ("alice@monorec.com", "Alice Anderson"),
            ("bob@monorec.com", "Bob Baker"),
            ("charlie@monorec.com", "Charlie Clark"),
            ("diana@monorec.com", "Diana Davis"),
            ("edward@monorec.com", "Edward Evans"),
            ("fiona@monorec.com", "Fiona Foster"),
            ("george@monorec.com", "George Green"),
            ("hannah@monorec.com", "Hannah Hall"),
            ("isaac@monorec.com", "Isaac Irving"),
            ("julia@monorec.com", "Julia Jackson")
        };
        foreach (var (email, fullName) in patientData)
        {
            await CreateUserWithRole(userManager, email, "Patient123!", "Patient", fullName);
        }

        // Keep the original test user for backward compatibility
        await CreateUserWithRole(userManager, "test@test.com", "Welcome123!", null, "Test User");
    }

    private static async Task CreateUserWithRole(UserManager<ApplicationUser> userManager, string email, string password, string? roleName, string? fullName = null)
    {
        var existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            return; // User already exists
        }

        var user = new ApplicationUser
        {
            UserName = fullName ?? email,
            Email = email,
            EmailConfirmed = true // Bypass email confirmation for seeded users
        };

        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            if (roleName != null)
            {
                await userManager.AddToRoleAsync(user, roleName);
                Console.WriteLine($"User '{fullName ?? email}' created with role '{roleName}'. Password: {password}");
            }
            else
            {
                Console.WriteLine($"User '{fullName ?? email}' created successfully. Password: {password}");
            }
        }
        else
        {
            Console.WriteLine($"Failed to create user '{fullName ?? email}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }

    public static async Task LinkUserToDoctorsOrPatients(IServiceProvider serviceProvider, string userId, string userRole)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<MonoRecDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        var user = await userManager.FindByIdAsync(userId);
        if (user == null) return;

        if (userRole == "Doctor")
        {
            // Check if this doctor already has a Doctor entity
            var existingDoctor = await context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);

            if (existingDoctor == null)
            {
                // Create a Doctor entity for this user
                var doctorEntity = new Doctor(user.Email?.Split('@')[0] ?? "Doctor")
                {
                    UserId = userId
                };
                await context.Doctors.AddAsync(doctorEntity);
                await context.SaveChangesAsync();
                existingDoctor = doctorEntity;
            }

            // Check if this doctor already has patient associations
            var existingAssociations = await context.DoctorsPatients
                .Where(dp => dp.DoctorId == existingDoctor.DoctorId)
                .CountAsync();

            if (existingAssociations == 0)
            {
                // Randomly assign 3 patients to this doctor
                var allPatients = await context.Patients.ToListAsync();
                var random = new Random();
                var selectedPatients = allPatients.OrderBy(x => random.Next()).Take(3).ToList();

                foreach (var patient in selectedPatients)
                {
                    var doctorPatient = new DoctorPatient(existingDoctor.DoctorId, patient.PatientId);
                    await context.DoctorsPatients.AddAsync(doctorPatient);
                }
                await context.SaveChangesAsync();
                Console.WriteLine($"Doctor {user.Email} linked to {selectedPatients.Count} patients.");
            }
        }
        else if (userRole == "Patient")
        {
            // Check if this patient already has a Patient entity
            var existingPatient = await context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);

            if (existingPatient == null)
            {
                // Create a Patient entity for this user
                var patientEntity = new Patient(user.Email?.Split('@')[0] ?? "Patient")
                {
                    UserId = userId
                };
                await context.Patients.AddAsync(patientEntity);
                await context.SaveChangesAsync();
                existingPatient = patientEntity;
            }

            // Check if this patient already has doctor associations
            var existingAssociations = await context.DoctorsPatients
                .Where(dp => dp.PatientId == existingPatient.PatientId)
                .CountAsync();

            if (existingAssociations == 0)
            {
                // Randomly assign 3 doctors to this patient
                var allDoctors = await context.Doctors.ToListAsync();
                var random = new Random();
                var selectedDoctors = allDoctors.OrderBy(x => random.Next()).Take(3).ToList();

                foreach (var doctor in selectedDoctors)
                {
                    var doctorPatient = new DoctorPatient(doctor.DoctorId, existingPatient.PatientId);
                    await context.DoctorsPatients.AddAsync(doctorPatient);
                }
                await context.SaveChangesAsync();
                Console.WriteLine($"Patient {user.Email} linked to {selectedDoctors.Count} doctors.");
            }
        }
    }
}
