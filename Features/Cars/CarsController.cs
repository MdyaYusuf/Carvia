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
    var result = await _carService.GetAllAsync(GetUserRole(), cancellationToken: cancellationToken);

    await PopulateCategoriesAsync(ViewBag, cancellationToken);

    return View(result.Data);
  }

  [HttpGet]
  public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
  {
    var result = await _carService.GetByIdAsync(id, userRole: GetUserRole(), cancellationToken: cancellationToken);

    if (result.Data == null)
    {
      return NotFound();
    }

    await PopulateCategoriesAsync(ViewBag, cancellationToken);

    return View(result.Data);
  }

  [HttpGet]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> Create(CancellationToken ct)
  {
    var viewModel = new CreateCarViewModel();

    await PopulateCategoriesAsync(viewModel, ct);

    return View(viewModel);
  }

  [HttpPost]
  [Authorize(Roles = "Admin")]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Create(CreateCarViewModel request, CancellationToken ct)
  {
    if (!ModelState.IsValid)
    {
      await PopulateCategoriesAsync(request, ct);

      return View(request);
    }

    var result = await _carService.AddAsync(request, GetUserRole(), ct);

    if (result.Success)
    {
      return RedirectToAction(nameof(Showcase));
    }

    ModelState.AddModelError("", result.Message ?? "Beklenmeyen bir hata meydana geldi.");

    await PopulateCategoriesAsync(request, ct);

    return View(request);
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
    CancellationToken ct)
  {
    if (!ModelState.IsValid)
    {
      return RedirectToAction(nameof(Showcase));
    }

    var result = await _carService.UpdateAsync(request, GetUserRole(), ct);

    if (result.Success)
    {
      return RedirectToAction(nameof(Details), new { id = request.Id });
    }

    return RedirectToAction(nameof(Showcase));
  }

  [HttpGet]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> GetEditData(Guid id, CancellationToken ct)
  {
    var result = await _carService.GetByIdAsync(id, GetUserRole(), cancellationToken: ct);

    if (result.Data == null)
    {
      return NotFound();
    }

    return Json(new
    {
      id = result.Data.Id,
      make = result.Data.Make,
      model = result.Data.Model,
      year = result.Data.Year,
      price = result.Data.Price,
      categoryId = result.Data.CategoryId.ToString().ToLower(),
      mainImageUrl = result.Data.MainImageUrl,
      description = result.Data.Description,
      existingGallery = result.Data.Gallery.Select(g => new { id = g.Id, url = g.Url })
    });
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
      Text = c.Name.ToUpper()
    });
  }
}
