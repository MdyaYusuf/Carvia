using Carvia.Features.Categories;
using Carvia.Infrastructure.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Carvia.Features.Cars;

public class CarsController(
  ICarService _carService,
  ICategoryService _categoryService) : BaseController
{
  [HttpGet]
  public async Task<IActionResult> Index(
    CancellationToken cancellationToken)
  {
    var result = await _carService.GetAllAsync(
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    return View(result.Data);
  }

  [HttpGet]
  public async Task<IActionResult> Showcase(CancellationToken cancellationToken)
  {
    var result = await _carService.GetAllAsync(
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    return View(result.Data);
  }

  [HttpGet]
  public async Task<IActionResult> Details(
    Guid id,
    CancellationToken cancellationToken)
  {
    var result = await _carService.GetByIdAsync(
      id,
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    return View(result.Data);
  }

  [HttpGet]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> Create(
    CancellationToken cancellationToken)
  {
    var viewModel = new CreateCarViewModel();
    await PopulateCategoriesAsync(viewModel, cancellationToken);

    return View(viewModel);
  }

  [HttpPost]
  [Authorize(Roles = "Admin")]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Create(
    CreateCarViewModel request,
    CancellationToken cancellationToken)
  {
    if (!ModelState.IsValid)
    {
      await PopulateCategoriesAsync(request, cancellationToken);

      return View(request);
    }

    await _carService.AddAsync(request, GetUserRole(), cancellationToken);

    return RedirectToAction(nameof(Index));
  }

  [HttpGet]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> Edit(
    Guid id,
    CancellationToken cancellationToken)
  {
    var carResult = await _carService.GetByIdAsync(id, GetUserRole(), cancellationToken: cancellationToken);

    var viewModel = new UpdateCarViewModel
    {
      Id = carResult.Data!.Id,
      Make = carResult.Data.Make,
      Model = carResult.Data.Model,
      Year = carResult.Data.Year,
      Price = carResult.Data.Price,
      Description = carResult.Data.Description,
      ExistingImageUrl = carResult.Data.MainImageUrl,
      CategoryId = carResult.Data.CategoryId
    };

    await PopulateCategoriesAsync(viewModel, cancellationToken);

    return View(viewModel);
  }

  [HttpPost]
  [Authorize(Roles = "Admin")]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Edit(
    UpdateCarViewModel request,
    CancellationToken cancellationToken)
  {
    if (!ModelState.IsValid)
    {
      await PopulateCategoriesAsync(request, cancellationToken);
      return View(request);
    }

    await _carService.UpdateAsync(request, GetUserRole(), cancellationToken);

    return RedirectToAction(nameof(Details), new { id = request.Id });
  }

  [HttpPost]
  [Authorize(Roles = "Admin")]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Delete(
    Guid id,
    CancellationToken cancellationToken)
  {
    await _carService.RemoveAsync(id, GetUserRole(), cancellationToken);

    return RedirectToAction(nameof(Index));
  }

  private async Task PopulateCategoriesAsync(
    dynamic viewModel,
    CancellationToken cancellationToken)
  {
    var categoriesResult = await _categoryService.GetAllAsync(GetUserRole(), cancellationToken: cancellationToken);

    viewModel.Categories = categoriesResult.Data?.Select(c => new SelectListItem
    {
      Value = c.Id.ToString(),
      Text = c.Name
    });
  }
}
