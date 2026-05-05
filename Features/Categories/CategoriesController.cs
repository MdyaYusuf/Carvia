using Carvia.Infrastructure.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Carvia.Features.Categories;

public class CategoriesController(
  ICategoryService _categoryService) : BaseController
{
  [HttpGet]
  public async Task<IActionResult> Index(CancellationToken cancellationToken)
  {
    var result = await _categoryService.GetAllAsync(
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    return View(result.Data);
  }

  [HttpGet]
  public async Task<IActionResult> Details(
    Guid id,
    CancellationToken cancellationToken)
  {
    var result = await _categoryService.GetByIdAsync(
      id,
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    return View(result.Data);
  }

  [HttpGet]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> Create()
  {
    return View(new CreateCategoryViewModel());
  }

  [HttpPost]
  [Authorize(Roles = "Admin")]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Create(
    CreateCategoryViewModel request,
    CancellationToken cancellationToken)
  {
    if (!ModelState.IsValid)
    {
      return View(request);
    }

    await _categoryService.AddAsync(request, GetUserRole(), cancellationToken);

    return RedirectToAction(nameof(Index));
  }

  [HttpGet]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> Edit(
    Guid id,
    CancellationToken cancellationToken)
  {
    var result = await _categoryService.GetByIdAsync(
      id,
      userRole: GetUserRole(),
      cancellationToken: cancellationToken);

    var viewModel = new UpdateCategoryViewModel
    {
      Id = result.Data!.Id,
      Name = result.Data.Name,
      Slug = result.Data.Slug,
      Description = result.Data.Description
    };

    return View(viewModel);
  }

  [HttpPost]
  [Authorize(Roles = "Admin")]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Edit(
    UpdateCategoryViewModel request,
    CancellationToken cancellationToken)
  {
    if (!ModelState.IsValid)
    {
      return View(request);
    }

    await _categoryService.UpdateAsync(request, GetUserRole(), cancellationToken);

    return RedirectToAction(nameof(Index));
  }

  [HttpPost]
  [Authorize(Roles = "Admin")]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Delete(
    Guid id,
    CancellationToken cancellationToken)
  {
    await _categoryService.RemoveAsync(id, GetUserRole(), cancellationToken);

    return RedirectToAction(nameof(Index));
  }
}
