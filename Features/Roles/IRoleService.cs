using Carvia.Core.Models;
using System.Linq.Expressions;

namespace Carvia.Features.Roles;

public interface IRoleService
{
  Task<ReturnModel<List<ShowcaseRoleViewModel>>> GetAllAsync(
    string? userRole,
    Expression<Func<Role, bool>>? filter = null,
    Func<IQueryable<Role>, IQueryable<Role>>? include = null,
    Func<IQueryable<Role>, IOrderedQueryable<Role>>? orderBy = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<ShowcaseRoleViewModel>> GetByIdAsync(
    int id,
    string? userRole,
    Func<IQueryable<Role>, IQueryable<Role>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<CreatedRoleViewModel>> AddAsync(
    CreateRoleViewModel request,
    string userRole,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<NoData>> RemoveAsync(
    int id,
    string userRole,
    CancellationToken cancellationToken = default);

  Task<ReturnModel<NoData>> UpdateAsync(
    UpdateRoleViewModel request,
    string userRole,
    CancellationToken cancellationToken = default);
}
