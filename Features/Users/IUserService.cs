using Carvia.Core.Models;
using System.Linq.Expressions;

namespace Carvia.Features.Users;

public interface IUserService
{
  Task<ReturnModel<List<ShowcaseUserViewModel>>> GetAllAsync(
    string? userRole,
    Expression<Func<User, bool>>? filter = null,
    Func<IQueryable<User>, IQueryable<User>>? include = null,
    Func<IQueryable<User>, IOrderedQueryable<User>>? orderBy = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<DetailedUserViewModel>> GetAsync(
    Expression<Func<User, bool>> predicate,
    string? userRole,
    Func<IQueryable<User>, IQueryable<User>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<DetailedUserViewModel>> GetByIdAsync(
    Guid id,
    string? userRole,
    Func<IQueryable<User>, IQueryable<User>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<CreatedUserViewModel>> AddAsync(
    CreateUserViewModel request,
    string userRole,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<NoData>> RemoveAsync(
    Guid id,
    string userRole,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<NoData>> UpdateAsync(
    UpdateUserViewModel request,
    string userRole,
    CancellationToken cancellationToken = default);
}
