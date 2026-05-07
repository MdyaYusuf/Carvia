using Carvia.Core.Models;
using Carvia.Features.Authentication;
using Carvia.Features.CarImages;
using Carvia.Features.Cars;
using Carvia.Features.Categories;
using Carvia.Features.CuratorItems;
using Carvia.Features.Roles;
using Carvia.Features.Users;
using Carvia.Infrastructure.Data;
using Carvia.Infrastructure.Middlewares;
using Carvia.Infrastructure.Razor;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataDependencies(builder.Configuration);
builder.Services.AddCarDependencies();
builder.Services.AddCarImageDependencies();
builder.Services.AddCategoryDependencies();
builder.Services.AddCuratorItemDependencies();
builder.Services.AddUserDependencies();
builder.Services.AddRoleDependencies();
builder.Services.AddAuthenticationDependencies();

builder.Services.AddControllersWithViews();
builder.Services.Configure<RazorViewEngineOptions>(options =>
{
  options.ViewLocationExpanders.Clear();
  options.ViewLocationExpanders.Add(new FeatureViewLocationExpander());
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.Configure<TokenOptions>(builder.Configuration.GetSection("TokenOptions"));

var tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<TokenOptions>() ?? throw new InvalidOperationException("TokenOptions bölümü yapılandırma dosyasında appsettings bulunamadı.");

builder.Services.AddAuthentication(options =>
{
  options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
  options.LoginPath = "/Account/Login";
  options.LogoutPath = "/Account/Logout";
  options.Cookie.HttpOnly = true;
  options.Cookie.SameSite = SameSiteMode.Strict;
  options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
  options.ExpireTimeSpan = TimeSpan.FromDays(7);
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
  options.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidIssuer = tokenOptions.Issuer,
    ValidAudience = tokenOptions.Audience,
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.SecurityKey)),
    ClockSkew = TimeSpan.Zero
  };
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