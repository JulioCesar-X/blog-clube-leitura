using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BlogClubeLeitura.Data;
using BlogClubeLeitura.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel (o servidor web)
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    //serverOptions.ListenLocalhost(5000); // Set HTTP port to 5000
    serverOptions.ListenAnyIP(5000);
});

// Adicione servi�os ao container
ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

// Configure o pipeline HTTP
ConfigureMiddleware(app);

// Configura as rotas
ConfigureRoutes(app);

// Popula a base de dados, se necess�rio
SeedDatabase(app);

app.Run();

void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

    services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false; // Desativa a necessidade de confirmação de email
    })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders()
        .AddDefaultUI();

    services.AddControllersWithViews();
    services.AddRazorPages();
}

void ConfigureMiddleware(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage(); // P�ginas de erro detalhadas em desenvolvimento
        app.UseMigrationsEndPoint(); // Endpoint para aplicar migra��es do EF Core
    }
    else
    {
        app.UseExceptionHandler("/Home/Error"); // P�gina de erro padr�o em produ��o
        app.UseHsts(); // HTTP Strict Transport Security
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
}

void ConfigureRoutes(WebApplication app)
{
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    app.MapControllerRoute(
      name: "books",
      pattern: "Books/{action=Index}/{id?}",
      defaults: new { controller = "Books" });

    app.MapControllerRoute(
        name: "posts",
        pattern: "Posts/{action=Index}/{id?}",
        defaults: new { controller = "Posts" });

    app.MapControllerRoute(
        name: "ratings",
        pattern: "Ratings/{action=Index}/{id?}",
        defaults: new { controller = "Ratings" });
    app.MapRazorPages(); // Necess�rio para as Razor Pages do Identity
}

void SeedDatabase(WebApplication app)
{
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
}