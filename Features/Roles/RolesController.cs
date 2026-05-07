using Carvia.Infrastructure.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Carvia.Features.Roles;

[Authorize(Roles = "Admin")]
public class RolesController(IRoleService _roleService) : BaseController
{
  [HttpGet]
  public async Task<IActionResult> Index(
    CancellationToken cancellationToken)
  {
    var result = await _roleService.GetAllAsync(
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    return View(result.Data);
  }

  [HttpGet]
  public IActionResult Create()
  {
    return View(new CreateRoleViewModel());
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Create(
    CreateRoleViewModel request,
    CancellationToken ct)
  {
    if (!ModelState.IsValid)
    {
      return View(request);
    }

    var result = await _roleService.AddAsync(request, GetUserRole(), ct);

    if (result.Success)
    {
      return RedirectToAction(nameof(Index));
    }

    ModelState.AddModelError("", result.Message ?? "Beklenmeyen bir hata meydana geldi.");

    return View(request);
  }

  [HttpGet]
  public async Task<IActionResult> Edit(
    int id,
    CancellationToken cancellationToken)
  {
    var roleResult = await _roleService.GetByIdAsync(id, GetUserRole(), cancellationToken: cancellationToken);

    var viewModel = new UpdateRoleViewModel
    {
      Id = roleResult.Data!.Id,
      Name = roleResult.Data.Name
    };

    return View(viewModel);
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Edit(
    UpdateRoleViewModel request,
    CancellationToken cancellationToken)
  {
    if (!ModelState.IsValid)
    {
      return View(request);
    }

    var result = await _roleService.UpdateAsync(request, GetUserRole(), cancellationToken);

    if (result.Success)
    {
      return RedirectToAction(nameof(Index));
    }

    ModelState.AddModelError("", result.Message ?? "Güncelleme sırasında bir hata oluştu.");

    return View(request);
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Delete(
    int id,
    CancellationToken cancellationToken)
  {
    await _roleService.RemoveAsync(id, GetUserRole(), cancellationToken);

    return RedirectToAction(nameof(Index));
  }
}
