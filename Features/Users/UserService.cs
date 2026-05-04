using Carvia.Core.Models;
using Carvia.Core.Persistence;
using Carvia.Core.Utilities.Files;
using Carvia.Features.Cars;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Carvia.Features.Users;

public class UserService(
  IUserRepository _userRepository,
  UserBusinessRules _businessRules,
  CarBusinessRules _carBusinessRules,
  UserMapper _mapper,
  IUnitOfWork _unitOfWork) : IUserService
{
  public async Task<ReturnModel<List<ShowcaseUserViewModel>>> GetAllAsync(
    string? userRole,
    Expression<Func<User, bool>>? filter = null,
    Func<IQueryable<User>, IQueryable<User>>? include = null,
    Func<IQueryable<User>, IOrderedQueryable<User>>? orderBy = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    var users = await _userRepository.GetAllAsync(
      filter,
      include,
      orderBy ?? (q => q.OrderBy(u => u.Username)),
      enableTracking,
      cancellationToken: cancellationToken);

    var response = _mapper.EntityToShowcaseViewModelList(users);

    return new ReturnModel<List<ShowcaseUserViewModel>>
    {
      Success = true,
      Message = "Kullanıcı listesi başarıyla getirildi.",
      Data = response,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<DetailedUserViewModel>> GetAsync(
    Expression<Func<User, bool>> predicate,
    string? userRole,
    Func<IQueryable<User>, IQueryable<User>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    var user = await _userRepository.GetAsync(
      predicate,
      include: query => query.Include(u => u.CuratorItems),
      enableTracking,
      cancellationToken);

    if (user == null)
    {
      return new ReturnModel<DetailedUserViewModel>
      {
        Success = true,
        Message = "Eşleşen kullanıcı bulunamadı.",
        Data = null,
        StatusCode = 200
      };
    }

    var response = _mapper.EntityToDetailedViewModel(user);

    return new ReturnModel<DetailedUserViewModel>
    {
      Success = true,
      Message = "Kullanıcı detayları başarıyla getirildi.",
      Data = response,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<DetailedUserViewModel>> GetByIdAsync(
    Guid id,
    string? userRole,
    Func<IQueryable<User>, IQueryable<User>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    var user = await _businessRules.GetUserIfExistAsync(
      id,
      include: query => query.Include(u => u.CuratorItems),
      enableTracking: false,
      cancellationToken: cancellationToken);

    var response = _mapper.EntityToDetailedViewModel(user);

    return new ReturnModel<DetailedUserViewModel>
    {
      Success = true,
      Message = "Kullanıcı bilgileri başarıyla getirildi.",
      Data = response,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<NoData>> RemoveAsync(
    Guid id,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    _carBusinessRules.UserRoleMustBeAdmin(userRole);

    var user = await _businessRules.GetUserIfExistAsync(
      id,
      enableTracking: true,
      cancellationToken: cancellationToken);

    FileHelper.DeleteImageFromDisk(user.ProfileImageUrl);

    _userRepository.Delete(user);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new ReturnModel<NoData>
    {
      Success = true,
      Message = "Kullanıcı hesabı başarıyla silindi.",
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<NoData>> UpdateAsync(
    UpdateUserViewModel request,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    var existingUser = await _businessRules.GetUserIfExistAsync(
      request.Id,
      enableTracking: true,
      cancellationToken: cancellationToken);

    if (existingUser.Email != request.Email)
    {
      await _businessRules.UserEmailMustBeUniqueAsync(request.Email, request.Id, cancellationToken);
    }

    if (existingUser.Username != request.Username)
    {
      await _businessRules.UsernameMustBeUniqueAsync(request.Username, request.Id, cancellationToken);
    }

    existingUser.ProfileImageUrl = await FileHelper.ReplaceImageOnDisk(
      request.NewProfileImage,
      existingUser.ProfileImageUrl,
      "users",
      request.Username,
      cancellationToken) ?? existingUser.ProfileImageUrl;

    _mapper.UpdateEntityFromViewModel(request, existingUser);

    _userRepository.Update(existingUser);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new ReturnModel<NoData>
    {
      Success = true,
      Message = "Profil başarıyla güncellendi.",
      StatusCode = 200
    };
  }
}
