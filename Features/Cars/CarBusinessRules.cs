using Carvia.Core.Exceptions;
using Carvia.Features.Categories;

namespace Carvia.Features.Cars;

public class CarBusinessRules(
  ICarRepository _carRepository,
  ICategoryRepository _categoryRepository)
{
  public async Task<Car> GetCarIfExistAsync(
    Guid id,
    Func<IQueryable<Car>, IQueryable<Car>>? include = null,
    bool enableTracking = false,
    CancellationToken cancellationToken = default)
  {
    var car = await _carRepository.GetByIdAsync(id, include, enableTracking, cancellationToken);

    if (car == null)
    {
      throw new NotFoundException($"{id} numaralı araç bulunamadı.");
    }

    return car;
  }

  public void UserRoleMustBeAdmin(string userRole)
  {
    if (userRole != "Admin")
    {
      throw new ForbiddenException("Bu işlemi yapmak için yetkiniz bulunmamaktadır.");
    }
  }

  public async Task CategoryMustExistAsync(
    Guid categoryId,
    CancellationToken cancellationToken = default)
  {
    var exists = await _categoryRepository.AnyAsync(x => x.Id == categoryId, cancellationToken);

    if (!exists)
    {
      throw new NotFoundException($"{categoryId} numaralı kategori bulunamadı.");
    }
  }

  public async Task CarMustNotBeDuplicateAsync(
    string make,
    string model,
    int year,
    Guid? id = null,
    CancellationToken cancellationToken = default)
  {
    var exists = await _carRepository.AnyAsync(c =>
      c.Make == make &&
      c.Model == model &&
      c.Year == year &&
      (id == null || c.Id != id),
      cancellationToken);

    if (exists)
    {
      throw new BusinessException("Bu marka, model ve yıla sahip bir araç zaten mevcut.");
    }
  }

  public void PriceMustBePositive(
    decimal price)
  {
    if (price <= 0)
    {
      throw new BusinessException("Araç fiyatı 0'dan büyük olmalıdır.");
    }
  }

  public void YearMustBeInValidRange(
    int year)
  {
    var currentYear = DateTime.Now.Year;

    if (year < 1886 || year > currentYear + 1)
    {
      throw new BusinessException($"Araç yılı 1886 ile {currentYear + 1} arasında olmalıdır.");
    }
  }
}
