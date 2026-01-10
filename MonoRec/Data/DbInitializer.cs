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
        // Populate DoctorEmail for existing doctors that have a UserId but no email
        var doctorsWithoutEmail = await context.Doctors
            .Where(d => d.UserId != null && d.DoctorEmail == null)
            .ToListAsync();

        if (doctorsWithoutEmail.Any())
        {
            foreach (var doctor in doctorsWithoutEmail)
            {
                var user = await userManager.FindByIdAsync(doctor.UserId);
                if (user != null)
                {
                    doctor.DoctorEmail = user.Email;
                }
            }
            await context.SaveChangesAsync();
            Console.WriteLine($"Populated emails for {doctorsWithoutEmail.Count} existing doctors.");
        }

        // Populate PatientEmail for existing patients that have a UserId but no email
        var patientsWithoutEmail = await context.Patients
            .Where(p => p.UserId != null && p.PatientEmail == null)
            .ToListAsync();

        if (patientsWithoutEmail.Any())
        {
            foreach (var patient in patientsWithoutEmail)
            {
                var user = await userManager.FindByIdAsync(patient.UserId);
                if (user != null)
                {
                    patient.PatientEmail = user.Email;
                }
            }
            await context.SaveChangesAsync();
            Console.WriteLine($"Populated emails for {patientsWithoutEmail.Count} existing patients.");
        }

        // Check if data already exists
        if (await context.Doctors.AnyAsync())
        {
            return; // Database has been seeded
        }

        // Seed 10 sample doctors (not linked to user accounts, but with email addresses for display)
        var doctorData = new[]
        {
            ("Smith", "smith@monorec.com"),
            ("Johnson", "johnson@monorec.com"),
            ("Williams", "williams@monorec.com"),
            ("Brown", "brown@monorec.com"),
            ("Jones", "jones@monorec.com"),
            ("Garcia", "garcia@monorec.com"),
            ("Miller", "miller@monorec.com"),
            ("Davis", "davis@monorec.com"),
            ("Rodriguez", "rodriguez@monorec.com"),
            ("Martinez", "martinez@monorec.com")
        };
        var doctors = new List<Doctor>();

        foreach (var (name, email) in doctorData)
        {
            var doctor = new Doctor($"Dr. {name}")
            {
                DoctorEmail = email
            };
            doctors.Add(doctor);
        }
        await context.Doctors.AddRangeAsync(doctors);
        await context.SaveChangesAsync();
        Console.WriteLine("Seeded 10 sample doctors.");

        // Seed 10 sample patients (not linked to user accounts, but with email addresses for display)
        var patientData = new[]
        {
            ("Alice", "alice@monorec.com"),
            ("Bob", "bob@monorec.com"),
            ("Charlie", "charlie@monorec.com"),
            ("Diana", "diana@monorec.com"),
            ("Edward", "edward@monorec.com"),
            ("Fiona", "fiona@monorec.com"),
            ("George", "george@monorec.com"),
            ("Hannah", "hannah@monorec.com"),
            ("Isaac", "isaac@monorec.com"),
            ("Julia", "julia@monorec.com")
        };
        var patients = new List<Patient>();

        foreach (var (name, email) in patientData)
        {
            var patient = new Patient($"{name} Patient")
            {
                PatientEmail = email
            };
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
            UserName = email, // Use email as username since fullName may contain spaces
            Email = email,
            Name = fullName, // Store the full name in the Name property for display
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
            // First check if this doctor already has a Doctor entity linked by UserId
            var existingDoctor = await context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);

            if (existingDoctor == null)
            {
                // Try to find a seeded doctor with matching email but no UserId
                existingDoctor = await context.Doctors.FirstOrDefaultAsync(d => d.DoctorEmail == user.Email && d.UserId == null);

                if (existingDoctor != null)
                {
                    // Link the existing seeded doctor to this user
                    existingDoctor.UserId = userId;
                    await context.SaveChangesAsync();
                    Console.WriteLine($"Linked existing doctor record to user {user.Email}");
                }
                else
                {
                    // Create a new Doctor entity for this user
                    var doctorEntity = new Doctor(user.Name ?? user.Email?.Split('@')[0] ?? "Doctor")
                    {
                        UserId = userId,
                        DoctorEmail = user.Email
                    };
                    await context.Doctors.AddAsync(doctorEntity);
                    await context.SaveChangesAsync();
                    existingDoctor = doctorEntity;
                    Console.WriteLine($"Created new doctor record for user {user.Email}");
                }
            }
            else
            {
                // Update email if it's missing
                if (string.IsNullOrEmpty(existingDoctor.DoctorEmail))
                {
                    existingDoctor.DoctorEmail = user.Email;
                    await context.SaveChangesAsync();
                }
            }

            // Check if this doctor already has patient associations
            var existingAssociations = await context.DoctorsPatients
                .Where(dp => dp.DoctorId == existingDoctor.DoctorId)
                .CountAsync();

            if (existingAssociations == 0)
            {
                // Link to 3 random seeded patients (those without UserId)
                var seededPatients = await context.Patients
                    .Where(p => p.UserId == null)
                    .ToListAsync();

                if (seededPatients.Any())
                {
                    var random = new Random();
                    var selectedPatients = seededPatients.OrderBy(x => random.Next()).Take(3).ToList();

                    foreach (var patient in selectedPatients)
                    {
                        var doctorPatient = new DoctorPatient(existingDoctor.DoctorId, patient.PatientId);
                        await context.DoctorsPatients.AddAsync(doctorPatient);
                    }
                    await context.SaveChangesAsync();
                    Console.WriteLine($"Doctor {user.Email} linked to {selectedPatients.Count} seeded patients.");
                }
                else
                {
                    Console.WriteLine($"Doctor {user.Email} entity created successfully (no seeded patients available).");
                }
            }
        }
        else if (userRole == "Patient")
        {
            // First check if this patient already has a Patient entity linked by UserId
            var existingPatient = await context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);

            if (existingPatient == null)
            {
                // Try to find a seeded patient with matching email but no UserId
                existingPatient = await context.Patients.FirstOrDefaultAsync(p => p.PatientEmail == user.Email && p.UserId == null);

                if (existingPatient != null)
                {
                    // Link the existing seeded patient to this user
                    existingPatient.UserId = userId;
                    await context.SaveChangesAsync();
                    Console.WriteLine($"Linked existing patient record to user {user.Email}");
                }
                else
                {
                    // Create a new Patient entity for this user
                    var patientEntity = new Patient(user.Name ?? user.Email?.Split('@')[0] ?? "Patient")
                    {
                        UserId = userId,
                        PatientEmail = user.Email
                    };
                    await context.Patients.AddAsync(patientEntity);
                    await context.SaveChangesAsync();
                    existingPatient = patientEntity;
                    Console.WriteLine($"Created new patient record for user {user.Email}");
                }
            }
            else
            {
                // Update email if it's missing
                if (string.IsNullOrEmpty(existingPatient.PatientEmail))
                {
                    existingPatient.PatientEmail = user.Email;
                    await context.SaveChangesAsync();
                }
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
