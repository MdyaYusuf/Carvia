using Carvia.Core.Models;
using Carvia.Core.Persistence;
using System.Linq.Expressions;

namespace Carvia.Features.Roles;

public class RoleService(
  IRoleRepository _roleRepository,
  RoleBusinessRules _businessRules,
  RoleMapper _mapper,
  IUnitOfWork _unitOfWork) : IRoleService
{
  public async Task<ReturnModel<List<ShowcaseRoleViewModel>>> GetAllAsync(
    string? userRole,
    Expression<Func<Role, bool>>? filter = null,
    Func<IQueryable<Role>, IQueryable<Role>>? include = null,
    Func<IQueryable<Role>, IOrderedQueryable<Role>>? orderBy = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    var roles = await _roleRepository.GetAllAsync(
      filter,
      include,
      orderBy,
      enableTracking,
      cancellationToken: cancellationToken);

    var response = _mapper.EntityToShowcaseViewModelList(roles);

    return new ReturnModel<List<ShowcaseRoleViewModel>>
    {
      Success = true,
      Message = "Rol listesi başarıyla getirildi.",
      Data = response,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<ShowcaseRoleViewModel>> GetByIdAsync(
    int id,
    string? userRole,
    Func<IQueryable<Role>, IQueryable<Role>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    var role = await _businessRules.GetRoleIfExistAsync(
      id,
      include,
      enableTracking,
      cancellationToken);

    var response = _mapper.EntityToShowcaseViewModel(role);

    return new ReturnModel<ShowcaseRoleViewModel>
    {
      Success = true,
      Message = "Rol bilgileri başarıyla getirildi.",
      Data = response,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<CreatedRoleViewModel>> AddAsync(
    CreateRoleViewModel request,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    await _businessRules.NameMustBeUniqueAsync(request.Name, cancellationToken);

    Role role = _mapper.CreateViewModelToEntity(request);

    await _roleRepository.AddAsync(role, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    var response = _mapper.EntityToCreatedViewModel(role);

    return new ReturnModel<CreatedRoleViewModel>
    {
      Success = true,
      Message = "Yeni rol başarıyla tanımlandı.",
      Data = response,
      StatusCode = 201
    };
  }

  public async Task<ReturnModel<NoData>> RemoveAsync(
    int id,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    var role = await _businessRules.GetRoleIfExistAsync(
      id,
      enableTracking: true,
      cancellationToken: cancellationToken);

    _roleRepository.Delete(role);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new ReturnModel<NoData>
    {
      Success = true,
      Message = "Rol sistemden başarıyla silindi.",
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<NoData>> UpdateAsync(
    UpdateRoleViewModel request,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    var existingRole = await _businessRules.GetRoleIfExistAsync(
      request.Id,
      enableTracking: true,
      cancellationToken: cancellationToken);

    if (existingRole.Name != request.Name)
    {
      await _businessRules.NameMustBeUniqueAsync(request.Name, cancellationToken);
    }

    _mapper.UpdateEntityFromViewModel(request, existingRole);

    _roleRepository.Update(existingRole);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new ReturnModel<NoData>
    {
      Success = true,
      Message = "Rol ismi başarıyla güncellendi.",
      StatusCode = 200
    };
  }
}
