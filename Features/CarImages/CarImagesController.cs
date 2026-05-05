using Carvia.Infrastructure.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Carvia.Features.CarImages;

public class CarImagesController(ICarImageService _carImageService) : BaseController
{
  [HttpGet("Cars/{carId:guid}/Gallery")]
  public async Task<IActionResult> Index(
    Guid carId,
    CancellationToken cancellationToken)
  {
    var result = await _carImageService.GetAllAsync(
      userRole: GetUserRole(),
      filter: ci => ci.CarId == carId,
      cancellationToken: cancellationToken);

    ViewBag.CarId = carId;

    return View(result.Data);
  }

  [HttpGet("Cars/{carId:guid}/Gallery/Add")]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> Create(
    Guid carId)
  {
    var viewModel = new CreateCarImageViewModel
    {
      CarId = carId,
      DisplayOrder = 0
    };

    return View(viewModel);
  }

  [HttpPost("Cars/{carId:guid}/Gallery/Add")]
  [Authorize(Roles = "Admin")]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Create(
    CreateCarImageViewModel request,
    CancellationToken cancellationToken)
  {
    if (!ModelState.IsValid)
    {
      return View(request);
    }

    await _carImageService.AddAsync(request, GetUserRole(), cancellationToken);

    return RedirectToAction(nameof(Index), new { carId = request.CarId });
  }

  [HttpGet]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> Edit(
    Guid id,
    CancellationToken cancellationToken)
  {
    var result = await _carImageService.GetByIdAsync(
      id,
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    var viewModel = new UpdateCarImageViewModel
    {
      Id = result.Data!.Id,
      CarId = result.Data.CarId,
      DisplayOrder = result.Data.DisplayOrder,
      ExistingImageUrl = result.Data.Url
    };

    return View(viewModel);
  }

  [HttpPost]
  [Authorize(Roles = "Admin")]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Edit(
    UpdateCarImageViewModel request,
    CancellationToken cancellationToken)
  {
    if (!ModelState.IsValid)
    {
      return View(request);
    }

    await _carImageService.UpdateAsync(request, GetUserRole(), cancellationToken);

    return RedirectToAction(nameof(Index), new { carId = request.CarId });
  }

  [HttpPost]
  [Authorize(Roles = "Admin")]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Delete(
    Guid id,
    Guid carId,
    CancellationToken cancellationToken)
  {
    await _carImageService.RemoveAsync(id, GetUserRole(), cancellationToken);

    return RedirectToAction(nameof(Index), new { carId = carId });
  }
}
