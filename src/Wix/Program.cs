using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.Caching.Memory;
using Wix.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddOData(opt =>
    {
        opt.EnableQueryFeatures();
    });

// In memory data storage as we're not using a real server
builder.Services.AddMemoryCache();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();


var app = builder.Build();

PopulateInitialStoreData(app.Services);


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseDeveloperExceptionPage();



app.UseAuthorization();

app.MapControllerRoute(
  name: "default",
  pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


void PopulateInitialStoreData(IServiceProvider services)
{
    using (var scope = services.CreateScope())
    {
        var memoryCache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();
        var items = new List<StoreModel>
        {
            new StoreModel { Id = "store-1", Title = "Gadget Haven", Content = "Find the latest in tech gadgets.", Views = 150, TimeStamp = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds() },
            new StoreModel { Id = "store-2", Title = "Book World", Content = "Explore our vast collection of books.", Views = 200, TimeStamp = (int)DateTimeOffset.UtcNow.AddSeconds(-3600).ToUnixTimeSeconds() },
            new StoreModel { Id = "store-3", Title = "Fashion Hub", Content = "Your one-stop shop for the latest fashion trends.", Views = 250, TimeStamp = (int)DateTimeOffset.UtcNow.AddSeconds(-7200).ToUnixTimeSeconds() },
            new StoreModel { Id = "store-4", Title = "Home Essentials", Content = "Everything you need for a cozy home.", Views = 75, TimeStamp = (int)DateTimeOffset.UtcNow.AddSeconds(-10800).ToUnixTimeSeconds() },
            new StoreModel { Id = "store-5", Title = "Outdoor Adventures", Content = "Gear up for your next outdoor adventure.", Views = 125, TimeStamp = (int)DateTimeOffset.UtcNow.AddSeconds(-14400).ToUnixTimeSeconds() }
        };
        memoryCache.Set("StoreData", items);
    }
}