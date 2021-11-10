using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Rystem;
using Rystem.Test.Accounting.Data;
using Rystem.Test.Accounting.Models;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var storage = builder.Configuration.GetSection("Storage");
// Add services to the container.
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(connectionString));
//builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();
services
    .AddRystem()
    .AddAzureService()
    .AddStorage(new(storage["Name"], storage["Key"]))
    .EndConfiguration();

services.AddTableStorageIdentity(x =>
{
    x.SignIn.RequireConfirmedPhoneNumber = false;
    x.User.RequireUniqueEmail = true;
    x.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    x.SignIn.RequireConfirmedEmail = true;
    x.SignIn.RequireConfirmedAccount = true;
    x.Lockout.AllowedForNewUsers = true;
    x.Lockout.MaxFailedAccessAttempts = 10;
    x.Password.RequireUppercase = true;
    x.Password.RequiredLength = 10;
    x.Password.RequireDigit = true;
    x.Password.RequireLowercase = true;
    x.Password.RequireNonAlphanumeric = true;
})
.AddDefaultUI()
.AddTokenProvider<IdentityServices>(TokenOptions.DefaultProvider)
.AddIdentityNotification<IdentityNotificator>()
    .AddAuthentication()
    .AddMicrosoftAccount(microsoftOptions =>
    {
        microsoftOptions.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"];
        microsoftOptions.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"];
    });

services.AddControllersWithViews();

var app = builder.Build();
app.UseRystem();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.Use(async (x, y) =>
{
    await ServiceLocator.GetService<RoleManager<IdentityRole>>().UpdateAsync(
               new IdentityRole()
               {
                   Id = Guid.NewGuid().ToString(),
                   Name = "Admin",
                   NormalizedName = "ADMIN",
                   ConcurrencyStamp = DateTime.UtcNow.ToString()
               }).NoContext();
    await y(x).NoContext();
});
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
