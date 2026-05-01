using Carvia.Core.Models;
using System.Linq.Expressions;

namespace Carvia.Features.CarImages;

public interface ICarImageService
{
  Task<ReturnModel<List<ShowcaseCarImageViewModel>>> GetAllAsync(
    string? userRole,
    Expression<Func<CarImage, bool>>? filter = null,
    Func<IQueryable<CarImage>, IQueryable<CarImage>>? include = null,
    Func<IQueryable<CarImage>, IOrderedQueryable<CarImage>>? orderBy = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<DetailedCarImageViewModel>> GetAsync(
    Expression<Func<CarImage, bool>> predicate,
    string? userRole,
    Func<IQueryable<CarImage>, IQueryable<CarImage>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<DetailedCarImageViewModel>> GetByIdAsync(
    Guid id,
    string? userRole,
    Func<IQueryable<CarImage>, IQueryable<CarImage>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<CreatedCarImageViewModel>> AddAsync(
    CreateCarImageViewModel request,
    string userRole,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<NoData>> RemoveAsync(
    Guid id,
    string userRole,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<NoData>> UpdateAsync(
    UpdateCarImageViewModel request,
    string userRole,
    CancellationToken cancellationToken = default);
}
