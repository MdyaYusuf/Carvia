using Carvia.Core.Exceptions;
using Carvia.Features.Cars;

namespace Carvia.Features.CarImages;

public class CarImageBusinessRules(
    ICarImageRepository _carImageRepository,
    ICarRepository _carRepository)
{
  public async Task<CarImage> GetCarImageIfExistAsync(
    Guid id,
    Func<IQueryable<CarImage>, IQueryable<CarImage>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    var carImage = await _carImageRepository.GetByIdAsync(id, include, enableTracking, cancellationToken);

    if (carImage == null)
    {
      throw new NotFoundException($"{id} numaralı görsel bulunamadı.");
    }

    return carImage;
  }

  public async Task CarMustExistAsync(
    Guid carId,
    CancellationToken cancellationToken = default)
  {
    var exists = await _carRepository.AnyAsync(x => x.Id == carId, cancellationToken);

    if (!exists)
    {
      throw new NotFoundException($"{carId} numaralı araç bulunamadı.");
    }
  }

  public async Task ImageUrlMustBeUniqueForCarAsync(
    string url,
    Guid carId,
    Guid? id = null,
    CancellationToken cancellationToken = default)
  {
    var exists = await _carImageRepository.AnyAsync(ci => ci.Url == url && ci.CarId == carId && (id == null || ci.Id != id), cancellationToken);

    if (exists)
    {
      throw new BusinessException("Bu görsel bu araç için zaten eklenmiş.");
    }
  }

  public void DisplayOrderCannotBeNegative(
    int displayOrder)
  {
    if (displayOrder < 0)
    {
      throw new BusinessException("Görsel sıralaması negatif bir değer olamaz.");
    }
  }
}
