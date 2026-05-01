using Carvia.Core.Exceptions;
using Carvia.Features.Cars;
using Carvia.Features.Users;

namespace Carvia.Features.CuratorItems;

public class CuratorItemBusinessRules(
  ICuratorItemRepository _curatorItemRepository,
  IUserRepository _userRepository,
  ICarRepository _carRepository)
{
  public async Task<CuratorItem> GetCuratorItemIfExistAsync(
    Guid id,
    Func<IQueryable<CuratorItem>, IQueryable<CuratorItem>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    var curatorItem = await _curatorItemRepository.GetByIdAsync(id, include, enableTracking, cancellationToken);

    if (curatorItem == null)
    {
      throw new NotFoundException($"{id} numaralı küratör öğesi bulunamadı.");
    }

    return curatorItem;
  }

  public async Task UserMustExistAsync(
    Guid userId,
    CancellationToken cancellationToken = default)
  {
    var exists = await _userRepository.AnyAsync(u => u.Id == userId, cancellationToken);

    if (!exists)
    {
      throw new NotFoundException($"{userId} numaralı kullanıcı bulunamadı.");
    }
  }

  public async Task CarMustExistAsync(
    Guid carId,
    CancellationToken cancellationToken = default)
  {
    var exists = await _carRepository.AnyAsync(c => c.Id == carId, cancellationToken);

    if (!exists)
    {
      throw new NotFoundException($"{carId} numaralı araç bulunamadı.");
    }
  }

  public async Task CuratorItemMustNotBeDuplicateAsync(
    Guid userId,
    Guid carId,
    CancellationToken cancellationToken = default)
  {
    var exists = await _curatorItemRepository.AnyAsync(ci => ci.UserId == userId && ci.CarId == carId, cancellationToken);

    if (exists)
    {
      throw new BusinessException("Bu araç zaten küratör listenizde mevcut.");
    }
  }

  public void UserMustOwnCuratorItem(
    CuratorItem curatorItem,
    Guid userId)
  {
    if (curatorItem.UserId != userId)
    {
      throw new ForbiddenException("Bu küratör öğesi üzerinde işlem yapma yetkiniz bulunmamaktadır.");
    }
  }
}
