using Carvia.Infrastructure.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Carvia.Features.Users;

public class UsersController(
  IUserService _userService) : BaseController
{
  [HttpGet]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> Index(
    CancellationToken cancellationToken)
  {
    var result = await _userService.GetAllAsync(
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    return View(result.Data);
  }

  [HttpGet]
  [Authorize]
  public async Task<IActionResult> Profile(
    CancellationToken cancellationToken)
  {
    var result = await _userService.GetByIdAsync(
      id: GetUserId(),
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    return View(result.Data);
  }

  [HttpGet]
  [Authorize]
  public async Task<IActionResult> Edit(
    CancellationToken cancellationToken)
  {
    var result = await _userService.GetByIdAsync(
      id: GetUserId(),
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    var viewModel = new UpdateUserViewModel
    {
      Id = result.Data!.Id,
      Username = result.Data.Username,
      Email = result.Data.Email,
      Bio = result.Data.Bio,
      ExistingProfileImageUrl = result.Data.ProfileImageUrl
    };

    return View(viewModel);
  }

  [HttpPost]
  [Authorize]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Edit(
    UpdateUserViewModel request,
    CancellationToken cancellationToken)
  {
    if (!ModelState.IsValid)
    {
      return View(request);
    }

    if (GetUserRole() != "Admin" && request.Id != GetUserId())
    {
      return Forbid();
    }

    await _userService.UpdateAsync(request, GetUserRole(), cancellationToken);

    return RedirectToAction(nameof(Profile));
  }

  [HttpPost]
  [Authorize(Roles = "Admin")]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Delete(
    Guid id,
    CancellationToken cancellationToken)
  {
    await _userService.RemoveAsync(id, GetUserRole(), cancellationToken);

    return RedirectToAction(nameof(Index));
  }
}
