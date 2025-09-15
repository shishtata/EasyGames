// Initially we are importing all the necessary tools like data, login system,
// database manager, models and services before we start working

using EasyGames.Data;
using EasyGames.Models;
using EasyGames.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

// Configures services for the web application (sets up dependency injection)

var builder = WebApplication.CreateBuilder(args);

// Registers MVC Controllers with views so we can use Controllers and Razor files
builder.Services.AddControllersWithViews();

// Keeps session data between requests and configures session settings
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(o =>
{
    // Cookie setting so JS can't read the session cookie
    o.Cookie.HttpOnly = true;

    // Session timeout after 30 minutes of inactivity
    o.IdleTimeout = TimeSpan.FromMinutes(30);
});

// Access to HttpContext and CartService for managing shopping cart functionality
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CartService>();

// Connects to SQL Server Database using AppDbContext and connection string
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Configures Identity for user authentication and authorization
builder.Services.AddIdentity<Users, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false; 
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})

 // Links identity to database context
.AddEntityFrameworkStores<AppDbContext>()

// Creates tokens for password resets, email confirmation, etc.
.AddDefaultTokenProviders();

// Finalizes the app configuration and builds the app
var app = builder.Build();

// Middleware pipeline configuration (done for error handling, security, routing)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Middleware components in order of execution
app.UseHttpsRedirection(); // Forces HTTPS
app.UseStaticFiles(); // Allow static files like css, js, images
app.UseRouting(); // Enables routing
app.UseSession(); // Enables session (cart functionality)
app.UseAuthentication(); // Enables Authentication
app.UseAuthorization(); // Enables Authorization

// Defines the default route for MVC Controllers
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


// Seeds the database with initial data (roles, admin, user, categories)
using (var scope = app.Services.CreateScope())
{
    await SeedService.SeedDatabase(scope.ServiceProvider);
}

// Runs the application
app.Run();
