using Carvia.Infrastructure.Controllers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Carvia.Features.Authentication;

public class AuthenticationController(
  IAuthenticationService _authService) : BaseController
{
  [HttpGet]
  public IActionResult Login()
  {
    return View(new LoginUserViewModel());
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Login(LoginUserViewModel request, CancellationToken cancellationToken)
  {
    if (!ModelState.IsValid)
    {
      return View(request);
    }

    var result = await _authService.LoginAsync(request, cancellationToken);

    if (result.Success)
    {
      await HttpContext.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        result.Data.Principal,
        new AuthenticationProperties
        {
          IsPersistent = true,
          ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
        });

      return RedirectToAction("Showcase", "Cars");
    }

    ModelState.AddModelError("", result.Message ?? "Giriş başarısız.");
    return View(request);
  }

  [HttpGet]
  public IActionResult Register()
  {
    return View(new RegisterUserViewModel());
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Register(RegisterUserViewModel request, CancellationToken cancellationToken)
  {
    if (!ModelState.IsValid)
    {
      return View(request);
    }

    var result = await _authService.RegisterAsync(request, cancellationToken);

    if (result.Success)
    {
      return RedirectToAction(nameof(Login));
    }

    ModelState.AddModelError("", result.Message ?? "Kayıt sırasında bir hata oluştu.");
    return View(request);
  }

  [HttpPost]
  [Authorize]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Logout(CancellationToken cancellationToken)
  {
    await _authService.RevokeRefreshTokenAsync(null, cancellationToken);

    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

    return RedirectToAction("Index", "Cars");
  }
}