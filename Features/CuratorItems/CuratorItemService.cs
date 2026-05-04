using Carvia.Core.Models;
using Carvia.Core.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Carvia.Features.CuratorItems;

public class CuratorItemService(
    ICuratorItemRepository _curatorItemRepository,
    CuratorItemBusinessRules _businessRules,
    CuratorItemMapper _mapper,
    IUnitOfWork _unitOfWork) : ICuratorItemService
{
  public async Task<ReturnModel<List<ShowcaseCuratorItemViewModel>>> GetAllAsync(
    string? userRole,
    Expression<Func<CuratorItem, bool>>? filter = null,
    Func<IQueryable<CuratorItem>, IQueryable<CuratorItem>>? include = null,
    Func<IQueryable<CuratorItem>, IOrderedQueryable<CuratorItem>>? orderBy = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    var curatorItems = await _curatorItemRepository.GetAllAsync(
      filter,
      include: query => query.Include(ci => ci.Car),
      orderBy,
      enableTracking,
      cancellationToken: cancellationToken);

    var response = _mapper.EntityToShowcaseViewModelList(curatorItems);

    return new ReturnModel<List<ShowcaseCuratorItemViewModel>>
    {
      Success = true,
      Message = "Küratör listesi başarıyla getirildi.",
      Data = response,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<DetailedCuratorItemViewModel>> GetAsync(
    Expression<Func<CuratorItem, bool>> predicate,
    string? userRole,
    Func<IQueryable<CuratorItem>, IQueryable<CuratorItem>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    var curatorItem = await _curatorItemRepository.GetAsync(
      predicate,
      include: query => query.Include(ci => ci.Car).Include(ci => ci.User),
      enableTracking,
      cancellationToken);

    if (curatorItem == null)
    {
      return new ReturnModel<DetailedCuratorItemViewModel>
      {
        Success = true,
        Message = "Eşleşen küratör öğesi bulunamadı.",
        Data = null,
        StatusCode = 200
      };
    }

    var response = _mapper.EntityToDetailedViewModel(curatorItem);

    return new ReturnModel<DetailedCuratorItemViewModel>
    {
      Success = true,
      Message = "Küratör öğesi detayları başarıyla getirildi.",
      Data = response,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<DetailedCuratorItemViewModel>> GetByIdAsync(
    Guid id,
    string? userRole,
    Func<IQueryable<CuratorItem>, IQueryable<CuratorItem>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    var curatorItem = await _businessRules.GetCuratorItemIfExistAsync(
      id,
      include: query => query.Include(ci => ci.Car).Include(ci => ci.User),
      enableTracking: false,
      cancellationToken: cancellationToken);

    var response = _mapper.EntityToDetailedViewModel(curatorItem);

    return new ReturnModel<DetailedCuratorItemViewModel>
    {
      Success = true,
      Message = "Küratör öğesi başarıyla getirildi.",
      Data = response,
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<CreatedCuratorItemViewModel>> AddAsync(
    CreateCuratorItemViewModel request,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    await _businessRules.UserMustExistAsync(request.UserId, cancellationToken);
    await _businessRules.CarMustExistAsync(request.CarId, cancellationToken);
    await _businessRules.CuratorItemMustNotBeDuplicateAsync(request.UserId, request.CarId, cancellationToken);

    CuratorItem curatorItem = _mapper.CreateViewModelToEntity(request);

    await _curatorItemRepository.AddAsync(curatorItem, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    var response = _mapper.EntityToCreatedViewModel(curatorItem);

    return new ReturnModel<CreatedCuratorItemViewModel>
    {
      Success = true,
      Message = "Araç küratör listenize başarıyla eklendi.",
      Data = response,
      StatusCode = 201
    };
  }

  public async Task<ReturnModel<NoData>> RemoveAsync(
    Guid id,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    var curatorItem = await _businessRules.GetCuratorItemIfExistAsync(
      id,
      enableTracking: true,
      cancellationToken: cancellationToken);

    if (userRole != "Admin")
    {
      _businessRules.UserMustOwnCuratorItem(curatorItem, curatorItem.UserId);
    }

    _curatorItemRepository.Delete(curatorItem);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new ReturnModel<NoData>
    {
      Success = true,
      Message = "Araç küratör listenizden kaldırıldı.",
      StatusCode = 200
    };
  }

  public async Task<ReturnModel<NoData>> UpdateAsync(
    UpdateCuratorItemViewModel request,
    string userRole,
    CancellationToken cancellationToken = default)
  {
    var existingItem = await _businessRules.GetCuratorItemIfExistAsync(
      request.Id,
      enableTracking: true,
      cancellationToken: cancellationToken);

    if (userRole != "Admin")
    {
      _businessRules.UserMustOwnCuratorItem(existingItem, existingItem.UserId);
    }

    _mapper.UpdateEntityFromViewModel(request, existingItem);

    _curatorItemRepository.Update(existingItem);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new ReturnModel<NoData>
    {
      Success = true,
      Message = "Küratör notu başarıyla güncellendi.",
      StatusCode = 200
    };
  }
}
