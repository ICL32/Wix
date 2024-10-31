using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.OData;
using MySql.Data.MySqlClient;
using System.Data;
using Wix.Models;
using Wix.Repository;
using Wix.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container with OData enabled
builder.Services.AddControllersWithViews()
    .AddOData(opt =>
    {
        opt.EnableQueryFeatures();
    });


// Configure MySQL connection with retry policy
builder.Services.AddScoped<IDbConnection>((sp) =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection");

    var connection = new MySqlConnection(connectionString);
    connection.Open();
    return connection;
});

// Register the MySQL-based StoreRepository and services
builder.Services.AddScoped<IStoreRepository, StoreRepository>();
builder.Services.AddScoped<IStoreService, StoreService>();

// Clear default logging providers and add console logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// Run initial data population
try
{
    PopulateInitialStoreData(app.Services);
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "An error occurred while populating initial store data.");
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Add middleware
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

// Map health checks endpoint for warmup
app.UseHealthChecks("/health");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

void PopulateInitialStoreData(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var storeService = scope.ServiceProvider.GetRequiredService<IStoreService>();

    if (!storeService.GetAllStores().Any())
    {
        var initialStores = new List<StoreModel>
        {
            new StoreModel { Id = "store-1", Title = "Gadget Haven", Content = "Find the latest in tech gadgets.", Views = 150, TimeStamp = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds() },
            new StoreModel { Id = "store-2", Title = "Book World", Content = "Explore our vast collection of books.", Views = 200, TimeStamp = (int)DateTimeOffset.UtcNow.AddSeconds(-3600).ToUnixTimeSeconds() },
            new StoreModel { Id = "store-3", Title = "Fashion Hub", Content = "Your one-stop shop for the latest fashion trends.", Views = 250, TimeStamp = (int)DateTimeOffset.UtcNow.AddSeconds(-7200).ToUnixTimeSeconds() },
            new StoreModel { Id = "store-4", Title = "Home Essentials", Content = "Everything you need for a cozy home.", Views = 75, TimeStamp = (int)DateTimeOffset.UtcNow.AddSeconds(-10800).ToUnixTimeSeconds() },
            new StoreModel { Id = "store-5", Title = "Outdoor Adventures", Content = "Gear up for your next outdoor adventure.", Views = 125, TimeStamp = (int)DateTimeOffset.UtcNow.AddSeconds(-14400).ToUnixTimeSeconds() }
        };

        foreach (var store in initialStores)
        {
            storeService.AddStore(store);
        }
    }
}
