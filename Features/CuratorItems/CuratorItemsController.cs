using Carvia.Infrastructure.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Carvia.Features.CuratorItems;

[Authorize]
public class CuratorItemsController(
  ICuratorItemService _curatorItemService) : BaseController
{
  [HttpGet]
  public async Task<IActionResult> Index(
    CancellationToken cancellationToken)
  {
    var result = await _curatorItemService.GetAllAsync(
      userRole: GetUserRole(),
      filter: ci => ci.UserId == GetUserId(),
      cancellationToken: cancellationToken);

    return View(result.Data);
  }

  [HttpGet]
  public async Task<IActionResult> Details(
    Guid id,
    CancellationToken cancellationToken)
  {
    var result = await _curatorItemService.GetByIdAsync(
      id,
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    return View(result.Data);
  }

  [HttpGet]
  public IActionResult Create(
    Guid carId)
  {
    var viewModel = new CreateCuratorItemViewModel
    {
      CarId = carId,
      UserId = GetUserId()
    };

    return View(viewModel);
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Create(
    CreateCuratorItemViewModel request,
    CancellationToken cancellationToken)
  {
    if (!ModelState.IsValid)
    {
      return View(request);
    }

    await _curatorItemService.AddAsync(request, GetUserRole(), cancellationToken);

    return RedirectToAction(nameof(Index));
  }

  [HttpGet]
  public async Task<IActionResult> Edit(
    Guid id,
    CancellationToken cancellationToken)
  {
    var result = await _curatorItemService.GetByIdAsync(
      id,
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    var viewModel = new UpdateCuratorItemViewModel
    {
      Id = result.Data!.Id,
      UserNote = result.Data.UserNote
    };

    return View(viewModel);
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Edit(
    UpdateCuratorItemViewModel request,
    CancellationToken cancellationToken)
  {
    if (!ModelState.IsValid)
    {
      return View(request);
    }

    await _curatorItemService.UpdateAsync(request, GetUserRole(), cancellationToken);

    return RedirectToAction(nameof(Index));
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Delete(
    Guid id,
    CancellationToken cancellationToken)
  {
    await _curatorItemService.RemoveAsync(id, GetUserRole(), cancellationToken);

    return RedirectToAction(nameof(Index));
  }
}
