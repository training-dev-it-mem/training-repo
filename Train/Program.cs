using Train.Infrastructure.AppSettingsModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Train.Data;
using Train.Infrastructure.ApplicationUserClaims;
using Train.Models;
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Train.Infrastructure;
using Train.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Train.Services;



var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

// Add services to the container.
var config = new ConfigurationBuilder().AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{environment}.json", optional: true).Build();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();


//builder.Services.AddIdentity<ApplicationUser,IdentityRole>(options =>
//    options.SignIn.RequireConfirmedAccount=false)
//    .AddEntityFrameworkStores<ApplicationDbContext>()
//    .AddDefaultTokenProviders()
//    .AddDefaultUI();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ApplicationUserClaimsPrincipalFactory>();
builder.Services.AddTransient<EmployeeService>();
builder.Services.Configure<IdentityOptions>(options =>
{
    // Default Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // Default Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Default SignIn settings.
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;

    // Default User settings.
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
});
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "CreativeTim.Argon.DotNetCore.AppCookie";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    // You might want to only set the application cookies over a secure connection:
    // options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
});

// As per https://github.com/aspnet/AspNetCore/issues/5828
// the settings for the cookie would get overwritten if using the default UI so
// we need to "post-configure" the authentication cookie
builder.Services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme, options =>
{
    options.AccessDeniedPath = "/access-denied";
    options.LoginPath = "/login";
    options.LogoutPath = "/logout";

    options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
});

/*builder.Services.AddDataProtection()
               .PersistKeysToDbContext<ApplicationDbContext>();
 */
builder.Services.AddAntiforgery();

builder.Services.Configure<ScriptTags>(builder.Configuration.GetSection(nameof(ScriptTags)));


builder.Services.AddControllersWithViews(options =>
{
// Slugify routes so that we can use /employee/employee-details/1 instead of
// the default /Employee/EmployeeDetails/1
//
// Using an outbound parameter transformer is a better choice as it also allows
// the creation of correct routes using view helpers
options.Conventions.Add(
    new RouteTokenTransformerConvention(
        new SlugifyParameterTransformer()));

// Enable Antiforgery feature by default on all controller actions
options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});

builder.Services.AddRazorPages(options =>
{
options.Conventions.AddAreaPageRoute("Identity", "/Account/Register", "/register");
options.Conventions.AddAreaPageRoute("Identity", "/Account/Login", "/login");
options.Conventions.AddAreaPageRoute("Identity", "/Account/Logout", "/logout");
options.Conventions.AddAreaPageRoute("Identity", "/Account/ForgotPassword", "/forgot-password");
})
.SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
.AddSessionStateTempDataProvider();


builder.Services.AddAuthorization(options =>
{
options.DefaultPolicy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();
});

// You probably want to use in-memory cache if not developing using docker-compose
// services.AddMemoryCache();
//builder.Services.AddDistributedRedisCache(action => { action.Configuration = Configuration["Redis:InstanceName"]; });

builder.Services.AddSession(options =>
{
// Set a short timeout for easy testing.
options.IdleTimeout = TimeSpan.FromMinutes(60);
options.Cookie.Name = "CreativeTim.Argon.DotNetCore.SessionCookie";
// You might want to only set the application cookies over a secure connection:
// options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
options.Cookie.SameSite = SameSiteMode.Strict;
options.Cookie.HttpOnly = true;
// Make the session cookie essential
options.Cookie.IsEssential = true;
});

// This adds a hosted service that, on application start-up, seeds the database with initial data.
// You can remove this if you want to prevent the seeding process or you can change the initial data
// to suit your needs in the IdentityDataSeeder class.

// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.

{
    // This is required to make the application work behind a proxy like NGINX or HAPROXY
    // that also provides TLS termination (switching from incoming HTTPS to HTTP)
    var app = builder.Build();
    app.UseForwardedHeaders();
    
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseDatabaseErrorPage();
    }
    else
    {
        app.UseExceptionHandler("/error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseStatusCodePagesWithReExecute("/status-code", "?code={0}");

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseCookiePolicy();

    app.UseSession();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
    app.MapRazorPages();

    app.Run();
}
