using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore;
using GestionPrestation.Data;
using GestionPrestation.Models;

var builder = WebApplication.CreateBuilder(args);

// Database Configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdmin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireClient", policy => policy.RequireRole("Client"));
    options.AddPolicy("RequirePrestataire", policy => policy.RequireRole("Prestataire"));
    options.AddPolicy("RequireCompany", policy => policy.RequireRole("Societe"));
    options.AddPolicy("RequireAuthenticated", policy => policy.RequireAuthenticatedUser());
});

builder.Services.AddControllersWithViews();

// HttpContext Accessor
builder.Services.AddHttpContextAccessor();

// Register Services
builder.Services.AddScoped<GestionPrestation.Services.IPrestationService, GestionPrestation.Services.PrestationService>();
builder.Services.AddScoped<GestionPrestation.Services.IUserManagementService, GestionPrestation.Services.UserManagementService>();
builder.Services.AddScoped<GestionPrestation.Services.ICompanyService, GestionPrestation.Services.CompanyService>();
builder.Services.AddScoped<GestionPrestation.Services.IClientService, GestionPrestation.Services.ClientService>();
builder.Services.AddScoped<GestionPrestation.Services.IPrestataireService, GestionPrestation.Services.PrestataireService>();

var app = builder.Build();

// Appliquer les migrations puis seed
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        // Applique les migrations pendantes (utile en dev / déploiement simple)
        await db.Database.MigrateAsync();

        await DbInitializer.SeedRolesAndAdmin(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating/seeding the database.");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();