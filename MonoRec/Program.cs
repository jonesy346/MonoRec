using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using MonoRec.Data;
using MonoRec.Models;
using MonoRec.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDbContext<MonoRecDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddIdentityServer()
    .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

builder.Services.AddAuthentication()
    .AddIdentityServerJwt();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddControllers();
builder.Services.AddScoped<IMonoRecRepository, MonoRecRepository>();

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
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseIdentityServer();
app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller}/{action=Index}/{id?}");
app.MapControllers();
app.MapRazorPages();

//app.UseMvc();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});


//app.MapWhen(x => x.Request.Path.Value.StartsWith("/api"), builder =>
//{
//    app.UseMvc();
//});

//app.MapWhen(x => !x.Request.Path.Value.StartsWith("/api"), builder =>
//{
//    app.UseSpa(spa =>
//    {
//        spa.Options.SourcePath = "ClientApp";

//        if (env.IsDevelopment())
//        {
//            spa.UseReactDevelopmentServer(npmScript: "start");
//        }
//    });
//});


//app.MapWhen(x => !x.Request.Path.Value.StartsWith("/api"), builder =>
//{
//    app.Run(async (context) =>
//    {
//        context.Response.ContentType = "text/html";
//        context.Response.Headers[HeaderNames.CacheControl] = "no-store, no-cache, must-revalidate";
//        await context.Response.SendFileAsync(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"));
//    });
//});

app.MapFallbackToFile("index.html"); ;

app.Run();

