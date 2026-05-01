using Carvia.Core.Models;
using System.Linq.Expressions;

namespace Carvia.Features.CuratorItems;

public interface ICuratorItemService
{
  Task<ReturnModel<List<ShowcaseCuratorItemViewModel>>> GetAllAsync(
    string? userRole,
    Expression<Func<CuratorItem, bool>>? filter = null,
    Func<IQueryable<CuratorItem>, IQueryable<CuratorItem>>? include = null,
    Func<IQueryable<CuratorItem>, IOrderedQueryable<CuratorItem>>? orderBy = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<DetailedCuratorItemViewModel>> GetAsync(
    Expression<Func<CuratorItem, bool>> predicate,
    string? userRole,
    Func<IQueryable<CuratorItem>, IQueryable<CuratorItem>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<DetailedCuratorItemViewModel>> GetByIdAsync(
    Guid id,
    string? userRole,
    Func<IQueryable<CuratorItem>, IQueryable<CuratorItem>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<CreatedCuratorItemViewModel>> AddAsync(
    CreateCuratorItemViewModel request,
    string userRole,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<NoData>> RemoveAsync(
    Guid id,
    string userRole,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<NoData>> UpdateAsync(
    UpdateCuratorItemViewModel request,
    string userRole,
    CancellationToken cancellationToken = default);
}
