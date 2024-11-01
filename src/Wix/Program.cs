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

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
System.Diagnostics.Debug.WriteLine($"Connection String: {connectionString}");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Connection string is not configured correctly.");
}

// Configure MySQL connection
builder.Services.AddScoped<IDbConnection>((sp) =>
{
    var connection = new MySqlConnection(connectionString);

    try
    {
        connection.Open();
        System.Diagnostics.Debug.WriteLine("Connection to the database was successful.");

        // Test the connection with a simple query
        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = "SELECT 1;";
            cmd.ExecuteNonQuery(); // This should succeed if the connection is valid
            System.Diagnostics.Debug.WriteLine("Test query executed successfully.");
        }

        // Set the database context
        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = "USE lithuania-wix;";
            cmd.ExecuteNonQuery();
        }
    }
    catch (MySqlException ex)
    {
        System.Diagnostics.Debug.WriteLine($"MySQL error: {ex.Message} - {ex.StackTrace}");
        throw new Exception("Unable to connect to the MySQL database.", ex);
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"An unexpected error occurred: {ex.Message}");
        throw new Exception("An unexpected error occurred while connecting to the database.", ex);
    }

    return connection;
});

// Register the MySQL-based StoreRepository and services
builder.Services.AddScoped<IStoreRepository, StoreRepository>();
builder.Services.AddScoped<IStoreService, StoreService>();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

try
{
    PopulateInitialStoreData(app.Services);
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "An error occurred while populating initial store data.");
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

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
