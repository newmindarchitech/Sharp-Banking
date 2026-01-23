using BankingVault.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<DatabaseContext>(options =>
{
    var DatabaseCon = builder.Configuration.GetConnectionString("DatabaseConnection");
    options.UseSqlServer(DatabaseCon);
});


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/User/Login";
    options.AccessDeniedPath = "/User/Forbidden";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(90);
    options.SlidingExpiration = true;
    options.Cookie.MaxAge=TimeSpan.FromDays(30);
    options.Cookie.SameSite=SameSiteMode.Strict;
    options.Validate();
});



builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserAuth", policy => policy.RequireRole("User").RequireAuthenticatedUser());
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin").RequireAuthenticatedUser());
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseCookiePolicy();
app.UseRequestLocalization();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.UseResponseCaching();
app.Run();
