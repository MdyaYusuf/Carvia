using Carvia.Core.Models;
using System.Linq.Expressions;

namespace Carvia.Features.Cars;

public interface ICarService
{
  Task<ReturnModel<List<ShowcaseCarViewModel>>> GetAllAsync(
    string? userRole,
    Expression<Func<Car, bool>>? filter = null,
    Func<IQueryable<Car>, IQueryable<Car>>? include = null,
    Func<IQueryable<Car>, IOrderedQueryable<Car>>? orderBy = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<DetailedCarViewModel>> GetAsync(
    Expression<Func<Car, bool>> predicate,
    string? userRole,
    Func<IQueryable<Car>, IQueryable<Car>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<DetailedCarViewModel>> GetByIdAsync(
    Guid id,
    string? userRole,
    Func<IQueryable<Car>, IQueryable<Car>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<CreatedCarViewModel>> AddAsync(
    CreateCarViewModel request,
    string userRole,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<NoData>> RemoveAsync(
    Guid id,
    string userRole,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<NoData>> UpdateAsync(
    UpdateCarViewModel request,
    string userRole,
    CancellationToken cancellationToken = default);
}
