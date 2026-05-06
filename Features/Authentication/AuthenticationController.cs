using Carvia.Infrastructure.Controllers;
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
  public async Task<IActionResult> Login(
    LoginUserViewModel request,
    CancellationToken cancellationToken)
  {
    if (!ModelState.IsValid)
    {
      return View(request);
    }

    await _authService.LoginAsync(request, cancellationToken);

    return RedirectToAction("Index", "Cars");
  }

  [HttpGet]
  public IActionResult Register()
  {
    return View(new RegisterUserViewModel());
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Register(
    RegisterUserViewModel request,
    CancellationToken cancellationToken)
  {
    if (!ModelState.IsValid)
    {
      return View(request);
    }

    await _authService.RegisterAsync(request, cancellationToken);

    return RedirectToAction(nameof(Login));
  }

  [HttpPost]
  [Authorize]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Logout(
    CancellationToken cancellationToken)
  {
    await _authService.RevokeRefreshTokenAsync(null, cancellationToken);

    return RedirectToAction("Index", "Cars");
  }
}
