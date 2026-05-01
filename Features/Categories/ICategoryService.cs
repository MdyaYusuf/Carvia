using Carvia.Core.Models;
using System.Linq.Expressions;

namespace Carvia.Features.Categories;

public interface ICategoryService
{
  Task<ReturnModel<List<ShowcaseCategoryViewModel>>> GetAllAsync(
    string? userRole,
    Expression<Func<Category, bool>>? filter = null,
    Func<IQueryable<Category>, IQueryable<Category>>? include = null,
    Func<IQueryable<Category>, IOrderedQueryable<Category>>? orderBy = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<DetailedCategoryViewModel>> GetAsync(
    Expression<Func<Category, bool>> predicate,
    string? userRole,
    Func<IQueryable<Category>, IQueryable<Category>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<DetailedCategoryViewModel>> GetByIdAsync(
    Guid id,
    string? userRole,
    Func<IQueryable<Category>, IQueryable<Category>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<CreatedCategoryViewModel>> AddAsync(
    CreateCategoryViewModel request,
    string userRole,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<NoData>> RemoveAsync(
    Guid id,
    string userRole,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<NoData>> UpdateAsync(
    UpdateCategoryViewModel request,
    string userRole,
    CancellationToken cancellationToken = default);
}
