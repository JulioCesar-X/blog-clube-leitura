using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BlogClubeLeitura.Data;
using BlogClubeLeitura.Models;

var builder = WebApplication.CreateBuilder(args);

// Adicione serviços ao contêiner.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure o pipeline HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
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

// Lógica de seeding
if (app.Environment.IsDevelopment() || Environment.GetEnvironmentVariable("SEED_DATABASE") == "true")
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var loggerFactory = services.GetRequiredService<ILoggerFactory>();
        try
        {
            SeedData.Initialize(services, "InitialSeed").Wait();
        }
        catch (Exception ex)
        {
            var logger = loggerFactory.CreateLogger<Program>();
            logger.LogError(ex, "An error occurred seeding the DB.");
        }
    }
}

app.Run();