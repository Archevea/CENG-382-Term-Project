using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore; 
using ReservationSystem.Data;
using ReservationSystem.Models;

// Create a new WebApplication builder instance
var builder = WebApplication.CreateBuilder(args);

// Get the connection string from configuration
var connectionString = builder.Configuration.GetConnectionString("ReservationDbConnection");

// Register the application's DbContext with SQL Server provider
builder.Services.AddDbContext<ReservationsDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configure ASP.NET Core Identity with custom password requirements
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
    options.SignIn.RequireConfirmedAccount = false; // No email confirmation required
    options.Password.RequireDigit = true; // Password must contain a digit
    options.Password.RequireLowercase = true; // Password must contain a lowercase letter
    options.Password.RequireUppercase = true; // Password must contain an uppercase letter
    options.Password.RequireNonAlphanumeric = true; // Password must contain a special character
    options.Password.RequiredLength = 8; // Minimum password length
})
.AddEntityFrameworkStores<ReservationsDbContext>() // Use EF Core for Identity stores
.AddDefaultTokenProviders() // Add default token providers for password reset, etc.
.AddDefaultUI(); // Add default UI for Identity

// Add authorization policy for Admin role
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
});

// Add Razor Pages services
builder.Services.AddRazorPages();

// Build the application
var app = builder.Build();

// Seed initial data (e.g., roles, admin user) at startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.Initialize(services);
}

// Configure error handling and HSTS for non-development environments
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Enable HTTPS redirection and serve static files
app.UseHttpsRedirection();
app.UseStaticFiles();

// Configure request routing
app.UseRouting();

// Enable authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Map Razor Pages endpoints
app.MapRazorPages();

// Run the application
app.Run();
