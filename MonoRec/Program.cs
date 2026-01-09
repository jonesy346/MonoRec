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

builder.Services.AddAuthentication(options =>
    {
        // Set cookie as the default scheme for challenges (redirects to login)
        options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
    })
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
