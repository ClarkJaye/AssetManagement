using Microsoft.EntityFrameworkCore;
using AssetManagement.Data;
using AssetManagement.Services;
using AssetManagement.Utility;
using ExternalLogin.Interfaces;
using ExternalLogin.Services;
using ExternalLogin;


var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json");

var CentralLoginConnectionString = builder.Configuration.GetConnectionString("CentralLoginConnection")
?? throw new InvalidOperationException("Connection string 'ExternalDbContext' not found.");

builder.Services.AddDbContext<ExternalDbContext>(options =>
    options.UseSqlServer(CentralLoginConnectionString));

//external login => AD Login
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IExternalLoginService, ExternalLoginService>();



// Configure database context
builder.Services.AddDbContext<AssetManagementContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")));


//ADLOGIN
builder.Services.Configure<LDAPSettings>(builder.Configuration.GetSection("LDAPSettings"));


//SESSION
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "MySessionCookie";
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});


// Add services to the container.
builder.Services.AddControllersWithViews(
    options => options.Filters.Add(typeof(ProfileAccessAuth)));


builder.Services.AddScoped<ProfileAccessService>();
builder.Services.AddHostedService<ProfileAccessBackgroundService>();

builder.Logging.AddConsole();
var app = builder.Build();

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

// Authentication and Session
app.UseAuthentication(); 
app.UseSession();
app.UseAuthorization();

// Routing configuration
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Users}/{action=Login}/{id?}");

app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllerRoute(
        name: "UserStore",
        pattern: "UserStore/Index/{page?}",
        defaults: new { controller = "UserStore", action = "Index" }
    );
});

app.Run();