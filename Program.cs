using Carvia.Common;
using Carvia.Features.CarImages;
using Carvia.Features.Cars;
using Carvia.Features.Categories;
using Carvia.Features.CuratorItems;
using Carvia.Features.Users;
using Carvia.Infrastructure.Contexts;
using Carvia.Infrastructure.Middlewares;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("SQLiteConnection");
builder.Services.AddDbContext<BaseDbContext>(options =>
{
  options.UseSqlite(connectionString);
});

builder.Services.AddCarDependencies();
builder.Services.AddCarImageDependencies();
builder.Services.AddCategoryDependencies();
builder.Services.AddCuratorItemDependencies();
builder.Services.AddUserDependencies();

builder.Services.AddControllersWithViews();
builder.Services.Configure<RazorViewEngineOptions>(options =>
{
  options.ViewLocationExpanders.Add(new FeatureViewLocationExpander());
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
  .AddCookie(options =>
  {
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
  });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
}
else
{
  app.UseExceptionHandler("/Shared/Error");
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
  name: "default",
  pattern: "{controller=Cars}/{action=Index}/{id?}");

app.Run();