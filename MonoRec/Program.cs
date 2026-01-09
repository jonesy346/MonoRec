using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using MonoRec.Data;
using MonoRec.Models;
using MonoRec.Repositories;
using MonoRec.Services;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var defaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var identityConnectionString = builder.Configuration.GetConnectionString("IdentityConnection");

// Using SQLite for local development
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(identityConnectionString));
builder.Services.AddDbContext<MonoRecDbContext>(options =>
    options.UseSqlite(defaultConnectionString));

// AWS SQL Server configuration (commented out for local development)
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(identityConnectionString));
//builder.Services.AddDbContext<MonoRecDbContext>(options =>
//    options.UseSqlServer(defaultConnectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 1;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Configure cookie settings for localhost development
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.None; // Allow cookies over HTTP for localhost

    // Hook into the sign-in event to link users to doctors/patients
    options.Events.OnSignedIn = async context =>
    {
        var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
        var user = await userManager.GetUserAsync(context.Principal);
        if (user != null)
        {
            var roles = await userManager.GetRolesAsync(user);
            var userRole = roles.FirstOrDefault();

            if (userRole == "Doctor" || userRole == "Patient")
            {
                await DbInitializer.LinkUserToDoctorsOrPatients(context.HttpContext.RequestServices, user.Id, userRole);
            }
        }
    };

    // For API requests, return 401 instead of redirecting to login page
    options.Events.OnRedirectToLogin = context =>
    {
        // Check if this is an API request (not requesting HTML)
        if (context.Request.Path.StartsWithSegments("/patient") ||
            context.Request.Path.StartsWithSegments("/doctor") ||
            context.Request.Path.StartsWithSegments("/visit"))
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        }

        // For non-API requests, do the normal redirect
        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };
});

builder.Services.AddIdentityServer()
    .AddApiAuthorization<ApplicationUser, ApplicationDbContext>(options =>
    {
        // Add role claim to JWT tokens
        options.IdentityResources["openid"].UserClaims.Add("role");
        options.ApiResources.Single().UserClaims.Add("role");
    })
    .AddProfileService<ProfileService>()
    .AddDeveloperSigningCredential(); // Add development signing credential

// Don't set default schemes - let controllers specify which schemes they accept
builder.Services.AddAuthentication()
    .AddIdentityServerJwt();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddControllers();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<IVisitRepository, VisitRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection(); // Only use HTTPS in production
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseIdentityServer();
app.UseAuthorization();

// Map API controllers and Razor pages first
app.MapControllers();
app.MapRazorPages();

// SPA fallback - MUST be last
// This only catches routes that didn't match any controller or Razor page above
app.MapFallbackToFile("index.html");

// Seed the database
await DbInitializer.Initialize(app.Services);

app.Run();
